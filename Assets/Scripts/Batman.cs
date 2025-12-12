using UnityEngine;

public enum BatmanState
{
    Normal,
    Stealth,
    Alert
}

public class Batman : MonoBehaviour
{
    [Header("Movement Settings")]
    public float normalSpeed = 5f;
    public float stealthSpeed = 2.5f;
    public float rotationSpeed = 140f;

    [Header("Acceleration (tweak in Inspector)")]
    public float acceleration = 20f;           // units/sec^2 when speeding up
    public float deceleration = 30f;           // units/sec^2 when slowing down / stopping
    public float rotationAcceleration = 600f;  // degrees/sec^2 for turning
    public float rotationDeceleration = 800f;  // degrees/sec^2 for stopping turn

    [Header("Light Settings")]
    public Light batmanLight;
    public float normalIntensity = 10f;
    public float stealthIntensity = 2f;
    public float alertFlashInterval = 0.3f;

    [Header("Mouse Look Settings")]
    public float mouseSensitivity = 200f;
    public Transform cameraPivot;
    private float xRotation = 0f;

    [Header("Alert Audio")]
    public AudioSource alertAudioSource; // Assign AudioSource with siren clip

    // internal state
    private BatmanState currentState = BatmanState.Normal;
    private BatmanState previousState = BatmanState.Normal;

    private float flashTimer = 0f;
    private bool isRed = true;

    // smoothed motion values
    private float currentForwardSpeed = 0f; // units/sec along local forward
    private float currentTurnSpeed = 0f;    // degrees/sec about Y

    void Update()
    {
        HandleStateInput();
        HandleMovement();       // uses acceleration smoothing
        HandleStateEffects();
        HandleMouseLook();
    }

    // ---------------------------------------------------------
    // Mouse Look (Up/Down)
    // ---------------------------------------------------------
    void HandleMouseLook()
    {
        float mouseY = Input.GetAxis("Mouse Y") * mouseSensitivity * Time.deltaTime;

        xRotation -= mouseY;
        xRotation = Mathf.Clamp(xRotation, -80f, 85f);

        if (cameraPivot != null)
            cameraPivot.localRotation = Quaternion.Euler(xRotation, 0f, 0f);
    }

    // ---------------------------------------------------------
    // Input for state switching
    // ---------------------------------------------------------
    void HandleStateInput()
    {
        if (Input.GetKeyDown(KeyCode.N))
            currentState = BatmanState.Normal;

        if (Input.GetKeyDown(KeyCode.C))
            currentState = BatmanState.Stealth;

        if (Input.GetKeyDown(KeyCode.Space))
            currentState = BatmanState.Alert;
    }

    // ---------------------------------------------------------
    // Movement with acceleration/deceleration
    // ---------------------------------------------------------
    void HandleMovement()
{
    float forwardInput = Input.GetAxis("Vertical"); // -1..1
    float turnInput = Input.GetAxis("Horizontal");  // -1..1

    // base speed from state
    float baseSpeed = (currentState == BatmanState.Stealth)
        ? stealthSpeed
        : normalSpeed;

    // SHIFT = sprint boost (only if NOT in stealth)
    if (Input.GetKey(KeyCode.LeftShift) && currentState != BatmanState.Stealth)
        baseSpeed *= 2.0f;   // sprint = double speed (change this as you like)

    // movement
    transform.Translate(Vector3.forward * baseSpeed * forwardInput * Time.deltaTime);

    // rotation
    transform.Rotate(0f, turnInput * rotationSpeed * Time.deltaTime, 0f);
}


    // ---------------------------------------------------------
    // Light + Audio logic
    // ---------------------------------------------------------
    void HandleStateEffects()
    {
        if (currentState != previousState)
        {
            HandleAlertAudio();
            previousState = currentState;
        }

        if (batmanLight == null) return;

        switch (currentState)
        {
            case BatmanState.Normal:
                batmanLight.enabled = true;
                batmanLight.intensity = normalIntensity;
                batmanLight.color = Color.white;
                flashTimer = 0f;
                break;

            case BatmanState.Stealth:
                batmanLight.enabled = true;
                batmanLight.intensity = stealthIntensity;
                batmanLight.color = Color.white;
                flashTimer = 0f;
                break;

            case BatmanState.Alert:
                HandleAlertLightFlashAndColor();
                break;
        }
    }

    // ---------------------------------------------------------
    // AUDIO: Play/Stop siren based on state
    // ---------------------------------------------------------
    void HandleAlertAudio()
    {
        if (alertAudioSource == null) return;

        if (currentState == BatmanState.Alert)
        {
            if (!alertAudioSource.isPlaying)
                alertAudioSource.Play();
        }
        else
        {
            if (alertAudioSource.isPlaying)
                alertAudioSource.Stop();
        }
    }

    // ---------------------------------------------------------
    // Alert flashing red/blue
    // ---------------------------------------------------------
    void HandleAlertLightFlashAndColor()
    {
        flashTimer += Time.deltaTime;

        if (flashTimer >= alertFlashInterval)
        {
            batmanLight.color = isRed ? Color.blue : Color.red;
            isRed = !isRed;
            flashTimer = 0f;
        }
    }
}
