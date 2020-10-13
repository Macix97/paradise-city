using UnityEngine;

// Time of a day colors
[System.Serializable]
public class DayColors
{
    public Color SkyColor;
    public Color EquatorColor;
    public Color GroundColor;
    public Color FogColor;
}

// Control day and night cycle
public class DayAndNightCycle : MonoBehaviour
{
    public DayColors TwilightColors;
    public DayColors DayColors;
    public DayColors NightColors;
    public int CurrentDay = 0;
    public Light Sun;
    public Light Moon;
    public Material SkyMaterial;
    public float SecondsInAFullDay = 0f;
    [Range(0, 1)]
    public float CurrentTime = 0f;
    private float _lightIntensity;
    // Flare strength
    private float _flareStrength;
    // Blend factor
    private float _blendFac;
    // Red colors (sky, equator, ground, fog)
    private float _rS;
    private float _rE;
    private float _rG;
    private float _rF;
    // Green colors (sky, equator, ground, fog)
    private float _gS;
    private float _gE;
    private float _gG;
    private float _gF;
    // Blue colors (sky, equator, ground, fog)
    private float _bS;
    private float _bE;
    private float _bG;
    private float _bF;
    // Difference between twilight and day colors (r, g, b)
    private float _twilightDayDiffRS;
    private float _twilightDayDiffRE;
    private float _twilightDayDiffRG;
    private float _twilightDayDiffRF;
    private float _twilightDayDiffGS;
    private float _twilightDayDiffGE;
    private float _twilightDayDiffGG;
    private float _twilightDayDiffGF;
    private float _twilightDayDiffBS;
    private float _twilightDayDiffBE;
    private float _twilightDayDiffBG;
    private float _twilightDayDiffBF;
    // Difference b_etween twilight and night colors (r, g, b)
    private float _twilightNightDiffRS;
    private float _twilightNightDiffRE;
    private float _twilightNightDiffRG;
    private float _twilightNightDiffRF;
    private float _twilightNightDiffGS;
    private float _twilightNightDiffGE;
    private float _twilightNightDiffGG;
    private float _twilightNightDiffGF;
    private float _twilightNightDiffBS;
    private float _twilightNightDiffBE;
    private float _twilightNightDiffBG;
    private float _twilightNightDiffBF;

    // Start is called before the first frame update
    private void Start()
    {
        Init();
    }

    // Update is called once per frame
    private void Update()
    {
        UpdateLight();
        UpdateTime();
    }

    // Initializate parameters
    private void Init()
    {
        // Set intensity
        _lightIntensity = 1f;
        // Set blend factor
        _blendFac = 1f;
        // Set sky blending
        SkyMaterial.SetFloat("_BlendCubemaps", _blendFac);
        // Set flare strength
        _flareStrength = RenderSettings.flareStrength = 1f;
        // Start at noon
        CurrentTime = 0.5f;
        // Set sky color
        RenderSettings.ambientSkyColor = DayColors.SkyColor;
        // Set equator color
        RenderSettings.ambientEquatorColor = DayColors.EquatorColor;
        // Set ground color
        RenderSettings.ambientGroundColor = DayColors.GroundColor;
        // Set fog color
        RenderSettings.fogColor = DayColors.FogColor;
        // Red colors (sky, equator, ground, fog)
        _rS = DayColors.SkyColor.r;
        _rE = DayColors.EquatorColor.r;
        _rG = DayColors.GroundColor.r;
        _rF = DayColors.FogColor.r;
        // Green colors (sky, equator, ground, fog)
        _gS = DayColors.SkyColor.g;
        _gE = DayColors.EquatorColor.g;
        _gG = DayColors.GroundColor.g;
        _gF = DayColors.FogColor.g;
        // Blue colors (sky, equator, ground, fog)
        _bS = DayColors.SkyColor.b;
        _bE = DayColors.EquatorColor.b;
        _bG = DayColors.GroundColor.b;
        _bF = DayColors.FogColor.b;
        CalculateColors();
    }

    // Calculate difference between colors
    private void CalculateColors()
    {
        // Adjust difference
        // Day
        // Red
        _twilightDayDiffRS = Mathf.Abs(TwilightColors.SkyColor.r - DayColors.SkyColor.r) * 20;
        _twilightDayDiffRE = Mathf.Abs(TwilightColors.EquatorColor.r - DayColors.EquatorColor.r) * 20;
        _twilightDayDiffRG = Mathf.Abs(TwilightColors.GroundColor.r - DayColors.GroundColor.r) * 20;
        _twilightDayDiffRF = Mathf.Abs(TwilightColors.FogColor.r - DayColors.FogColor.r) * 20;
        // Green
        _twilightDayDiffGS = Mathf.Abs(TwilightColors.SkyColor.g - DayColors.SkyColor.g) * 20;
        _twilightDayDiffGE = Mathf.Abs(TwilightColors.EquatorColor.g - DayColors.EquatorColor.g) * 20;
        _twilightDayDiffGG = Mathf.Abs(TwilightColors.GroundColor.g - DayColors.GroundColor.g) * 20;
        _twilightDayDiffGF = Mathf.Abs(TwilightColors.FogColor.g - DayColors.FogColor.g) * 20;
        // Blue
        _twilightDayDiffBS = Mathf.Abs(TwilightColors.SkyColor.b - DayColors.SkyColor.b) * 20;
        _twilightDayDiffBE = Mathf.Abs(TwilightColors.EquatorColor.b - DayColors.EquatorColor.b) * 20;
        _twilightDayDiffBG = Mathf.Abs(TwilightColors.GroundColor.b - DayColors.GroundColor.b) * 20;
        _twilightDayDiffBF = Mathf.Abs(TwilightColors.FogColor.b - DayColors.FogColor.b) * 20;
        // Night
        // Red
        _twilightNightDiffRS = Mathf.Abs(TwilightColors.SkyColor.r - NightColors.SkyColor.r) * 20;
        _twilightNightDiffRE = Mathf.Abs(TwilightColors.EquatorColor.r - NightColors.EquatorColor.r) * 20;
        _twilightNightDiffRG = Mathf.Abs(TwilightColors.GroundColor.r - NightColors.GroundColor.r) * 20;
        _twilightNightDiffRF = Mathf.Abs(TwilightColors.FogColor.r - NightColors.FogColor.r) * 20;
        // Green
        _twilightNightDiffGS = Mathf.Abs(TwilightColors.SkyColor.g - NightColors.SkyColor.g) * 20;
        _twilightNightDiffGE = Mathf.Abs(TwilightColors.EquatorColor.g - NightColors.EquatorColor.g) * 20;
        _twilightNightDiffGG = Mathf.Abs(TwilightColors.GroundColor.g - NightColors.GroundColor.g) * 20;
        _twilightNightDiffGF = Mathf.Abs(TwilightColors.FogColor.g - NightColors.FogColor.g) * 20;
        // Blue
        _twilightNightDiffBS = Mathf.Abs(TwilightColors.SkyColor.b - NightColors.SkyColor.b) * 20;
        _twilightNightDiffBE = Mathf.Abs(TwilightColors.EquatorColor.b - NightColors.EquatorColor.b) * 20;
        _twilightNightDiffBG = Mathf.Abs(TwilightColors.GroundColor.b - NightColors.GroundColor.b) * 20;
        _twilightNightDiffBF = Mathf.Abs(TwilightColors.FogColor.b - NightColors.FogColor.b) * 20;
    }

    // Update game time
    private void UpdateTime()
    {
        // Adjust time
        CurrentTime += Time.deltaTime / SecondsInAFullDay;
        // Validate time
        if (CurrentTime >= 1f)
        {
            CurrentTime = 0f;
            CurrentDay++;
        }
    }

    // Update light on scene
    private void UpdateLight()
    {
        // Rotate sun
        Sun.transform.localRotation = Quaternion.Euler((CurrentTime * 360f) - 90f, 170f, 0f);
        // Rotate moon
        Moon.transform.localRotation = Quaternion.Euler((CurrentTime * 360f) + 90f, 170f, 0f);
        // Weaken moon
        if (CurrentTime > 0.18f && CurrentTime <= 0.2f)
            _flareStrength -= Time.deltaTime / SecondsInAFullDay * 50;
        // Power sun
        if (CurrentTime > 0.28f && CurrentTime <= 0.3f)
            _flareStrength += Time.deltaTime / SecondsInAFullDay * 50;
        // Late night
        if (CurrentTime <= 0.2f)
        {
            _lightIntensity = 0f;
            _blendFac = 0.02f;
        }
        // Night to twilight
        if (CurrentTime > 0.2f && CurrentTime <= 0.25f)
        {
            _lightIntensity += Time.deltaTime / SecondsInAFullDay * 10;
            _blendFac += Time.deltaTime / SecondsInAFullDay * 10;
            // Night color to twilight color
            _rS += Time.deltaTime / SecondsInAFullDay * _twilightNightDiffRS;
            _rE += Time.deltaTime / SecondsInAFullDay * _twilightNightDiffRE;
            _rG += Time.deltaTime / SecondsInAFullDay * _twilightNightDiffRG;
            _rF += Time.deltaTime / SecondsInAFullDay * _twilightNightDiffRF;
            _gS += Time.deltaTime / SecondsInAFullDay * _twilightNightDiffGS;
            _gE += Time.deltaTime / SecondsInAFullDay * _twilightNightDiffGE;
            _gG += Time.deltaTime / SecondsInAFullDay * _twilightNightDiffGG;
            _gF += Time.deltaTime / SecondsInAFullDay * _twilightNightDiffGF;
            _bS += Time.deltaTime / SecondsInAFullDay * _twilightNightDiffBS;
            _bE += Time.deltaTime / SecondsInAFullDay * _twilightNightDiffBE;
            _bG += Time.deltaTime / SecondsInAFullDay * _twilightNightDiffBG;
            _bF += Time.deltaTime / SecondsInAFullDay * _twilightNightDiffBF;
            // Check sky color
            if (_rS > TwilightColors.SkyColor.r || _gS > TwilightColors.SkyColor.g
                || _bS > TwilightColors.SkyColor.b)
            {
                _rS = TwilightColors.SkyColor.r;
                _gS = TwilightColors.SkyColor.g;
                _bS = TwilightColors.SkyColor.b;
            }
            // Check equator color
            if (_rE > TwilightColors.EquatorColor.r || _gE > TwilightColors.EquatorColor.g
                || _bE > TwilightColors.EquatorColor.b)
            {
                _rE = TwilightColors.EquatorColor.r;
                _gE = TwilightColors.EquatorColor.g;
                _bE = TwilightColors.EquatorColor.b;
            }
            // Check ground color
            if (_rG > TwilightColors.GroundColor.r || _gG > TwilightColors.GroundColor.g
                || _bG > TwilightColors.GroundColor.b)
            {
                _rG = TwilightColors.GroundColor.r;
                _gG = TwilightColors.GroundColor.g;
                _bG = TwilightColors.GroundColor.b;
            }
            // Check fog color
            if (_rF > TwilightColors.FogColor.r || _gF > TwilightColors.FogColor.g
                || _bF > TwilightColors.FogColor.b)
            {
                _rF = TwilightColors.FogColor.r;
                _gF = TwilightColors.FogColor.g;
                _bF = TwilightColors.FogColor.b;
            }
        }
        // Twilight to day
        if (CurrentTime > 0.25f && CurrentTime <= 0.3f)
        {
            _lightIntensity += Time.deltaTime / SecondsInAFullDay * 10;
            _blendFac += Time.deltaTime / SecondsInAFullDay * 10;
            // Twilight color to day color
            _rS += Time.deltaTime / SecondsInAFullDay * _twilightDayDiffRS;
            _rE += Time.deltaTime / SecondsInAFullDay * _twilightDayDiffRE;
            _rG += Time.deltaTime / SecondsInAFullDay * _twilightDayDiffRG;
            _rF += Time.deltaTime / SecondsInAFullDay * _twilightDayDiffRF;
            _gS += Time.deltaTime / SecondsInAFullDay * _twilightDayDiffGS;
            _gE += Time.deltaTime / SecondsInAFullDay * _twilightDayDiffGE;
            _gG += Time.deltaTime / SecondsInAFullDay * _twilightDayDiffGG;
            _gF += Time.deltaTime / SecondsInAFullDay * _twilightDayDiffGF;
            _bS += Time.deltaTime / SecondsInAFullDay * _twilightDayDiffBS;
            _bE += Time.deltaTime / SecondsInAFullDay * _twilightDayDiffBE;
            _bG += Time.deltaTime / SecondsInAFullDay * _twilightDayDiffBG;
            _bF += Time.deltaTime / SecondsInAFullDay * _twilightDayDiffBF;
            // Check sky colo_r
            if (_rS > DayColors.SkyColor.r || _gS > DayColors.SkyColor.g
                || _bS > DayColors.SkyColor.b)
            {
                _rS = DayColors.SkyColor.r;
                _gS = DayColors.SkyColor.g;
                _bS = DayColors.SkyColor.b;
            }
            // Check equator color
            if (_rE > DayColors.EquatorColor.r || _gE > DayColors.EquatorColor.g
                || _bE > DayColors.EquatorColor.b)
            {
                _rE = DayColors.EquatorColor.r;
                _gE = DayColors.EquatorColor.g;
                _bE = DayColors.EquatorColor.b;
            }
            // Check ground color
            if (_rG > DayColors.GroundColor.r || _gG > DayColors.GroundColor.g
                || _bG > DayColors.GroundColor.b)
            {
                _rG = DayColors.GroundColor.r;
                _gG = DayColors.GroundColor.g;
                _bG = DayColors.GroundColor.b;
            }
            // Check fog color
            if (_rF > DayColors.FogColor.r || _gF > DayColors.FogColor.g
                || _bF > DayColors.FogColor.b)
            {
                _rF = DayColors.FogColor.r;
                _gF = DayColors.FogColor.g;
                _bF = DayColors.FogColor.b;
            }
        }
        // Afternoon
        if (CurrentTime > 0.3f && CurrentTime <= 0.7f)
        {
            _lightIntensity = 1f;
            _blendFac = 1f;
        }
        // Day to twilight
        if (CurrentTime > 0.7f && CurrentTime <= 0.75f)
        {
            _lightIntensity -= Time.deltaTime / SecondsInAFullDay * 10;
            _blendFac -= Time.deltaTime / SecondsInAFullDay * 10;
            // Day color to twilight color
            _rS -= Time.deltaTime / SecondsInAFullDay * _twilightDayDiffRS;
            _rE -= Time.deltaTime / SecondsInAFullDay * _twilightDayDiffRE;
            _rG -= Time.deltaTime / SecondsInAFullDay * _twilightDayDiffRG;
            _rF -= Time.deltaTime / SecondsInAFullDay * _twilightDayDiffRF;
            _gS -= Time.deltaTime / SecondsInAFullDay * _twilightDayDiffGS;
            _gE -= Time.deltaTime / SecondsInAFullDay * _twilightDayDiffGE;
            _gG -= Time.deltaTime / SecondsInAFullDay * _twilightDayDiffGG;
            _gF -= Time.deltaTime / SecondsInAFullDay * _twilightDayDiffGF;
            _bS -= Time.deltaTime / SecondsInAFullDay * _twilightDayDiffBS;
            _bE -= Time.deltaTime / SecondsInAFullDay * _twilightDayDiffBE;
            _bG -= Time.deltaTime / SecondsInAFullDay * _twilightDayDiffBG;
            _bF -= Time.deltaTime / SecondsInAFullDay * _twilightDayDiffBF;
            // Check sky colo_r
            if (_rS < TwilightColors.SkyColor.r || _gS < TwilightColors.SkyColor.g
                || _bS < TwilightColors.SkyColor.b)
            {
                _rS = TwilightColors.SkyColor.r;
                _gS = TwilightColors.SkyColor.g;
                _bS = TwilightColors.SkyColor.b;
            }
            // Check equator color
            if (_rE < TwilightColors.EquatorColor.r || _gE < TwilightColors.EquatorColor.g
                || _bE < TwilightColors.EquatorColor.b)
            {
                _rE = TwilightColors.EquatorColor.r;
                _gE = TwilightColors.EquatorColor.g;
                _bE = TwilightColors.EquatorColor.b;
            }
            // Check ground color
            if (_rG < TwilightColors.GroundColor.r || _gG < TwilightColors.GroundColor.g
                || _bG < TwilightColors.GroundColor.b)
            {
                _rG = TwilightColors.GroundColor.r;
                _gG = TwilightColors.GroundColor.g;
                _bG = TwilightColors.GroundColor.b;
            }
            // Check fog color
            if (_rF < TwilightColors.FogColor.r || _gF < TwilightColors.FogColor.g
                || _bF < TwilightColors.FogColor.b)
            {
                _rF = TwilightColors.FogColor.r;
                _gF = TwilightColors.FogColor.g;
                _bF = TwilightColors.FogColor.b;
            }
        }
        // Weaken sun
        if (CurrentTime > 0.68f && CurrentTime <= 0.7f)
            _flareStrength -= Time.deltaTime / SecondsInAFullDay * 50;
        // Power moon
        if (CurrentTime > 0.78f && CurrentTime <= 0.8f)
            _flareStrength += Time.deltaTime / SecondsInAFullDay * 50;
        // Twilight to night
        if (CurrentTime > 0.75f && CurrentTime <= 0.8f)
        {
            _lightIntensity -= Time.deltaTime / SecondsInAFullDay * 10;
            _blendFac -= Time.deltaTime / SecondsInAFullDay * 10;
            // Twilight color to night color
            _rS -= Time.deltaTime / SecondsInAFullDay * _twilightNightDiffRS;
            _rE -= Time.deltaTime / SecondsInAFullDay * _twilightNightDiffRE;
            _rG -= Time.deltaTime / SecondsInAFullDay * _twilightNightDiffRG;
            _rF -= Time.deltaTime / SecondsInAFullDay * _twilightNightDiffRF;
            _gS -= Time.deltaTime / SecondsInAFullDay * _twilightNightDiffGS;
            _gE -= Time.deltaTime / SecondsInAFullDay * _twilightNightDiffGE;
            _gG -= Time.deltaTime / SecondsInAFullDay * _twilightNightDiffGG;
            _gF -= Time.deltaTime / SecondsInAFullDay * _twilightNightDiffGF;
            _bS -= Time.deltaTime / SecondsInAFullDay * _twilightNightDiffBS;
            _bE -= Time.deltaTime / SecondsInAFullDay * _twilightNightDiffBE;
            _bG -= Time.deltaTime / SecondsInAFullDay * _twilightNightDiffBG;
            _bF -= Time.deltaTime / SecondsInAFullDay * _twilightNightDiffBF;
            // Check sky color
            if (_rS < NightColors.SkyColor.r || _gS < NightColors.SkyColor.g
                || _bS < NightColors.SkyColor.b)
            {
                _rS = NightColors.SkyColor.r;
                _gS = NightColors.SkyColor.g;
                _bS = NightColors.SkyColor.b;
            }
            // Check equator color
            if (_rE < NightColors.EquatorColor.r || _gE < NightColors.EquatorColor.g
                || _bE < NightColors.EquatorColor.b)
            {
                _rE = NightColors.EquatorColor.r;
                _gE = NightColors.EquatorColor.g;
                _bE = NightColors.EquatorColor.b;
            }
            // Check ground color
            if (_rG < NightColors.GroundColor.r || _gG < NightColors.GroundColor.g
                || _bG < NightColors.GroundColor.b)
            {
                _rG = NightColors.GroundColor.r;
                _gG = NightColors.GroundColor.g;
                _bG = NightColors.GroundColor.b;
            }
            // Check fog color
            if (_rF < NightColors.FogColor.r || _gF < NightColors.FogColor.g
                || _bF < NightColors.FogColor.b)
            {
                _rF = NightColors.FogColor.r;
                _gF = NightColors.FogColor.g;
                _bF = NightColors.FogColor.b;
            }
        }
        // Evening
        if (CurrentTime > 0.8f)
        {
            _lightIntensity = 0f;
            _blendFac = 0.02f;
        }
        // Validate light
        if (_lightIntensity > 1f)
            _lightIntensity = 1f;
        if (_lightIntensity < 0f)
            _lightIntensity = 0f;
        if (_blendFac > 1f)
            _blendFac = 1f;
        if (_blendFac < 0.02f)
            _blendFac = 0.02f;
        if (_flareStrength > 1f)
            _flareStrength = 1f;
        if (_flareStrength < 0f)
            _flareStrength = 0f;
        // Set light intensity
        Sun.intensity = _lightIntensity;
        // Set sky blending
        SkyMaterial.SetFloat("_BlendCubemaps", _blendFac);
        // Set flare strength
        RenderSettings.flareStrength = _flareStrength;
        // Set sky color
        RenderSettings.ambientSkyColor = new Color(_rS, _gS, _bS, 1f);
        // Set equator color
        RenderSettings.ambientEquatorColor = new Color(_rE, _gE, _bE, 1f);
        // Set ground color
        RenderSettings.ambientGroundColor = new Color(_rG, _gG, _bG, 1f);
        // Set fog color
        RenderSettings.fogColor = new Color(_rF, _gF, _bF, 1f);
    }
}