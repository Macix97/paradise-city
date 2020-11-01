using System.Collections.Generic;
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
    Standing,
    Admiring,
    Watching
};

// Control character behavior
public class HumanBehavior : MonoBehaviour
{
    // Angle accuracy
    [Range(5f, 10f)]
    public float AngleAccuracy;
    // Waiting time in way point
    [Range(1f, 3f)]
    public float WaitingTime;
    // Action time in way point
    [Range(2f, 5f)]
    public float ActionTime;
    // Rotation speed
    [Range(1f, 5f)]
    public float RotationSpeed;
    [Range(-0.05f, -0.01f)]
    public float SittingOffset;
    // Targets (way points)
    private Transform[] _targets;
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
    // Check if is right rotation
    private bool _isRotatingRight;
    // Check if is left rotation
    private bool _isRotatingLeft;
    // Check if character is turning
    private bool _isTurning;
    // Check if character is admiring
    private bool _isAdmiring;
    // Check if character is watching
    private bool _isWatching;
    // Animator walking value
    private string _animWalk = "isWalking";
    // Animator rotating right value
    private string _animRotateRight = "isRotatingRight";
    // Animator rotating left value
    private string _animRotateLeft = "isRotatingLeft";
    // Animator turning value
    private string _animTurning = "isTurning";
    // Animator admiring value
    private string _animAdmiring = "isAdmiring";
    // Animator watching value
    private string _animWatching = "isWatching";
    // Current target index
    private int _currentTarget;
    // Current waiting time
    private float _currentTime;
    // Translation time
    private float _translationTime;
    // Current action
    private ActionType _currentAction;
    // Human types
    private Transform[] _humanTypes;
    // Gender
    private string _gender;

    // Awake is called when the script instance is being loaded
    private void Awake()
    {
        Init();
    }

    // Update is called once per frame
    private void Update()
    {
        SwitchActions();
    }

    // Initializate parameters
    private void Init()
    {
        // Load eyes materials
        Material[] eyesMaterials = Resources.LoadAll<Material>("People/Materials/Eyes");
        // Get person gender
        _gender = name = name.Replace("(Clone)", "");
        // Get all objects in human
        Transform[] humanTransforms = transform.GetComponentsInChildren<Transform>();
        // Create temporary list
        List<Transform> humanTypes = new List<Transform>();
        // Get all human types
        foreach (Transform trans in humanTransforms)
        {
            // Check object tag
            if (trans.tag.Equals("HumanType"))
                // Add to list
                humanTypes.Add(trans);
        }
        // Transform list to array
        _humanTypes = humanTypes.ToArray();
        // Random human type
        int humanIndex = Random.Range(1, _humanTypes.Length + 1);
        // Random eye color
        int eyeIndex = Random.Range(0, eyesMaterials.Length);
        // Get eye renderer
        SkinnedMeshRenderer eyesRenderer = transform.Find(_gender + "Eyes").GetComponent<SkinnedMeshRenderer>();
        // Set new eyes material
        eyesRenderer.material = eyesMaterials[eyeIndex];
        // Disable other types
        foreach (Transform trans in _humanTypes)
        {
            // Check human index
            if (trans.name.Contains("0" + humanIndex + "LOD"))
                // Go to next step
                continue;
            // Get mesh renderers
            SkinnedMeshRenderer[] skinnedMeshRenderers = trans.GetComponentsInChildren<SkinnedMeshRenderer>();
            // Search mesh renderers
            foreach (SkinnedMeshRenderer renderer in skinnedMeshRenderers)
                // Disable mesh renderer
                renderer.enabled = false;
        }
        // Man
        if (humanIndex.Equals(_humanTypes.Length) && _gender.Equals("Man"))
        {
            // Get body renderer
            SkinnedMeshRenderer bodyRenderer = transform.Find("ManBody").GetComponent<SkinnedMeshRenderer>();
            // Set new eyes material (dark)
            eyesRenderer.material = eyesMaterials[eyesMaterials.Length - 1];
            // Set new body material (dark)
            bodyRenderer.material = Resources.Load<Material>("People/Man/Materials/ManBodyBlack");
        }
        // Woman
        if (_gender.Equals("Woman"))
        {
            // Set new skin and eyes (dark)
            if (Random.Range(0, 2).Equals(1))
            {
                // Get body renderer
                SkinnedMeshRenderer bodyRenderer = transform.Find("WomanBody")
                    .GetComponent<SkinnedMeshRenderer>();
                // Set new eyes material (dark)
                eyesRenderer.material = eyesMaterials[eyesMaterials.Length - 1];
                // Set new body material (dark)
                bodyRenderer.material = Resources.Load<Material>("People/Woman/Materials/WomanBodyDusky");
            }
            // Find fingernails
            SkinnedMeshRenderer skinnedMeshRenderer =
                transform.Find("WomanFingernails").GetComponent<SkinnedMeshRenderer>();
            // Set fingernails
            skinnedMeshRenderer.material =
                Resources.Load<Material>("People/Woman/Materials/Woman0" + humanIndex + "Fingernails");
            // Find high heels
            skinnedMeshRenderer = transform.Find("WomanHighHeels").GetComponent<SkinnedMeshRenderer>();
            // Set high heels
            skinnedMeshRenderer.material =
                Resources.Load<Material>("People/Woman/Materials/Woman0" + humanIndex + "HighHeels");
            // Find panty
            skinnedMeshRenderer = transform.Find("WomanPanty").GetComponent<SkinnedMeshRenderer>();
            // Set panty
            skinnedMeshRenderer.material =
                Resources.Load<Material>("People/Woman/Materials/Woman0" + humanIndex + "Panty");
        }
        // Set proper human area
        GameObject area = transform.parent.gameObject;
        // Get all area children
        Transform[] areaPoints = area.GetComponentsInChildren<Transform>();
        // Create temporary list
        List<Transform> wayPointsList = new List<Transform>();
        // Search way points
        foreach (Transform areaPoint in areaPoints)
        {
            // Check if it is way point
            if (areaPoint.name.Equals("Look Point") || areaPoint.name.Equals("Way Point"))
                // Set way point
                wayPointsList.Add(areaPoint);
        }
        // Transform list
        _targets = wayPointsList.ToArray();
        _agent = gameObject.GetComponent<NavMeshAgent>();
        _animator = gameObject.GetComponent<Animator>();
        _isWalking = _isRotatingRight = _isRotatingLeft = _isAdmiring = _isTurning = _isWatching = false;
        _currentAction = ActionType.Idling;
        _currentTarget = 0;
        _currentTime = 0f;
        _standardOffset = _agent.baseOffset;
        _animator.SetBool(_animTurning, _isTurning);
        _animator.SetBool(_animAdmiring, _isAdmiring);
        _animator.SetBool(_animWalk, _isWalking);
        _animator.SetBool(_animRotateRight, _isRotatingRight);
        _animator.SetBool(_animRotateLeft, _isRotatingLeft);
    }

    // Wait a while in postion
    private void WaitAWhile()
    {
        // Check waiting time
        if (_currentTime > WaitingTime)
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
        _agent.SetDestination(_targets[_currentTarget].position);
        // Destination is reached
        if (!_agent.pathPending && _agent.remainingDistance < _agent.stoppingDistance)
        {
            // Check target type - bench or monument
            if (_targets[_currentTarget].name.Equals("Look Point"))
            {
                // Set animation
                _agent.isStopped = true;
                _isWalking = false;
                _animator.SetBool(_animWalk, _isWalking);
                // Set rotating action
                _currentAction = ActionType.Rotating;
                // Break action
                return;
            }
            // Set new way point
            else
            {
                // Set animation
                _isWalking = true;
                _animator.SetBool(_animWalk, _isWalking);
                // Set next target
                SetNextTarget();
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
        // Get child
        Transform target = _targets[_currentTarget].GetChild(0);
        // Get look rotation
        _lookRotation = Quaternion.LookRotation(target.position - transform.position);
        // Check if rotatin is completed
        if (Quaternion.Angle(transform.rotation, _lookRotation) < AngleAccuracy)
        {
            // Check target type - rotation
            if (target.name.Equals("Rotation Point"))
            {
                // Change state
                _currentAction = ActionType.Idling;
                // Set new target
                SetNextTarget();
            }
            // Check target type - bench
            if (target.name.Equals("Sit Point"))
                // Set turning action
                _currentAction = ActionType.Turning;
            // Check target type - monument
            if (target.name.Equals("Admire Point"))
                // Set admiring action
                _currentAction = ActionType.Admiring;
            // Check target type - window
            if (target.name.Equals("Watch Point"))
                // Set admiring action
                _currentAction = ActionType.Watching;
            // Change values
            _isRotatingLeft = _isRotatingRight = false;
            // Set animation
            _animator.SetBool(_animRotateRight, _isRotatingRight);
            _animator.SetBool(_animRotateLeft, _isRotatingLeft);
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
        transform.rotation = Quaternion.Slerp(transform.rotation, _lookRotation, Time.deltaTime * RotationSpeed);
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
        if (_translationTime >= 1.2f)
        {
            // Reset translation time
            _translationTime = 0f;
            // Set waiting action
            _currentAction = ActionType.Waiting;
            // Break action
            return;
        }
        // Set new nav mesh offset
        _agent.baseOffset = SittingOffset;
        // Increase translation time
        _translationTime += Time.deltaTime;
        transform.Translate(new Vector3(0f, 0f, Time.deltaTime / 2.5f), Space.Self);
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
        // Set next target
        SetNextTarget();
        // Set standard offset
        _agent.baseOffset = _standardOffset;
        // Set idling action
        _currentAction = ActionType.Idling;
    }

    // Stand front of monument and start looking at it
    private void AdmireMonument()
    {
        // Check time
        if (_currentTime > ActionTime)
        {
            // Set animation
            _isAdmiring = false;
            _animator.SetBool(_animAdmiring, _isAdmiring);
            // Reset time
            _currentTime = 0f;
            // Change state
            _currentAction = ActionType.Idling;
            // Set new target
            SetNextTarget();
            // Break action
            return;
        }
        // Increase time
        _currentTime += Time.deltaTime;
        // Set animation
        _isAdmiring = true;
        _animator.SetBool(_animAdmiring, _isAdmiring);
    }

    // Stand front of window and start looking through
    private void WatchThroughWindow()
    {
        // Check time
        if (_currentTime > ActionTime)
        {
            // Set animation
            _isWatching = false;
            _animator.SetBool(_animWatching, _isWatching);
            // Reset time
            _currentTime = 0f;
            // Change state
            _currentAction = ActionType.Idling;
            // Set new target
            SetNextTarget();
            // Break action
            return;
        }
        // Increase time
        _currentTime += Time.deltaTime;
        // Set animation
        _isWatching = true;
        _animator.SetBool(_animWatching, _isWatching);
    }

    // Set new target after destination
    private void SetNextTarget()
    {
        // Check current target
        if (_currentTarget.Equals(_targets.Length - 1))
            // Reset path
            _currentTarget = 0;
        // Set another target
        else
            _currentTarget++;
    }

    // Switch human actions
    private void SwitchActions()
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
            case ActionType.Admiring:
                AdmireMonument();
                break;
            case ActionType.Watching:
                WatchThroughWindow();
                break;
        }
    }
}