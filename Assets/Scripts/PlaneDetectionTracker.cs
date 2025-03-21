using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.XR.ARFoundation;
using UnityEngine.XR.ARSubsystems;
using System;

public class PlaneDetectionTracker : MonoBehaviour
{
    private ARRaycastManager raycastManager;
    private ARPlaneManager planeManager;

    public UnityEvent OnScanFinishEvent;
    public event Action<ARPlane> OnScanningFinished;

    private ARPlane detectedPlane;
    private bool scanConfirmed = false;
    public Transform car;
    public float minimumRequiredPlaneSize = 0.5f;

    private static List<ARRaycastHit> hits = new List<ARRaycastHit>();

    private void Awake()
    {
        raycastManager = GetComponent<ARRaycastManager>();
        planeManager = GetComponent<ARPlaneManager>();
    }

    private void Update()
    {
        if (scanConfirmed) return;

        Vector2 screenCenter = new Vector2(Screen.width / 2, Screen.height / 2);

        if (raycastManager.Raycast(screenCenter, hits, TrackableType.PlaneWithinPolygon))
        {
            ARPlane plane = planeManager.GetPlane(hits[0].trackableId);
            if (plane != null && plane != detectedPlane && CheckPlaneSize(plane))
            {
                detectedPlane = plane;
                OnScanFinishEvent?.Invoke();
                Debug.Log("✅ New plane detected!");
            }
        }
    }

    private bool CheckPlaneSize(ARPlane plane)
    {
        Debug.Log("X size " + plane.size.x);
        Debug.Log("Y size " + plane.size.y);
        return plane.size.x >= minimumRequiredPlaneSize && plane.size.y >= minimumRequiredPlaneSize;
    }

    public void FinishScanning()
    {
        if (detectedPlane != null && !scanConfirmed)
        {
            scanConfirmed = true;
            Debug.Log("✅ Scanning confirmed! Invoking event.");
            OnScanningFinished?.Invoke(detectedPlane);
            car.position = detectedPlane.center + Vector3.up;
        }
        else
        {
            Debug.LogWarning("⚠ No valid plane detected to confirm scanning.");
        }
    }
}
