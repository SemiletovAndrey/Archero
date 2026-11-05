using Template.Gameplay;

namespace Contexts.Gameplay
{
    public class GamePlayStatePayloads
    {
        public GamePlayStatePayloads(int level)
        {
            Level = level;
        }

        public int Level { get; private set; }
    }
}