using UnityEngine;
using UnityEngine.AI;

// List of actions
public enum ActionType
{
    Idling,
    Walking,
    Rotating,
    Turning,
    Sitting,
    Waiting,
    Standing
};

// Control character behavior
public class ManBehavior : MonoBehaviour
{
    // Angle accuracy
    [Range(2f, 5f)]
    public float AngleAccuracy;
    // Waiting time in way point
    [Range(1f, 3f)]
    public float WaitingTime;
    // Rotation speed
    [Range(1f, 5f)]
    public float RotationSpeed;
    // Targets (way points)
    public Transform[] Targets;
    // Agent offset
    private float _standardOffset;
    // Nav mesh agent
    private NavMeshAgent _agent;
    // Animator
    private Animator _animator;
    // Current look rotation
    private Quaternion _lookRotation;
    // Check if character is waliking
    private bool _isWalking;
    private bool _isRotatingRight;
    // Check if is left rotation
    private bool _isRotatingLeft;
    // Check if character is turning
    private bool _isTurning;
    // Animator walking value
    private string _animWalk = "isWalking";
    // Animator rotating right value
    private string _animRotateRight = "isRotatingRight";
    // Animator rotating left value
    private string _animRotateLeft = "isRotatingLeft";
    // Animator turning value
    private string _animTurning = "isTurning";
    // Current target index
    private int _currentTarget;
    // Current waiting time
    private float _currentTime;
    // Translation time
    private float _translationTime;
    // Current action
    private ActionType _currentAction;

    // Start is called before the first frame update
    private void Start()
    {
        Init();
    }

    // Update is called once per frame
    private void Update()
    {
        switch (_currentAction)
        {
            case ActionType.Idling:
                WaitAWhile();
                break;
            case ActionType.Walking:
                GoToTarget();
                break;
            case ActionType.Rotating:
                RotateToTarget();
                break;
            case ActionType.Turning:
                TurnCharacter();
                break;
            case ActionType.Sitting:
                SitOnBench();
                break;
            case ActionType.Waiting:
                WaitOnBench();
                break;
            case ActionType.Standing:
                StandUpAndSetTarget();
                break;
        }
    }

    // Initializate parameters
    private void Init()
    {
        _agent = gameObject.GetComponent<NavMeshAgent>();
        _animator = gameObject.GetComponent<Animator>();
        _isWalking = _isRotatingRight = _isRotatingLeft = false;
        _currentAction = ActionType.Idling;
        _currentTarget = 0;
        _currentTime = 0f;
        _standardOffset = _agent.baseOffset;
        _animator.SetBool(_animWalk, _isWalking);
        _animator.SetBool(_animRotateRight, _isRotatingRight);
        _animator.SetBool(_animRotateLeft, _isRotatingLeft);
    }

    // Wait a while in postion
    private void WaitAWhile()
    {
        // Check waiting time
        if (_currentTime >= WaitingTime)
        {
            // Reset current time
            _currentTime = 0f;
            // Set walking action
            _currentAction = ActionType.Walking;
            // Break action
            return;
        }
        // Increase time
        _currentTime += Time.deltaTime;
    }

    // Go to selected target
    private void GoToTarget()
    {
        // Set destination
        _agent.SetDestination(Targets[_currentTarget].position);
        // Destination is reached
        if (!_agent.pathPending && _agent.remainingDistance < _agent.stoppingDistance)
        {
            // Set animation
            _agent.isStopped = true;
            _isWalking = false;
            _animator.SetBool(_animWalk, _isWalking);
            // Check target type
            if (Targets[_currentTarget].name.Equals("SitPoint"))
            {
                // Set rotating action
                _currentAction = ActionType.Rotating;
                // Break action
                return;
            }
            // Set new target
            else
            {
                // Check current target
                if (_currentTarget.Equals(Targets.Length - 1))
                    // Reset path
                    _currentTarget = 0;
                // Set another target
                else
                    _currentTarget++;
                // Set idling action
                _currentAction = ActionType.Idling;
            }
        }
        // Go to target
        else
        {
            // Set animation
            _agent.isStopped = false;
            _isWalking = true;
            _animator.SetBool(_animWalk, _isWalking);
        }
    }

    // Rotate character to target
    private void RotateToTarget()
    {
        // Get look rotation
        _lookRotation = Quaternion.LookRotation(Targets[_currentTarget].parent.position - transform.position);
        // Check if rotatin is completed
        if (Quaternion.Angle(transform.rotation, _lookRotation) < AngleAccuracy)
        {
            // Change values
            _isRotatingLeft = _isRotatingRight = false;
            // Set animation
            _animator.SetBool(_animRotateRight, _isRotatingRight);
            _animator.SetBool(_animRotateLeft, _isRotatingLeft);
            // Set turning action
            _currentAction = ActionType.Turning;
            // Break action
            return;
        }
        // Check rotation direction
        _isRotatingRight = GetRotateDirection(transform.rotation, _lookRotation);
        _isRotatingLeft = !_isRotatingRight;
        // Set animation
        _animator.SetBool(_animRotateRight, _isRotatingRight);
        _animator.SetBool(_animRotateLeft, _isRotatingLeft);
        // Rotate only y axis
        _lookRotation.x = _lookRotation.z = 0f;
        // Rotate smoothly
        transform.rotation = Quaternion.Slerp(transform.rotation, _lookRotation,
            Time.deltaTime * RotationSpeed);
    }

    // Check if character is rotating right or left
    private bool GetRotateDirection(Quaternion from, Quaternion to)
    {
        // Character
        float fromY = from.eulerAngles.y;
        // Target
        float toY = to.eulerAngles.y;
        // Left rotation
        float clockWise = 0f;
        // Right rotation
        float counterClockWise = 0f;
        // Check rotation direction
        if (fromY <= toY)
        {
            clockWise = toY - fromY;
            counterClockWise = fromY + (360f - toY);
        }
        else
        {
            clockWise = (360f - fromY) + toY;
            counterClockWise = fromY - toY;
        }
        // Return result
        return (clockWise <= counterClockWise);
    }

    // Turn character in opposite direction
    private void TurnCharacter()
    {
        // Set animation
        _isTurning = true;
        _animator.SetBool(_animTurning, _isTurning);
    }

    // Sit on selected bench
    private void SitOnBench()
    {
        // Set that character is sitting
        _isTurning = false;
        _animator.SetBool(_animTurning, _isTurning);
        // Check transition time
        if (_translationTime >= 0.5f)
        {
            // Reset translation time
            _translationTime = 0f;
            // Set waiting action
            _currentAction = ActionType.Waiting;
            // Break action
            return;
        }
        // Set new nav mesh offset
        _agent.baseOffset = -0.05f;
        // Increase translation time
        _translationTime += Time.deltaTime;
        transform.Translate(new Vector3(0f, -Time.deltaTime, Time.deltaTime), Space.Self);

    }

    // Wait on bench
    private void WaitOnBench()
    {
        // Do nothing
    }

    // Trigger standing action
    public void TriggerStanding()
    {
        if (_currentAction.Equals(ActionType.Waiting))
            _currentAction = ActionType.Standing;
    }

    // Trigger sitting action
    public void TriggerSitting()
    {
        if (_currentAction.Equals(ActionType.Turning))
            _currentAction = ActionType.Sitting;
    }

    // Stand up from bench and set new target
    private void StandUpAndSetTarget()
    {
        // Check current target
        if (_currentTarget.Equals(Targets.Length - 1))
            // Reset path
            _currentTarget = 0;
        // Set another target
        else
            _currentTarget++;
        // Set standard offset
        _agent.baseOffset = _standardOffset;
        // Set idling action
        _currentAction = ActionType.Idling;
    }
}