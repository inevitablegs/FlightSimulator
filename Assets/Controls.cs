using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class Controls : MonoBehaviour
{
    public float maxSpeed = 60f;
    public float acceleration = 10f;
    public float deceleration = 5f;
    public float idleDrag = 2f;
    public float turnSpeed = 80f;

    [Header("Realistic Takeoff Settings")]
    public float takeoffSpeed = 20f;
    public float maxClimbAngle = 5f;
    public float noseUpRate = 0.8f;
    public float levelOutRate = 0.3f;
    public float liftBuildUpSpeed = 0.5f;

    [Header("Landing Settings")]
    public float noseDiveRate = 1.0f;
    public float descentSpeed = 5f;
    public float groundCheckDistance = 5f;

    [Header("Flight Dynamics")]
    public float maxBankAngle = 45f;
    public float rollControlSpeed = 3f;
    public float pitchControlSpeed = 60f;

    private Rigidbody rb;
    private float currentSpeed = 0f;
    private float currentLiftBlend = 0f;
    private bool isAirborne = false;
    private float groundLevel = 0f;

    // NEW: Smooth landing state
    private bool isLanding = false;
    private float landingSinkRate = 0f;       // Current vertical descent speed (smoothed)
    private float landingLevelBlend = 0f;     // How much the nose has leveled (0=diving, 1=flat)

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        rb.sleepThreshold = 0f;
        rb.linearDamping = 0.2f;
        rb.angularDamping = 2f;
        rb.useGravity = false;
        groundLevel = transform.position.y;
    }

    void FixedUpdate()
    {
        bool nearGround = Physics.Raycast(transform.position, Vector3.down, groundCheckDistance);
        float altitudeAboveGround = transform.position.y - groundLevel;

        // 1. Turn
        float turn = Input.GetAxis("Horizontal") * turnSpeed * Time.fixedDeltaTime;
        Quaternion turnRotation = Quaternion.Euler(0f, turn, 0f);
        rb.MoveRotation(rb.rotation * turnRotation);

        // 2. Speed Control
        float forwardInput = Input.GetAxis("Vertical");

        if (forwardInput > 0.1f)
        {
            currentSpeed += acceleration * forwardInput * Time.fixedDeltaTime;
        }
        else if (forwardInput < -0.1f)
        {
            currentSpeed += deceleration * forwardInput * Time.fixedDeltaTime;
        }
        else
        {
            if (!isAirborne)
                currentSpeed = Mathf.MoveTowards(currentSpeed, 0f, idleDrag * Time.fixedDeltaTime);
            else
                currentSpeed = Mathf.MoveTowards(currentSpeed, 0f, idleDrag * 0.2f * Time.fixedDeltaTime);
        }

        currentSpeed = Mathf.Clamp(currentSpeed, 0f, maxSpeed);

        // =============================================
        // TAKEOFF with Space
        // =============================================
        if (Input.GetKey(KeyCode.Space))
        {
            // Reset landing state if taking off again
            isLanding = false;
            landingSinkRate = 0f;
            landingLevelBlend = 0f;

            if (!isAirborne && currentSpeed < takeoffSpeed)
            {
                Vector3 groundVel = transform.forward * currentSpeed;
                groundVel.y = 0f;
                rb.linearVelocity = groundVel;
            }
            else
            {
                isAirborne = true;

                float currentPitch = transform.eulerAngles.x;
                if (currentPitch > 180f) currentPitch -= 360f;

                if (currentPitch > -maxClimbAngle)
                {
                    float pitchAmount = noseUpRate * Time.fixedDeltaTime;
                    Quaternion pitchRotation = Quaternion.Euler(-pitchAmount, 0f, 0f);
                    rb.MoveRotation(rb.rotation * pitchRotation);
                }

                currentLiftBlend = Mathf.MoveTowards(currentLiftBlend, 1f, liftBuildUpSpeed * Time.fixedDeltaTime);

                Vector3 fullThrust = transform.forward * currentSpeed;
                Vector3 flatThrust = new Vector3(fullThrust.x, 0f, fullThrust.z).normalized * currentSpeed;
                Vector3 finalVelocity = Vector3.Lerp(flatThrust, fullThrust, currentLiftBlend);

                rb.linearVelocity = finalVelocity;
            }
        }
        // =============================================
        // LANDING with Left Shift (SMOOTH)
        // =============================================
        else if (Input.GetKey(KeyCode.LeftShift) && isAirborne)
        {
            isLanding = true;

            // --- How far above ground (0 to 1 ratio for blending) ---
            // At altitude > 10m, ratio = 1 (full descent mode)
            // At altitude = 0, ratio = 0 (fully landed)
            float altitudeRatio = Mathf.Clamp01(altitudeAboveGround / 10f);

            // --- SMOOTH NOSE LEVEL ---
            // The closer to the ground, the more we level out
            // landingLevelBlend goes from 0 (nose down) to 1 (fully flat)
            float targetLevelBlend = 1f - altitudeRatio; // Close to ground = more level
            landingLevelBlend = Mathf.MoveTowards(landingLevelBlend, targetLevelBlend, 0.3f * Time.fixedDeltaTime);

            // Pitch: blend between diving and flat
            float currentPitch = transform.eulerAngles.x;
            if (currentPitch > 180f) currentPitch -= 360f;

            // Only pitch nose down if we haven't leveled out much yet
            float maxDiveAngle = 10f;
            float allowedDive = maxDiveAngle * (1f - landingLevelBlend);

            if (currentPitch < allowedDive && landingLevelBlend < 0.8f)
            {
                float landingPitch = noseDiveRate * (1f - landingLevelBlend) * Time.fixedDeltaTime;
                Quaternion pitchRotation = Quaternion.Euler(landingPitch, 0f, 0f);
                rb.MoveRotation(rb.rotation * pitchRotation);
            }
            else
            {
                // Smoothly level the nose
                Vector3 euler = transform.eulerAngles;
                Quaternion targetRotation = Quaternion.Euler(0f, euler.y, 0f);
                float levelSpeed = Mathf.Lerp(0.5f, 3f, landingLevelBlend);
                Quaternion smoothLevel = Quaternion.RotateTowards(
                    rb.rotation, targetRotation, levelSpeed * Time.fixedDeltaTime);
                rb.MoveRotation(smoothLevel);
            }

            // --- SMOOTH SINK RATE ---
            // Higher up = sink faster, near ground = sink very gently
            float targetSinkRate = Mathf.Lerp(0.1f, descentSpeed, altitudeRatio);
            // Smoothly transition the sink rate (no sudden changes)
            landingSinkRate = Mathf.Lerp(landingSinkRate, targetSinkRate, 0.5f * Time.fixedDeltaTime);

            Vector3 descendVelocity = transform.forward * currentSpeed;
            descendVelocity.y = -landingSinkRate;
            rb.linearVelocity = descendVelocity;

            // --- SMOOTH TOUCHDOWN ---
            if (altitudeAboveGround <= 0.1f)
            {
                // Gently settle onto the ground
                Vector3 pos = transform.position;
                pos.y = Mathf.Lerp(pos.y, groundLevel, 2f * Time.fixedDeltaTime);
                transform.position = pos;

                // Final level out
                Vector3 euler = transform.eulerAngles;
                Quaternion targetRot = Quaternion.Euler(0f, euler.y, 0f);
                rb.MoveRotation(Quaternion.RotateTowards(rb.rotation, targetRot, 2f * Time.fixedDeltaTime));

                Vector3 landVel = transform.forward * currentSpeed;
                landVel.y = 0f;
                rb.linearVelocity = landVel;

                // Fully landed
                isAirborne = false;
                isLanding = false;
                currentLiftBlend = 0f;
                landingSinkRate = 0f;
                landingLevelBlend = 0f;
            }
        }
        // =============================================
        // CRUISING / FLOATING
        // =============================================
        else if (isAirborne)
        {
            Vector3 currentEuler = transform.eulerAngles;
            float pitchError = Mathf.DeltaAngle(currentEuler.x, 0f);

            if (Mathf.Abs(pitchError) > 0.05f)
            {
                Quaternion targetRotation = Quaternion.Euler(0f, currentEuler.y, 0f);
                float recoverSpeed = levelOutRate * Time.fixedDeltaTime;
                Quaternion leveledRotation = Quaternion.RotateTowards(
                    rb.rotation, targetRotation, recoverSpeed);
                rb.MoveRotation(leveledRotation);
            }

            Vector3 flightVelocity = transform.forward * currentSpeed;
            flightVelocity.y = 0f;
            rb.linearVelocity = flightVelocity;
        }
        // =============================================
        // ON GROUND
        // =============================================
        else
        {
            Vector3 groundVelocity = transform.forward * currentSpeed;

            if (transform.position.y > groundLevel + 0.1f)
            {
                groundVelocity.y = -5f;
            }
            else
            {
                groundVelocity.y = 0f;
                Vector3 pos = transform.position;
                pos.y = Mathf.Lerp(pos.y, groundLevel, 5f * Time.fixedDeltaTime);
                transform.position = pos;
            }

            rb.linearVelocity = groundVelocity;
        }
    }

    void OnGUI()
    {
        float displaySpeed = currentSpeed * 3.6f;
        float altitudeAboveGround = transform.position.y - groundLevel;

        GUIStyle style = new GUIStyle();
        style.fontSize = 30;
        style.fontStyle = FontStyle.Bold;
        style.normal.textColor = Color.white;

        GUI.Label(new Rect(20, 20, 300, 50), "Speed: " + Mathf.RoundToInt(displaySpeed) + " km/h", style);
        GUI.Label(new Rect(20, 60, 300, 50), "Altitude: " + altitudeAboveGround.ToString("F1") + " m", style);

        string status = isAirborne ? (isLanding ? "LANDING" : "AIRBORNE") : "GROUND";
        style.normal.textColor = isAirborne ? (isLanding ? Color.red : Color.cyan) : Color.yellow;
        GUI.Label(new Rect(20, 100, 300, 50), status, style);
    }
}



















