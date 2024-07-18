# Jeopardy

Хаб реализован через библиотеку SignalR. Соответственно, клиентская сторона (если сделана на JavaScript) выполняется в соответствии с [ихней документацией](https://learn.microsoft.com/en-us/aspnet/core/signalr/javascript-client?view=aspnetcore-7.0&tabs=visual-studio).

## Оглавление
- [События, отправляемые хабом](#события-отправляемые-хабом)
  + [OnGameCreated](#ongamecreated)
  + [DisplayErrorMessage](#displayerrormessage)
  + [OnPlayerChat](#onplayerchat)
  + [OnPanic](#onpanic)
  + [OnPlayerJoined](#onplayerjoined)
  + [OnPlayerDisabled](#onplayerdisabled)
  + [OnPlayerEnabled](#onplayerenabled)
  + [OnPlayerScoreChanged](#onplayerscorechanged)
  + [OnPlayerHandRaised](#onplayerhandraised)
  + [OnPlayerHandUnraised](#onplayerhandunraised)
  + [OnGameStart](#ongamestart)
  + [OnGamePaused](#ongamepaused)
  + [OnGameUnpaused](#ongameunpaused)
  + [OnGameFinished](#ongamefinished)
  + [OnQuestionDisplay](#onquestiondisplay)
  + [OnShowAnswerToProf](#onshowanswertoprof)
  + [OnAnswerGiven](#onanswergiven)
  + [OnAnswerAccepted](#onansweraccepted)
  + [OnAnswerRejected](#onanswerrejected)
  + [OnTurnPassed](#onturnpassed)
  + [OnTimerExpired](#ontimerexpired)
  + [OnTimerChanged](#ontimerchanged)
  + [OnTimerUpdate](#ontimerupdate)
- [Вызываемые методы хаба](#вызываемые-методы-хаба)
  + [ProfCreateNewGame](#profcreatenewgame)
  + [ProfNukeLobby](#profnukelobby)
  + [JoinGame](#joingame)
  + [LeaveGame](#leavegame)
  + [Chat](#chat)
  + [ProfStartGame](#profstartgame)
  + [StudChooseQuestion](#studchoosequestion)
  + [ProfChooseQuestion](#profchoosequestion)
  + [DisplayHint](#displayhint)
  + [RaiseHand](#raisehand)
  + [UnraiseHand](#unraisehand)
  + [SendAnswer](#sendanswer)
  + [ProfAcceptAnswer](#profacceptanswer)
  + [ProfAcceptAnswer](#profacceptanswer-1)
  + [ProfPassTurn](#profpassturn)
  + [ProfKickPlayer](#profkickplayer)
  + [ProfChangeScore](#profchangescore)
  + [ProfAddTimeControl](#profaddtimecontrol)
  + [ProfPauseGame](#profpausegame)
  + [ProfUnpauseGame](#profunpausegame)
- [Трансферные объекты](#трансферные-объекты)
  + [GameStateDto](#gamestatedto)
  + [Team](#team)


# События, отправляемые хабом
	
## OnGameCreated

Игра успешно создалась по запросу, перенаправьте препода на игровой экран

### Параметры

+ **gameStateJson** (string) - JSON с состоянием игровой сессии (см. [GameStateDto](#GameStateDto)). Оттуда можно извлечь айдишник.

## DisplayErrorMessage 

Что-то пошло по пизде, юзеру следует высветить какое-нибудь окошко типа соси жопу. Там таких дофига, на случай неправильных инпутов например. Возникает как при подключении (на стороне Никиты) так и в игре (на стороне Макса).

### Параметры

+ **message** (string) - текст сообщения об ошибке

## OnPlayerChat

Кто-то что-то написал в чат сессии

### Параметры

+ **teamName** (string) - имя игрока, отправившего сообщение
+ **message** (string) - текст сообщения

## OnPanic

Что-то пошло по пизде или я почему-то не выделил под произошедшее событие отдельное сообщение. При нем следует заново отрисовать окошко с игрой.

### Параметры

+ **gameStateJson** (string) - JSON с состоянием игровой сессии (см. [GameStateDto](#GameStateDto))

## OnPlayerJoined

### Параметры

+ **gameStateJson** (string) - JSON с состоянием игровой сессии (см. [GameStateDto](#GameStateDto))
+ **playerId** (string) - идентификатор подключившегося игрока чтоб вы проиграли какую-нибудь анимацию если хотите

## OnPlayerDisabled

У нас никто не умирает окончательно, пропавшие челы висят эфемерными духами. Закрасьте их серым наверное. Проверьте что вы не работаете с челом которого отключили, пошлите его нахер из сессии - при бане кого-то преподом отправляется это же сообщение

### Параметры

+ **gameStateJson** (string) - JSON с состоянием игровой сессии (см. [GameStateDto](#GameStateDto))
+ **playerId** (string) - идентификатор отключенного игрока чтоб вы проиграли какую-нибудь анимацию если хотите

## OnPlayerEnabled

Обратное тому что выше

### Параметры

+ **gameStateJson** (string) - JSON с состоянием игровой сессии (см. [GameStateDto](#GameStateDto))
+ **playerId** (string) - идентификатор вернувшегося игрока

## OnPlayerScoreChanged

Препод изменил счет какого-то игрока вручную

### Параметры

+ **gameStateJson** (string) - JSON с состоянием игровой сессии (см. [GameStateDto](#GameStateDto))
+ **playerId** (string) - идентификатор игрока которому изменили счет

## OnPlayerHandRaised

Поднятие руки у нас - тупо графический флаг, можете рамочку сделать какую-нибудь

### Параметры

+ **gameStateJson** (string) - JSON с состоянием игровой сессии (см. [GameStateDto](#GameStateDto))
+ **playerId** (string) - идентификатор поднявшего руку игрока

## OnPlayerHandUnraised

Обратное тому что выше

### Параметры

+ **gameStateJson** (string) - JSON с состоянием игровой сессии (см. [GameStateDto](#GameStateDto))
+ **playerId** (string) - идентификатор опустившего руку игрока

## OnGameStart

Препод нажал кнопку "погнали нахуй". Если всё норм то игра рандомно выбирает игрока и дает ему право выбрать вопрос из таблицы, рисуйте соответственно

### Параметры

+ **gameStateJson** (string) - JSON с состоянием игровой сессии (см. [GameStateDto](#GameStateDto))

## OnGamePaused

Препод приостановил игру

### Параметры

+ **gameStateJson** (string) - JSON с состоянием игровой сессии (см. [GameStateDto](#GameStateDto))

## OnGameUnpaused

Препод возобновил игру

### Параметры

+ **gameStateJson** (string) - JSON с состоянием игровой сессии (см. [GameStateDto](#GameStateDto))

## OnGameFinished

Игра завершилась. Перенаправьте на экран результатов.

### Параметры

+ **gameStateJson** (string) - JSON с состоянием игровой сессии (см. [GameStateDto](#GameStateDto))

## OnQuestionDisplay

Игрок (или препод) выбрал вопрос, его нужно вывести

### Параметры

+ **gameStateJson** (string) - JSON с состоянием игровой сессии (см. [GameStateDto](#GameStateDto))
+ **questionText** (string) - текст вопроса

## OnShowAnswerToProf

Отправляется преподу после текста вопроса. Это правильный ответ который выводится ему на экран

### Параметры

+ **gameStateJson** (string) - JSON с состоянием игровой сессии (см. [GameStateDto](#GameStateDto))
+ **answerText** (string) - текст правильного ответа

## OnAnswerGiven

Игрок дал ответ на вопрос, его нужно вывести

### Параметры

+ **gameStateJson** (string) - JSON с состоянием игровой сессии (см. [GameStateDto](#GameStateDto))
+ **playerId** (string) - идентификатор ответившего игрока
+ **answerText** (string) - текст ответа

## OnAnswerAccepted

Препод засчитал ответ на вопрос, проиграйте соответствующую анимацию и отрисуйте в соответствии с состоянием игры, оно может быть разным

### Параметры

+ **gameStateJson** (string) - JSON с состоянием игровой сессии (см. [GameStateDto](#GameStateDto))
+ **playerId** (string) - идентификатор ответившего игрока

## OnAnswerRejected

Препод не засчитал ответ на вопрос, проиграйте соответствующую анимацию и отрисуйте в соответствии с состоянием игры, оно может быть разным - или преподу нужно выбрать кому передать ход или игрок выбирает вопрос

### Параметры

+ **gameStateJson** (string) - JSON с состоянием игровой сессии (см. [GameStateDto](#GameStateDto))
+ **playerId** (string) - идентификатор не ответившего игрока

## OnTurnPassed

Препод передал право ответа другому игроку

### Параметры

+ **gameStateJson** (string) - JSON с состоянием игровой сессии (см. [GameStateDto](#GameStateDto))
+ **playerId** (string) - идентификатор нового отвечающего игрока

## OnTimerExpired

Таймер на ответ истек, вычитаются баллы и дальше то же самое что и если бы препод отклонил ответ

### Параметры

+ **gameStateJson** (string) - JSON с состоянием игровой сессии (см. [GameStateDto](#GameStateDto))
+ **playerId** (string) - идентификатор не ответившего игрока

## OnTimerChanged

Препод добавил/убавил время на таймере

### Параметры

+ **gameStateJson** (string) - JSON с состоянием игровой сессии (см. [GameStateDto](#GameStateDto))

## OnTimerUpdate

Таймер тикнул. Синхронизируйте полосочку если сделаете её. Вызывается каждую секунду когда игрок думает над ответом

### Параметры

+ **gameStateJson** (string) - JSON с состоянием игровой сессии (см. [GameStateDto](#GameStateDto))

# Вызываемые методы хаба

## ProfCreateNewGame

Создает игровую сессию. Может ответить сообщением об ошибке (см. [DisplayErrorMessage](#DisplayErrorMessage)) или вернуть GameStateDto (см. [OnGameCreated](#OnGameCreated)).

### Параметры

+ Требует JWT-токен
+ **packageID** (string) - идентификатор пака вопросов
+ **lobbyCapacity** (int) - вместимость лобби

## ProfNukeLobby

Завершает игру. Может ответить сообщением об ошибке (см. [DisplayErrorMessage](#DisplayErrorMessage)) или сообщением о завершении игры (см. [OnGameFinished](#OnGameFinished)).

### Параметры

+ Требует JWT-токен
+ **gameSessionID** (string) - идентификатор игровой сессии

## JoinGame

Пытается подключиться к существующей игре. Может ответить сообщением об ошибке (см. [DisplayErrorMessage](#DisplayErrorMessage)) или разослать сообщение о подключении игрока (см. [OnPlayerJoined](#OnPlayerJoined)).

### Параметры

+ **gameSessionID** (string) - идентификатор игровой сессии
+ **team** (Team) - данные команды (см. [Team](#team))

## LeaveGame

Отключает игрока. Может ответить сообщением об ошибке (см. [DisplayErrorMessage](#DisplayErrorMessage)).

### Параметры

+ **gameSessionID** (string) - идентификатор игровой сессии

## Chat

Рассылает текстовое сообщение всем игрокам в сессии. Может ответить сообщением об ошибке (см. [DisplayErrorMessage](#DisplayErrorMessage)) или разослать сообщение (см. [OnPlayerChat](#OnPlayerChat)).

### Параметры

+ **gameSessionID** (string) - идентификатор игровой сессии
+ **message** (string) - текстовое сообщение

## ProfStartGame

Запускает игру. Может ответить сообщением об ошибке (см. [DisplayErrorMessage](#DisplayErrorMessage)).

### Параметры

+ Требует JWT-токен
+ **gameSessionID** (string) - идентификатор игровой сессии

## StudChooseQuestion

Выбор вопроса ИГРОКОМ. Может ответить сообщением об ошибке (см. [DisplayErrorMessage](#DisplayErrorMessage)).

### Параметры

+ **gameSessionID** (string) - идентификатор игровой сессии
+ **questionID** (string) - идентификатор вопроса в таблице (см. [GameStateDto](#GameStateDto))

## ProfChooseQuestion

Выбор вопроса ПРЕПОДОМ за тупящего игрока. Может ответить сообщением об ошибке (см. [DisplayErrorMessage](#DisplayErrorMessage)).

### Параметры

+ Требует JWT-токен
+ **gameSessionID** (string) - идентификатор игровой сессии
+ **questionID** (string) - идентификатор вопроса в таблице (см. [GameStateDto](#GameStateDto))

## DisplayHint

Разослать всем игрокам подсказку вопроса. Может ответить сообщением об ошибке (см. [DisplayErrorMessage](#DisplayErrorMessage)), может разослать всем подсказку (см. [HintDisplayed](#OnHintDisplayed)).

### Параметры

+ Требует JWT-токен
+ **gameSessionID** (string) - идентификатор игровой сессии

## RaiseHand

Поднятие руки игроком. Может ответить сообщением об ошибке (см. [DisplayErrorMessage](#DisplayErrorMessage)), может разослать всем что игрок поднял руку (см. [OnPlayerHandRaised](#OnPlayerHandRaised)).

### Параметры

+ **gameSessionID** (string) - идентификатор игровой сессии

## UnraiseHand

Опускание руки игроком. Может ответить сообщением об ошибке (см. [DisplayErrorMessage](#DisplayErrorMessage)), может разослать всем что игрок опустил руку (см. [OnPlayerHandUnraised](#OnPlayerHandUnraised)).

### Параметры

+ **gameSessionID** (string) - идентификатор игровой сессии

## SendAnswer

Игрок отправляет ответ на вопрос. Может ответить сообщением об ошибке (см. [DisplayErrorMessage](#DisplayErrorMessage)), может разослать всем что игрок ответил (см. [OnAnswerGiven](#OnAnswerGiven)).

### Параметры

+ **gameSessionID** (string) - идентификатор игровой сессии
+ **answer** (string) - текст ответа

## ProfAcceptAnswer

Препод засчитывает висящий ответ на вопрос. Может ответить сообщением об ошибке (см. [DisplayErrorMessage](#DisplayErrorMessage)), может разослать всем что препод засчитал ответ (см. [OnAnswerAccepted](#OnAnswerAccepted)).

### Параметры

+ Требует JWT-токен
+ **gameSessionID** (string) - идентификатор игровой сессии

## ProfAcceptAnswer

Препод отклоняет висящий ответ на вопрос. Может ответить сообщением об ошибке (см. [DisplayErrorMessage](#DisplayErrorMessage)), может разослать всем что препод отклонил ответ (см. [OnAnswerReject](#OnAnswerReject)).

### Параметры

+ Требует JWT-токен
+ **gameSessionID** (string) - идентификатор игровой сессии

## ProfPassTurn

Препод передает право ответа другому игроку. Может ответить сообщением об ошибке (см. [DisplayErrorMessage](#DisplayErrorMessage)), может разослать всем что препод передал ход (см. [OnTurnPassed](#OnTurnPassed)).

### Параметры

+ Требует JWT-токен
+ **gameSessionID** (string) - идентификатор игровой сессии
+ **playerID** (string) - идентификатор игрока который будет отвечать (см. [GameStateDto](#GameStateDto))

## ProfKickPlayer

Препод выкидывает игрока из лобби. Может ответить сообщением об ошибке (см. [DisplayErrorMessage](#DisplayErrorMessage)), в остальном аналогичен [LeaveGame](#LeaveGame).

### Параметры

+ Требует JWT-токен
+ **gameSessionID** (string) - идентификатор игровой сессии
+ **playerID** (string) - идентификатор игрока которого нужно выкинуть (см. [GameStateDto](#GameStateDto))

## ProfChangeScore

Препод вручную устанавливает счет игроку. Может ответить сообщением об ошибке (см. [DisplayErrorMessage](#DisplayErrorMessage)), либо рассылает всем что препод поменял счет (см. [OnPlayerScoreChanged](#OnPlayerScoreChanged)).

### Параметры

+ Требует JWT-токен
+ **gameSessionID** (string) - идентификатор игровой сессии
+ **playerID** (string) - идентификатор игрока которому нужно изменить счет (см. [GameStateDto](#GameStateDto))
+ **value** (int) - новый счет игрока

## ProfAddTimeControl

Препод добавляет время на таймере. Может ответить сообщением об ошибке (см. [DisplayErrorMessage](#DisplayErrorMessage)), либо рассылает всем что препод вмешался в таймер (см. [OnTimerChanged](#OnTimerChanged)).

### Параметры

+ Требует JWT-токен
+ **gameSessionID** (string) - идентификатор игровой сессии
+ **value** (int) - надбавка к таймеру (может быть отрицательной)

## ProfPauseGame

Препод приостанавливает игру, замораживая таймер и запрещая игрокам вводить команды. Может ответить сообщением об ошибке (см. [DisplayErrorMessage](#DisplayErrorMessage)), либо рассылает всем сообщение что препод остановил игру (см. [OnGamePaused](#OnGamePaused)).

### Параметры

+ Требует JWT-токен
+ **gameSessionID** (string) - идентификатор игровой сессии

## ProfUnpauseGame

Препод возобновляет игру после паузы. Может ответить сообщением об ошибке (см. [DisplayErrorMessage](#DisplayErrorMessage)), либо рассылает всем сообщение что препод возобновил игру (см. [OnGameUnpaused](#OnGameUnpaused)).

### Параметры

+ Требует JWT-токен
+ **gameSessionID** (string) - идентификатор игровой сессии

# Трансферные объекты

## GameStateDto 

Этот объект отправляется JSON-ом каждым вызовом связанным с логикой. Пока что он выглядит вот так:

### JSON

```
{
    "Id": "f56b88d7-9bcb-4f7f-8d24-0c0ed97e61a5",
    "LobbyCapacity": 6,
    "ProfName": "Professor Oak",
    "Players": [
        {
            "Id": "93c0a9f3-3c16-4136-a499-25113cfd36b6",
            "Name": "ВИА Железная Дева им Александра Панина",
            "IsActive": true,
            "GameScore": 500,
            "RaisedHand": true,
            "GaveAnswer": false
        },
        {
            "Id": "51d20c6e-0196-4d0c-84a3-86e1e225cccd",
            "Name": "ЧВК Редан",
            "IsActive": false,
            "GameScore": 0,
            "RaisedHand": false,
            "GaveAnswer": false
        }
    ],
    "Themes": [
        {
            "ThemeName": "Исламские пантеоны",
            "Questions": [
                {
                    "Id": "bde749a5-31e3-4cc3-b3a3-55a0dc3f9a4e",
                    "Cost": 100,
                    "IsAvaliable": true
                },
                {
                    "Id": "5180fb9b-3d3e-4f80-9ea6-750e1e0f04f7",
                    "Cost": 200,
                    "IsAvaliable": true
                }
            ]
        },
        {
            "ThemeName": "Покемоны по сиськам",
            "Questions": [
                {
                    "Id": "fb7ed1b1-4e7d-4f90-85f3-c61c636012db",
                    "Cost": 100,
                    "IsAvaliable": true
                },
                {
                    "Id": "60e1ddc6-8d9a-40eb-95b3-f50d6f2a6a55",
                    "Cost": 200,
                    "IsAvaliable": false
                }
            ]
        }
    ],
    "FlowState": 1,
    "LogicStage": 2,
    "TimeRemaining": 60,
    "CurrentQuestionId": "bde749a5-31e3-4cc3-b3a3-55a0dc3f9a4e",
    "AnsweringPlayerId": "93c0a9f3-3c16-4136-a499-25113cfd36b6",
    "ChoosingPlayerId": null
}
```
За значениями для FlowState и LogicStage смотрите ниже

### В коде сервера 
Лежит в Api.Hubs.GameStateDto.cs


```C#
public class GameStateDto
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
     }
```
```C#
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
```

Лежит в Api.Hubs.GameLogic.cs
```C#
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
```

## Team 

JSON для подключения команды к игровой сессии. Требуется только название команды (string) и список членов (string):
```
{
    "title": "Скибиди воп вуп",
    "members": [
        "скибиди",
        "воп",
        "вуп"
    ]
}
```
