using UnityEngine;

// Modify street light
public class AdjustStreetLight : MonoBehaviour
{
    // Light intensity
    [Range(0, 5)]
    public float LightIntensity = 0f;
    // Light emission color
    public Color EmissionColor;
    // Day and night cycle
    private DayAndNightCycle _dayAndNightCycle;
    // Material property block
    private MaterialPropertyBlock _matBlock;
    // Renderer
    private Renderer _lightRenderer;
    // Spot light
    private Light _spotLight;
    private float _currentTime;
    private float _secondsInAFullDay;
    // Red color
    private float _r;
    // Green color
    private float _g;
    // Blue color
    private float _b;

    // Start is called before the first frame update
    private void Start()
    {
        Init();
    }

    // Update is called once per frame
    private void Update()
    {
        AdjustLight();
    }

    // Initializate parameters
    private void Init()
    {
        // Get day and night cycle script
        _dayAndNightCycle = GameObject.Find("Day And Night Cycle").GetComponent<DayAndNightCycle>();
        // Get spot light
        _spotLight = gameObject.GetComponentInChildren<Light>();
        // Set light intensity
        LightIntensity = _spotLight.intensity = 0f;
        // Create block
        _matBlock = new MaterialPropertyBlock();
        // Get street light renderer
        _lightRenderer = gameObject.GetComponent<Renderer>();
        // Set colors
        _r = _g = _b = 0f;
        // Check if it is traffic post
        if (gameObject.name.Equals("Traffic Post"))
            // Set proper emission map
            _matBlock.SetTexture("_EmissionMap", Resources.Load<Texture>("Textures/TrafficLightsEmission03"));
        // Disable emission
        _matBlock.SetColor("_EmissionColor", new Color(0f, 0f, 0f, 1f));
        // Apply changes
        _lightRenderer.SetPropertyBlock(_matBlock);
    }

    // Control street light
    private void AdjustLight()
    {
        _currentTime = _dayAndNightCycle.CurrentTime;
        _secondsInAFullDay = _dayAndNightCycle.SecondsInAFullDay;
        // Turn off the light
        if (_currentTime > 0.2f && _currentTime <= 0.25f)
        {
            LightIntensity -= Time.deltaTime / _secondsInAFullDay * 100;
            _r -= Time.deltaTime / _secondsInAFullDay * EmissionColor.r * 20;
            _g -= Time.deltaTime / _secondsInAFullDay * EmissionColor.g * 20;
            _b -= Time.deltaTime / _secondsInAFullDay * EmissionColor.b * 20;
            // Validate colors
            if (_r < 0f || _g < 0f || _b < 0f)
                _r = _g = _b = 0f;
        }
        // Turn on the light
        if (_currentTime > 0.75f && _currentTime <= 0.8f)
        {
            LightIntensity += Time.deltaTime / _secondsInAFullDay * 100;
            _r += Time.deltaTime / _secondsInAFullDay * EmissionColor.r * 20;
            _g += Time.deltaTime / _secondsInAFullDay * EmissionColor.g * 20;
            _b += Time.deltaTime / _secondsInAFullDay * EmissionColor.b * 20;
            // Validate colors
            if (_r > EmissionColor.r || _g > EmissionColor.g || _b > EmissionColor.b)
            {
                _r = EmissionColor.r;
                _g = EmissionColor.g;
                _b = EmissionColor.b;
            }
        }
        // Validate light
        if (LightIntensity < 0f)
            LightIntensity = 0f;
        if (LightIntensity > 5f)
            LightIntensity = 5f;
        // Set light
        _spotLight.intensity = LightIntensity;
        // Apply changes
        _lightRenderer.SetPropertyBlock(_matBlock);
        // Set emission
        _matBlock.SetColor("_EmissionColor", new Color(_r, _g, _b, 1f));
    }
}