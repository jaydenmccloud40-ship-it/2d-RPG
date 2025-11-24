// 9/29/2025 AI-Tag
// This was created with the help of Assistant, a Unity Artificial Intelligence product.

using UnityEngine;

public class GoalWheelSpin : MonoBehaviour
{
    public float rotationSpeed = 360f; // Speed of rotation in degrees per second

    private bool isSpinning = false;
    private float targetRotation;
    private float currentRotation;

    private void Start()
    {
        // Initialize the current rotation
        currentRotation = transform.eulerAngles.z;
    }

    private void OnMouseDown()
    {
        // Trigger the spin if not already spinning
        if (!isSpinning)
        {
            isSpinning = true;
            targetRotation = currentRotation + 180f; // Add 180 degrees to the current rotation
        }
    }

    private void Update()
    {
        if (isSpinning)
        {
            // Smoothly rotate the wheel towards the target rotation
            currentRotation = Mathf.MoveTowards(currentRotation, targetRotation, rotationSpeed * Time.deltaTime);
            transform.eulerAngles = new Vector3(0, 0, currentRotation);

            // Stop spinning when the target rotation is reached
            if (Mathf.Approximately(currentRotation, targetRotation))
            {
                isSpinning = false;
            }
        }
    }
}