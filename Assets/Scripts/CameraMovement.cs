using UnityEngine;

/// <summary>
/// Manages the movement of the camera.
/// </summary>
public class CameraMovement : MonoBehaviour
{
    // Control buttons
    internal KeyCode MoveForward;
    internal KeyCode MoveBack;
    internal KeyCode MoveLeft;
    internal KeyCode MoveRight;
    internal KeyCode Climb;
    internal KeyCode Drop;
    internal KeyCode MoveFaster;
    internal KeyCode MoveSlower;
    // Factors
    private float CameraSensitivity = 90f;
    private float ClimbSpeed = 4f;
    private float NormalMoveSpeed = 10f;
    private float SlowMoveFactor = 0.25f;
    private float FastMoveFactor = 3f;
    // Rotation
    private float _rotationX = 0f;
    private float _rotationY = 0f;

    // Update is called once per frame
    private void Update()
    {
        ControlCamera();
    }

    /// <summary>
    /// Controls the camera movement depending on the current inputs.
    /// </summary>
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
    }
}