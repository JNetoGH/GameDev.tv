using UnityEngine;

namespace JNeto_Terminal.JNeto_Terminal_Scripts
{
    public class JNetoTerminalDraggingController : MonoBehaviour
    {
        
        public bool IsDragging { get; set; }
        private RectTransform _terminalRectTransform;
        private float _initialDistanceFromMouseInX;
        private float _initialDistanceFromMouseInY;

        public void SetInitialDistanceBetweenTerminalAndMouse()
        {
            _initialDistanceFromMouseInX = _terminalRectTransform.position.x - Input.mousePosition.x;
            _initialDistanceFromMouseInY = _terminalRectTransform.position.y - Input.mousePosition.y;
        }

        private void Start()
        {
            _terminalRectTransform = GetComponent<RectTransform>();
        }
        
        private void Update()
        {
            Vector2 mousePos = Input.mousePosition;
            Vector3 currentPos = _terminalRectTransform.position;
            if (IsDragging)
            {
                float newPosInX = mousePos.x + _initialDistanceFromMouseInX;
                float newPosInY = mousePos.y + _initialDistanceFromMouseInY;
                _terminalRectTransform.position = new Vector3(newPosInX, newPosInY, currentPos.z);
            }
        }
        
    }
}
