namespace Game.Events
{
    public sealed class FinalScorePublished : IGameEvent
    {
        public int MyScore;

        public int OpponentScore;
    }
}