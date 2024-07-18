using Data.Models;
using System;

using Api.Hubs.ViewModels;

namespace Api.Hubs
{
    public enum GameLogicMessageType
    {
        PlayerJoined,
        PlayerDisabled,
        PlayerEnabled,
        PlayerScoreManuallyChanged,
        PlayerHandRaised,
        PlayerHandUnraised,

        GameStarted,
        GamePaused,
        GameUnpaused,
        GameFinished,

        GameQuestionChosen,
        GameAnswerGiven,
        GameAnswerAccepted,
        GameAnswerRejected,
        GameTurnPassed,

        TimerExpired,
        TimerManuallyChanged,
        TimerUpdate,

        Panic
    }

    public interface IGameStateObserver
    {
        public Task OnGameStateChanged(GameLogic logic, GameLogicMessageType message, string? playerId = null);
        public Task OnError(GameLogic logic, string? message);
    }

    public class GameLogic
    {
        public enum GameLogicFlow
        {
            NotStarted,
            InProgress,
            Paused,
            Finished
        }

        public enum GameLogicStage
        {
            NotStarted,
            ChoosingQuestion,
            WaitingForAnswer,
            WaitingForProfEvaluation,
            WaitingForProfChoosingPlayer,
            Finished
        }



        private readonly List<IGameStateObserver> observers = new List<IGameStateObserver>();

        public Dictionary<int, GameLogicThemeView> GameThemes { get; set; } = new Dictionary<int, GameLogicThemeView>();
        public Dictionary<string, GameLogicPlayerView> Players { get; set; } = new Dictionary<string, GameLogicPlayerView>();

        public GameLogicFlow FlowState { get; set; } = GameLogicFlow.NotStarted;
        public GameLogicStage LogicStage { get; set; } = GameLogicStage.NotStarted;

        public Timer GameTimer { get; set; }
        public int GameTimeRemaining { get; set; }

        public string CurrentQuestionId { get; set; }
        public string AnsweringPlayerId { get; set; }
        public string ChoosingPlayerId { get; set; }



        private bool CheckForQuestionsRemaining()
        {
            if (!GameThemes.Any(t => t.Value.Questions.Any(q => q.Value.IsAvaliable)))
            {
                FlowState = GameLogicFlow.Finished;
                LogicStage = GameLogicStage.Finished;

                foreach (var i in observers)
                {
                    i.OnGameStateChanged(this, GameLogicMessageType.GameFinished);
                }

                return false;
            }
            return true;
        }

        private void TimerHandler(object state)
        {
            if (FlowState != GameLogicFlow.InProgress)
            {
                return;
            }
            if (LogicStage != GameLogicStage.WaitingForAnswer)
            {
                return;
            }

            GameTimeRemaining--;

            if (GameTimeRemaining > 0)
            {
                foreach (var i in observers)
                {
                    i.OnGameStateChanged(this, GameLogicMessageType.TimerUpdate);
                }
                return;
            }

            foreach (var i in observers)
            {
                i.OnGameStateChanged(this, GameLogicMessageType.TimerExpired);
            }
            LogicStage = GameLogicStage.WaitingForProfEvaluation;
            RejectAnswer();
        }


        public void AddObserver(IGameStateObserver observer)
        {
            observers.Add(observer);
        }
        public void RemoveObserver(IGameStateObserver observer)
        {
            observers.Remove(observer);
        }


        public void AddPlayer(string playerId)
        {
            if (Players.ContainsKey(playerId))
            {
                foreach (var i in observers)
                {
                    i.OnError(this, "InvalidPlayerId");
                }
                return;
            }
            Players.Add(playerId, new GameLogicPlayerView());

            foreach (var i in observers)
            {
                i.OnGameStateChanged(this, GameLogicMessageType.PlayerJoined, playerId);
            }
        }

        public void DisablePlayer(string playerId)
        {
            if (!Players.ContainsKey(playerId))
            {
                foreach (var i in observers)
                {
                    i.OnError(this, "InvalidPlayerId");
                }
                return;
            }
            Players[playerId].IsActive = false;
            foreach (var i in observers)
            {
                i.OnGameStateChanged(this, GameLogicMessageType.PlayerDisabled, playerId);
            }

        }

        public void EnablePlayer(string playerId)
        {
            if (!Players.ContainsKey(playerId))
            {
                foreach (var i in observers)
                {
                    i.OnError(this, "InvalidPlayerId");
                }
                return;
            }
            Players[playerId].IsActive = false;
            foreach (var i in observers)
            {
                i.OnGameStateChanged(this, GameLogicMessageType.PlayerEnabled, playerId);
            }
        }

        public void ChangePlayerScore(string playerId, int value)
        {
            if (!Players.ContainsKey(playerId))
            {
                foreach (var i in observers)
                {
                    i.OnError(this, "InvalidPlayerId");
                }
                return;
            }
            Players[playerId].GameScore = value;
            foreach (var i in observers)
            {
                i.OnGameStateChanged(this, GameLogicMessageType.PlayerScoreManuallyChanged, playerId);
            }
        }

        public void RaiseHand(string playerId)
        {
            if (!Players.ContainsKey(playerId))
            {
                foreach (var i in observers)
                {
                    i.OnError(this, "InvalidPlayerId");
                }
                return;
            }
            Players[playerId].RaisedHand = true;
            foreach (var i in observers)
            {
                i.OnGameStateChanged(this, GameLogicMessageType.PlayerHandRaised, playerId);
            }
        }

        public void UnraiseHand(string playerId)
        {
            if (!Players.ContainsKey(playerId))
            {
                foreach (var i in observers)
                {
                    i.OnError(this, "InvalidPlayerId");
                }
                return;
            }
            Players[playerId].RaisedHand = false;
            foreach (var i in observers)
            {
                i.OnGameStateChanged(this, GameLogicMessageType.PlayerHandUnraised, playerId);
            }
        }


        public void GameStart()
        {
            FlowState = GameLogicFlow.InProgress;
            LogicStage = GameLogicStage.ChoosingQuestion;

            Random rnd = new Random();
            ChoosingPlayerId = Players.ElementAt(rnd.Next(0, Players.Count)).Key;
            Players.Values.ToList().ForEach(p => { p.RaisedHand = false; p.GaveAnswer = false; });
            AnsweringPlayerId = "null";
            CurrentQuestionId = "null";

            GameTimer = new Timer(TimerHandler, null, 1000, 1000);

            if (CheckForQuestionsRemaining())
            {
                foreach (var i in observers)
                {
                    i.OnGameStateChanged(this, GameLogicMessageType.GameStarted);
                }
            }
        }


        public void Pause()
        {
            if (FlowState != GameLogicFlow.InProgress)
            {
                foreach (var i in observers)
                {
                    i.OnError(this, "GameIsPaused");
                }
                return;
            }
            FlowState = GameLogicFlow.Paused;
            foreach (var i in observers)
            {
                i.OnGameStateChanged(this, GameLogicMessageType.GamePaused);
            }
        }

        public void Unpause()
        {
            if (FlowState != GameLogicFlow.Paused)
            {
                foreach (var i in observers)
                {
                    i.OnError(this, "GameIsUnaused");
                }
                return;
            }
            FlowState = GameLogicFlow.InProgress;
            foreach (var i in observers)
            {
                i.OnGameStateChanged(this, GameLogicMessageType.GameUnpaused);
            }
        }

        public void AddTimeControl(int value)
        {
            if (LogicStage != GameLogicStage.WaitingForAnswer)
            {
                foreach (var i in observers)
                {
                    i.OnError(this, "WrongGameStage");
                }
                return;
            }
            GameTimeRemaining += value;
            foreach (var i in observers)
            {
                i.OnGameStateChanged(this, GameLogicMessageType.TimerManuallyChanged);
            }
        }


        public void ChooseQuestion(string questionId)
        {
            if (FlowState != GameLogicFlow.InProgress)
            {
                foreach (var i in observers)
                {
                    i.OnError(this, "GameIsPaused");
                }
                return;
            }
            if (LogicStage != GameLogicStage.ChoosingQuestion)
            {
                foreach (var i in observers)
                {
                    i.OnError(this, "WrongGameStage");
                }
                return;
            }

            var question = GameThemes.Values
                .SelectMany(t => t.Questions.Values)
                .FirstOrDefault(q => q.QuestionId == questionId);

            if (question == null)
            {
                foreach (var i in observers)
                {
                    i.OnError(this, "InvalidQuestionId");
                }

                if (CheckForQuestionsRemaining())
                {
                    foreach (var i in observers)
                    {
                        i.OnGameStateChanged(this, GameLogicMessageType.Panic);
                    }
                }
                return;
            }
            if (!question.IsAvaliable)
            {
                foreach (var i in observers)
                {
                    i.OnError(this, "QuestionAlreadyPlayed");
                }

                if (CheckForQuestionsRemaining())
                {
                    foreach (var i in observers)
                    {
                        i.OnGameStateChanged(this, GameLogicMessageType.Panic);
                    }
                }
                return;
            }

            CurrentQuestionId = questionId;
            AnsweringPlayerId = ChoosingPlayerId;
            LogicStage = GameLogicStage.WaitingForAnswer;

            GameTimeRemaining = question.TimeControl;
            foreach (var i in observers)
            {
                i.OnGameStateChanged(this, GameLogicMessageType.GameQuestionChosen);
            }
        }

        public void ChoosePlayerToAnswer(string playerId)
        {
            if (FlowState != GameLogicFlow.InProgress)
            {
                foreach (var i in observers)
                {
                    i.OnError(this, "GameIsPaused");
                }
                return;
            }
            if (LogicStage != GameLogicStage.WaitingForProfChoosingPlayer)
            {
                foreach (var i in observers)
                {
                    i.OnError(this, "WrongGameStage");
                }
                return;
            }
            if (!Players.ContainsKey(playerId))
            {
                foreach (var i in observers)
                {
                    i.OnError(this, "InvalidPlayerId");
                }
                return;
            }

            var question = GameThemes.Values
                .SelectMany(t => t.Questions.Values)
                .FirstOrDefault(q => q.QuestionId == CurrentQuestionId);

            if (question == null)
            {
                foreach (var i in observers)
                {
                    i.OnError(this, "InvalidQuestionId");
                }

                LogicStage = GameLogicStage.ChoosingQuestion;
                Players.Values.ToList().ForEach(p => { p.RaisedHand = false; p.GaveAnswer = false; });
                AnsweringPlayerId = "null";
                CurrentQuestionId = "null";

                if (CheckForQuestionsRemaining())
                {
                    foreach (var i in observers)
                    {
                        i.OnGameStateChanged(this, GameLogicMessageType.Panic);
                    }
                }
                return;
            }

            AnsweringPlayerId = playerId;
            LogicStage = GameLogicStage.WaitingForAnswer;
            GameTimeRemaining = question.TimeControl;

            foreach (var i in observers)
            {
                i.OnGameStateChanged(this, GameLogicMessageType.GameTurnPassed);
            }
        }

        public void GetAnswer()
        {
            if (LogicStage != GameLogicStage.WaitingForAnswer)
            {
                foreach (var i in observers)
                {
                    i.OnError(this, "WrongGameStage");
                }
                return;
            }
            LogicStage = GameLogicStage.WaitingForProfEvaluation;
            foreach (var i in observers)
            {
                i.OnGameStateChanged(this, GameLogicMessageType.GameAnswerGiven);
            }
        }

        public void AcceptAnswer()
        {
            if (FlowState != GameLogicFlow.InProgress)
            {
                foreach (var i in observers)
                {
                    i.OnError(this, "GameIsPaused");
                }
                return;
            }
            if (LogicStage != GameLogicStage.WaitingForProfEvaluation)
            {
                foreach (var i in observers)
                {
                    i.OnError(this, "WrongGameStage");
                }
                return;
            }

            var question = GameThemes.Values
                .SelectMany(t => t.Questions.Values)
                .FirstOrDefault(q => q.QuestionId == CurrentQuestionId);

            if (question == null)
            {
                foreach (var i in observers)
                {
                    i.OnError(this, "InvalidQuestionId");
                }

                LogicStage = GameLogicStage.ChoosingQuestion;
                Players.Values.ToList().ForEach(p => { p.RaisedHand = false; p.GaveAnswer = false; });
                AnsweringPlayerId = "null";
                CurrentQuestionId = "null";

                if (CheckForQuestionsRemaining())
                {
                    foreach (var i in observers)
                    {
                        i.OnGameStateChanged(this, GameLogicMessageType.Panic);
                    }
                }
                return;
            }

            Players[AnsweringPlayerId].GameScore += question.QuestionCost;
            question.IsAvaliable = false;

            LogicStage = GameLogicStage.ChoosingQuestion;

            Players.Values.ToList().ForEach(p => { p.RaisedHand = false; p.GaveAnswer = false; });
            ChoosingPlayerId = AnsweringPlayerId;

            var tempAPID = AnsweringPlayerId;

            AnsweringPlayerId = "null";
            CurrentQuestionId = "null";

            if (CheckForQuestionsRemaining())
            {
                foreach (var i in observers)
                {
                    i.OnGameStateChanged(this, GameLogicMessageType.GameAnswerAccepted, tempAPID);
                }
            }
        }

        public void RejectAnswer()
        {
            if (FlowState != GameLogicFlow.InProgress)
            {
                foreach (var i in observers)
                {
                    i.OnError(this, "GameIsPaused");
                }
                return;
            }
            if (LogicStage != GameLogicStage.WaitingForProfEvaluation)
            {
                foreach (var i in observers)
                {
                    i.OnError(this, "WrongGameStage");
                }
                return;
            }

            var question = GameThemes.Values
                .SelectMany(t => t.Questions.Values)
                .FirstOrDefault(q => q.QuestionId == CurrentQuestionId);

            if (question == null)
            {
                foreach (var i in observers)
                {
                    i.OnError(this, "InvalidQuestionId");
                }

                LogicStage = GameLogicStage.ChoosingQuestion;
                Players.Values.ToList().ForEach(p => { p.RaisedHand = false; p.GaveAnswer = false; });
                AnsweringPlayerId = "null";
                CurrentQuestionId = "null";

                if (CheckForQuestionsRemaining())
                {
                    foreach (var i in observers)
                    {
                        i.OnGameStateChanged(this, GameLogicMessageType.Panic);
                    }
                }
                return;
            }

            Players[AnsweringPlayerId].GameScore -= question.QuestionCost;
            Players[AnsweringPlayerId].GaveAnswer = true;

            if (Players.Any(p => !p.Value.GaveAnswer && p.Value.IsActive))
            {
                LogicStage = GameLogicStage.WaitingForProfChoosingPlayer;

                foreach (var i in observers)
                {
                    i.OnGameStateChanged(this, GameLogicMessageType.GameAnswerRejected, AnsweringPlayerId);
                }
            }
            else
            {
                LogicStage = GameLogicStage.ChoosingQuestion;

                Players.Values.ToList().ForEach(p => { p.RaisedHand = false; p.GaveAnswer = false; });

                var tempAPID = AnsweringPlayerId;

                AnsweringPlayerId = "null";
                CurrentQuestionId = "null";

                question.IsAvaliable = false;

                if (CheckForQuestionsRemaining())
                {
                    foreach (var i in observers)
                    {
                        i.OnGameStateChanged(this, GameLogicMessageType.GameAnswerRejected, tempAPID);
                    }
                }
            }
        }
    }
}
