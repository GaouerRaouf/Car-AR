using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.XR.ARFoundation;
using UnityEngine.XR.ARSubsystems;
using System;

public class PlaneDetectionTracker : MonoBehaviour, IPausable
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
            }
        }
    }

    public void Pause()
    {
        enabled = false;
    }
    public void Unpause()
    {
        enabled = true;
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
            Debug.Log("Scanning confirmed!");


            Vector3 targetPosition = detectedPlane.transform.position + Vector3.up * 2f;
            Vector3 targetScale = new Vector3(1, 1, 1) * Math.Min(detectedPlane.size.x, detectedPlane.size.y) * 0.1f;


            if (Physics.Raycast(targetPosition, Vector3.down, out RaycastHit hit, 5f))
            {
                targetPosition = hit.point + Vector3.up * 0.1f;
            }

            // Change the car's position and scale to match the detected plane
            car.position = targetPosition;
            car.localScale = targetScale;


            Rigidbody carRb = car.GetComponent<Rigidbody>();
            if (carRb != null)
            {
                carRb.linearVelocity = Vector3.zero;
                carRb.angularVelocity = Vector3.zero;
            }

            OnScanningFinished?.Invoke(detectedPlane);
        }
        else
        {
            Debug.LogWarning("No valid plane detected to confirm scanning.");
        }
    }

}
