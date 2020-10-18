using UnityEngine;

// Modify reflection probe
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
    // Temporary blended texture
    private RenderTexture _tmpBlend;
    // Blend factor
    private float _blendFac;

    // Start is called before the first frame update
    private void Start()
    {
        Init();
    }

    // Update is called once per frame
    private void Update()
    {
        AdjustReflection();
    }

    // Initializate parameters
    private void Init()
    {
        // Get day and night cycle script
        _dayAndNightCycle = GameObject.Find("Day And Night Cycle").GetComponent<DayAndNightCycle>();
        // Get reflection probe
        _reflectionProbe = gameObject.GetComponent<ReflectionProbe>();
        // Load twilight texture
        _twilightTex = Resources.Load<Texture>("Light/Twilight/" + gameObject.name);
        // Load day texture
        _dayTex = Resources.Load<Texture>("Light/Day/" + gameObject.name);
        // Load night texture
        _nightTex = Resources.Load<Texture>("Light/Night/" + gameObject.name);
        // Set probe intensity
        _currentTime = _dayAndNightCycle.CurrentTime;
        _secondsInAFullDay = _dayAndNightCycle.SecondsInAFullDay;
        // Prepare blend texture
        _tmpBlend = new RenderTexture(_dayTex.width, _dayTex.height, 0);
        _tmpBlend.dimension = UnityEngine.Rendering.TextureDimension.Cube;
        _tmpBlend.useMipMap = true;
        _blendFac = 0f;
    }

    // Control reflection probe
    private void AdjustReflection()
    {
        _currentTime = _dayAndNightCycle.CurrentTime;
        _secondsInAFullDay = _dayAndNightCycle.SecondsInAFullDay;
        // Night
        if (_currentTime <= 0.2f)
            _reflectionProbe.customBakedTexture = _nightTex;
        // Night to twilight
        if (_currentTime > 0.2f && _currentTime <= 0.25f)
        {
            _blendFac += Time.deltaTime / _secondsInAFullDay * 20;
            // Validate blend factor
            if (_blendFac > 1f)
                _blendFac = 1f;
            ReflectionProbe.BlendCubemap(_nightTex, _twilightTex, _blendFac, _tmpBlend);
            _reflectionProbe.customBakedTexture = _tmpBlend;
        }
        // Twilight to day
        if (_currentTime > 0.25f && _currentTime <= 0.3f)
        {
            _blendFac -= Time.deltaTime / _secondsInAFullDay * 20;
            // Validate blend factor
            if (_blendFac < 0f)
                _blendFac = 0f;
            ReflectionProbe.BlendCubemap(_dayTex, _twilightTex, _blendFac, _tmpBlend);
            _reflectionProbe.customBakedTexture = _tmpBlend;
        }
        // Afternoon
        if (_currentTime > 0.3f && _currentTime <= 0.7f)
            _reflectionProbe.customBakedTexture = _dayTex;
        // Day to twilight
        if (_currentTime > 0.7f && _currentTime <= 0.75f)
        {
            _blendFac += Time.deltaTime / _secondsInAFullDay * 20;
            // Validate blend factor
            if (_blendFac > 1f)
                _blendFac = 1f;
            ReflectionProbe.BlendCubemap(_dayTex, _twilightTex, _blendFac, _tmpBlend);
            _reflectionProbe.customBakedTexture = _tmpBlend;
        }
        // Twilight to night
        if (_currentTime > 0.75f && _currentTime <= 0.8f)
        {
            _blendFac -= Time.deltaTime / _secondsInAFullDay * 20;
            // Validate blend factor
            if (_blendFac < 0f)
                _blendFac = 0f;
            ReflectionProbe.BlendCubemap(_nightTex, _twilightTex, _blendFac, _tmpBlend);
            _reflectionProbe.customBakedTexture = _tmpBlend;
        }
        // Evening
        if (_currentTime > 0.8f)
            _reflectionProbe.customBakedTexture = _nightTex;
    }
}