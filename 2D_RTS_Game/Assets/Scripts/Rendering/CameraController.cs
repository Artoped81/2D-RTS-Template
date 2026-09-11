using UnityEngine;
using UnityEngine.InputSystem;

namespace RTSTemplate.Rendering
{
    [RequireComponent(typeof(Camera))]
    public class CameraController : MonoBehaviour
    {
        [SerializeField] private float panSpeed = 8f;
        [SerializeField] private float zoomSpeed = 0.02f;
        [SerializeField] private float minZoom = 3f;
        [SerializeField] private float maxZoom = 12f;

        private Camera cam;

        private void Awake()
        {
            cam = GetComponent<Camera>();
        }

        private void Update()
        {
            HandlePan();
            HandleZoom();
        }

        private void HandlePan()
        {
            var keyboard = Keyboard.current;
            if (keyboard == null) return;

            Vector2 move = Vector2.zero;
            if (keyboard.wKey.isPressed || keyboard.upArrowKey.isPressed) move.y += 1f;
            if (keyboard.sKey.isPressed || keyboard.downArrowKey.isPressed) move.y -= 1f;
            if (keyboard.dKey.isPressed || keyboard.rightArrowKey.isPressed) move.x += 1f;
            if (keyboard.aKey.isPressed || keyboard.leftArrowKey.isPressed) move.x -= 1f;

            if (move.sqrMagnitude > 0f)
                transform.position += (Vector3)(move.normalized * panSpeed * Time.deltaTime);
        }

        private void HandleZoom()
        {
            var mouse = Mouse.current;
            if (mouse == null || !cam.orthographic) return;

            float scroll = mouse.scroll.ReadValue().y;
            if (Mathf.Approximately(scroll, 0f)) return;

            cam.orthographicSize = Mathf.Clamp(
                cam.orthographicSize - scroll * zoomSpeed,
                minZoom, maxZoom);
        }
    }
}
