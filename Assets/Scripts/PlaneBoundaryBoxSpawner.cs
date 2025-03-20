using UnityEngine;
using UnityEngine.XR.ARFoundation;

public class PlaneBoundaryBoxSpawner : MonoBehaviour
{
    public GameObject boxPrefab;

    private void Start()
    {
        PlaneDetectionTracker tracker = GetComponent<PlaneDetectionTracker>();
        if (tracker != null)
        {
            tracker.OnScanningFinished+=SpawnBoundaryBoxes;
            Debug.Log("✅ Subscribed to OnScanningFinished event.");
        }
        else
        {
            Debug.LogError("⚠ PlaneDetectionTracker not found in the scene.");
        }
    }

    private void SpawnBoundaryBoxes(ARPlane plane)
    {
        if (boxPrefab == null || plane.boundary == null) return;

        Debug.Log("✅ Spawning boxes on confirmed plane.");
        
        // Clear any existing boxes on this plane
        foreach (Transform child in plane.transform)
        {
            Destroy(child.gameObject);
        }

        // Spawn boxes at each boundary point
        foreach (Vector2 point in plane.boundary)
        {
            Vector3 worldPosition = plane.transform.TransformPoint(new Vector3(point.x, 0, point.y));
            GameObject box = Instantiate(boxPrefab, worldPosition, Quaternion.identity, plane.transform);
            box.transform.localScale = new Vector3(0.1f, 0.1f, 0.1f);
        }
    }
}
