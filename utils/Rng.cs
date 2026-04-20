using Godot;

namespace ProtectEarth.Utils
{
    public static class RNG
    {
        private static readonly RandomNumberGenerator _rng = new();

        static RNG()
        {
            _rng.Randomize();
        }

        public static float Range(float min, float max)
        {
            return _rng.RandfRange(min, max);
        }

        public static int Range(int min, int max)
        {
            return _rng.RandiRange(min, max);
        }
    }
}