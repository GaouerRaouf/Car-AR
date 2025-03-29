using UnityEngine;




public class CarController : MonoBehaviour, IPausable
{
    private Rigidbody rb;
    public float acceleration = 10f;
    public float maxSpeed = 20f;
    public float turnSpeed = 100f;
    public float brakingForce = 5f;
    public float drag = 1f; // Helps with smoother stopping

    private void Start()
    {
        rb = GetComponent<Rigidbody>();
        rb.linearDamping = drag; // Helps slow the car smoothly
    }

    private void FixedUpdate()
    {
        float moveInput = SimpleInput.GetAxis("Vertical");
        float turnInput = SimpleInput.GetAxis("Horizontal");
        MoveCar(moveInput, turnInput);
    }

    public void Pause()
    {
        rb.isKinematic = true;
    }
    public void Unpause()
    {
        rb.isKinematic = false;
    }

    private void MoveCar(float moveInput, float turnInput)
    {
        Vector3 forwardForce = transform.forward * moveInput * acceleration;

        // Smooth acceleration & deceleration
        if (moveInput != 0)
        {
            if (rb.linearVelocity.magnitude < maxSpeed)
            {
                rb.AddForce(forwardForce, ForceMode.Force);
            }
        }
        else
        {
            // Smooth deceleration when no input
            rb.linearVelocity = Vector3.Lerp(rb.linearVelocity, Vector3.zero, brakingForce * Time.fixedDeltaTime);
        }

        // Smooth steering (scales turn based on speed)
        float speedFactor = Mathf.Clamp01(rb.linearVelocity.magnitude / maxSpeed);
        float turnAmount = turnInput * turnSpeed * speedFactor * Time.fixedDeltaTime;
        transform.Rotate(Vector3.up, turnAmount);
    }
}
