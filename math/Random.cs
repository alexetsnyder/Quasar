using Godot;
using System.Collections.Generic;

namespace Quasar.math
{
    public static partial class Random
    {
        public static T RandomChoice<T>(RandomNumberGenerator rng, List<T> alts)
        {
            if (alts.Count <= 0)
            {
                return default;
            }

            return alts[rng.RandiRange(0, alts.Count - 1)];
        }

        public static List<Vector2> PoissonDiskSampling(RandomNumberGenerator rng, float radius, int k, float width, float height)
        {
            int N = 2;
            List<Vector2> points = [];
            List<Vector2> active = [];

            Vector2 p0 = new(rng.RandfRange(0, width), rng.RandfRange(0, height));
            float cellSize = Mathf.Floor(radius / Mathf.Sqrt(N));

            int nCellsWidth = Mathf.CeilToInt(width / cellSize) + 1;
            int nCellsHeight = Mathf.CeilToInt(height / cellSize) + 1;

            Vector2?[,] grid = new Vector2?[nCellsWidth, nCellsHeight];
            for (int i = 0; i < nCellsWidth; i++)
            {
                for (int j = 0; j < nCellsHeight; j++)
                {
                    grid[i, j] = null;
                }
            }

            InsertPoint(grid, cellSize, p0);
            points.Add(p0);
            active.Add(p0);

            while (active.Count > 0)
            {
                int randomIndex = rng.RandiRange(0, active.Count - 1);
                Vector2 p = active[randomIndex];

                bool isFound = false;
                for (int tries = 0; tries < k; tries++)
                {
                    float theta = rng.RandfRange(0, 360);
                    float newRadius = rng.RandfRange(radius, 2 * radius);
                    float pNewX = p.X + newRadius * Mathf.Cos(Mathf.DegToRad(theta));
                    float pNewY = p.Y + newRadius * Mathf.Sin(Mathf.DegToRad(theta));
                    Vector2 pNew = new(pNewX, pNewY);

                    if (!IsValidPoint(grid, cellSize, nCellsWidth, nCellsHeight, pNew, radius, width, height))
                    {
                        continue;
                    }

                    points.Add(pNew);
                    InsertPoint(grid, cellSize, pNew);
                    active.Add(pNew);
                    isFound = true;
                    break;
                }

                if (!isFound)
                {
                    active.Remove(p);
                }
            }

            return points;
        }

        private static bool IsValidPoint(Vector2?[,] grid, float cellSize, int gWidth, int gHeight, Vector2 p, float radius, float width, float height)
        {
            if (p.X < 0 || p.X >= width || p.Y < 0 || p.Y >= height)
            {
                return false;
            }

            int xIndex = Mathf.FloorToInt(p.X / cellSize);
            int yIndex = Mathf.FloorToInt(p.Y / cellSize);
            int i0 = System.Math.Max(xIndex - 1, 0);
            int i1 = System.Math.Min(xIndex + 1, gWidth - 1);
            int j0 = System.Math.Max(yIndex - 1, 0);
            int j1 = System.Math.Min(yIndex + 1, gHeight - 1);

            for (int i = i0; i <= i1; i++)
            {
                for (int j = j0; j <= j1; j++)
                {
                    if (grid[i, j] != null && Math.Distance(grid[i, j].Value, p) < radius)
                    {
                        return false;
                    }
                }
            }

            return true;
        }

        private static void InsertPoint(Vector2?[,] grid, float cellSize, Vector2 point)
        {
            int xIndex = Mathf.FloorToInt(point.X / cellSize);
            int yIndex = Mathf.FloorToInt(point.Y / cellSize);
            grid[xIndex, yIndex] = point;
        }
    }
}