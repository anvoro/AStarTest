using System;
using Assets.Scripts.Model;
using UnityEngine;

namespace Assets.Scripts.View
{
    public class NodeView : MonoBehaviour
    {
        private Node _model;

        [SerializeField]
        private SpriteRenderer _spriteRenderer;

        public void Init(Node model)
        {
            this._model = model;
            this._model.OnSetRoad += this.SetRoadColor;

            if(this._model.IsWall)
                this.SetWall();
        }

        private void SetRoadColor()
        {
            this._spriteRenderer.color = this._model.IsRoad ? Color.red : Color.white;
        }

        private void SetWall()
        {
            this._spriteRenderer.color = Color.black;
        }

        private void OnMouseUp()
        {
            switch (DrawModeSelector.Instance.GetCurrentDrawMode())
            {
                case DrawMode.Walls:
                    this._model.IsWall = !this._model.IsWall;
                    this._model.IsFinish = false;
                    this._model.IsStart = false;
                    this._spriteRenderer.color = this._model.IsWall ? Color.black : Color.white;
                    break;

                case DrawMode.Start:
                    this._model.IsStart = !this._model.IsStart;
                    this._model.IsFinish = false;
                    this._model.IsWall = false;
                    this._spriteRenderer.color = this._model.IsStart ? Color.blue : Color.white;
                    break;

                case DrawMode.Finish:
                    this._model.IsFinish = !this._model.IsFinish;
                    this._model.IsWall = false;
                    this._model.IsStart = false;
                    this._spriteRenderer.color = this._model.IsFinish ? Color.green : Color.white;
                    break;

                default:
                    throw new ArgumentOutOfRangeException();
            }
        }
    }
}
