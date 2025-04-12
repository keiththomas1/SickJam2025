using KinematicCharacterController.Examples;
using UnityEngine;
using UnityEngine.AI;
using static UnityEngine.Rendering.DebugUI;

public class NPCNavigation : MonoBehaviour
{
    public SickCharacterController Character;
    public Camera CharacterCamera;

    [Header("Navigation Settings")]
    public float pathRecalculationTime = 1f;
    public float dotProductThreshold = 0.7f;
    public float maxInputMagnitude = 1.0f;

    // Movement direction flags
    private PlayerCharacterInputs _characterInputs = new PlayerCharacterInputs();

    // Navigation components
    private NavMeshPath path;
    private float lastPathCalculationTime;

    private const float JUMP_DELAY_MIN = 1f;
    private const float JUMP_DELAY_MAX = 6f;
    private float _jumpTimer = 0f;

    public Transform Destination
    {
        get; set;
    }

    void Start()
    {
        // Initialize path
        path = new NavMeshPath();

        // Validate required components
        if (CharacterCamera == null)
        {
            CharacterCamera = Camera.main;
            if (CharacterCamera == null)
            {
                Debug.LogError("No camera found! Assign a camera or make sure there's a main camera in the scene.");
                enabled = false;
                return;
            }
        }

        this.Character.IsNPC = true;

        this._characterInputs.CrouchDown = false;
        this._characterInputs.CrouchUp = true;

        this._jumpTimer = Random.Range(JUMP_DELAY_MIN, JUMP_DELAY_MAX);

        // Initial path calculation
        CalculateNewPath();
    }

    void Update()
    {
        this._characterInputs.CameraRotation = this.CharacterCamera.transform.rotation;

        // Recalculate path periodically
        if (Time.time - lastPathCalculationTime > pathRecalculationTime)
        {
            CalculateNewPath();
        }

        // Determine movement direction based on camera perspective
        DetermineMovementDirection();

        if (this._jumpTimer > 0f)
        {
            this._jumpTimer -= Time.deltaTime;

            if (this._jumpTimer <= 0f)
            {
                this._characterInputs.JumpDown = true;
                this._jumpTimer = Random.Range(JUMP_DELAY_MIN, JUMP_DELAY_MAX);
            }
        }

        this.Character.SetInputs(ref this._characterInputs);

        this._characterInputs.JumpDown = false;
    }

    void CalculateNewPath()
    {
        if (this.Destination == null)
            return;

        // Calculate path from current position to destination
        NavMesh.CalculatePath(transform.position, this.Destination.position, NavMesh.AllAreas, path);
        if (path.corners.Length >= 2)
        {
            path.corners[1] = path.corners[1] + Random.insideUnitSphere;
        }
        lastPathCalculationTime = Time.time;
    }

    void DetermineMovementDirection()
    {
        // Reset movement values
        this._characterInputs.MoveAxisForward = 0f;
        this._characterInputs.MoveAxisRight = 0f;

        // If no path or destination, return
        if (path.corners.Length < 2 || this.Destination == null)
            return;

        // Get the next point in the path
        Vector3 nextPointInPath = path.corners[1];

        // Calculate the direction to the next point
        Vector3 directionToNextPoint = (nextPointInPath - transform.position).normalized;

        // Camera forward and right vectors (projected onto the horizontal plane)
        Vector3 cameraForward = Vector3.ProjectOnPlane(CharacterCamera.transform.forward, Vector3.up).normalized;
        Vector3 cameraRight = Vector3.ProjectOnPlane(CharacterCamera.transform.right, Vector3.up).normalized;

        // Calculate dot products to determine direction relative to camera
        float forwardDot = Vector3.Dot(directionToNextPoint, cameraForward);
        float rightDot = Vector3.Dot(directionToNextPoint, cameraRight);

        // Set the movement values directly from dot products
        // This gives us a vector that ranges from -1 to 1 in both axes
        this._characterInputs.MoveAxisForward = Mathf.Clamp(forwardDot, -1f, 1f);
        this._characterInputs.MoveAxisRight = Mathf.Clamp(rightDot, -1f, 1f);

        // Normalize the input if the magnitude exceeds maxInputMagnitude
        Vector2 inputVector = new Vector2(this._characterInputs.MoveAxisRight, this._characterInputs.MoveAxisForward);
        if (inputVector.magnitude > maxInputMagnitude)
        {
            inputVector = inputVector.normalized * maxInputMagnitude;
            this._characterInputs.MoveAxisForward = inputVector.y;
            this._characterInputs.MoveAxisRight = inputVector.x;
        }
    }

    // Accessor methods for the movement values
    public float GetHorizontal() { return this._characterInputs.MoveAxisRight; }
    public float GetVertical() { return this._characterInputs.MoveAxisForward; }
    public Vector2 GetMovementInput() { return new Vector2(this._characterInputs.MoveAxisRight, this._characterInputs.MoveAxisForward); }
}
