namespace Api.Hubs.ViewModels
{
    public class HubSessionView
    {
        public string DbId { get; set; }
        public GameLogic Logic { get; set; }
        public int LobbyCapacity { get; set; }
        public HubProfessorView Prof { get; set; }
        public List<HubPlayerView> Players { get; set; }
        /// <summary>
        /// Ответ игрока, данный на вопрос
        /// </summary>
        public string Answer = "";
    }
}
