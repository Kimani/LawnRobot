using System;
using System.Numerics;

namespace LawnRobot
{
    public enum LawnType
    {
        TallGrass,
        ShortGrass,
        Obstacle,
        Unvisited,
    }

    public class LawnNode
    {
        public LawnNode Up     { get; private set; }
        public LawnNode Down   { get; private set; }
        public LawnNode Left   { get; private set; }
        public LawnNode Right  { get; private set; }
        public Lawn     Parent { get; private set; }
    }

    public class Lawn
    {
        public static readonly int LAWN_COLUMN_COUNT = 15;
        public static readonly int LAWN_ROW_COUNT = 10;

        private LawnType[,] LawnData = new LawnType[LAWN_COLUMN_COUNT, LAWN_ROW_COUNT];
        private LawnNode[,] LawnNodes = new LawnNode[LAWN_COLUMN_COUNT+2, LAWN_ROW_COUNT+2]; // Additional to represent beyond the edges. Mind the indicies!

        public Lawn()
        {
            for (int i = 0; i < LAWN_COLUMN_COUNT; i++)
            {
                for (int j = 0; j < LAWN_ROW_COUNT; j++)
                {
                    LawnData[i, j] = LawnType.Obstacle;
                }
            }
        }

        public void SetLawnNodeType(int x, int y, LawnType type)
        {
            LawnData[x,y] = type;
        }

        public void InitializeNodes()
        {

        }
    }

    internal static class LawnGenerator
    {
        public static Lawn BuildLawn()
        {
            var lawn = new Lawn();

            // Randomly create a blob. We're not going to be terribly efficent here but that's fine.
            // For now, we'll randomly populate the space with a blob that start at a point somewhere
            // within the potential lawn, put 20 points away from the blob all around it at 0.5-1.0 randomly
            // from the start to the edge, then blur the values a bit, and then set each lawn grid
            // that lies within this shape to TallGrass.
            Random random = new Random((int)DateTime.Now.Ticks);
            double startX = 3 + random.NextDouble() * 9;
            double startY = 2 + random.NextDouble() * 6;

            double[] distanceMultipliersToUsePreBlur = new double[20];
            for (int i = 0; i < 20; ++i)
            {
                distanceMultipliersToUsePreBlur[i] = 0.5 + random.NextDouble() * 0.5;
            }

            double[] distanceMultipliersToUse = new double[20];
            for (int i = 0; i < 20; ++i)
            {
                for (int j = -2; j <= 2; ++j)
                {
                    int nextIndex = (i + j + 20) % 20;
                    distanceMultipliersToUse[i] += distanceMultipliersToUsePreBlur[nextIndex];
                }
                distanceMultipliersToUse[i] /= 5.0;
            }

            double angleIncrement = 2 * Math.PI / 20;
            Vector2[] radialPoints = new Vector2[20];
            for (int i = 0; i < 20; ++i)
            {
                double angle = i * angleIncrement;
                double deltaX = Math.Cos(angle);
                double deltaY = Math.Sin(angle);

                Vector2 intersectionPointAtMaxDistance = GetIntersectionPoint(
                    new Vector2((float)startX, (float)startY),
                    new Vector2((float)deltaX, (float)deltaY),
                    new Vector2(0, 0),
                    new Vector2(Lawn.LAWN_COLUMN_COUNT, Lawn.LAWN_ROW_COUNT));
                double distanceMultiplierToUse = 0.5 + random.NextDouble() * 0.5;

                double distanceX = intersectionPointAtMaxDistance.X - startX;
                double distanceY = intersectionPointAtMaxDistance.Y - startY;
                double maxDistance = Math.Sqrt(distanceX * distanceX + distanceY * distanceY);

                double distanceToUse = maxDistance * distanceMultipliersToUse[i];
                radialPoints[i] = new Vector2(
                    (float)(startX + (distanceToUse * deltaX)),
                    (float)(startY + (distanceToUse * deltaY)));
            }

            for (int x = 0; x < 15; x++)
            {
                for (int y = 0; y < 10; ++y)
                {
                    Vector2 pointOnLawn = new Vector2(x, y);
                    for (int triangleA = 0; triangleA < 20; ++triangleA)
                    {
                        int triangleB = (triangleA + 1) % 20;
                        Vector2 triangle1 = new Vector2((float)startX, (float)startY);
                        Vector2 triangle2 = radialPoints[triangleA];
                        Vector2 triangle3 = radialPoints[triangleB];

                        if (IsPointInTriangle(pointOnLawn, triangle1, triangle2, triangle3))
                        {
                            lawn.SetLawnNode(x, y, LawnType.TallGrass);
                        }
                    }
                }
            }

            lawn.InitializeNodes();
            return lawn;
        }

        public static Vector2 GetIntersectionPoint(Vector2 point, Vector2 direction, Vector2 boxMin, Vector2 boxMax)
        {
            // Point is assumed to be inside the box.
            double distance = double.MaxValue;
            if (direction.X != 0.0f)
            {
                {
                    double xMinDistance = ((boxMin.X - point.X) / direction.X);
                    if (xMinDistance > 0)
                    {
                        distance = Math.Min(distance, xMinDistance);
                    }
                }

                {
                    double xMaxDistance = ((boxMax.X - point.X) / direction.X);
                    if (xMaxDistance > 0)
                    {
                        distance = Math.Min(distance, xMaxDistance);
                    }
                }
            }

            if (direction.Y != 0.0f)
            {
                {
                    double yMinDistance = ((boxMin.Y - point.Y) / direction.Y);
                    if (yMinDistance > 0)
                    {
                        distance = Math.Min(distance, yMinDistance);
                    }
                }

                {
                    double yMaxDistance = ((boxMax.Y - point.Y) / direction.Y);
                    if (yMaxDistance > 0)
                    {
                        distance = Math.Min(distance, yMaxDistance);
                    }
                }
            }

            return point + (direction * (float)distance);
        }

        public static bool IsPointInTriangle(Vector2 p, Vector2 v1, Vector2 v2, Vector2 v3)
        {
            // Compute vectors
            Vector2 v2v1 = v2 - v1;
            Vector2 v3v1 = v3 - v1;
            Vector2 pv1 = p - v1;

            // Compute dot products
            float dot00 = Vector2.Dot(v3v1, v3v1);
            float dot01 = Vector2.Dot(v3v1, v2v1);
            float dot02 = Vector2.Dot(v3v1, pv1);
            float dot11 = Vector2.Dot(v2v1, v2v1);
            float dot12 = Vector2.Dot(v2v1, pv1);

            // Compute barycentric coordinates
            float invDenom = 1 / (dot00 * dot11 - dot01 * dot01);
            float u = (dot11 * dot02 - dot01 * dot12) * invDenom;
            float v = (dot00 * dot12 - dot01 * dot02) * invDenom;

            // Check if point is in triangle
            return (u >= 0) && (v >= 0) && (u + v < 1);
        }
    }
}
