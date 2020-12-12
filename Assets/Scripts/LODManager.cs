using UnityEngine;
using UnityEditor;

// Set LOD properties in specific objects
public class LODManager : EditorWindow
{
    // LOD level
    private float _lodLevel;

    // Open window in editor
    [MenuItem("Tools/LOD Manager")]
    public static void Open()
    {
        EditorWindow.GetWindow<LODManager>();
    }

    // Draw GUI
    public void OnGUI()
    {
        // Set LOD level
        _lodLevel = EditorGUILayout.Slider("LOD level", _lodLevel, 0.01f, 0.9f);
        // Check button click
        if (GUILayout.Button("Set new LOD"))
        {
            // Get selected objects
            GameObject[] selectedObjects = Selection.gameObjects;
            // Search objects and set LOD quality
            foreach (GameObject obj in selectedObjects)
            {
                // Check if component exists
                if (obj.TryGetComponent<LODGroup>(out LODGroup lodGroup01))
                {
                    Renderer[] rend = new Renderer[1];
                    rend[0] = obj.GetComponent<Renderer>();
                    LOD[] lod = new LOD[1];
                    lod[0] = new LOD(_lodLevel, rend);
                    lodGroup01.SetLODs(lod);
                    lodGroup01.RecalculateBounds();
                    Debug.Log("Operation completed!");
                }
                // Add component
                else
                {
                    LODGroup lodGroup02 = obj.AddComponent<LODGroup>();
                    Renderer[] rend = new Renderer[1];
                    rend[0] = obj.GetComponent<Renderer>();
                    LOD[] lod = new LOD[1];
                    lod[0] = new LOD(_lodLevel, rend);
                    lodGroup02.SetLODs(lod);
                    lodGroup02.RecalculateBounds();
                    Debug.Log("Operation completed!");
                }
            }
        }
    }
}