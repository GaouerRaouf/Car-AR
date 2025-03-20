using UnityEngine;
using UnityEngine.XR.ARFoundation;
using System;

public class PlaneDetectionTracker : MonoBehaviour
{
    private ARPlaneManager planeManager;
    public event Action<ARPlane> OnScanningFinished; // Use C# event instead of UnityEvent
    private ARPlane detectedPlane;
    private bool scanConfirmed = false;
    public Transform car;
    public float minimumRequiredPlaneSize = 0.5f;

    private void Awake()
    {
        planeManager = GetComponent<ARPlaneManager>();
    }

    private void OnEnable()
    {
        planeManager.planesChanged += OnPlanesChanged;
    }

    private void OnDisable()
    {
        planeManager.planesChanged -= OnPlanesChanged;
    }

    private void OnPlanesChanged(ARPlanesChangedEventArgs args)
    {
        if (scanConfirmed) return;

        foreach (var plane in args.added)
        {
            if (CheckPlaneSize(plane)) return;
        }

        foreach (var plane in args.updated)
        {
            if (CheckPlaneSize(plane)) return;
        }
    }

    private bool CheckPlaneSize(ARPlane plane)
    {
        float planeArea = plane.extents.x * plane.extents.y;
        if (planeArea >= minimumRequiredPlaneSize)
        {
            detectedPlane = plane;
            Debug.Log("✅ Plane detected with area >= 0.5m²! Waiting for confirmation...");
            return true;
        }
        return false;
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
