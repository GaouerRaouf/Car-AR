using UnityEngine;

public class CarController : MonoBehaviour
{
    private Rigidbody rb;
    public float acceleration = 10f;
    public float maxSpeed = 20f;
    public float turnSpeed = 5f;

    private void Start()
    {
        rb = GetComponent<Rigidbody>();
    }

    private void FixedUpdate()
    {
        float moveInput = SimpleInput.GetAxis("Vertical");  
        float turnInput = SimpleInput.GetAxis("Horizontal"); 
        MoveCar(moveInput, turnInput);
    }

    private void MoveCar(float moveInput, float turnInput)
    {
        if (rb.linearVelocity.magnitude < maxSpeed)
        {
            rb.AddForce(transform.forward * moveInput * acceleration, ForceMode.Acceleration);
        }
        transform.Rotate(Vector3.up, turnInput * turnSpeed * Time.fixedDeltaTime);
    }
}
