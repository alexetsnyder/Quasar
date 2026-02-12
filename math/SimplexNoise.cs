using Godot;

namespace Quasar.math
{
    public partial class SimplexNoise
    {
        private RandomNumberGenerator _rng = new();

        private readonly FastNoiseLite _noise;

        public SimplexNoise() 
        { 
            _noise = new FastNoiseLite();
            _noise.NoiseType = FastNoiseLite.NoiseTypeEnum.Simplex;
            _noise.FractalType = FastNoiseLite.FractalTypeEnum.Fbm;
            int seed = _rng.RandiRange(int.MinValue, int.MaxValue);
            GD.Print(seed);
            _noise.Seed = seed;

        }

        public float GetNoise(float x, float y)
        {
            var noiseValue = _noise.GetNoise2D(x, y);
            var remapNoise = Math.Remap(noiseValue, -1.0f, 1.0f, 0.0f, 100.0f);

            return remapNoise;
        }
    }
}