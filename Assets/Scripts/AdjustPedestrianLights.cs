using UnityEngine;

/// <summary>
/// Manages switching the pedestrian lights.
/// </summary>
public class AdjustPedestrianLights : MonoBehaviour
{
    // List of light actions
    public enum PedestrianActionType
    {
        StartingLighting,
        LightingStop,
        LightingWalk
    };
    // Green color
    private Color _green = new Color(0.176f, 0.788f, 0.215f);
    // Red color
    private Color _red = new Color(0.8f, 0.196f, 0.196f);
    // Pedestrian light
    private Renderer _light;
    // Material property block
    private MaterialPropertyBlock _matBlock;
    // Walk texture
    private Texture _walkTexture;
    // Stop texture
    private Texture _stopTexture;
    // Traffic lights script
    private AdjustTrafficLights _trafficLights;
    // Current state
    private PedestrianActionType _curState;
    // Current time
    private float _curTime;
    // Time to first lighting
    private float _startTime;
    // Time needed to start next cycle
    private float _cycleTime;
    // Waiting time
    private float _waitingTime;
    // Audio source
    private AudioSource _audioSrc;
    // Check if sound is playing
    private bool _isPlaying;

    // Start is called before the first frame update
    private void Start()
    {
        // Get all renderers
        Renderer[] renderers = GetComponentsInChildren<Renderer>();
        // Get audio source
        _audioSrc = GetComponent<AudioSource>();
        // Set proper clip
        _audioSrc.clip = Resources.Load<AudioClip>("Sounds/PedestrianLights");
        // Search renderers
        foreach (Renderer renderer in renderers)
        {
            // Check renderer
            if (renderer.name.Equals("Light"))
                // Set proper renderer
                _light = renderer;
        }
        // Get proper script
        _trafficLights = transform.parent.parent.GetChild(0)
            .GetChild(0).GetComponent<AdjustTrafficLights>();
        // Create block
        _matBlock = new MaterialPropertyBlock();
        // Load stop texture
        _stopTexture = Resources.Load<Texture>("Textures/TrafficLightsEmission01");
        // Load walk texture
        _walkTexture = Resources.Load<Texture>("Textures/TrafficLightsEmission02");
        // Set proper color
        SetPedestrianLights(_stopTexture, _red);
        // Get group name
        string groupName = transform.parent.name;
        // Create temporary index
        string groupIndex = groupName.Replace("Lights0", "");
        // Set proper index
        int index = System.Int32.Parse(groupIndex);
        // Set temporary short waiting time
        float shortWaitingTime = _trafficLights.ShortWaitingTime;
        // Set waiting time
        _waitingTime = _trafficLights.LongWaitingTime + (2 * shortWaitingTime);
        // Set start time
        _startTime = _waitingTime * (index - 1);
        // Get group size
        int groupSize = transform.parent.parent.childCount;
        // Set cycle time
        _cycleTime = _waitingTime * (groupSize - 1);
        _curTime = 0f;
        _isPlaying = false;
        // Set starting state
        _curState = PedestrianActionType.StartingLighting;
    }

    // Update is called once per frame
    private void Update()
    {
        SwitchPedestrianLights();
    }

    /// <summary>
    /// Sets proper color of the pedestrian lights.
    /// </summary>
    /// <param name="texture">An object that represents a texture.</param>
    /// <param name="color">A structure that represents a color.</param>
    private void SetPedestrianLights(Texture texture, Color color)
    {
        // Set texture
        _matBlock.SetTexture("_EmissionMap", texture);
        // Set emission
        _matBlock.SetColor("_EmissionColor", color);
        // Apply changes
        _light.SetPropertyBlock(_matBlock);
    }

    /// <summary>
    /// Switches states of the pedestrian lights basing on the current state.
    /// </summary>
    private void SwitchPedestrianLights()
    {
        switch (_curState)
        {
            case PedestrianActionType.StartingLighting:
                StartPedestrianLight();
                break;
            case PedestrianActionType.LightingStop:
                SetStopping();
                break;
            case PedestrianActionType.LightingWalk:
                SetWalking();
                break;
        }
    }

    /// <summary>
    /// Sets the beginning state of the pedestrian lights.
    /// </summary>
    private void StartPedestrianLight()
    {
        // Set proper colors of lights
        SetPedestrianLights(_stopTexture, _red);
        // Check waiting time
        if (_curTime > _startTime)
        {
            // Reset current time
            _curTime = 0f;
            // Set next state
            _curState = PedestrianActionType.LightingWalk;
            // Break action
            return;
        }
        // Increase time
        _curTime += Time.deltaTime;
    }

    /// <summary>
    /// Sets walking texture for the pedestrian lights.
    /// </summary>
    private void SetWalking()
    {
        // Set proper color of light
        SetPedestrianLights(_walkTexture, _green);
        // Check if sound is playing
        if (!_isPlaying)
        {
            // Play proper sound
            _audioSrc.Play();
            // Set that sound is playing
            _isPlaying = true;
        }
        // Check waiting time
        if (_curTime > _waitingTime)
        {
            // Reset current time
            _curTime = 0f;
            // Stop playing sound
            _audioSrc.Stop();
            // Set that sound is not playing
            _isPlaying = false;
            // Set next state
            _curState = PedestrianActionType.LightingStop;
            // Break action
            return;
        }
        // Increase time
        _curTime += Time.deltaTime;
    }

    /// <summary>
    /// Sets stopping texture for the pedestrian lights.
    /// </summary>
    private void SetStopping()
    {
        // Set proper color of light
        SetPedestrianLights(_stopTexture, _red);
        // Check waiting time
        if (_curTime > _cycleTime)
        {
            // Reset current time
            _curTime = 0f;
            // Set next state
            _curState = PedestrianActionType.LightingWalk;
            // Break action
            return;
        }
        // Increase time
        _curTime += Time.deltaTime;
    }
}