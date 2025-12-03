using UnityEngine;
using System.Collections;

public class DidMove : MonoBehaviour
{
    private enum State
    {
        Roaming,

        KnockedBack
    }
    private State currentState = State.Roaming;


    [SerializeField] private float moveSpeed = 5f;
    [SerializeField] private int maxHealth = 5;
    private int currentHealth;

    private Rigidbody2D rb;
    private Animator animator;
    private Vector2 moveDir;

    private Vector3 initialScale;


    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        initialScale = transform.localScale;
        currentHealth = maxHealth;
    }

    void Start()
    {
        StartCoroutine(RoamingRoutine());
    }

    void FixedUpdate()
    {
        if (currentState == State.Roaming)
        {
            rb.MovePosition(rb.position + moveDir * (moveSpeed * Time.fixedDeltaTime));
        }
    }

    private IEnumerator RoamingRoutine()
    {
        while (true)
        {
            moveDir = new Vector2(Random.Range(-1f, 1f), Random.Range(-1f, 1f)).normalized;
            AdjustAnimation();
            yield return new WaitForSeconds(Random.Range(1f, 3f));
        }

    }

    private void AdjustAnimation()
    {
        if (animator != null)
        {
            if (Mathf.Abs(moveDir.x) > Mathf.Abs(moveDir.y))
            {
                animator.SetFloat("MoveX", moveDir.x);
                animator.SetFloat("MoveY", 0);

                // Mirror sprite horizontally when moving left (MoveX < 0), restore when moving right
                Vector3 scale = transform.localScale;
                if (moveDir.x < 0f)
                {
                    scale.x = -Mathf.Abs(initialScale.x);
                }
                else if (moveDir.x > 0f)
                {
                    scale.x = Mathf.Abs(initialScale.x);
                }
                transform.localScale = scale;
            }
            else
            {
                animator.SetFloat("MoveX", 0);
                animator.SetFloat("MoveY", moveDir.y);
            }
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("PlayerAttack") && currentState != State.KnockedBack)
        {
            Vector2 knockbackDir = (rb.position - (Vector2)collision.transform.position).normalized;
            rb.AddForce(knockbackDir * 500f);
            StartCoroutine(KnockbackRoutine(knockbackDir));
        }
    }

    private IEnumerator KnockbackRoutine(Vector2 knockbackDir)
    {
        currentState = State.KnockedBack;
        rb.linearVelocity = Vector2.zero;
        rb.AddForce(knockbackDir * 500f);

        // reduce health by one each knockback
        currentHealth--;

        // Trigger the Damage animation and reset the trigger after 0.5 seconds
        if (animator != null)
        {
            animator.SetTrigger("Damage");
            StartCoroutine(ResetDamageTrigger());
        }

        // If health depleted, play death, freeze the enemy, wait 1s and destroy
        if (currentHealth <= 0)
        {
            if (animator != null)
                animator.SetTrigger("Death");

            Collider2D col = GetComponent<Collider2D>();
            if (col != null)
                col.enabled = false;

            // stop movement and freeze physics
            rb.linearVelocity = Vector2.zero;
            rb.angularVelocity = 0f;
            rb.constraints = RigidbodyConstraints2D.FreezeAll;

            // wait for death animation to play, then destroy
            yield return new WaitForSeconds(1.5f);
            Destroy(gameObject);
            yield break;
        }

        yield return new WaitForSeconds(0.3f);

        currentState = State.Roaming;
    }

    private IEnumerator ResetDamageTrigger()
    {
        yield return new WaitForSeconds(0.5f);
        if (animator != null)
            animator.ResetTrigger("Damage");
    }
}
