// 10/8/2025 AI-Tag
// This was created with the help of Assistant, a Unity Artificial Intelligence product.

using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    [Header("Dash Light")]
    [SerializeField] GameObject DashLight;

    [SerializeField] float movementSpeed = 20f;
    [SerializeField] float dashSpeed = 40f;
    [SerializeField] float dashDuration = 0.2f;
    [SerializeField] float dashCooldown = 5f;
    [SerializeField] Transform attackArea;
    [SerializeField] Transform specialAttackArea;
    [SerializeField] Transform attackArea2;

    [Header("Second Player")]
    [SerializeField] Rigidbody2D rb2;
    private Vector2 movement2;
    private Vector2 dashDirection2;
    private bool isDashing2 = false;
    private float dashTimer2 = 0f;
    private float dashCooldownTimer2 = 0f;

    [SerializeField] private int maxHealth = 1; // player's max health (1)
    private int currentHealth;

    private PlayerControls playerControls;

    private Vector2 movement;
    private Vector2 dashDirection;

    private Rigidbody2D rb;

    private bool isDashing = false;
    private float dashTimer = 0f;
    private float dashCooldownTimer = 0f;

    private Animator myAnimator;
    private SpriteRenderer mySpriteRenderer;

    void Awake()
    {
        if (DashLight != null)
            DashLight.SetActive(false);

        playerControls = new PlayerControls();
        rb = GetComponent<Rigidbody2D>();
        playerControls.Movement.Dash.performed += ctx => TryStartDash();
        playerControls.Movement.Dash2.performed += ctx => TryStartDash2();
        myAnimator = GetComponent<Animator>();
        mySpriteRenderer = GetComponent<SpriteRenderer>();

        // initialize health
        currentHealth = maxHealth;
    }

    private void OnEnable()
    {
        playerControls.Enable();
        playerControls.Attack.SwordAttack.performed += OnSwordAttack;
        playerControls.Attack.SpecialAttack.performed += OnSpecialAttack;
        playerControls.Attack.PunchAttack.performed += OnPunchAttack;
        playerControls.Attack.PunchAttackUp.performed += OnPunchAttackUp;
        playerControls.Attack.PunchAttackRight.performed += OnPunchAttackRight;
        playerControls.Attack.SpecialAttack2.performed += SpecialAttack2;
    }

    private void OnDisable()
    {
        playerControls.Disable();
        playerControls.Attack.SwordAttack.performed -= OnSwordAttack;
        playerControls.Attack.SpecialAttack.performed -= OnSpecialAttack;
        playerControls.Attack.PunchAttack.performed -= OnPunchAttack;
        playerControls.Attack.PunchAttackUp.performed -= OnPunchAttackUp;
        playerControls.Attack.PunchAttackRight.performed -= OnPunchAttackRight;
        playerControls.Attack.SpecialAttack2.performed -= SpecialAttack2;
    }

    void Update()
    {
        PlayerInput();

        // First player dash timer
        if (isDashing)
        {
            dashTimer -= Time.deltaTime;
            if (DashLight != null)
            {
                DashLight.SetActive(true);
                DashLight.transform.position = transform.position;
            }
            if (dashTimer <= 0f)
            {
                isDashing = false;
                dashCooldownTimer = dashCooldown;
                if (DashLight != null)
                    DashLight.SetActive(false);
            }
        }
        else
        {
            if (DashLight != null)
                DashLight.SetActive(false);
            if (dashCooldownTimer > 0f)
                dashCooldownTimer -= Time.deltaTime;
        }

        // Second player dash timer
        if (isDashing2)
        {
            dashTimer2 -= Time.deltaTime;
            if (dashTimer2 <= 0f)
            {
                isDashing2 = false;
                dashCooldownTimer2 = dashCooldown;
            }
        }
        else
        {
            if (dashCooldownTimer2 > 0f)
                dashCooldownTimer2 -= Time.deltaTime;
        }
    }

    void FixedUpdate()
    {
        Move();
        ChangeDirection();
        Move2();
    }

    private void PlayerInput()
    {
        movement = playerControls.Movement.Move.ReadValue<Vector2>();
        myAnimator.SetFloat("MoveX", movement.x);
        myAnimator.SetFloat("MoveY", movement.y);
        movement2 = playerControls.Movement.Move2.ReadValue<Vector2>();
        // Do not update dashDirection if already dashing
        // Dash direction is set only when dash starts
    }

    private void TryStartDash()
    {
        if (!isDashing && dashCooldownTimer <= 0f)
        {
            Vector2 input = playerControls.Movement.Dash.ReadValue<Vector2>();
            if (input != Vector2.zero)
            {
                dashDirection = input.normalized;
                isDashing = true;
                dashTimer = dashDuration;
                myAnimator.SetTrigger("Dash"); // Play Dash animation
            }
        }
    }

    private void TryStartDash2()
    {
        if (!isDashing2 && dashCooldownTimer2 <= 0f)
        {
            Vector2 input = playerControls.Movement.Dash2.ReadValue<Vector2>();
            if (input != Vector2.zero)
            {
                dashDirection2 = input.normalized;
                isDashing2 = true;
                dashTimer2 = dashDuration;
            }
        }
    }

    private void Move()
    {
        if (isDashing)
        {
            Vector2 newPosition = rb.position + dashDirection * (dashSpeed * Time.fixedDeltaTime);
            rb.MovePosition(newPosition);
        }
        else
        {
            Vector2 newPosition = rb.position + movement * (movementSpeed * Time.fixedDeltaTime);
            rb.MovePosition(newPosition);
        }
    }

    private void Move2()
    {
        if (rb2 == null) return;
        if (isDashing2)
        {
            Vector2 newPosition = rb2.position + dashDirection2 * (dashSpeed * Time.fixedDeltaTime);
            rb2.MovePosition(newPosition);

            // Rotate the second player for a rolling effect
            float rotationAmount = 360f * (Time.fixedDeltaTime / dashDuration); // 360 degrees over dashDuration
            rb2.transform.Rotate(0f, 0f, rotationAmount);
        }
        else
        {
            Vector2 newPosition = rb2.position + movement2 * (movementSpeed * Time.fixedDeltaTime);
            rb2.MovePosition(newPosition);

            // Reset rotation after dash ends (optional)
            rb2.transform.rotation = Quaternion.identity;
        }
    }

    private void ChangeDirection()
    {
        Vector2 mousePos = Mouse.current.position.ReadValue();
        Vector2 screenPoint = Camera.main.WorldToScreenPoint(transform.position);

        if (mousePos.x < screenPoint.x)
        {
            mySpriteRenderer.flipX = true;
            attackArea.rotation = Quaternion.Euler(0, 180, 0);
        }
        else
        {
            mySpriteRenderer.flipX = false;
            attackArea.rotation = Quaternion.Euler(0, 0, 0);
        }
    }



    private void OnSwordAttack(InputAction.CallbackContext context)
    {
        myAnimator.SetTrigger("SwordAttack");
        attackArea.gameObject.SetActive(true);
    }
    private void EndAttack()
    {
        attackArea.gameObject.SetActive(false);
    }



    private void OnSpecialAttack(InputAction.CallbackContext context)
    {
        myAnimator.SetTrigger("SpecialAttack");
        specialAttackArea.gameObject.SetActive(true);
    }
    private void EndSpecialAttack()
    {
        specialAttackArea.gameObject.SetActive(false);
    }

    private void OnPunchAttack(InputAction.CallbackContext context)
    {
        myAnimator.SetTrigger("PunchAttack");
    }

    private void OnPunchAttackUp(InputAction.CallbackContext context)
    {
        myAnimator.SetTrigger("PunchAttackUp");
    }



    private void OnPunchAttackRight(InputAction.CallbackContext context)
    {
        myAnimator.SetTrigger("PunchAttackRight");
        attackArea2.gameObject.SetActive(true);
    }

    private void EndAttack2()
    {
        attackArea2.gameObject.SetActive(false);
    }



    private void SpecialAttack2(InputAction.CallbackContext context)
    {
        myAnimator.SetTrigger("SpecialAttack2");
    }

    // Handle taking damage from colliders tagged as EnemyAttack
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("EnemyAttack"))
        {
            // reduce health
            currentHealth--;

            // trigger damage animation
            if (myAnimator != null)
            {
                myAnimator.SetTrigger("Damage");
                StartCoroutine(ResetDamageTrigger());
            }

            // if dead, trigger death behavior
            if (currentHealth <= 0)
            {
                if (myAnimator != null)
                    myAnimator.SetTrigger("Death");

                Collider2D col = GetComponent<Collider2D>();
                if (col != null)
                    col.enabled = false;

                // stop movement and freeze physics
                rb.linearVelocity = Vector2.zero;
                rb.angularVelocity = 0f;
                rb.constraints = RigidbodyConstraints2D.FreezeAll;

                // destroy after delay to allow death animation
                StartCoroutine(DestroyAfterDelay(1.5f));
            }
        }
    }

    private System.Collections.IEnumerator ResetDamageTrigger()
    {
        yield return new WaitForSeconds(0.5f);
        if (myAnimator != null)
            myAnimator.ResetTrigger("Damage");
    }

    private System.Collections.IEnumerator DestroyAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);
        Destroy(gameObject);
    }

}