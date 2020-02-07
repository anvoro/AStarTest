using System;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

namespace Assets.Scripts.View
{
    public enum DrawMode
    {
        Walls = 0,

        Start = 1,

        Finish = 2
    }

    public class DrawModeSelector : MonoBehaviour
    {
        [SerializeField]
        private ToggleGroup _drawModeToggles;

        [SerializeField]
        private Toggle[] _toggles;

        public static DrawModeSelector Instance { get; private set; }

        private void Start()
        {
            Instance = this;
        }

        public DrawMode GetCurrentDrawMode()
        {
            switch (Array.IndexOf(this._toggles, this._drawModeToggles.ActiveToggles().First()))
            {
                case 0:
                    return DrawMode.Walls;

                case 1:
                    return DrawMode.Start;

                case 2:
                    return DrawMode.Finish;

                default:
                    throw new ArgumentOutOfRangeException();
            }
        }
    }
}
