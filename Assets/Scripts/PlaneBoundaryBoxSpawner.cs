using UnityEngine;
using UnityEngine.XR.ARFoundation;
using System.Collections.Generic;

public class PlaneBoundaryBoxSpawner : MonoBehaviour
{
    public GameObject boxPrefab;
    public float boxSpacing = 0.1f; // Adjust for desired density

    private void Start()
    {
        PlaneDetectionTracker tracker = GetComponent<PlaneDetectionTracker>();
        if (tracker != null)
        {
            tracker.OnScanningFinished += SpawnBoundaryBoxes;
            Debug.Log("✅ Subscribed to OnScanningFinished event.");
        }
        else
        {
            Debug.LogError("⚠ PlaneDetectionTracker not found in the scene.");
        }
    }

    private void SpawnBoundaryBoxes(ARPlane plane)
    {
        if (boxPrefab == null || !plane.boundary.IsCreated || plane.boundary.Length < 2) return;

        Debug.Log("✅ Spawning boxes along the plane's boundary edges.");

        // Clear previous boundary boxes
        foreach (Transform child in plane.transform)
        {
            Destroy(child.gameObject);
        }

        int pointsCount = plane.boundary.Length;
        for (int i = 0; i < pointsCount; i++)
        {
            Vector2 start = plane.boundary[i];
            Vector2 end = plane.boundary[(i + 1) % pointsCount]; // Loop back to the first point

            SpawnBoxesBetweenPoints(plane, start, end);
        }

        Debug.Log($"✅ Spawned boundary boxes along {pointsCount} edges.");
    }

    private void SpawnBoxesBetweenPoints(ARPlane plane, Vector2 start, Vector2 end)
    {
        Vector3 worldStart = plane.transform.TransformPoint(new Vector3(start.x, 0, start.y));
        Vector3 worldEnd = plane.transform.TransformPoint(new Vector3(end.x, 0, end.y));

        float distance = Vector3.Distance(worldStart, worldEnd);
        int boxCount = Mathf.Max(1, Mathf.FloorToInt(distance / boxSpacing));

        for (int i = 0; i <= boxCount; i++)
        {
            float t = (float)i / boxCount;
            Vector3 spawnPos = Vector3.Lerp(worldStart, worldEnd, t);
            GameObject box = Instantiate(boxPrefab, spawnPos, Quaternion.identity, plane.transform);
            box.transform.localScale = new Vector3(0.1f, 0.1f, 0.1f);
        }
    }
}
