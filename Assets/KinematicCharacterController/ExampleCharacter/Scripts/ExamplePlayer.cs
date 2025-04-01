using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using KinematicCharacterController;
using KinematicCharacterController.Examples;
using UnityEngine.InputSystem;

namespace KinematicCharacterController.Examples
{
    public class ExamplePlayer : MonoBehaviour
    {
        public ExampleCharacterController Character;
        public Camera CharacterCamera;

        private const string MouseXInput = "Mouse X";
        private const string MouseYInput = "Mouse Y";
        private const string MouseScrollInput = "Mouse ScrollWheel";
        private const string HorizontalInput = "Horizontal";
        private const string VerticalInput = "Vertical";

        private PlayerCharacterInputs _characterInputs = new PlayerCharacterInputs();

        private void Update()
        {
            HandleCharacterInput();
        }

        private void HandleCharacterInput()
        {
            this._characterInputs.CameraRotation = this.CharacterCamera.transform.rotation;

            // Apply inputs to character
            Character.SetInputs(ref this._characterInputs);

            this._characterInputs.JumpDown = false;
        }

        public void OnJump(InputValue value)
        {
            this._characterInputs.JumpDown = value.Get<float>() > 0.5f;
        }
        public void OnCrouch(InputValue value)
        {
            this._characterInputs.CrouchDown = value.Get<float>() > 0.5f;
        }
        public void OnMove(InputValue value)
        {
            this._characterInputs.MoveAxisForward = value.Get<Vector2>().y;
            this._characterInputs.MoveAxisRight = value.Get<Vector2>().x;
        }
    }
}