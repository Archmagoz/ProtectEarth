using Godot;

namespace ProtectEarth.Utils
{
    public static class RNG
    {
        // Centralized random number generator for consistent randomness across the game.
        private static readonly RandomNumberGenerator _rng = new();

        // Static constructor to initialize the random number generator.
        static RNG()
        {
            _rng.Randomize();
        }

        // Generates a random float between min (inclusive) and max (exclusive).
        public static float Range(float min, float max)
        {
            return _rng.RandfRange(min, max);
        }

        // Generates a random integer between min (inclusive) and max (inclusive).
        public static int Range(int min, int max)
        {
            return _rng.RandiRange(min, max);
        }
    }
}
