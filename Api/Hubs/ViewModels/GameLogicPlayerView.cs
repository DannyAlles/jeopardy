namespace Api.Hubs.ViewModels
{
    public class GameLogicPlayerView
    {
        public bool IsActive { get; set; } = true;
        public int GameScore { get; set; } = 0;
        public bool RaisedHand { get; set; } = false;
        public bool GaveAnswer { get; set; } = false;
    }
}
