using Godot;

namespace Quasar.math
{
    public partial class SimplexNoise
    {
        public int Seed { get => _noise.Seed; set => _noise.Seed = value; }

        private readonly FastNoiseLite _noise;

        public SimplexNoise(int seed = 0) 
        { 
            _noise = new FastNoiseLite();
            _noise.NoiseType = FastNoiseLite.NoiseTypeEnum.Simplex;
            _noise.FractalType = FastNoiseLite.FractalTypeEnum.Fbm;
            Seed = seed;
            GD.Print($"Seed: {Seed}");

        }

        public float GetNoise(float x, float y)
        {
            var noiseValue = _noise.GetNoise2D(x, y);
            var remapNoise = Math.Remap(noiseValue, -1.0f, 1.0f, 0.0f, 100.0f);

            return remapNoise;
        }
    }
}