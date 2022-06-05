using System;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Ziggurat
{
    public class CameraMovement : MonoBehaviour
    {
        [SerializeField]
        Camera _mainCamera;
        private NewInput _inputs;
        [SerializeField, Range(0.1f, 100f)]
        private float _moveSpeed = 20f;
        [SerializeField, Range(0.1f, 100f)]
        private float _rotateSpeed = 20f;
        [SerializeField, Range(0.1f, 300f)]
        private float _upDownSpeed = 20f;
        private bool _isRotate = false;
        private int _gateMask;
        private int _botMask;
        [SerializeField]
        ShowZigguratProps _panelProperties;
        [SerializeField]
        private Material _ultraBotMaterial;
        private void OnEnable()
        {
            _inputs = new NewInput();
            _inputs.Enable();
            _inputs.CameraControl.ActivateRotation.performed += ActivateRotation_performed;
            _inputs.CameraControl.Scale.performed += Scale_performed;
            _inputs.CameraControl.Focus.performed += Focus_performed;
        }
        private void Start()
        {
            _gateMask = LayerMask.GetMask("Gate");
            _botMask = LayerMask.GetMask("Bot");
        }

        private void Focus_performed(UnityEngine.InputSystem.InputAction.CallbackContext obj)
        {
            var ray = _mainCamera.ScreenPointToRay(Mouse.current.position.ReadValue());
            if (Physics.Raycast(ray,out var hit, 1000f, _gateMask| _botMask))
            {
                var ziggurat=hit.collider.gameObject.GetComponent<ZigguratParameter>();
                if (ziggurat != null)
                {
                    _panelProperties.UpdateUI(ziggurat);
                }
                var bot= hit.collider.gameObject.GetComponent<BotUnit>();
                if (bot != null)
                {
                    if (_ultraBotMaterial != null)
                    {
                        bot.SetShieldMaterial(_ultraBotMaterial);
                        bot.GoUltra();
                    }
                }
            }
        }

        private void Scale_performed(UnityEngine.InputSystem.InputAction.CallbackContext context)
        {
            var move = context.ReadValue<Vector2>();
            if (move.y > 0)
            {
                transform.position += Vector3.up * _upDownSpeed * Time.deltaTime;
            }
            else
            {
                transform.position -= Vector3.up * _upDownSpeed * Time.deltaTime;
            }
        }

        private void ActivateRotation_performed(UnityEngine.InputSystem.InputAction.CallbackContext context)
        {
            _isRotate = context.ReadValue<Single>() == 1;
            if (_isRotate)
            {
                Cursor.lockState = CursorLockMode.Locked;
            }
            else
            {
                Cursor.lockState = CursorLockMode.None;
            }
        }

        private void Update()
        {
            if (_inputs.CameraControl.Move.IsPressed())
            {
                var move = _inputs.CameraControl.Move.ReadValue<Vector2>();
                var projectionToXZ = new Vector3(transform.forward.x, 0, transform.forward.z);
                var perpendicularToProjection = Quaternion.Euler(0, 90, 0) * projectionToXZ;
                var WSMovement = projectionToXZ.normalized * move.y * _moveSpeed * Time.deltaTime;
                var ADMovement = perpendicularToProjection.normalized * move.x * _moveSpeed * Time.deltaTime;
                transform.position += WSMovement + ADMovement;
            }
            if (_isRotate)
            {
                var direction = _inputs.CameraControl.Rotate.ReadValue<Vector2>();
                var angle = transform.eulerAngles;
                angle.x -= direction.y * _rotateSpeed * Time.deltaTime;
                angle.y += direction.x * _rotateSpeed * Time.deltaTime;
                angle.z = 0;
                transform.eulerAngles = angle;
            }
        }
        private void OnDisable()
        {
            _inputs.Disable();
        }
    }
}
