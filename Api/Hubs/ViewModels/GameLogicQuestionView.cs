namespace Api.Hubs.ViewModels
{
    public class GameLogicQuestionView
    {
        public string QuestionId { get; set; }
        public int QuestionCost { get; set; } = 0;
        public int TimeControl { get; set; } = 100;
        public bool IsAvaliable { get; set; } = true;
    }
}
