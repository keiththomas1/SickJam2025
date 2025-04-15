using KinematicCharacterController;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ConveyorBelt : MonoBehaviour, IMoverController
{
    public PhysicsMover Mover;

    public Vector3 TranslationAxis = Vector3.right;
    public float TranslationSpeed = 1;

    private Vector3 _originalPosition;
    private Quaternion _originalRotation;

    private float _currentTime;

    private void Start()
    {
        _originalPosition = Mover.Rigidbody.position;
        _originalRotation = Mover.Rigidbody.rotation;
        this._currentTime = 0f;

        Mover.MoverController = this;
    }

    public void UpdateMovement(out Vector3 goalPosition, out Quaternion goalRotation, float deltaTime)
    {
        this._currentTime += Time.deltaTime;

        goalPosition = (_originalPosition + (TranslationAxis.normalized * this._currentTime * TranslationSpeed));
        goalRotation = _originalRotation;
    }
}
