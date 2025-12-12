using UnityEngine;

public class BatSignal : MonoBehaviour
{
    [Header("Spotlight Settings")]
    public Light spotlight;               // Assign the spotlight (child)
    public float rotationSpeed = 20f;     // Speed of rotating the light

    [Header("Toggle Settings")]
    public KeyCode toggleKey = KeyCode.B;
    private bool isActive = false;

    void Start()
    {
        if (spotlight != null)
            spotlight.enabled = false; // start OFF
    }

    void Update()
    {
        HandleToggle();
        RotateSpotlight();
    }

    // Toggle the bat signal ON/OFF
    void HandleToggle()
    {
        if (Input.GetKeyDown(toggleKey))
        {
            isActive = !isActive;

            if (spotlight != null)
                spotlight.enabled = isActive;

            foreach (Transform child in spotlight.transform)
                child.gameObject.SetActive(isActive);
        }
    }

    // Rotate only the spotlight, not the whole BatSignal object
    void RotateSpotlight()
    {
        if (isActive && spotlight != null)
        {
            spotlight.transform.Rotate(Vector3.up * rotationSpeed * Time.deltaTime, Space.World);
        }
    }
}
