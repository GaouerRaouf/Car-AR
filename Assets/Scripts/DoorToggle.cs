using System.Collections;
using UnityEngine;

public class DoorToggle : MonoBehaviour
{
    public enum RotationAxis { X, Y, Z } // Enum to choose the axis
    public RotationAxis rotationAxis = RotationAxis.Y; // Default is Y-axis

    public float rotationAngle = 90f; // Angle to rotate when opening/closing
    public float rotationSpeed = 2f; // Speed of the rotation
    private bool isOpen = false; // Track if door is open
    private Coroutine rotationCoroutine;

    public void ToggleDoor(Transform pivotPoint)
    {
        if (rotationCoroutine != null)
            StopCoroutine(rotationCoroutine);

        float targetAngle = isOpen ? -rotationAngle : rotationAngle;
        rotationCoroutine = StartCoroutine(RotateDoor(pivotPoint, targetAngle));
        isOpen = !isOpen;
    }

    private IEnumerator RotateDoor(Transform pivotPoint, float angle)
    {
        float rotated = 0f;
        float step = rotationSpeed * Time.deltaTime * Mathf.Sign(angle);
        Vector3 axis = GetRotationAxis();

        while (Mathf.Abs(rotated) < Mathf.Abs(angle))
        {
            float rotateAmount = Mathf.Clamp(step, -Mathf.Abs(angle - rotated), Mathf.Abs(angle - rotated));
            transform.RotateAround(pivotPoint.position, axis, rotateAmount);
            rotated += rotateAmount;
            yield return null;
        }
    }

    private Vector3 GetRotationAxis()
    {
        switch (rotationAxis)
        {
            case RotationAxis.X: return Vector3.right;
            case RotationAxis.Y: return Vector3.up;
            case RotationAxis.Z: return Vector3.forward;
            default: return Vector3.up;
        }
    }
}
