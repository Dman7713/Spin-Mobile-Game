using UnityEngine;

public class OrbitingObjectOneButtonWithTrail : MonoBehaviour
{
    [Header("Orbit Settings")]
    public Transform centerObject; // The object to orbit around
    public float orbitRadius = 2f; // Default orbit radius
    public float orbitSpeed = 50f; // Default orbit speed
    public float maxBoostSpeed = 200f; // Maximum boosted speed
    public float boostTime = 3f; // Time to reach max speed during hold

    [Header("Jump Settings")]
    public float jumpRadiusIncrease = 1f; // How much the radius increases during jump
    public float jumpDuration = 0.5f; // Duration of the jump effect

    [Header("Rotation Settings")]
    public float rotationMultiplier = 1f; // Multiplies the z-axis rotation speed

    [Header("Trail Settings")]
    public TrailRenderer trailRenderer; // Assign the Trail Renderer here

    private float angle; // Current angle of rotation
    private int direction = 1; // 1 for clockwise, -1 for counterclockwise
    private float currentSpeed; // Current speed of the orbit
    private float boostTimer = 0f; // Timer for how long the button is held
    private bool isJumping = false; // Is the object jumping?
    private float jumpTimer = 0f; // Timer for the jump animation

    private bool isHolding = false; // Is the button currently held?
    private float holdTime = 0f; // Tracks how long the button is held
    private float tapThreshold = 0.2f; // Max time for a tap (vs hold)

    void Start()
    {
        currentSpeed = orbitSpeed; // Initialize orbit speed
        if (trailRenderer == null)
        {
            trailRenderer = GetComponent<TrailRenderer>();
        }
    }

    void Update()
    {
        if (centerObject == null)
        {
            Debug.LogWarning("Center object is not assigned!");
            return;
        }

        // Detect touch or mouse input
        if (Input.GetMouseButtonDown(0)) // On button press
        {
            isHolding = true;
            holdTime = 0f; // Reset hold time
        }

        if (Input.GetMouseButton(0)) // While holding
        {
            holdTime += Time.deltaTime; // Track how long the button is held

            // Gradual speed boost
            boostTimer += Time.deltaTime;
            float t = Mathf.Clamp01(boostTimer / boostTime);
            currentSpeed = Mathf.Lerp(orbitSpeed, maxBoostSpeed, t);
        }

        if (Input.GetMouseButtonUp(0)) // On button release
        {
            isHolding = false;
            boostTimer = 0f;
            currentSpeed = orbitSpeed; // Reset to normal speed

            if (holdTime <= tapThreshold) // Short tap: Change orbit direction
            {
                direction *= -1;
            }
            else // Long hold: Trigger jump
            {
                if (!isJumping)
                {
                    isJumping = true;
                    jumpTimer = 0f; // Reset jump timer
                }
            }
        }

        // Increment the angle based on current speed and direction
        angle += direction * currentSpeed * Time.deltaTime;

        // Convert angle to radians
        float angleInRadians = angle * Mathf.Deg2Rad;

        // Adjust radius for jumping
        float currentRadius = orbitRadius;
        if (isJumping)
        {
            jumpTimer += Time.deltaTime;

            // Parabolic jump motion
            float normalizedTime = jumpTimer / jumpDuration;
            if (normalizedTime >= 1f)
            {
                isJumping = false; // End jump
            }
            else
            {
                float jumpOffset = Mathf.Sin(normalizedTime * Mathf.PI); // Smooth curve
                currentRadius += jumpRadiusIncrease * jumpOffset;
            }
        }

        // Calculate the new position
        float x = centerObject.position.x + Mathf.Cos(angleInRadians) * currentRadius;
        float y = centerObject.position.y + Mathf.Sin(angleInRadians) * currentRadius;

        // Apply the new position
        transform.position = new Vector2(x, y);

        // Rotate the object on the z-axis based on direction and speed
        float rotation = direction * currentSpeed * rotationMultiplier * Time.deltaTime;
        transform.Rotate(0f, 0f, rotation);

        // Enable or adjust the trail during specific actions
        if (trailRenderer != null)
        {
            trailRenderer.emitting = true; // Ensure the trail is active
        }
    }
}
