using System.Collections.Generic;
using System.Linq;
using Assets.Scripts.Model;
using UnityEngine;

namespace Assets.Scripts.AStar
{
    public class Pathfinder
    {
        //TODO: Для оптимизации заменить на очередь с приоритетом
        private readonly Queue<Node> _openNodes;
        private readonly List<Node> _resultPath;
        private readonly HashSet<int> _closeSet;
        private readonly HashSet<int> _openSet;

        private readonly Dictionary<int, Dictionary<int, Node>> _nodeByXY;

        public Pathfinder(List<Node> grid)
        {
            this._resultPath = new List<Node>();
            this._closeSet = new HashSet<int>();
            this._openSet = new HashSet<int>();

            this._openNodes = new Queue<Node>();

            this._nodeByXY = grid.GroupBy(e => e.PositionX)
                .ToDictionary(k1 => k1.Key, v1 => v1.ToDictionary(k2 => k2.PositionY, v2 => v2));
        }

        public List<Node> GetPath(Node from, Node to)
        {
            this._resultPath.Clear();

            this._openNodes.Enqueue(from);
            this._openSet.Add(from.Id);

            List<Node> neighbors = new List<Node>(4);

            Node currentNode = null;
            while (this._openNodes.Count > 0)
            {
                currentNode = this._openNodes.Dequeue();
                this._openSet.Remove(currentNode.Id);

                this._closeSet.Add(currentNode.Id);

                if (Equals(currentNode, to))
                    break;

                neighbors.Clear();
                this.UpdateCurrentNeighbors(currentNode, neighbors);

                for (int i = neighbors.Count - 1; i >= 0; --i)
                {
                    Node neighborPathNode = neighbors[i];
                    if (neighborPathNode == null)
                        continue;

                    if (this._closeSet.Contains(neighborPathNode.Id))
                        continue;

                    bool isAtOpenQueue = this._openSet.Contains(neighborPathNode.Id);

                    float movementCostToNeighbor = currentNode.GCost + GetDistance(currentNode, neighborPathNode);
                    if (movementCostToNeighbor < neighborPathNode.GCost || !isAtOpenQueue)
                    {
                        neighborPathNode.GCost = movementCostToNeighbor;
                        neighborPathNode.HCost = GetDistance(neighborPathNode, to);
                        neighborPathNode.Parent = currentNode;

                        if (!isAtOpenQueue)
                        {
                            this._openNodes.Enqueue(neighborPathNode);
                            this._openSet.Add(neighborPathNode.Id);
                        }
                    }
                }
            }

            while (currentNode.Parent != null && !Equals(currentNode, from))
            {
                this._resultPath.Add(currentNode);

                currentNode = currentNode.Parent;
            }

            this._resultPath.Add(from);

            this._openNodes.Clear();
            this._closeSet.Clear();
            this._openSet.Clear();

            if (!this._resultPath.Contains(to))
                return null;

            return this._resultPath;
        }

        private static float GetDistance(Node targetFromNode, Node targetToNode)
        {
            int fromPositionX = targetFromNode.PositionX;
            int toPositionX = targetToNode.PositionX;
            int fromPositionY = targetFromNode.PositionY;
            int toPositionY = targetToNode.PositionY;

            return Mathf.Max(Mathf.Abs(fromPositionX - toPositionX), Mathf.Abs(fromPositionY - toPositionY));
        }

        private void UpdateCurrentNeighbors(Node targetNode, List<Node> result)
        {
            result.Add(this.GetNeighborAtDirection(targetNode, 0));
            result.Add(this.GetNeighborAtDirection(targetNode, 1));
            result.Add(this.GetNeighborAtDirection(targetNode, 2));
            result.Add(this.GetNeighborAtDirection(targetNode, 3));
        }

        private Node GetNeighborAtDirection(Node targetNode, int targetDirection)
        {
            GetNeighborPosition(targetNode, targetDirection, out int positionX, out int positionY);

            if (this._nodeByXY.ContainsKey(positionX))
            {
                if (this._nodeByXY[positionX].ContainsKey(positionY))
                {
                    if (this._nodeByXY[positionX][positionY].IsWall)
                    {
                        return null;
                    }

                    return this._nodeByXY[positionX][positionY];
                }
            }

            return null;
        }

        private static void GetNeighborPosition(Node targetNode, int targetDirection, out int targetPositionX, out int targetPositionY)
        {
            targetPositionX = targetNode.PositionX;
            targetPositionY = targetNode.PositionY;

            switch (targetDirection)
            {
                case 0:
                    targetPositionX -= 1;
                    break;
                case 1:
                    targetPositionY += 1;
                    break;
                case 2:
                    targetPositionX += 1;
                    break;
                case 3:
                    targetPositionY -= 1;
                    break;
            }
        }
    }
}
