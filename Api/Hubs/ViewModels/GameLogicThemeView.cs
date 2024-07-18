namespace Api.Hubs.ViewModels
{
    public class GameLogicThemeView
    {
        public string ThemeId { get; set; }
        public string ThemeName { get; set; }
        public Dictionary<int, GameLogicQuestionView> Questions { get; set; }
    }
}
