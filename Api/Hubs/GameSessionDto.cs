using Api.Hubs.ViewModels;
using Data.Migrations;
using Data.Models;
using static Api.Hubs.GameLogic;

namespace Api.Hubs
{
    public class GameSessionDto
    {
        public string Id { get; set; }
        public int LobbyCapacity { get; set; }
        public string ProfName { get; set; }
        public List<DtoPlayerView> Players { get; set; }

        public List<DtoTableThemeView> Themes { get; set; }

        public GameLogicFlow FlowState { get; set; }
        public GameLogicStage LogicStage { get; set; }

        public int TimeRemaining { get; set; }

        public string CurrentQuestionId { get; set; }
        public string AnsweringPlayerId { get; set; }
        public string ChoosingPlayerId { get; set; }


        public static explicit operator GameSessionDto(HubSessionView hubSessionView)
        {
            var session = new GameSessionDto
            {
                Id = hubSessionView.DbId,
                LobbyCapacity = hubSessionView.LobbyCapacity,
                ProfName = hubSessionView.Prof.Name,
                Players = hubSessionView.Players.Select(p => new DtoPlayerView
                {
                    Id = p.InGameId,
                    Name = p.TeamName,
                    IsActive = hubSessionView.Logic.Players[p.InGameId].IsActive,
                    GameScore = hubSessionView.Logic.Players[p.InGameId].GameScore,
                    RaisedHand = hubSessionView.Logic.Players[p.InGameId].RaisedHand,
                    GaveAnswer = hubSessionView.Logic.Players[p.InGameId].GaveAnswer
                }).ToList(),
                Themes = hubSessionView.Logic.GameThemes.Select(t => new DtoTableThemeView
                {
                    ThemeName = t.Value.ThemeName,
                    Questions = t.Value.Questions.Select(q => new DtoTableQuestionView
                    {
                        Id = q.Value.QuestionId,
                        Cost = q.Value.QuestionCost,
                        IsAvaliable = q.Value.IsAvaliable
                    }).ToList()
                }).ToList(),
                FlowState = hubSessionView.Logic.FlowState,
                LogicStage = hubSessionView.Logic.LogicStage,
                TimeRemaining = hubSessionView.Logic.GameTimeRemaining,
                CurrentQuestionId = hubSessionView.Logic.CurrentQuestionId,
                AnsweringPlayerId = hubSessionView.Logic.AnsweringPlayerId,
                ChoosingPlayerId = hubSessionView.Logic.ChoosingPlayerId
            };
            return session;
        }

        public class DtoTableThemeView
        {
            public string ThemeName { get; set; }
            public List<DtoTableQuestionView> Questions { get; set; }
        }

        public class DtoTableQuestionView
        {
            public string Id { get; set; }
            public int Cost { get; set; }
            public bool IsAvaliable { get; set; }
        }

        public class DtoPlayerView
        {
            public string Id { get; set; }
            public string Name { get; set; }
            public bool IsActive { get; set; }
            public int GameScore { get; set; }
            public bool RaisedHand { get; set; }
            public bool GaveAnswer { get; set; }
        }
    }  
}
