using System;

namespace Assets.Scripts.Model
{
    public class Node
    {
        private bool _isRoad;

        public Node Parent { get; set; }

        public float GCost { get; set; }
        public float HCost { get; set; }
        public float FCost => this.GCost + this.HCost;

        public bool IsWall { get; set; }
        public bool IsStart { get; set; }
        public bool IsFinish { get; set; }

        public bool IsRoad
        {
            get => this._isRoad;
            set
            {
                this._isRoad = value;
                this.OnSetRoad?.Invoke();
            }
        }

        public int PositionX { get; }
        public int PositionY { get; }

        public int Id { get; }

        public event Action OnSetRoad;

        public Node(int id, int positionX, int positionY)
        {
            this.Id = id;

            this.PositionX = positionX;
            this.PositionY = positionY;
        }

        public override bool Equals(object obj)
        {
            Node otherNode = obj as Node;
            if (otherNode == null)
                return false;

            return this.Id == otherNode.Id;
        }

        protected bool Equals(Node other)
        {
            return this.Id == other.Id;
        }

        public override int GetHashCode()
        {
            return this.Id;
        }

        public int CompareTo(Node other)
        {
            if (ReferenceEquals(this, other)) return 0;
            if (ReferenceEquals(null, other)) return 1;

            return this.FCost.CompareTo(other.FCost);
        }

        public override string ToString()
        {
            return this.FCost.ToString();
        }
    }
}
