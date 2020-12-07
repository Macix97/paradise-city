using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

/// <summary>
/// Controls the behavior of a selected vehicle during the simulation.
/// </summary>
public class VehicleBehavior : MonoBehaviour
{
    // List of vehicle actions
    public enum VehicleActionType
    {
        Stopping,
        Riding,
    };
    // Checking distance during riding
    [Range(2f, 6f)]
    public float CheckingDistance;
    // Waiting time in way point
    [Range(1f, 3f)]
    public float WaitingTime;
    // Destinations (way points)
    private Transform[] _destinations;
    // Nav mesh agent
    private NavMeshAgent _agent;
    // Animators
    private Animator[] _animators;
    // Check if vehicle is moving
    private bool _isMoving;
    // Current destination index
    private int _curDestination;
    // Current action
    private VehicleActionType _curAction;
    // Current waiting time
    private float _curTime;
    // Nav mesh path
    private NavMeshPath _path;

    // Awake is called when the script instance is being loaded
    private void Awake()
    {
        Init();
    }

    // Initializate parameters
    private void Init()
    {
        // Load paints
        Material[] paints = Resources.LoadAll<Material>("Vehicles/Paints");
        // Random car color
        int paintIndex = Random.Range(0, paints.Length);
        // Get car mesh renderer
        MeshRenderer renderer = transform.Find("Car/Body").GetComponent<MeshRenderer>();
        // Set new color
        renderer.material = paints[paintIndex];
        _agent = GetComponentInChildren<NavMeshAgent>();
        _animators = GetComponentsInChildren<Animator>();
        // Search animators
        foreach (Animator animator in _animators)
        {
            // It is right wheel
            if (animator.transform.localEulerAngles.y.Equals(180))
                // Set animation
                animator.SetBool("isRight", true);
            // It is left wheel
            else
                // Set animation
                animator.SetBool("isLeft", true);
        }
        // Set proper vehicle area
        GameObject area = transform.parent.gameObject;
        // Get all area children
        Transform[] areaPoints = area.GetComponentsInChildren<Transform>();
        // Create temporary list
        List<Transform> wayPointsList = new List<Transform>();
        // Search way points
        foreach (Transform areaPoint in areaPoints)
        {
            // Check if it is way point
            if (areaPoint.name.Equals("Way Point"))
                // Set way point
                wayPointsList.Add(areaPoint);
        }
        // Transform list
        _destinations = wayPointsList.ToArray();
        _path = new NavMeshPath();
        _isMoving = false;
        _curAction = VehicleActionType.Stopping;
        _curDestination = 0;
        _curTime = 0f;
    }

    // Update is called once per frame
    private void Update()
    {
        SwitchActions();
    }

    // Stop vehicle and wait
    private void StopVehicleAndWait()
    {
        // Check waiting time
        if (_curTime > WaitingTime)
        {
            // Reset current time
            _curTime = 0f;
            // Set riding action
            _curAction = VehicleActionType.Riding;
            // Break action
            return;
        }
        // Increase time
        _curTime += Time.deltaTime;
    }

    /// <summary>
    /// Naivgates specific vehicle to the destination.
    /// </summary>
    private void RideToDestination()
    {

        // TODO: Stop vehicle in specific situation

        // Set new path
        SetNewVehiclePath(_curDestination);
        // Check conditions
        if (_agent.remainingDistance < CheckingDistance)
            // Set next vehicle destination
            SetNextVehicleDestinaton();
        // Rotate vehicle
        transform.eulerAngles = new Vector3(_destinations[_curDestination].eulerAngles.x,
            transform.eulerAngles.y, transform.eulerAngles.z);
        // Move vehicle
        _agent.isStopped = false;
        _isMoving = true;
        // Search animators
        foreach (Animator animator in _animators)
            // Set animation
            animator.SetBool("isMoving", _isMoving);
    }

    // Set new vehicle destination
    private void SetNextVehicleDestinaton()
    {
        // Check current destination
        if (_curDestination.Equals(_destinations.Length - 1))
            // Reset path
            _curDestination = 0;
        // Set another destination
        else
            _curDestination++;
    }

    /// <summary>
    /// Calculates the next path for the vehicle and sets proper destination.
    /// </summary>
    private void SetNewVehiclePath(int destIndex)
    {
        // Calculate path
        _agent.CalculatePath(_destinations[destIndex].position, _path);
        // Assign new path
        _agent.SetPath(_path);
    }

    // Switch human actions
    private void SwitchActions()
    {
        switch (_curAction)
        {
            case VehicleActionType.Stopping:
                StopVehicleAndWait();
                break;
            case VehicleActionType.Riding:
                RideToDestination();
                break;
        }
    }
}