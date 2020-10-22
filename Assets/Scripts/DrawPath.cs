using UnityEngine;

// Draw lines between points
public class DrawPath : MonoBehaviour
{
    private void OnDrawGizmos()
    {
        // Set color
        Gizmos.color = Color.yellow;
        GameObject[] regions = GameObject.FindGameObjectsWithTag("Region");
        // Search regions
        foreach (GameObject reg in regions)
        {
            // Get areas
            Transform[] areas = reg.GetComponentsInChildren<Transform>();
            // Search areas
            foreach (Transform area in areas)
            {
                // Get point count
                int points = area.childCount;
                // Search points
                for (int cnt = 0; cnt < points; cnt++)
                {
                    // Validate loop
                    if (cnt.Equals(points - 1))
                    {
                        // Connect last point and first point
                        Gizmos.DrawLine(area.GetChild(cnt).position, area.GetChild(0).position);
                        // Break action
                        break;
                    }
                    // Draw line
                    Gizmos.DrawLine(area.GetChild(cnt).position, area.GetChild(cnt + 1).position);
                }
            }
        }
    }
}