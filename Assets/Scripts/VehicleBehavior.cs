using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

/// <summary>
/// Controls the behavior of a selected vehicle during the simulation.
/// </summary>
public class VehicleBehavior : MonoBehaviour
{
    public struct TrafficPoint
    {
        public string ID;
        public AdjustTrafficLights TrafficLights;
    };
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
    [Range(0.25f, 1.5f)]
    public float WaitingTime;
    // Stoping distance during riding
    [Range(2f, 4f)]
    public float StoppingDistance;
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
    // Angular speed
    private float _angularSpeed;
    // Nav mesh path
    private NavMeshPath _path;
    // Front point of this vehicle
    private Transform _frontPoint;
    // Back of vehicles
    private Transform[] _backPoints;
    // Traffic points
    private TrafficPoint[] _trafficPoints;
    // Rear lights color (ride)
    private Color _rideColor = new Color(0.2f, 0f, 0f);
    // Rear lights color (stop)
    private Color _stopColor = new Color(0.8f, 0f, 0f);
    // Rear lights renderer
    private Renderer _rearLights;
    // Material property block
    private MaterialPropertyBlock _matBlock;

    // Awake is called when the script instance is being loaded
    private void Awake()
    {
        Init();
    }

    // Start is called before the first frame update
    private void Start()
    {
        PrepareVehicles();
        PrepareDriver();
    }

    // Initializate parameters
    private void Init()
    {
        // Set vehicle name
        name = name.Replace("(Clone)", "");
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
        // Create block
        _matBlock = new MaterialPropertyBlock();
        // Find rear lights
        _rearLights = transform.Find("Car/RearLights").GetComponent<Renderer>();
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
        // Create temporary list for way points
        List<Transform> wayPointsList = new List<Transform>();
        // Create temporary list for traffic points
        List<TrafficPoint> trafficPointsList = new List<TrafficPoint>();
        // Search way points
        foreach (Transform areaPoint in areaPoints)
        {
            // Check if it is way point
            if (areaPoint.name.Equals("Way Point"))
                // Set way point
                wayPointsList.Add(areaPoint);
            // Check if it is traffic point
            if (areaPoint.name.Equals("Traffic Light Point"))
            {
                // Set way point
                wayPointsList.Add(areaPoint);
                // Get proper objects
                GameObject[] objects = GameObject.FindGameObjectsWithTag(areaPoint.tag);
                // Prepare counter
                int cnt = 0;
                // Search obtained objects
                for (cnt = 0; cnt < objects.Length; cnt++)
                {
                    // There are traffic lights
                    if (objects[cnt].name.Contains("Lights"))
                        // Break loop
                        break;
                }
                // Set proper script
                AdjustTrafficLights trafficScript = objects[cnt].GetComponent<AdjustTrafficLights>();
                // Create temporary traffic point
                TrafficPoint trafficPoint = new TrafficPoint();
                // Set ID
                trafficPoint.ID = trafficScript.tag;
                // Set state
                trafficPoint.TrafficLights = trafficScript;
                // Add point to list
                trafficPointsList.Add(trafficPoint);
            }
        }
        // Transform lists
        _destinations = wayPointsList.ToArray();
        _trafficPoints = trafficPointsList.ToArray();
        _path = new NavMeshPath();
        _isMoving = false;
        _curAction = VehicleActionType.Stopping;
        _angularSpeed = _agent.angularSpeed;
        _curDestination = 0;
        _curTime = 0f;
    }

    /// <summary>
    /// Searches the vehicle befor the start of the symulation.
    /// It allows to obtain the information about the front and the back of the specific vehicle.
    /// </summary>
    private void PrepareVehicles()
    {
        // Get all vehicles
        GameObject[] vehicles = GameObject.FindGameObjectsWithTag("Vehicle");
        // Create temporary list of back points
        List<Transform> backPoints = new List<Transform>();
        // Search vehicles
        foreach (GameObject vehicle in vehicles)
        {
            // Check vehicle (compare parent of parent (zones))
            if (vehicle.transform.parent.parent.name.Equals(transform.parent.parent.name)
                && !vehicle.transform.parent.name.Equals(transform.parent.name))
                // Add navigation point to list
                backPoints.Add(vehicle.transform.Find("Back"));
        }
        // Transform list
        _backPoints = backPoints.ToArray();
        // Set front point of this vehicle
        _frontPoint = transform.Find("Front");
    }

    private void PrepareDriver()
    {
        // Find game controller and get proper script
        GenerateObject generateObject = GameObject.Find("Game Controller")
            .GetComponent<GenerateObject>();
        // Find man point
        Transform manPoint = transform.Find("ManDriver");
        // Find woman point
        Transform womanPoint = transform.Find("WomanDriver");
        // Generate driver
        GameObject driver = generateObject.GenerateDriver(transform, manPoint, womanPoint);
        // Disable nav mesh agent
        driver.GetComponent<NavMeshAgent>().enabled = false;
        // Disable behavior script
        driver.GetComponent<HumanBehavior>().enabled = false;
        // Create new animator controller
        RuntimeAnimatorController controller =
            Resources.Load<RuntimeAnimatorController>("Animators/Driver");
        // Get animator
        Animator animator = driver.GetComponent<Animator>();
        // Set driver animation controller
        animator.runtimeAnimatorController = controller;
    }

    // Update is called once per frame
    private void Update()
    {
        SwitchVehicleActions();
    }

    // Set color of rear lights
    private void SetRearLights(Color color)
    {
        // Set emission
        _matBlock.SetColor("_EmissionColor", color);
        // Apply changes
        _rearLights.SetPropertyBlock(_matBlock);
    }

    /// <summary>
    /// Stops the vehicle in the specific position.
    /// The stopped car stays in the position some time.
    /// </summary>
    private void StopVehicleAndWait()
    {
        // Set rear lights color
        SetRearLights(_stopColor);
        // Reset angular speed
        _agent.angularSpeed = 0f;
        // Stop vehicle
        _agent.isStopped = true;
        _isMoving = false;
        // Check waiting time
        if (_curTime > WaitingTime)
        {
            // Reset current time
            _curTime = 0f;
            // Check if it is traffic light point
            if (_destinations[_curDestination].name.Equals("Traffic Light Point"))
            {
                // Find this point and check state
                foreach (TrafficPoint trafficPoint in _trafficPoints)
                {
                    // Check ID
                    if (!trafficPoint.ID.Equals(_destinations[_curDestination].tag))
                        // Go to next point
                        continue;
                    // Traffic lights are red
                    if (!trafficPoint.TrafficLights.IsActive)
                        // Break action
                        return;
                    // Break loop
                    break;
                }
            }
            // Check if some vehicle is near
            if (IsVehicleNear())
                // Break action
                return;
            // Set riding action
            _curAction = VehicleActionType.Riding;
            // Set next destination
            SetNextVehicleDestinaton();
            // Set angular speed
            _agent.angularSpeed = _angularSpeed;
            // Break action
            return;
        }
        // Increase time
        _curTime += Time.deltaTime;
        // Set proper animations
        SetVehicleAnimation();
    }

    /// <summary>
    /// Naivgates the specific vehicle to the destination.
    /// </summary>
    private void RideToDestination()
    {
        // Check vehicles distance
        if (IsVehicleNear())
        {
            // Stop vehicle
            _curAction = VehicleActionType.Stopping;
            // Break action
            return;
        }
        // Set rear lights color
        SetRearLights(_rideColor);
        // Set new path
        SetNewVehiclePath(_curDestination);
        // Check conditions
        if (_agent.remainingDistance < CheckingDistance)
        {
            // Check if it is traffic light point
            if (_destinations[_curDestination].name.Equals("Traffic Light Point"))
            {
                // Find this point and check state
                foreach (TrafficPoint trafficPoint in _trafficPoints)
                {
                    // Check ID
                    if (!trafficPoint.ID.Equals(_destinations[_curDestination].tag))
                        // Go to next point
                        continue;
                    // Traffic lights are green
                    if (trafficPoint.TrafficLights.IsActive)
                        // Break loop
                        break;
                    // Stop vehicle (traffic lights are red)
                    _curAction = VehicleActionType.Stopping;
                    // Break action
                    return;
                }
            }
            // Set next vehicle destination
            SetNextVehicleDestinaton();
        }
        // Rotate vehicle
        transform.eulerAngles = new Vector3(_destinations[_curDestination].eulerAngles.x,
            transform.eulerAngles.y, transform.eulerAngles.z);
        // Move vehicle
        _agent.isStopped = false;
        _isMoving = true;
        // Set proper animations
        SetVehicleAnimation();
    }

    /// <summary>
    /// Sets the proper destination for the vehicle.
    /// If the route is over then is resetting.
    /// </summary>
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
    /// Calculates the next path for the vehicle and sets the proper destination.
    /// </summary>
    private void SetNewVehiclePath(int destIndex)
    {
        // Calculate path
        _agent.CalculatePath(_destinations[destIndex].position, _path);
        // Assign new path
        _agent.SetPath(_path);
    }

    /// <summary>
    /// Sets the proper animation for the vehicle.
    /// If the car stays in position then the wheels does not rotate.
    /// </summary>
    private void SetVehicleAnimation()
    {
        // Search animators
        foreach (Animator animator in _animators)
            // Set animation
            animator.SetBool("isMoving", _isMoving);
    }

    /// <summary>
    /// Checks distance between the vehicles in the same zone.
    /// If some car is near then the car stops.
    /// The actions include the cars that are behind some vehicle at time.
    /// </summary>
    private bool IsVehicleNear()
    {
        // Search vehicles
        foreach (Transform backPoint in _backPoints)
        {
            // Calculate distance between front point and back points
            float dist = Vector3.Distance(_frontPoint.position, backPoint.position);
            // Check distance
            if (dist <= StoppingDistance)
                // Some vehicle is near
                return true;
        }
        // Vehicles are far
        return false;
    }

    /// <summary>
    /// Switches the actions of the vehicle.
    /// Based on the current action, it determines the state of the vehicle.
    /// </summary>
    private void SwitchVehicleActions()
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