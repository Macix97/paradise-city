using UnityEngine;

/// <summary>
/// Controls the state of the selected traffic lights.
/// </summary>
public class AdjustTrafficLights : MonoBehaviour
{
    // List of traffic lights actions
    public enum TrafficActionType
    {
        StartingLighting,
        LightingYellowFirst,
        LightingGreen,
        LightingYellowSecond,
        LightingRed
    };
    [Range(10f, 20f)]
    // Long waiting time
    public float LongWaitingTime;
    [Range(1f, 2f)]
    // Short waiting time
    public float ShortWaitingTime;
    // Green color
    private Color _green = new Color(0.176f, 0.788f, 0.215f);
    // Dark green color
    private Color _greenDark = new Color(0.022f, 0.099f, 0.027f);
    // Yellow color
    private Color _yellow = new Color(0.905f, 0.705f, 0.086f);
    // Dark yellow color
    private Color _yellowDark = new Color(0.3f, 0.232f, 0.028f);
    // Red color
    private Color _red = new Color(0.8f, 0.196f, 0.196f);
    // Dark red color
    private Color _redDark = new Color(0.25f, 0.058f, 0.058f);
    // Green light renderer
    private Renderer _greenLight;
    // Yellow light renderer
    private Renderer _yellowLight;
    // Red light renderer
    private Renderer _redLight;
    // Material property block
    private MaterialPropertyBlock _matBlock;
    // Current state
    private TrafficActionType _curState;
    // Current time
    private float _curTime;
    // Time to first lighting
    private float _startTime;
    // Time needed to start next cycle
    private float _cycleTime;
    // Check if light is active
    internal bool IsActive;

    // Start is called before the first frame update
    private void Start()
    {
        Init();
    }

    // Initializate parameters
    private void Init()
    {
        // Get all renderers
        Renderer[] renderers = GetComponentsInChildren<Renderer>();
        // Search renderers
        foreach (Renderer renderer in renderers)
        {
            // It is green light
            if (renderer.name.Contains("Green"))
                _greenLight = renderer;
            // It is yellow light
            if (renderer.name.Contains("Yellow"))
                _yellowLight = renderer;
            // It is red light
            if (renderer.name.Contains("Red"))
                _redLight = renderer;
        }
        // Create block
        _matBlock = new MaterialPropertyBlock();
        // Set proper colors
        SetTrafficLights(ref _greenLight, _greenDark);
        SetTrafficLights(ref _yellowLight, _yellowDark);
        SetTrafficLights(ref _redLight, _redDark);
        // Get group name
        string groupName = transform.parent.name;
        // Create temporary index
        string groupIndex = groupName.Replace("Lights0", "");
        // Set proper index
        int index = System.Int32.Parse(groupIndex);
        _startTime = (LongWaitingTime + (ShortWaitingTime * 2)) * (index - 1);
        // Get group size
        int groupSize = transform.parent.parent.childCount;
        // Set cycle time
        _cycleTime = (LongWaitingTime + (ShortWaitingTime * 2)) * (groupSize - 1);
        IsActive = false;
        _curTime = 0f;
        // Set starting state
        _curState = TrafficActionType.StartingLighting;
    }

    // Update is called once per frame
    private void Update()
    {
        SwitchTrafficLights();
    }

    /// <summary>
    /// Sets the proper color of the traffic lights.
    /// </summary>
    private void SetTrafficLights(ref Renderer renderer, Color color)
    {
        // Set emission
        _matBlock.SetColor("_EmissionColor", color);
        // Apply changes
        renderer.SetPropertyBlock(_matBlock);
    }

    /// <summary>
    /// Switches the behavior of traffic lights based on the current state.
    /// </summary>
    private void SwitchTrafficLights()
    {
        switch (_curState)
        {
            case TrafficActionType.StartingLighting:
                StartTrafficLight();
                break;
            case TrafficActionType.LightingYellowFirst:
                LightYellowFirst();
                break;
            case TrafficActionType.LightingGreen:
                LightGreen();
                break;
            case TrafficActionType.LightingYellowSecond:
                LightYellowSecond();
                break;
            case TrafficActionType.LightingRed:
                LightRed();
                break;
        }
    }

    /// <summary>
    /// Establishes the beginning state of the traffic lights.
    /// </summary>
    private void StartTrafficLight()
    {
        // Set proper colors of lights
        SetTrafficLights(ref _yellowLight, _yellowDark);
        SetTrafficLights(ref _greenLight, _greenDark);
        SetTrafficLights(ref _redLight, _red);
        // Check waiting time
        if (_curTime > _startTime)
        {
            // Reset current time
            _curTime = 0f;
            // Set next state
            _curState = TrafficActionType.LightingYellowFirst;
            // Break action
            return;
        }
        // Increase time
        _curTime += Time.deltaTime;
    }

    /// <summary>
    /// Sets the yellow color of the traffic lights.
    /// The action starts when the red light is turned off.
    /// </summary>
    private void LightYellowFirst()
    {
        // Set proper colors of lights
        SetTrafficLights(ref _yellowLight, _yellow);
        SetTrafficLights(ref _greenLight, _greenDark);
        SetTrafficLights(ref _redLight, _redDark);
        // Check waiting time
        if (_curTime > ShortWaitingTime)
        {
            // Reset current time
            _curTime = 0f;
            // Set next state
            _curState = TrafficActionType.LightingGreen;
            // Break action
            return;
        }
        // Increase time
        _curTime += Time.deltaTime;
    }

    /// <summary>
    /// Sets the green color of the traffic lights.
    /// The action starts when the yellow light is turned off.
    /// </summary>
    private void LightGreen()
    {
        // Set proper colors of lights
        SetTrafficLights(ref _greenLight, _green);
        SetTrafficLights(ref _yellowLight, _yellowDark);
        SetTrafficLights(ref _redLight, _redDark);
        // Activate light
        IsActive = true;
        // Check waiting time
        if (_curTime > LongWaitingTime)
        {
            // Reset current time
            _curTime = 0f;
            // Deactivate light
            IsActive = false;
            // Set next state
            _curState = TrafficActionType.LightingYellowSecond;
            // Break action
            return;
        }
        // Increase time
        _curTime += Time.deltaTime;
    }

    /// <summary>
    /// Sets the yellow color of the traffic lights.
    /// The action starts when the green light is turned off.
    /// </summary>
    private void LightYellowSecond()
    {
        // Set proper colors of lights
        SetTrafficLights(ref _yellowLight, _yellow);
        SetTrafficLights(ref _greenLight, _greenDark);
        SetTrafficLights(ref _redLight, _redDark);
        // Check waiting time
        if (_curTime > ShortWaitingTime)
        {
            // Reset current time
            _curTime = 0f;
            // Set next state
            _curState = TrafficActionType.LightingRed;
            // Break action
            return;
        }
        // Increase time
        _curTime += Time.deltaTime;
    }

    /// <summary>
    /// Sets the red color of the traffic lights.
    /// The action starts when the yellow light is turned off.
    /// This stage begins the next cycle.
    /// </summary>
    private void LightRed()
    {
        // Set proper colors of lights
        SetTrafficLights(ref _redLight, _red);
        SetTrafficLights(ref _yellowLight, _yellowDark);
        SetTrafficLights(ref _greenLight, _greenDark);
        // Check waiting time
        if (_curTime > _cycleTime)
        {
            // Reset current time
            _curTime = 0f;
            // Set next state
            _curState = TrafficActionType.LightingYellowFirst;
            // Break action
            return;
        }
        // Increase time
        _curTime += Time.deltaTime;
    }
}