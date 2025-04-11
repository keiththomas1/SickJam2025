using UnityEngine;
using UnityEngine.InputSystem;

namespace KinematicCharacterController.Examples
{
    public class ExamplePlayer : MonoBehaviour
    {
        public SickCharacterController Character;
        public Camera CharacterCamera;

        private const string MouseXInput = "Mouse X";
        private const string MouseYInput = "Mouse Y";
        private const string MouseScrollInput = "Mouse ScrollWheel";
        private const string HorizontalInput = "Horizontal";
        private const string VerticalInput = "Vertical";

        private PlayerCharacterInputs _characterInputs = new PlayerCharacterInputs();

        private void Awake()
        {
            this._characterInputs.CrouchDown = false;
            this._characterInputs.CrouchUp = true;
        }

        private void Update()
        {
            HandleCharacterInput();
        }

        private void HandleCharacterInput()
        {
            this._characterInputs.CameraRotation = this.CharacterCamera.transform.rotation;

            // Apply inputs to character
            this.Character.SetInputs(ref this._characterInputs);

            this._characterInputs.JumpDown = false;
        }

        public void OnJump(InputValue value)
        {
            this._characterInputs.JumpDown = value.Get<float>() > 0.5f;
        }
        public void OnCrouch(InputValue value)
        {
            if (value.Get<float>() > 0.5f)
            {
                this._characterInputs.CrouchDown = !this._characterInputs.CrouchDown;
                this._characterInputs.CrouchUp = !this._characterInputs.CrouchUp;
            }
        }
        public void OnMove(InputValue value)
        {
            this._characterInputs.MoveAxisForward = value.Get<Vector2>().y;
            this._characterInputs.MoveAxisRight = value.Get<Vector2>().x;
        }
    }
}