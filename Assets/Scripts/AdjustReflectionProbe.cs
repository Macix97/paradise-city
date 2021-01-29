using UnityEngine;

/// <summary>
/// Manages the state of the reflection probe.
/// </summary>
public class AdjustReflectionProbe : MonoBehaviour
{
    // Day and night cycle
    private DayAndNightCycle _dayAndNightCycle;
    // Reflection probe
    private ReflectionProbe _reflectionProbe;
    // Twilight texture
    private Texture _twilightTex;
    // Day texture
    private Texture _dayTex;
    // Night texture
    private Texture _nightTex;
    private float _currentTime;
    private float _secondsInAFullDay;
    // Blending factor
    private float _blendFac;
    // Current quality level
    private int _curLvl;

    // Start is called before the first frame update
    private void Start()
    {
        Init();
    }

    // Update is called once per frame
    private void Update()
    {
        LoadProperTextures();
        AdjustReflection();
    }

    // Initializate parameters
    private void Init()
    {
        // Get day and night cycle script
        _dayAndNightCycle = GameObject.Find("Day And Night Cycle").GetComponent<DayAndNightCycle>();
        // Get reflection probe
        _reflectionProbe = gameObject.GetComponent<ReflectionProbe>();
        // Set probe intensity
        _currentTime = _dayAndNightCycle.CurrentTime;
        _secondsInAFullDay = _dayAndNightCycle.SecondsInAFullDay;
        _curLvl = -1;
        _blendFac = 0f;
    }

    /// <summary>
    /// Loads proper textures depending on the current quality settings.
    /// </summary>
    private void LoadProperTextures()
    {
        // Get current settings
        int level = QualitySettings.GetQualityLevel();
        // Check current quality level
        if (!_curLvl.Equals(level))
        {
            // Set current level
            _curLvl = level;
            // Get proper name
            string curName = QualitySettings.names[level];
            // Load twilight texture
            _twilightTex = Resources.Load<Texture>("Light/" + curName + "/Twilight/" + gameObject.name);
            // Load day texture
            _dayTex = Resources.Load<Texture>("Light/" + curName + "/Day/" + gameObject.name);
            // Load night texture
            _nightTex = Resources.Load<Texture>("Light/" + curName + "/Night/" + gameObject.name);
        }
    }

    /// <summary>
    /// Sets proper textures depending on the current time of a day.
    /// </summary>
    private void AdjustReflection()
    {
        _currentTime = _dayAndNightCycle.CurrentTime;
        _secondsInAFullDay = _dayAndNightCycle.SecondsInAFullDay;
        // Night
        if (_currentTime <= 0.2f)
            _reflectionProbe.customBakedTexture = _nightTex;
        // Night to twilight
        if (_currentTime > 0.2f && _currentTime <= 0.25f)
            SetProperCubemap(_nightTex, _twilightTex, false);
        // Twilight to day
        if (_currentTime > 0.25f && _currentTime <= 0.3f)
            SetProperCubemap(_dayTex, _twilightTex, true);
        // Afternoon
        if (_currentTime > 0.3f && _currentTime <= 0.7f)
            _reflectionProbe.customBakedTexture = _dayTex;
        // Day to twilight
        if (_currentTime > 0.7f && _currentTime <= 0.75f)
            SetProperCubemap(_dayTex, _twilightTex, false);
        // Twilight to night
        if (_currentTime > 0.75f && _currentTime <= 0.8f)
            SetProperCubemap(_nightTex, _twilightTex, true);
        // Evening
        if (_currentTime > 0.8f)
            _reflectionProbe.customBakedTexture = _nightTex;
    }

    /// <summary>
    /// Creates and sets the temporary blended texture from two different maps.
    /// </summary>
    /// <param name="texture1">An object that represents the first texture.</param>
    /// <param name="texture1">An object that represents the second texture.</param>
    /// <param name="isReversed">A boolean that informs if the blended texture will be reversed.</param>
    private void SetProperCubemap(Texture texture1, Texture texture2, bool isReversed)
    {
        // Create render texture
        RenderTexture tmpBlend = new RenderTexture(_dayTex.width, _dayTex.height, 0);
        tmpBlend.dimension = UnityEngine.Rendering.TextureDimension.Cube;
        tmpBlend.useMipMap = true;
        // It is normal cubemap
        if (!isReversed)
        {
            // Increase blend factor
            _blendFac += Time.deltaTime / _secondsInAFullDay * 20;
            // Validate blend factor
            if (_blendFac > 1f)
                _blendFac = 1f;
        }
        // It is reversed cubemap
        else
        {
            // Decrease blend factor
            _blendFac -= Time.deltaTime / _secondsInAFullDay * 20;
            // Validate blend factor
            if (_blendFac < 0f)
                _blendFac = 0f;
        }
        // Generate texture
        ReflectionProbe.BlendCubemap(texture1, texture2, _blendFac, tmpBlend);
        // Set cubemap
        _reflectionProbe.customBakedTexture = tmpBlend;
    }
}