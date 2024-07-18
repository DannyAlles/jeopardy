using Microsoft.AspNetCore.SignalR;
using System.Text.Json;
using System.Collections.Concurrent;

using Data.Repositories;
using Data.Models;
using Domain.Services;
using Api.Hubs.ViewModels;
using SignalRSwaggerGen.Attributes;
using EntityFramework.DbContextScope.Interfaces;    
using Data;
using Microsoft.Extensions.Options;

namespace Api.Hubs
{
    /// <summary>
    /// Хаб для работы с логикой в играх
    /// </summary>
    [SignalRHub]
    public class GameSessionHub : Hub, IGameStateObserver
    {
        private readonly ITeamService _teamService;
        private readonly IPackageService _packageService;
        private readonly IProfessorService _professorService;
        private readonly IQuestionService _questionService;
        private readonly ISessionService _sessionService;
        private readonly IQuestionOfPackageService _questionOfPackageService;
        private readonly IThemeRepository _themeRepository;
        private readonly IHubContext<GameSessionHub> _hubContext;
        private readonly IAuthenticationService _authService;
        private readonly IDbContextScopeFactory _dbContextScopeFactory;

        private readonly static ConcurrentDictionary<string, HubSessionView> _gameLogics = new();

        public GameSessionHub(
            ITeamService teamService,
            IPackageService packageService,
            IProfessorService professorService,
            IQuestionService questionService,
            ISessionService sessionService,
            IQuestionOfPackageService questionOfPackageService,
            IThemeRepository themeRepository,
            IHubContext<GameSessionHub> hubContext,
            IAuthenticationService authService,
            IDbContextScopeFactory dbContextScopeFactory)
        {
            _teamService                = teamService;
            _packageService             = packageService;
            _professorService           = professorService;
            _questionService            = questionService;
            _sessionService             = sessionService;
            _questionOfPackageService   = questionOfPackageService;
            _themeRepository            = themeRepository;
            _hubContext                 = hubContext;
            _authService                = authService;
            _dbContextScopeFactory      = dbContextScopeFactory;
        }


        async Task IGameStateObserver.OnError(GameLogic gameState, string? message) 
        {
            var sessionId   = _gameLogics.FirstOrDefault(s => s.Value.Logic == gameState).Key.ToString();
            var session     = _gameLogics[sessionId];
            if (session == null)
            {
                Console.WriteLine("Hub OnError error: Invalid gameState parameter");
                return;
            }

            var gameStateJson = JsonSerializer.Serialize((GameSessionDto)session);

            await _hubContext.Clients.Group(sessionId)
                .SendAsync("DisplayErrorMessage", message);
            await _hubContext.Clients.Group(sessionId)
                .SendAsync("OnPanic", gameStateJson);
        }

        async Task IGameStateObserver.OnGameStateChanged(GameLogic gameState, GameLogicMessageType message, string? playerId) 
        {         
            string sessionId = _gameLogics.FirstOrDefault(s => s.Value.Logic == gameState).Key.ToString();
            var session = _gameLogics[sessionId];
            if (session == null)
            {
                Console.WriteLine("Hub OnGameStateChanged error: Invalid gameState parameter");
                return;
            }

            string gameStateJson = JsonSerializer.Serialize((GameSessionDto)session);

            switch (message)
            {
                case GameLogicMessageType.Panic:
                    await _hubContext.Clients.Group(sessionId)
                        .SendAsync("OnPanic", gameStateJson);
                    break;

                case GameLogicMessageType.PlayerJoined:                   
                    await _hubContext.Clients.Group(sessionId)
                        .SendAsync("OnPlayerJoined", gameStateJson, playerId);
                    break;

                case GameLogicMessageType.PlayerDisabled:
                    await _hubContext.Clients.Group(sessionId)
                        .SendAsync("OnPlayerDisabled", gameStateJson, playerId);
                    break;

                case GameLogicMessageType.PlayerEnabled:
                    await _hubContext.Clients.Group(sessionId)
                        .SendAsync("OnPlayerEnabled", gameStateJson, playerId);
                    break;

                case GameLogicMessageType.PlayerScoreManuallyChanged:                   
                    try
                    {
                        var team    = session.Players.FirstOrDefault(p => p.InGameId == playerId);
                        Guid teamId = Guid.Parse(team.DbId);

                        using (var dbContextScope = _dbContextScopeFactory.Create())
                        {
                            var dbContext   = dbContextScope.DbContexts.Get<JeopardyContext>();
                            var oldTeam     = dbContext.Teams.FirstOrDefault(x => x.Id == teamId);
                            oldTeam.Score   = session.Logic.Players[playerId].GameScore;
                            dbContext.Teams.Update(oldTeam);
                            await dbContext.SaveChangesAsync().ConfigureAwait(false);
                        }

                        await _hubContext.Clients.Group(sessionId)
                            .SendAsync("OnPlayerScoreChanged", gameStateJson, playerId);
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine("Hub PlayerScoreManuallyChanged exception: " + ex);

                        await _hubContext.Clients.Group(sessionId)
                            .SendAsync("DisplayErrorMessage", "Something went wrong on server");

                        await _hubContext.Clients.Group(sessionId)
                            .SendAsync("OnPanic", gameStateJson);
                    }
                    break;

                case GameLogicMessageType.PlayerHandRaised:
                    await _hubContext.Clients.Group(sessionId)
                        .SendAsync("OnPlayerHandRaised", gameStateJson, playerId);
                    break;

                case GameLogicMessageType.PlayerHandUnraised:
                    await _hubContext.Clients.Group(sessionId)
                        .SendAsync("OnPlayerHandUnraised", gameStateJson, playerId);
                    break;

                case GameLogicMessageType.GameStarted:
                    await _hubContext.Clients.Group(sessionId)
                        .SendAsync("OnGameStart", gameStateJson);
                    break;

                case GameLogicMessageType.GamePaused:
                    await _hubContext.Clients.Group(sessionId)
                        .SendAsync("OnGamePaused", gameStateJson);
                    break;

                case GameLogicMessageType.GameUnpaused:
                    await _hubContext.Clients.Group(sessionId)
                        .SendAsync("OnGameUnpaused", gameStateJson);
                    break;

                case GameLogicMessageType.GameFinished:
                    await _hubContext.Clients.Group(sessionId)
                        .SendAsync("OnGameFinished", gameStateJson);
                    break;

                case GameLogicMessageType.GameQuestionChosen:
                    try
                    {
                        var questionId = Guid.Parse(gameState.CurrentQuestionId);
                        Question? question;

                        using (var dbContextScope = _dbContextScopeFactory.Create())
                        {
                            var dbContext   = dbContextScope.DbContexts.Get<JeopardyContext>();
                            question        = dbContext.Questions.FirstOrDefault(x => x.Id == questionId);
                        }
                        
                        string questionText = question.QuestionText;
                        string answertext   = question.AnswerText;

                        await _hubContext.Clients.Group(sessionId)
                            .SendAsync("OnQuestionDisplay", gameStateJson, questionText);
                        await _hubContext.Clients.Client(session.Prof.SignalRConnectionId)
                            .SendAsync("OnShowAnswerToProf", gameStateJson, answertext);
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine("Hub GameQuestionChosen exception: " + ex);

                        await _hubContext.Clients.Group(sessionId)
                            .SendAsync("DisplayErrorMessage", "Something went wrong on server");

                        _gameLogics[sessionId].Logic.RejectAnswer();
                    }
                    break;

                case GameLogicMessageType.GameAnswerGiven:
                    await _hubContext.Clients.Group(sessionId)
                        .SendAsync("OnAnswerGiven", gameStateJson, playerId, session.Answer);
                    break;

                case GameLogicMessageType.GameAnswerAccepted:  
                    try
                    {
                        var team    = session.Players.FirstOrDefault(p => p.InGameId == playerId);
                        Guid teamId = Guid.Parse(team.DbId);

                        using (var dbContextScope = _dbContextScopeFactory.Create())
                        {
                            var dbContext   = dbContextScope.DbContexts.Get<JeopardyContext>();
                            var oldTeam     = dbContext.Teams.FirstOrDefault(x => x.Id == teamId);
                            oldTeam.Score   = session.Logic.Players[playerId].GameScore;
                            dbContext.Teams.Update(oldTeam);
                            await dbContext.SaveChangesAsync().ConfigureAwait(false);
                        }

                        await _hubContext.Clients.Group(sessionId)
                            .SendAsync("OnAnswerAccepted", gameStateJson, playerId);
                    }
                    catch (Exception ex)
                    {
                        await _hubContext.Clients.Group(sessionId)
                            .SendAsync("DisplayErrorMessage", "Something went wrong on server");

                        await _hubContext.Clients.Group(sessionId)
                            .SendAsync("OnPanic", gameStateJson);
                    }
                    break;

                case GameLogicMessageType.GameAnswerRejected:
                    try
                    {
                        var team = session.Players.FirstOrDefault(p => p.InGameId == playerId);
                        Guid teamId = Guid.Parse(team.DbId);

                        using (var dbContextScope = _dbContextScopeFactory.Create())
                        {
                            var dbContext   = dbContextScope.DbContexts.Get<JeopardyContext>();
                            var oldTeam     = dbContext.Teams.FirstOrDefault(x => x.Id == teamId);
                            oldTeam.Score   = session.Logic.Players[playerId].GameScore;
                            dbContext.Teams.Update(oldTeam);
                            await dbContext.SaveChangesAsync().ConfigureAwait(false);
                        }

                        await _hubContext.Clients.Group(sessionId)
                            .SendAsync("OnAnswerRejected", gameStateJson, playerId);
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine("Hub GameAnswerAccepted exception: " + ex);

                        await _hubContext.Clients.Group(sessionId)
                            .SendAsync("DisplayErrorMessage", "Something went wrong on server");

                        await _hubContext.Clients.Group(sessionId)
                            .SendAsync("OnPanic", gameStateJson);
                    }
                    break;

                case GameLogicMessageType.GameTurnPassed:
                    await _hubContext.Clients.Group(sessionId)
                        .SendAsync("OnTurnPassed", gameStateJson, playerId);
                    break;

                case GameLogicMessageType.TimerExpired:
                    await _hubContext.Clients.Group(sessionId)
                        .SendAsync("OnTimerExpired", gameStateJson, playerId);
                    break;

                case GameLogicMessageType.TimerManuallyChanged:
                    await _hubContext.Clients.Group(sessionId)
                        .SendAsync("OnTimerChanged", gameStateJson);
                    break;

                case GameLogicMessageType.TimerUpdate:
                    await _hubContext.Clients.Group(sessionId)
                        .SendAsync("OnTimerUpdate", gameStateJson);
                    break;

                default:
                    await _hubContext.Clients.Group(sessionId)
                        .SendAsync("OnPanic", gameStateJson);
                    break;
            }
        }


        /// <summary>
        /// Обработчик события когда игрок тупо отключается от сервера и висит мертвой душой
        /// </summary>
        /// <param name="exception"></param>
        public override async Task OnDisconnectedAsync(Exception? exception)
        {
            await base.OnDisconnectedAsync(exception);
        }


        /// <summary>
        /// Обработчик события когда игрок подключился. В первую очередь на случай если он переподключится
        /// </summary>
        public override async Task OnConnectedAsync()
        {
            Console.WriteLine("Client connected to hub:\n\tConnectionID = " + Context.ConnectionId);
            await base.OnConnectedAsync();
        }

        /// <summary>
        /// Создать новую игровую сессию
        /// </summary>
        /// <param name="packageId">Идентификатор пака вопросов</param>
        /// <param name="lobbyCapacity">Вместимость лобби</param>
        [Authorize]
        [HubMethodName("ProfCreateNewGame")]
        public async Task CreateNewGame(string packageId, int lobbyCapacity)
        {
            try {
                string jwt      = Context.GetHttpContext().Request.Query["access_token"];
                string profId   = _authService.ValidateToken(jwt).ToString();
                
                var prof = await _professorService.GetById(Guid.Parse(profId));
                var pack = await _packageService.GetPackage(Guid.Parse(packageId));

                if (prof == null)
                {
                    await Clients.Caller
                        .SendAsync("DisplayErrorMessage", "Invalid professor ID");
                    return;
                }
                if (pack == null)
                {
                    await Clients.Caller
                        .SendAsync("DisplayErrorMessage", "Invalid pack ID");
                    return;
                }

                var ses = await _sessionService.CreateSession(new Session
                {
                    PackageId = pack.Id,
                    ProfessorId = prof.Id,
                    Teams = new List<Team>()
                }, prof);

                if (ses == null)
                {
                    await Clients.Caller.SendAsync("DisplayErrorMessage", "Couldn't create session");
                    return;
                }

                var questionofpack      = await _questionOfPackageService.GetQuestionOfPackageList(pack.Id);
                var gameThemes          = new Dictionary<int, GameLogicThemeView>();

                // TODO: Привести в порядок, я ваще потерял нить повествования в этом цикле
                foreach (var theme in questionofpack.GroupBy(q => q.Y).OrderBy(g => g.Key))
                {
                    var c = await _questionService.GetQuestion(theme.First().QuestionId).ConfigureAwait(false);
                    var t = await _themeRepository.GetThemeById(c.ThemeId).ConfigureAwait(false);

                    var gameTheme = new GameLogicThemeView
                    {
                        ThemeId = theme.First().PackageId.ToString(),
                        ThemeName = t.Title, // pack.Title
                        Questions = new Dictionary<int, GameLogicQuestionView>()
                    };

                    foreach (var question in theme) //.OrderBy(q => q.X))
                    {
                        var q = await _questionService.GetQuestion(question.QuestionId).ConfigureAwait(false);

                        var gameQuestion = new GameLogicQuestionView
                        {
                            QuestionId      = question.QuestionId.ToString(),
                            QuestionCost    = (question.X + 1) * 100,
                            TimeControl     = 30,
                            IsAvaliable     = true
                        };

                        gameTheme.Questions.Add(question.X, gameQuestion);
                    }

                    gameTheme.Questions = gameTheme.Questions.OrderBy(x => x.Value.QuestionCost).ToDictionary(x=> x.Key, x=> x.Value);

                    gameThemes.Add(theme.Key, gameTheme);
                }

                HubSessionView lobby = new()
                {
                    DbId = ses.Id.ToString(),
                    Logic = new()
                    {
                        GameThemes = gameThemes
                    },
                    LobbyCapacity = lobbyCapacity,
                    Prof = new()
                    {
                        DbId                = profId,
                        SignalRConnectionId = Context.ConnectionId,
                        Name                = prof.FIO
                    },
                    Players = new()
                };

                lobby.Logic.AddObserver(this);
                _gameLogics.TryAdd(ses.Id.ToString(), lobby);

                string gameStateJson = JsonSerializer.Serialize((GameSessionDto)lobby);

                await Clients.Caller.SendAsync("OnGameCreated", gameStateJson);
                await _hubContext.Groups.AddToGroupAsync(Context.ConnectionId, ses.Id.ToString());
            }
            catch (Exception ex)
            {
                await Clients.Caller
                    .SendAsync("DisplayErrorMessage", ex.Message);
            }
        }


        /// <summary>
        /// Удаляет игровую сессию
        /// </summary>
        /// <param name="gameSessionId">Идентификатор игровой сессии</param>
        [Authorize]
        [HubMethodName("ProfNukeLobby")]
        public async Task NukeGame(string gameSessionId)
        {
            if (!_gameLogics.TryGetValue(gameSessionId, out var session))
            {
                await Clients.Caller
                    .SendAsync("DisplayErrorMessage", "Game session not found");
                return;
            }
            if (Context.ConnectionId != session.Prof.SignalRConnectionId)
            {
                await Clients.Caller
                    .SendAsync("DisplayErrorMessage", "You are not a professor of this session");
                return;
            }

            string gameStateJson = JsonSerializer.Serialize((GameSessionDto)session);

            await _hubContext.Clients.Group(gameSessionId)
                .SendAsync("OnGameFinished", gameStateJson);

            foreach (var p in session.Players)
            {
                await _hubContext.Groups
                    .RemoveFromGroupAsync(gameSessionId, p.SignalRConnectionId);
            }

            _gameLogics.TryRemove(gameSessionId, out var removedSession);

            // TODO: Удалить сессию из бд
        }

        /// <summary>
        /// Подключает ученика к существующей игровой комнате
        /// </summary>
        /// <param name="gameSessionId">Идентификатор игровой сессии</param>
        /// <param name="teamEntry">Название команды и массив участников</param>
        [HubMethodName("JoinGame")]
        public async Task JoinGameSession(string gameSessionId, HubTeamParameterView teamEntry)
        {
            try
            {
                if (!_gameLogics.ContainsKey(gameSessionId))
                {
                    await Clients.Caller
                        .SendAsync("DisplayErrorMessage", "Game session not found");
                    return;
                }
                if (_gameLogics[gameSessionId].Players.Count() >= _gameLogics[gameSessionId].LobbyCapacity)
                {
                    await Clients.Caller
                        .SendAsync("DisplayErrorMessage", "Game session is at capacity");
                    return;
                }
                if (_gameLogics[gameSessionId].Players.Any(p => p.TeamName == teamEntry.Title))
                {
                    await Clients.Caller
                        .SendAsync("DisplayErrorMessage", "Name already taken");
                    return;
                }

                List<Member> dbMembers = new();

                var dbTeamEntry = await _teamService.CreateTeam(new()
                {
                    SessionId   = Guid.Parse(gameSessionId),
                    Score       = 0,
                    Title       = teamEntry.Title,
                    Members     = dbMembers
                });

                foreach (var memberName in teamEntry.Members)
                {
                    dbMembers.Add(new()
                    {
                        FIO     = memberName,
                        Team    = dbTeamEntry,
                        TeamId  = dbTeamEntry.Id
                    });
                }

                dbTeamEntry.Members = dbMembers;
                await _teamService.UpdateTeam(dbTeamEntry);

                await _hubContext.Groups.AddToGroupAsync(Context.ConnectionId, gameSessionId);

                HubPlayerView hubPlayerEntry = new()
                {
                    InGameId            = Guid.NewGuid().ToString(),
                    DbId                = dbTeamEntry.Id.ToString(),
                    SignalRConnectionId = Context.ConnectionId,
                    TeamName            = teamEntry.Title,
                    TeamMembers         = teamEntry.Members.ToList()
                };

                _gameLogics[gameSessionId].Players.Add(hubPlayerEntry);
                _gameLogics[gameSessionId].Logic.AddPlayer(hubPlayerEntry.InGameId);
            }
            catch (Exception ex)
            {
                await Clients.Caller
                    .SendAsync("DisplayErrorMessage", ex.Message);
                return;
            }
        }


        /// <summary>
        /// Отключает ученика от существующей комнаты и удаляет информацию о нем из сессии
        /// </summary>
        /// <param name="gameSessionId">Идентификатор игровой сессии</param>
        [HubMethodName("LeaveGame")]
        public async Task LeaveGameSession(string gameSessionId)
        {
            if (!_gameLogics.ContainsKey(gameSessionId))
            {
                await Clients.Caller
                    .SendAsync("DisplayErrorMessage", "Game session not found");
                return;
            }
            
            HubPlayerView? player = _gameLogics[gameSessionId].Players
                .FirstOrDefault(p => p.SignalRConnectionId == Context.ConnectionId);
            
            if (player == null)
            {
                await Clients.Caller
                    .SendAsync("DisplayErrorMessage", "You are not a member of this game session");
                return;
            }

            _gameLogics[gameSessionId].Logic.DisablePlayer(player.InGameId);
            await _hubContext.Groups
                .RemoveFromGroupAsync(Context.ConnectionId, gameSessionId);
        }


        /// <summary>
        /// Отправка сообщения в чат игроком
        /// </summary>
        /// <param name="gameSessionId">Идентификатор игровой сессии</param>
        /// <param name="message">Текст сообщения</param>
        [HubMethodName("Chat")]
        public async Task Chat(string gameSessionId, string message)
        {
            if (!_gameLogics.TryGetValue(gameSessionId, out var gameLogic))
            {
                await Clients.Caller.SendAsync("DisplayErrorMessage", "Game session not found");
                return;
            }

            HubProfessorView prof = _gameLogics[gameSessionId].Prof;

            if (prof.SignalRConnectionId == Context.ConnectionId)
            {
                await _hubContext.Clients.Groups(gameSessionId)
                    .SendAsync("OnPlayerChat", prof.Name, message);
                return;
            }

            HubPlayerView? player = _gameLogics[gameSessionId].Players
                .FirstOrDefault(p => p.SignalRConnectionId == Context.ConnectionId);

            if (player != null)
            {
                await _hubContext.Clients.Groups(gameSessionId)
                    .SendAsync("OnPlayerChat", player.TeamName, message);
                return;
            }

            await Clients.Caller
                .SendAsync("DisplayErrorMessage", "You are not a member of this session");
        }


        /// <summary>
        /// Начать игру
        /// </summary>
        /// <param name="gameSessionId">Идентификатор игровой сессии</param>
        [Authorize]
        [HubMethodName("ProfStartGame")]
        public async Task StartGame(string gameSessionId)
        {
            if (!_gameLogics.TryGetValue(gameSessionId, out var gameLogic))
            {
                await Clients.Caller
                    .SendAsync("DisplayErrorMessage", "Game session not found");
                return;
            }
            if (Context.ConnectionId != gameLogic.Prof.SignalRConnectionId)
            {
                await Clients.Caller
                    .SendAsync("DisplayErrorMessage", "You are not a professor of this session");
                return;
            }
            if (gameLogic.Players.Count == 0)
            {
                await Clients.Caller
                    .SendAsync("DisplayErrorMessage", "Too little players to start the game");
                return;
            }

            _gameLogics[gameSessionId].Logic.GameStart();
        }


        /// <summary>
        /// Выбрать вопрос из таблицы игроком
        /// </summary>
        /// <param name="gameSessionId">Идентификатор игровой сессии</param>
        /// <param name="questionId">Идентификатор выбранного вопроса</param>
        [HubMethodName("StudChooseQuestion")]
        public async Task PlayerChooseQuestion(string gameSessionId, string questionId)
        {        
            if (!_gameLogics.TryGetValue(gameSessionId, out var gameLogic))
            {
                await Clients.Caller
                    .SendAsync("DisplayErrorMessage", "Game session not found");
                return;
            }

            HubPlayerView? player = _gameLogics[gameSessionId].Players
                .FirstOrDefault(p => p.SignalRConnectionId == Context.ConnectionId);

            if (player == null)
            {
                await Clients.Caller
                    .SendAsync("DisplayErrorMessage", "You are not a part of this game");
                return;
            }
            if (gameLogic.Logic.LogicStage != GameLogic.GameLogicStage.ChoosingQuestion)
            {
                await Clients.Caller
                    .SendAsync("DisplayErrorMessage", "Wrong game stage");
                return;
            }
            if (player.InGameId != gameLogic.Logic.ChoosingPlayerId)
            {
                await Clients.Caller
                    .SendAsync("DisplayErrorMessage", "Other player is choosing the question");
                return;
            }
            if (gameLogic.Logic.FlowState != GameLogic.GameLogicFlow.InProgress)
            {
                await Clients.Caller
                    .SendAsync("DisplayErrorMessage", "Game paused");
                return;
            }

            _gameLogics[gameSessionId].Logic.ChooseQuestion(questionId);
        }


        /// <summary>
        /// Выбрать вопрос из таблицы преподавателем
        /// </summary>
        /// <param name="gameSessionId">Идентификатор игровой сессии</param>
        /// <param name="questionId">Идентификатор выбранного вопроса</param>
        [Authorize]
        [HubMethodName("ProfChooseQuestion")]
        public async Task ProfessorChooseQuestion(string gameSessionId, string questionId)
        {
            if (!_gameLogics.TryGetValue(gameSessionId, out var gameLogic))
            {
                await Clients.Caller
                    .SendAsync("DisplayErrorMessage", "Game session not found");
                return;
            }
            if (Context.ConnectionId != gameLogic.Prof.SignalRConnectionId)
            {
                await Clients.Caller
                    .SendAsync("DisplayErrorMessage", "You are not a part of this game");
                return;
            }
            if (gameLogic.Logic.LogicStage != GameLogic.GameLogicStage.ChoosingQuestion)
            {
                await Clients.Caller
                    .SendAsync("DisplayErrorMessage", "Wrong game stage");
                return;
            }
            if (gameLogic.Logic.FlowState != GameLogic.GameLogicFlow.InProgress)
            {
                await Clients.Caller
                    .SendAsync("DisplayErrorMessage", "Game paused");
                return;
            }

            _gameLogics[gameSessionId].Logic.ChooseQuestion(questionId);
        }


        /// <summary>
        /// Выводит подсказку на вопрос всем участникам сессии
        /// </summary>
        /// <param name="gameSessionId">Идентификатор игровой сессии</param>
        [Authorize]
        [HubMethodName("DisplayHint")]
        public async Task DisplayHint(string gameSessionId)
        {
            if (!_gameLogics.TryGetValue(gameSessionId, out var gameLogic))
            {
                await Clients.Caller
                    .SendAsync("DisplayErrorMessage", "Game session not found");
                return;
            }
            if (Context.ConnectionId != gameLogic.Prof.SignalRConnectionId)
            {
                await Clients.Caller
                    .SendAsync("DisplayErrorMessage", "You are not a professor of this game");
                return;
            }
            if (gameLogic.Logic.LogicStage != GameLogic.GameLogicStage.WaitingForAnswer)
            {
                await Clients.Caller
                    .SendAsync("DisplayErrorMessage", "Wrong game stage");
                return;
            }
            if (gameLogic.Logic.FlowState != GameLogic.GameLogicFlow.InProgress)
            {
                await Clients.Caller
                    .SendAsync("DisplayErrorMessage", "Game paused");
                return;
            }

            try
            {
                Guid quid = Guid.Parse(gameLogic.Logic.CurrentQuestionId);
                Question? question;

                using (var dbContextScope = _dbContextScopeFactory.Create())
                {
                    var dbContext = dbContextScope.DbContexts.Get<JeopardyContext>();
                    question = dbContext.Questions.FirstOrDefault(x => x.Id == quid);
                }

                if (question == null)
                {
                    await Clients.Caller.SendAsync("DisplayErrorMessage", "Invalid question ID");
                }

                await _hubContext.Clients.Group(gameSessionId)
                    .SendAsync("HintDisplayed", question.Hint);
            }
            catch(Exception ex)
            {
                await Clients.Caller
                    .SendAsync("DisplayErrorMessage", ex.Message);
            }        
        }


        /// <summary>
        /// Претендовать на ответ
        /// </summary>
        /// <param name="gameSessionId">Идентификатор игровой сессии</param>
        [HubMethodName("RaiseHand")]
        public async Task RaiseHand(string gameSessionId)
        {
            if (!_gameLogics.TryGetValue(gameSessionId, out var gameLogic))
            {
                await Clients.Caller
                    .SendAsync("DisplayErrorMessage", "Game session not found");
                return;
            }

            HubPlayerView? player = _gameLogics[gameSessionId].Players
                .FirstOrDefault(p => p.SignalRConnectionId == Context.ConnectionId);

            if (player == null)
            {
                await Clients.Caller
                    .SendAsync("DisplayErrorMessage", "You are not a part of this game");
                return;
            }
            if (gameLogic.Logic.LogicStage != GameLogic.GameLogicStage.WaitingForAnswer)
            {
                await Clients.Caller
                    .SendAsync("DisplayErrorMessage", "Wrong game stage");
                return;
            }           
            if (gameLogic.Logic.FlowState != GameLogic.GameLogicFlow.InProgress)
            {
                await Clients.Caller
                    .SendAsync("DisplayErrorMessage", "Game paused");
                return;
            }

            _gameLogics[gameSessionId].Logic.RaiseHand(player.InGameId);
        }

        /// <summary>
        /// Перестать претендовать на ответ
        /// </summary>
        /// <param name="gameSessionId">Идентификатор игровой сессии</param>
        [HubMethodName("UnraiseHand")]
        public async Task UnraiseHand(string gameSessionId)
        {
            if (!_gameLogics.TryGetValue(gameSessionId, out var gameLogic))
            {
                await Clients.Caller
                    .SendAsync("DisplayErrorMessage", "Game session not found");
                return;
            }

            HubPlayerView? player = _gameLogics[gameSessionId].Players
                .FirstOrDefault(p => p.SignalRConnectionId == Context.ConnectionId);

            if (player == null)
            {
                await Clients.Caller
                    .SendAsync("DisplayErrorMessage", "You are not a part of this game");
                return;
            }
            if (gameLogic.Logic.LogicStage != GameLogic.GameLogicStage.WaitingForAnswer)
            {
                await Clients.Caller
                    .SendAsync("DisplayErrorMessage", "Wrong game stage");
                return;
            }
            if (gameLogic.Logic.FlowState != GameLogic.GameLogicFlow.InProgress)
            {
                await Clients.Caller
                    .SendAsync("DisplayErrorMessage", "Game paused");
                return;
            }

            _gameLogics[gameSessionId].Logic.UnraiseHand(player.InGameId);
        }

        /// <summary>
        /// Отправить ответ на вопрос
        /// </summary>
        /// <param name="gameSessionId">Идентификатор игровой сессии</param>
        /// <param name="answer">Идентификатор выбранного вопроса</param>
        [HubMethodName("SendAnswer")]
        public async Task SendAnswer(string gameSessionId, string answer)
        {
            if (!_gameLogics.TryGetValue(gameSessionId, out var gameLogic))
            {
                await Clients.Caller
                    .SendAsync("DisplayErrorMessage", "Game session not found");
                return;
            }

            HubPlayerView? player = _gameLogics[gameSessionId].Players
                .FirstOrDefault(p => p.SignalRConnectionId == Context.ConnectionId);

            if (player == null)
            {
                await Clients.Caller
                    .SendAsync("DisplayErrorMessage", "You are not a part of this game");
                return;
            }
            if (gameLogic.Logic.LogicStage != GameLogic.GameLogicStage.WaitingForAnswer)
            {
                await Clients.Caller
                    .SendAsync("DisplayErrorMessage", "Wrong game stage");
                return;
            }
            if (gameLogic.Logic.AnsweringPlayerId != player.InGameId)
            {
                await Clients.Caller
                    .SendAsync("DisplayErrorMessage", "Other player is giving answer");
                return;
            }
            if (gameLogic.Logic.FlowState != GameLogic.GameLogicFlow.InProgress)
            {
                await Clients.Caller
                    .SendAsync("DisplayErrorMessage", "Game paused");
                return;
            }

            _gameLogics[gameSessionId].Answer = answer;
            _gameLogics[gameSessionId].Logic.GetAnswer();

            await _hubContext.Clients.Group(gameSessionId)
                .SendAsync("AnswerGiven", player.InGameId, answer);
        }


        /// <summary>
        /// Принять ответ на вопрос
        /// </summary>
        /// <param name="gameSessionId">Идентификатор сесси</param>
        [Authorize]
        [HubMethodName("ProfAcceptAnswer")]
        public async Task AcceptAnswer(string gameSessionId)
        {
            if (!_gameLogics.TryGetValue(gameSessionId, out var gameLogic))
            {
                await Clients.Caller
                    .SendAsync("DisplayErrorMessage", "Game session not found");
                return;
            }
            if (Context.ConnectionId != gameLogic.Prof.SignalRConnectionId)
            {
                await Clients.Caller
                    .SendAsync("DisplayErrorMessage", "You are not a part of this game");
                return;
            }
            if (gameLogic.Logic.LogicStage != GameLogic.GameLogicStage.WaitingForProfEvaluation)
            {
                await Clients.Caller
                    .SendAsync("DisplayErrorMessage", "Wrong game stage");
                return;
            }
            if (gameLogic.Logic.FlowState != GameLogic.GameLogicFlow.InProgress)
            {
                await Clients.Caller
                    .SendAsync("DisplayErrorMessage", "Game paused");
                return;
            }

            _gameLogics[gameSessionId].Logic.AcceptAnswer();
        }


        /// <summary>
        /// Отклонить ответ на вопрос
        /// </summary>
        /// <param name="gameSessionId">Идентификатор сесси</param>
        [Authorize]
        [HubMethodName("ProfRejectAnswer")]
        public async Task RejectAnswer(string gameSessionId)
        {
            if (!_gameLogics.TryGetValue(gameSessionId, out var gameLogic))
            {
                await Clients.Caller
                    .SendAsync("DisplayErrorMessage", "Game session not found");
                return;
            }
            if (Context.ConnectionId != gameLogic.Prof.SignalRConnectionId)
            {
                await Clients.Caller
                    .SendAsync("DisplayErrorMessage", "You are not a part of this game");
                return;
            }
            if (gameLogic.Logic.LogicStage != GameLogic.GameLogicStage.WaitingForProfEvaluation)
            {
                await Clients.Caller
                    .SendAsync("DisplayErrorMessage", "Wrong game stage");
                return;
            }
            if (gameLogic.Logic.FlowState != GameLogic.GameLogicFlow.InProgress)
            {
                await Clients.Caller
                    .SendAsync("DisplayErrorMessage", "Game paused");
                return;
            }

            _gameLogics[gameSessionId].Logic.RejectAnswer();
        }


        /// <summary>
        /// Преподаватель передает право ответа другому игроку
        /// </summary>
        /// <param name="gameSessionId">Идентификатор сесси</param>
        /// <param name="playerId">Идентификатор целевого игрока</param>
        [Authorize]
        [HubMethodName("ProfPassTurn")]
        public async Task ProfPassTurn(string gameSessionId, string playerId)
        {
            if (!_gameLogics.TryGetValue(gameSessionId, out var gameLogic))
            {
                await Clients.Caller
                    .SendAsync("DisplayErrorMessage", "Game session not found");
                return;
            }
            if (Context.ConnectionId != gameLogic.Prof.SignalRConnectionId)
            {
                await Clients.Caller
                    .SendAsync("DisplayErrorMessage", "You are not a part of this game");
                return;
            }
            if (!gameLogic.Players.Any(p => p.InGameId == playerId))
            {
                await Clients.Caller
                    .SendAsync("DisplayErrorMessage", "Invalid player ID");
                return;
            }
            if (gameLogic.Logic.LogicStage != GameLogic.GameLogicStage.WaitingForProfChoosingPlayer)
            {
                await Clients.Caller
                    .SendAsync("DisplayErrorMessage", "Wrong game stage");
                return;
            }
            if (gameLogic.Logic.FlowState != GameLogic.GameLogicFlow.InProgress)
            {
                await Clients.Caller
                    .SendAsync("DisplayErrorMessage", "Game paused");
                return;
            }

            _gameLogics[gameSessionId].Logic.ChoosePlayerToAnswer(playerId);
        }


        /// <summary>
        /// Препод выкидывает игрока из сессии
        /// </summary>
        /// <param name="gameSessionId">Идентификатор сесси</param>
        /// <param name="playerId">Идентификатор игрока</param>
        [Authorize]
        [HubMethodName("ProfKickPlayer")]
        public async Task KickPlayer(string gameSessionId, string playerId)
        {
            if (!_gameLogics.TryGetValue(gameSessionId, out var gameLogic))
            {
                await Clients.Caller
                    .SendAsync("DisplayErrorMessage", "Game session not found");
                return;
            }
            if (Context.ConnectionId != gameLogic.Prof.SignalRConnectionId)
            {
                await Clients.Caller
                    .SendAsync("DisplayErrorMessage", "You are not a part of this game");
                return;
            }
            if (!gameLogic.Players.Any(p => p.InGameId == playerId))
            {
                await Clients.Caller
                    .SendAsync("DisplayErrorMessage", "Invalid player ID");
                return;
            }

            _gameLogics[gameSessionId].Logic.DisablePlayer(playerId);
        }


        /// <summary>
        /// Изменить счет игрока
        /// </summary>
        /// <param name="gameSessionId">Идентификатор игровой сессии</param>
        /// <param name="playerId">Идентификатор целевога игрока</param>
        /// <param name="value">Новое значение</param>
        [Authorize]
        [HubMethodName("ProfChangeScore")]
        public async Task ChangeScore(string gameSessionId, string playerId, int value)
        {
            if (!_gameLogics.TryGetValue(gameSessionId, out var gameLogic))
            {
                await Clients.Caller
                    .SendAsync("DisplayErrorMessage", "Game session not found");
                return;
            }
            if (Context.ConnectionId != gameLogic.Prof.SignalRConnectionId)
            {
                await Clients.Caller
                    .SendAsync("DisplayErrorMessage", "You are not a part of this game");
                return;
            }
            if (!gameLogic.Players.Any(p => p.InGameId == playerId))
            {
                await Clients.Caller
                    .SendAsync("DisplayErrorMessage", "Invalid player ID");
                return;
            }

            _gameLogics[gameSessionId].Logic.ChangePlayerScore(playerId, value);
        }


        /// <summary>
        /// Добавить время к таймеру
        /// </summary>
        /// <param name="gameSessionId">Идентификатор игровой сессии</param>
        /// <param name="value">Добавленное время в секундах</param>
        [Authorize]
        [HubMethodName("ProfAddTimeControl")]
        public async Task AddTimeControl(string gameSessionId, int value)
        {
            if (!_gameLogics.TryGetValue(gameSessionId, out var gameLogic))
            {
                await Clients.Caller
                    .SendAsync("DisplayErrorMessage", "Game session not found");
                return;
            }
            if (Context.ConnectionId != gameLogic.Prof.SignalRConnectionId)
            {
                await Clients.Caller
                    .SendAsync("DisplayErrorMessage", "You are not a part of this game");
                return;
            }
            if (gameLogic.Logic.LogicStage != GameLogic.GameLogicStage.WaitingForAnswer)
            {
                await Clients.Caller
                    .SendAsync("DisplayErrorMessage", "Wrong game stage");
                return;
            }

            _gameLogics[gameSessionId].Logic.AddTimeControl(value);
        }


        /// <summary>
        /// Поставить паузу в игре
        /// </summary>
        /// <param name="gameSessionId">Идентификатор игровой сессии</param>
        [Authorize]
        [HubMethodName("ProfPauseGame")]
        public async Task PauseGame(string gameSessionId)
        {
            if (!_gameLogics.TryGetValue(gameSessionId, out var gameLogic))
            {
                await Clients.Caller
                    .SendAsync("DisplayErrorMessage", "Game session not found");
                return;
            }
            if (Context.ConnectionId != gameLogic.Prof.SignalRConnectionId)
            {
                await Clients.Caller
                    .SendAsync("DisplayErrorMessage", "You are not a part of this game");
                return;
            }
            if (gameLogic.Logic.FlowState != GameLogic.GameLogicFlow.InProgress)
            {
                await Clients.Caller
                    .SendAsync("DisplayErrorMessage", "Game is not in progress");
                return;
            }

            _gameLogics[gameSessionId].Logic.Pause();
        }


        /// <summary>
        /// Убрать паузу в игре
        /// </summary>
        /// <param name="gameSessionId">Идентификатор игровой сессии</param>
        [Authorize]
        [HubMethodName("ProfUnpauseGame")]
        public async Task UnpauseGame(string gameSessionId)
        {
            if (!_gameLogics.TryGetValue(gameSessionId, out var gameLogic))
            {
                await Clients.Caller
                    .SendAsync("DisplayErrorMessage", "Game session not found");
                return;
            }
            if (Context.ConnectionId != gameLogic.Prof.SignalRConnectionId)
            {
                await Clients.Caller
                    .SendAsync("DisplayErrorMessage", "You are not a part of this game");
                return;
            }
            if (gameLogic.Logic.FlowState != GameLogic.GameLogicFlow.Paused)
            {
                await Clients.Caller
                    .SendAsync("DisplayErrorMessage", "Game is not paused");
                return;
            }

            _gameLogics[gameSessionId].Logic.Unpause();
        }
    }
}
