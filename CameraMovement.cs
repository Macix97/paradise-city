using UnityEngine;

public class CameraMovement : MonoBehaviour
{
    // Control buttons
    public KeyCode MoveForward = KeyCode.W;
    public KeyCode MoveBack = KeyCode.S;
    public KeyCode MoveLeft = KeyCode.A;
    public KeyCode MoveRight = KeyCode.D;
    public KeyCode Climb = KeyCode.Q;
    public KeyCode Drop = KeyCode.E;
    public KeyCode MoveFaster = KeyCode.LeftShift;
    public KeyCode MoveSlower = KeyCode.LeftControl;
    public KeyCode LockCursor = KeyCode.Tab;
    // Factors
    public float CameraSensitivity = 90f;
    public float ClimbSpeed = 4f;
    public float NormalMoveSpeed = 10f;
    public float SlowMoveFactor = 0.25f;
    public float FastMoveFactor = 3f;
    // Rotation
    private float _rotationX = 0f;
    private float _rotationY = 0f;

    // Update is called once per frame
    private void Update()
    {
        ControlCamera();
    }

    // Control camera movement
    private void ControlCamera()
    {
        // Rotate camera
        _rotationX += Input.GetAxis("Mouse X") * CameraSensitivity * Time.deltaTime;
        _rotationY += Input.GetAxis("Mouse Y") * CameraSensitivity * Time.deltaTime;
        _rotationY = Mathf.Clamp(_rotationY, -90, 90);
        transform.localRotation = Quaternion.AngleAxis(_rotationX, Vector3.up);
        transform.localRotation *= Quaternion.AngleAxis(_rotationY, Vector3.left);
        // Fast move
        if (Input.GetKey(MoveFaster))
        {
            if (Input.GetKey(MoveForward))
                transform.position += transform.forward * NormalMoveSpeed * FastMoveFactor * Time.deltaTime;
            if (Input.GetKey(MoveBack))
                transform.position -= transform.forward * NormalMoveSpeed * FastMoveFactor * Time.deltaTime;
            if (Input.GetKey(MoveRight))
                transform.position += transform.right * NormalMoveSpeed * FastMoveFactor * Time.deltaTime;
            if (Input.GetKey(MoveLeft))
                transform.position -= transform.right * NormalMoveSpeed * FastMoveFactor * Time.deltaTime;
        }
        // Slow move
        else if (Input.GetKey(MoveSlower))
        {
            if (Input.GetKey(MoveForward))
                transform.position += transform.forward * NormalMoveSpeed * SlowMoveFactor * Time.deltaTime;
            if (Input.GetKey(MoveBack))
                transform.position -= transform.forward * NormalMoveSpeed * SlowMoveFactor * Time.deltaTime;
            if (Input.GetKey(MoveRight))
                transform.position += transform.right * NormalMoveSpeed * SlowMoveFactor * Time.deltaTime;
            if (Input.GetKey(MoveLeft))
                transform.position -= transform.right * NormalMoveSpeed * SlowMoveFactor * Time.deltaTime;
        }
        // Standard move
        else
        {
            if (Input.GetKey(MoveForward))
                transform.position += transform.forward * NormalMoveSpeed * Time.deltaTime;
            if (Input.GetKey(MoveBack))
                transform.position -= transform.forward * NormalMoveSpeed * Time.deltaTime;
            if (Input.GetKey(MoveRight))
                transform.position += transform.right * NormalMoveSpeed * Time.deltaTime;
            if (Input.GetKey(MoveLeft))
                transform.position -= transform.right * NormalMoveSpeed * Time.deltaTime;
        }
        // Climb
        if (Input.GetKey(Climb))
            transform.position += transform.up * ClimbSpeed * Time.deltaTime;
        // Drop
        if (Input.GetKey(Drop))
            transform.position -= transform.up * ClimbSpeed * Time.deltaTime;
        // Lock cursor
        if (Input.GetKeyDown(LockCursor))
        {
            if (Cursor.lockState == CursorLockMode.None)
                Cursor.lockState = CursorLockMode.Locked;
            else
                Cursor.lockState = CursorLockMode.None;
        }
    }
}