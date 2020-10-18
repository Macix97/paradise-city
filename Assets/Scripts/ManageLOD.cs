using UnityEngine;
using UnityEditor;

// Set LOD properties in specific objects
public class ManageLOD : EditorWindow
{
    // Open window in editor
    [MenuItem("Tools/Manage LOD")]
    public static void Open()
    {
        EditorWindow.GetWindow<ManageLOD>();
    }

    // Draw GUI
    public void OnGUI()
    {
        // Check button click
        if (GUILayout.Button("Set new LOD"))
        {
            // Get selected objects
            GameObject[] selectedObjects = Selection.gameObjects;
            // Search objects and set LOD quality
            foreach (GameObject obj in selectedObjects)
            {
                LODGroup lodGroup = obj.GetComponent<LODGroup>();
                Renderer[] rend = new Renderer[1];
                rend[0] = obj.GetComponent<Renderer>();
                LOD[] lod = new LOD[1];
                lod[0] = new LOD(0.1f, rend);
                lodGroup.SetLODs(lod);
                lodGroup.RecalculateBounds();
            }
        }
    }
}