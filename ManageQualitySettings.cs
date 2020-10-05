using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Rendering.PostProcessing;

// Control quality settings
public class ManageQualitySettings : MonoBehaviour
{
    // Canvas
    public Canvas Canvas;
    // Post-processing
    public PostProcessVolume Volume;
    public PostProcessProfile[] Profiles;
    // Terrain
    public Terrain Terrain;
    // Show or hide canvas button
    public KeyCode ShowOrHideCanvas = KeyCode.CapsLock;
    // Layer
    private PostProcessLayer _layer;

    // Start is called before the first frame update
    private void Start()
    {
        Init();
    }

    // Update is called once per frame
    private void Update()
    {
        HideShowQualityCanvas();
    }

    // Initializate parameters
    private void Init()
    {
        Canvas.transform.Find("QualityPanel/HideShowText").GetComponent<Text>().text =
            "Press \"" + ShowOrHideCanvas + "\" to show or hide the menu";
        _layer = Camera.main.GetComponent<PostProcessLayer>();
    }

    // Set low quality
    public void SetLowQuality()
    {
        _layer.antialiasingMode = PostProcessLayer.Antialiasing.None;
        QualitySettings.SetQualityLevel(0);
        Terrain.detailObjectDensity = 0f;
        Volume.profile = Profiles[0];
        Camera.main.farClipPlane = RenderSettings.fogEndDistance = 300f;
        Terrain.Flush();
    }

    // Set medium quality
    public void SetMediumQuality()
    {
        _layer.antialiasingMode = PostProcessLayer.Antialiasing.FastApproximateAntialiasing;
        QualitySettings.SetQualityLevel(1);
        Terrain.detailObjectDensity = 0.25f;
        Volume.profile = Profiles[1];
        Camera.main.farClipPlane = RenderSettings.fogEndDistance = 900f;
        Terrain.Flush();
    }

    // Set high quality
    public void SetHighQuality()
    {
        _layer.antialiasingMode = PostProcessLayer.Antialiasing.SubpixelMorphologicalAntialiasing;
        QualitySettings.SetQualityLevel(2);
        Terrain.detailObjectDensity = 1f;
        Volume.profile = Profiles[2];
        Camera.main.farClipPlane = RenderSettings.fogEndDistance = 1500f;
        Terrain.Flush();
    }

    // Hide or show quality canvas
    public void HideShowQualityCanvas()
    {
        if (Input.GetKeyDown(ShowOrHideCanvas))
            Canvas.enabled = !Canvas.enabled;
    }
}