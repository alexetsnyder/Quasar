using Godot;

namespace Quasar.math
{
    public partial class SimplexNoise
    {
        private readonly FastNoiseLite _noise;

        public SimplexNoise() 
        { 
            _noise = new FastNoiseLite();
            _noise.NoiseType = FastNoiseLite.NoiseTypeEnum.SimplexSmooth;
            _noise.FractalType = FastNoiseLite.FractalTypeEnum.Fbm;
        }

        public float GetNoise(float x, float y)
        {
            var noiseValue = _noise.GetNoise2D(x, y);
            return noiseValue;
        }
    }
}