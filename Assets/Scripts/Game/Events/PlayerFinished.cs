namespace Game.Events
{
    public sealed class PlayerFinished : IGameEvent
    {
        public float ScoreMod;

        public PlayerFinished(float scoreMod)
        {
            ScoreMod = scoreMod;
        }
    }
}