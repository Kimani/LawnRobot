﻿using System;
using System.Collections.Generic;
using System.Numerics;
using System.Xml.Serialization;

namespace LawnRobot.Model
{
    public enum LawnType
    {
        TallGrass,
        ShortGrass,
        Obstacle,
        Unvisited,
    }

    public enum LawnDisplayType
    {
        Empty,
        TallGrass,
        ShortGrass,
        FenceHorizontal,
        FenceVertical,
        FenceEndUp,
        FenceEndLeft,
        FenceEndRight,
        FenceEndDown,
        FenceCornerUpLeft,
        FenceCornerUpRight,
        FenceCornerDownLeft,
        FenceCornerDownRight,
        FenceTNoUp,
        FenceTNoLeft,
        FenceTNoRight,
        FenceTNoDown,
    }

    public class LawnNode : ILawnNode
    {
        public   ILawnNode? Up      { get; internal set; } = null;
        public   ILawnNode? Down    { get; internal set; } = null;
        public   ILawnNode? Left    { get; internal set; } = null;
        public   ILawnNode? Right   { get; internal set; } = null;
        internal Lawn       Parent  { get; private set; }
        public   int        X       { get; private set; }
        public   int        Y       { get; private set; }
        internal bool       Edge    { get; private set; }
        public   bool       Fence   { get; set; }
        internal bool       IsGrass { get => Parent.GetBaseLawnNodeType(X, Y) == LawnType.TallGrass; }

        public event Action GrassChanged;

        public LawnDisplayType DisplayType
        {
            get
            {
                if (Fence)
                {
                    bool upFence   =  (Up != null)    ? ((LawnNode)Up).Fence : false;
                    bool leftFence  = (Left != null)  ? ((LawnNode)Left).Fence : false;
                    bool rightFence = (Right != null) ? ((LawnNode)Right).Fence : false;
                    bool downFence  = (Down != null)  ? ((LawnNode)Down).Fence : false;

                    if (upFence  && !leftFence && !rightFence && !downFence) { return LawnDisplayType.FenceEndUp; }
                    if (!upFence && leftFence  && !rightFence && !downFence) { return LawnDisplayType.FenceEndLeft; }
                    if (!upFence && !leftFence && rightFence  && !downFence) { return LawnDisplayType.FenceEndRight; }
                    if (!upFence && !leftFence && !rightFence && downFence)  { return LawnDisplayType.FenceEndDown; }
                    if (upFence  && !leftFence && !rightFence && downFence)  { return LawnDisplayType.FenceVertical; }
                    if (!upFence && leftFence  && rightFence  && !downFence) { return LawnDisplayType.FenceHorizontal; }
                    if (upFence  && leftFence && !rightFence && !downFence)  { return LawnDisplayType.FenceCornerUpLeft; }
                    if (upFence  && !leftFence && rightFence && !downFence)  { return LawnDisplayType.FenceCornerUpRight; }
                    if (!upFence && leftFence && !rightFence && downFence)   { return LawnDisplayType.FenceCornerDownLeft; }
                    if (!upFence && !leftFence && rightFence && downFence)   { return LawnDisplayType.FenceCornerDownRight; }
                    if (!upFence && leftFence && rightFence && downFence)    { return LawnDisplayType.FenceTNoUp; }
                    if (upFence && !leftFence && rightFence && downFence)    { return LawnDisplayType.FenceTNoLeft; }
                    if (upFence && leftFence && !rightFence && downFence)    { return LawnDisplayType.FenceTNoRight; }
                    if (upFence && leftFence && rightFence && !downFence)    { return LawnDisplayType.FenceTNoDown; }
                }
                else if (Parent.GetBaseLawnNodeType(X, Y) == LawnType.TallGrass)
                {
                    return _Mowed ? LawnDisplayType.ShortGrass : LawnDisplayType.TallGrass;
                }
                return LawnDisplayType.Empty;
            }
        }

        public LawnType Type
        {
            get
            {
                if (Edge || Parent.GetBaseLawnNodeType(X, Y) == LawnType.Obstacle)
                {
                    return LawnType.Obstacle;
                }
                if (_Mowed)
                {
                    return LawnType.ShortGrass;
                }
                if (!_Visited)
                {
                    return LawnType.Unvisited;
                }
                return LawnType.TallGrass;
            }
        }

        public LawnNodeType NodeType => throw new NotImplementedException();

        private bool _Mowed = false;
        private bool _Visited = false;

        public LawnNode(Lawn parent, bool edge, int x, int y)
        {
            Parent = parent;
            Edge = edge;
            X = x;
            Y = y;
        }

        public void Mow()
        {
            if (Type == LawnType.TallGrass)
            {
                _Mowed = true;
                GrassChanged?.Invoke();
            }
        }
    }

    public class Lawn
    {
        public static readonly int LAWN_COLUMN_COUNT = 15;
        public static readonly int LAWN_ROW_COUNT = 10;

        private LawnType[,] LawnData = new LawnType[LAWN_COLUMN_COUNT, LAWN_ROW_COUNT];
        private LawnNode[,] LawnNodes = new LawnNode[LAWN_COLUMN_COUNT + 2, LAWN_ROW_COUNT + 2]; // Additional to represent beyond the edges. Mind the indicies!

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

        public LawnType GetBaseLawnNodeType(int x, int y)
        {
            if (x < 0 || y < 0 || x >= LAWN_COLUMN_COUNT || y >= LAWN_ROW_COUNT)
            {
                return LawnType.Obstacle;
            }
            return LawnData[x, y];
        }

        public void SetLawnNodeType(int x, int y, LawnType type)
        {
            LawnData[x, y] = type;
        }

        public void InitializeNodes()
        {
            for (int i = 0; i < LAWN_COLUMN_COUNT + 2; i++)
            {
                for (int j = 0; j < LAWN_ROW_COUNT + 2; j++)
                {
                    bool isEdge = i == 0 || j == 0 || i == LAWN_COLUMN_COUNT + 1 || j == LAWN_ROW_COUNT + 1;
                    LawnNodes[i, j] = new LawnNode(this, isEdge, i - 1, j - 1);
                }
            }

            for (int i = 0; i < LAWN_COLUMN_COUNT + 2; i++)
            {
                for (int j = 0; j < LAWN_ROW_COUNT + 2; j++)
                {
                    LawnNode currentNode = LawnNodes[i, j];
                    if (i > 0)
                    {
                        currentNode.Left = LawnNodes[i - 1, j];
                    }
                    if (i < LAWN_COLUMN_COUNT + 1)
                    {
                        currentNode.Right = LawnNodes[i + 1, j];
                    }
                    if (j > 0)
                    {
                        currentNode.Down = LawnNodes[i, j - 1];
                    }
                    if (j < LAWN_ROW_COUNT + 1)
                    {
                        currentNode.Up = LawnNodes[i, j + 1];
                    }
                }
            }

            Tuple<int, int>[] allDirectionsOffset = new Tuple<int, int>[]
            {
                new Tuple<int,int>(-1, -1),
                new Tuple<int,int>(-1, 0),
                new Tuple<int,int>(-1, 1),
                new Tuple<int,int>(0, 1),
                new Tuple<int,int>(0, -1),
                new Tuple<int,int>(1, -1),
                new Tuple<int,int>(1, 0),
                new Tuple<int,int>(1, 1),
            };

            for (int i = 0; i < LAWN_COLUMN_COUNT + 2; i++)
            {
                for (int j = 0; j < LAWN_ROW_COUNT + 2; j++)
                {
                    LawnNode currentNode = LawnNodes[i, j];
                    if (currentNode.IsGrass)
                    {
                        foreach (Tuple<int, int> offset in allDirectionsOffset)
                        {
                            Initialize_TryToSetAsFence(i + offset.Item1, j + offset.Item2);
                        }
                    }
                }
            }
        }

        public void Initialize_TryToSetAsFence(int i, int j)
        {
            if ((i < 0) || (j < 0) || (i >= LAWN_COLUMN_COUNT + 2) || (j >= LAWN_ROW_COUNT + 2))
            {
                return;
            }

            LawnNode currentNode = LawnNodes[i, j];
            if (!currentNode.IsGrass)
            {
                currentNode.Fence = true;
            }
        }

        public List<LawnNode> GetLawnNodes()
        {
            var nodeList = new List<LawnNode>();
            foreach (LawnNode node in LawnNodes)
            {
                if ((!node.Edge && GetBaseLawnNodeType(node.X, node.Y) != LawnType.Obstacle) || node.Fence)
                {
                    nodeList.Add(node);
                }
            }
            return nodeList;
        }

        public LawnNode GetLawnStartNode()
        {
            // Quick answer here, just grab them all and then a random one.
            var nodeList = new List<LawnNode>();
            foreach (LawnNode node in LawnNodes)
            {
                if (!node.Edge && GetBaseLawnNodeType(node.X, node.Y) != LawnType.Obstacle)
                {
                    nodeList.Add(node);
                }
            }

            Random r = new Random((int)DateTime.Now.Ticks);
            return nodeList[r.Next(nodeList.Count)];
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
                    (float)(startX + distanceToUse * deltaX),
                    (float)(startY + distanceToUse * deltaY));
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
                            lawn.SetLawnNodeType(x, y, LawnType.TallGrass);
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
                    double xMinDistance = (boxMin.X - point.X) / direction.X;
                    if (xMinDistance > 0)
                    {
                        distance = Math.Min(distance, xMinDistance);
                    }
                }

                {
                    double xMaxDistance = (boxMax.X - point.X) / direction.X;
                    if (xMaxDistance > 0)
                    {
                        distance = Math.Min(distance, xMaxDistance);
                    }
                }
            }

            if (direction.Y != 0.0f)
            {
                {
                    double yMinDistance = (boxMin.Y - point.Y) / direction.Y;
                    if (yMinDistance > 0)
                    {
                        distance = Math.Min(distance, yMinDistance);
                    }
                }

                {
                    double yMaxDistance = (boxMax.Y - point.Y) / direction.Y;
                    if (yMaxDistance > 0)
                    {
                        distance = Math.Min(distance, yMaxDistance);
                    }
                }
            }

            return point + direction * (float)distance;
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
            return u >= 0 && v >= 0 && u + v < 1;
        }
    }
}
