using UnityEngine;

public class Player : MonoBehaviour
{
    [SerializeField] private float jumpForce = 15f;
    [SerializeField] private float fastFallSpeed = 20f;
    private bool isFastFalling = false;

    private Rigidbody2D rb;
    private bool isGrounded;
    [SerializeField] private Transform groundCheck;

    [SerializeField] private float groundCheckRadius = 0.2f;
    [SerializeField] private LayerMask groundLayer;
    private Animator anim;

    [SerializeField] private BoxCollider2D runCollider;
    [SerializeField] private CapsuleCollider2D slideCollider;

    public static Player instance;
    private bool isSliding;
    private bool isDead = false;

    void Awake()
    {
        if(instance == null) instance = this;
    }


    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();
        anim.SetBool("isStarted", true);
        runCollider.enabled = true;
        slideCollider.enabled = false;
    }

    // Update is called once per frame
    void Update()
    {
        isGrounded = CheckIsGrounded();
        
        if(GameManager.instance.IsGameStarted)
        {
            Jump();
            Slide();
            RunningSFX();
        }
    }
    
    private bool CheckIsGrounded()
    {
        return Physics2D.OverlapCircle(groundCheck.position, groundCheckRadius, groundLayer);
    }

    private void OnDrawGizmosSelected()
    {
       
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(groundCheck.position, groundCheckRadius);
        
    }

    private void Jump()
    {
        if(Input.GetKeyDown(KeyCode.Space) && isGrounded)
        {
            rb.linearVelocity = Vector2.up * jumpForce;
            anim.SetBool("isJump", true);
            AudioManager.instance.PlayJumpSFX();
        }
        anim.SetBool("isJump", !isGrounded);
    }

    private void Slide()
    {
        if (Input.GetKeyDown(KeyCode.LeftControl) && isGrounded)
        {
            runCollider.enabled = false;
            slideCollider.enabled = true;
            anim.SetBool("isSlide", true);
            isSliding = true;
            AudioManager.instance.PlaySlideSFX();
        }

        else if (Input.GetKey(KeyCode.LeftControl) && !isGrounded)
        {
            runCollider.enabled = false;
            slideCollider.enabled = true;
            isFastFalling = true;
            rb.linearVelocity = new Vector2(rb.linearVelocity.x, -fastFallSpeed);

        }

        if(isFastFalling && isGrounded)
        {
            runCollider.enabled = false;
            slideCollider.enabled = true;
            anim.SetBool("isSlide", true);
            isFastFalling = false;
            isSliding = true;
            AudioManager.instance.PlaySlideSFX();
        }

        else if (Input.GetKeyUp(KeyCode.LeftControl))
        {
            runCollider.enabled = true;
            slideCollider.enabled = false;
            anim.SetBool("isSlide", false);
            isFastFalling = false;
            isSliding = false;
            AudioManager.instance.StopSlideSFX();
        }
    }

    public void Die()
    {
        if(isDead) return;
        isDead = true;

        if (runCollider != null) runCollider.enabled = false;
        if (slideCollider != null) slideCollider.enabled = true;
        
        anim.SetTrigger("isDie");
        AudioManager.instance.PlayFallSFX();
    }

    private void RunningSFX()
    {
        // Nhân vật phải chạm đất, game đang chạy, không chết và KHÔNG trong trạng thái trượt/lướt
        bool isRunning = isGrounded && !isFastFalling 
        && GameManager.instance.IsGameStarted && !GameManager.instance.isGameOver 
        && !isSliding && !GameManager.instance.isStopSfxRun;
        
        if (isRunning)
        {
            AudioManager.instance.PlayRunSFX();
        }
        else
        {
            AudioManager.instance.StopRunSFX();
        }
    }

}
