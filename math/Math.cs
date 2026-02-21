using Godot;
using System;
using System.Collections.Generic;

namespace Quasar.math
{
    public static partial class Math
    {
        public static float Remap(float inputValue, float inputStart, float inputEnd, float outputStart, float outputEnd)
        {
            float r = inputEnd - inputStart;
            float R = outputEnd - outputStart;

            return outputStart + (R / r) * (inputValue - inputStart);
        }

        public static float SigmoidFallOffMapCircular(float x, float y, int width, int height)
        {
            int midX = width / 2;
            int midY = height / 2;

            float d = Distance(x, y, midX, midY);
            float dMax = Distance(0, 0, midX, midY);

            return 1 / (1 + MathF.Pow(MathF.E, (d * 12 / dMax - 6)));
        }

        public static float Distance(Vector2I v1, Vector2I v2)
        {
            return Distance(v1.X, v1.Y, v2.X, v2.Y);
        }

        public static float Distance(float x1, float y1, float x2, float y2)
        {
            return MathF.Sqrt(DistanceSquared(x1, y1, x2, y2));
        }

        public static float DistanceSquared(float x1, float y1, float x2, float y2)
        {
            float xDiff = x2 - x1;
            float yDiff = y2 - y1;

            return ((xDiff * xDiff) + (yDiff * yDiff));
        }

        public static Vector2I? MinDistanceToPoint(List<Vector2I> fromPoints, Vector2I toPoint)
        {
            float minDistance = float.MaxValue;
            Vector2I? minDistCellCoord = null;

            foreach (var point in fromPoints)
            {
                var d = Distance(point, toPoint);
                if (d < minDistance)
                {
                    minDistance = d;
                    minDistCellCoord = point;
                }
            }

            return minDistCellCoord;
        }
    }
}
