using UnityEngine;
using UnityEditor;

public class ManageLOD : EditorWindow
{
    [MenuItem("Tools/Manage LOD")]
    public static void Open()
    {
        EditorWindow.GetWindow<ManageLOD>();
    }

    public void OnGUI()
    {
        if (GUILayout.Button("Set new LOD"))
        {
            GameObject[] selectedObjects = Selection.gameObjects;

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