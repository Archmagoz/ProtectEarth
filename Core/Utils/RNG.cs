using Godot;

namespace ProtectEarth.Core.Utils
{
    public static class RNG
    {
        // Shared random number generator instance — ensures consistent randomness across systems.
        private static readonly RandomNumberGenerator _rng = new();

        // Static initialization — seeds the generator once at application startup.
        static RNG()
        {
            _rng.Randomize();
        }

        // Generates a floating-point value within the specified range [min, max].
        public static float Range(float min, float max)
        {
            return _rng.RandfRange(min, max);
        }

        // Generates an integer value within the specified range [min, max].
        public static int Range(int min, int max)
        {
            return _rng.RandiRange(min, max);
        }
    }
}