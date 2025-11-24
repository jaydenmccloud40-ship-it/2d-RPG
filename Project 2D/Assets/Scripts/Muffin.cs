// 9/29/2025 AI-Tag
// This was created with the help of Assistant, a Unity Artificial Intelligence product.

using UnityEngine;

public class MuffinClickHandler : MonoBehaviour
{
    public GameObject explodePrefab; // Assign the Explode prefab in the Inspector
    public float explosionDuration = 0.5f; // Time before the explosion disappears

    private void OnMouseDown()
    {
        // Replace Muffin with Explode
        if (explodePrefab != null)
        {
            Vector3 muffinPosition = transform.position;
            Quaternion muffinRotation = transform.rotation;

            // Instantiate the Explode prefab
            GameObject explosion = Instantiate(explodePrefab, muffinPosition, muffinRotation);

            // Activate the AudioSource on the Explode prefab
            AudioSource explodeAudioSource = explosion.GetComponent<AudioSource>();
            if (explodeAudioSource != null)
            {
                explodeAudioSource.Play();
            }
            else
            {
                Debug.LogError("No AudioSource found on the Explode prefab!");
            }

            // Destroy the Muffin GameObject
            Destroy(gameObject);

            // Destroy the explosion after the specified duration
            Destroy(explosion, explosionDuration);
        }
    }
}