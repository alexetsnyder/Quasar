using Godot;
using System;
using System.Collections.Generic;
using System.Linq;

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

        /// <summary>
        /// Return n points that are closest to the toPoint.
        /// </summary>
        /// <param name="fromPoints"></param>
        /// <param name="toPoint"></param>
        /// <param name="n"></param>
        /// <returns></returns>
        public static Vector2I?[] MinDistanceToPoint(List<Vector2I> fromPoints, Vector2I toPoint, int n = 1)
        {
            float[] minDistances = new float[n];
            var points = new Vector2I?[n];

            for (int i = 0; i < n; i++)
            {
                minDistances[i] = float.MaxValue;
                points[i] = null;
            }

            foreach (var point in fromPoints)
            {
                var d = Distance(point, toPoint);
                for (int i = 0; i < n; i++)
                {
                    if (d < minDistances[i])
                    {
                        BubbleMinRec(minDistances, points, n, i);
                        minDistances[i] = d;
                        points[i] = point;
                        break;
                    }
                }
            }

            return points;
        }

        private static void BubbleMinRec(float[] minDistances, Vector2I?[] points, int n, int index)
        {
            for (int j = index + 1; j < n; j++)
            {
                if (minDistances[index] < minDistances[j])
                {
                    BubbleMinRec(minDistances, points, n, j);
                    minDistances[j] = minDistances[index];
                    points[j] = points[index];
                    break;
                }
            }
        }

        public static List<Vector2I> MaxConnectedArea(List<Vector2I> allPoints, Func<Vector2I, bool> validPoint)
        {
            List<Vector2I> adjDir = [new(1, 0), new(0, 1), new(-1, 0), new(0, -1),
                                     new(1, 1), new(-1, 1), new(1, -1), new(-1, -1)];
            Dictionary<Vector2I, bool> visited = allPoints.ToDictionary(p => p, _ => false);

            List<Vector2I> maxConnectedArea = [];

            foreach (var cellCoord in allPoints)
            {
                if (visited[cellCoord])
                {
                    continue;
                }

                Queue<Vector2I> queue = [];
                List<Vector2I> connectedArea = [];

                queue.Enqueue(cellCoord);

                while (queue.Count > 0)
                {
                    var nextCoord = queue.Dequeue();

                    if (!validPoint(nextCoord) || visited[nextCoord])
                    {
                        continue;
                    }

                    visited[nextCoord] = true;

                    connectedArea.Add(nextCoord);

                    foreach (var adjCoord in adjDir.Select(c => nextCoord + c))
                    {
                        queue.Enqueue(adjCoord);
                    }
                }

                if (connectedArea.Count > 0)
                {
                    GD.Print($"Area Count: {connectedArea.Count}");
                }

                if (connectedArea.Count > maxConnectedArea.Count)
                {
                    maxConnectedArea = connectedArea;
                }
            }

            return maxConnectedArea;
        }
    }
}
