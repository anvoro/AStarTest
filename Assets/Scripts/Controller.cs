using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Assets.Scripts.AStar;
using Assets.Scripts.Model;
using Assets.Scripts.View;
using UnityEngine;
using Object = UnityEngine.Object;

namespace Assets.Scripts
{
    [Serializable]
    public class Point
    {
        public Point(int x, int y)
        {
            this.X = x;
            this.Y = y;
        }

        public Point() { }

        public int X;

        public int Y;
    }

    [Serializable]
    public class PointsCollection
    {
        public List<Point> Points;
    }

    public class Controller : MonoBehaviour
    {
        [SerializeField]
        private int _gridX;
        [SerializeField]
        private int _gridY;

        [SerializeField]
        private GameObject _nodePrefab;

        private Pathfinder _pathfinder;

        private List<Node> _nodes;
        
        private void Start()
        {
            float halfX = (float)this._gridX / 2;
            float halfY = (float)this._gridY / 2;

            int counter = 0;

            string saveData = File.ReadAllText(Path.Combine(Application.streamingAssetsPath, "save.json"));
            PointsCollection walls = JsonUtility.FromJson<PointsCollection>(saveData);

            this._nodes = new List<Node>(this._gridX * this._gridY);

            for (int x = 0; x < this._gridX; x++)
            {
                for (int y = 0; y < this._gridY; y++)
                {
                    NodeView nodeView = Object.Instantiate(this._nodePrefab, new Vector3(x - halfX, y - halfY), Quaternion.identity, this.transform)
                        .GetComponent<NodeView>();

                    nodeView.gameObject.name = $"Node_{x}_{y}";

                    Node nodeModel = new Node(counter++, x, y);
                    this._nodes.Add(nodeModel);

                    if (walls.Points.Any(e => e.X == x && e.Y == y))
                        nodeModel.IsWall = true;

                    nodeView.Init(nodeModel);
                }
            }

            this._pathfinder = new Pathfinder(this._nodes);
        }

        public void CalculatePath()
        {
            this.Clear();

            var start = this._nodes.FirstOrDefault(e => e.IsStart);
            if (start == null)
            {
                Debug.LogWarning("Не задана начальная точка");
                return;
            }

            var end = this._nodes.FirstOrDefault(e => e.IsFinish);
            if (end == null)
            {
                Debug.LogWarning("Не задана конечная точка");
                return;
            }

            var path = this._pathfinder.GetPath(start, end);
            if (path == null)
            {
                Debug.LogWarning("Невозможно найти путь");
                return;
            }

            foreach (Node node in path)
            {
                node.IsFinish = false;
                node.IsStart = false;
                node.IsRoad = true;
            }
        }

        public void Save()
        {
            var list = this._nodes.Where(e => e.IsWall)
                .Select(e => new Point(e.PositionX, e.PositionY)).ToList();

            var points = new PointsCollection()
            {
                Points = list
            };

            var json = JsonUtility.ToJson(points);

            File.WriteAllText(Path.Combine(Application.streamingAssetsPath, "save.json"), json);
        }

        private void Clear()
        {
            foreach (Node node in this._nodes.Where(e => !e.IsWall && !e.IsStart && !e.IsFinish))
            {
                node.IsRoad = false;
            }
        }
    }
}
