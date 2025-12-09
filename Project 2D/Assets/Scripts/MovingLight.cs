using UnityEngine;

public class MovingLight : MonoBehaviour
{
    [SerializeField] private float moveSpeed = 1.5f;
    [SerializeField] private float roamRadius = 5f;
    [SerializeField] private Vector2 changeIntervalRange = new Vector2(1f, 3f);

    [Header("Floating")]
    [SerializeField] private float floatAmplitude = 0.25f;
    [SerializeField] private float floatFrequency = 1f;

    private Vector2 homePosition;
    private Vector2 targetPosition;
    private Vector2 basePosition; // position without vertical float

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        homePosition = transform.position;
        basePosition = homePosition;
        targetPosition = basePosition;
        StartCoroutine(WanderRoutine());
    }

    // Update is called once per frame
    void Update()
    {
        // Move base position toward target smoothly
        basePosition = Vector2.MoveTowards(basePosition, targetPosition, moveSpeed * Time.deltaTime);

        // Apply sine vertical float on top of base position
        float floatOffset = Mathf.Sin(Time.time * floatFrequency * Mathf.PI * 2f) * floatAmplitude;

        transform.position = new Vector3(basePosition.x, basePosition.y + floatOffset, transform.position.z);
    }

    private System.Collections.IEnumerator WanderRoutine()
    {
        while (true)
        {
            float wait = Random.Range(changeIntervalRange.x, changeIntervalRange.y);
            yield return new WaitForSeconds(wait);

            // choose a random point inside circle around home position
            Vector2 randomPoint = Random.insideUnitCircle * roamRadius;
            targetPosition = homePosition + randomPoint;
        }
    }

    // Visualize roam radius in editor
    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.cyan * 0.6f;
        Gizmos.DrawWireSphere(transform.position, roamRadius);
    }
}
