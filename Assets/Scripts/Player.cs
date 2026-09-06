using UnityEngine;

public class Player : MonoBehaviour
{

    [SerializeField] private float jumpForce = 15f;
    private Rigidbody2D rb;
    private bool isGrounded;
    [SerializeField] private Transform groundCheck;

    [SerializeField] private float groundCheckRadius = 0.2f;
    [SerializeField] private LayerMask groundLayer;
    private Animator anim;

    [SerializeField] private BoxCollider2D runCollider;
    [SerializeField] private CapsuleCollider2D slideCollider;

    public static Player instance;

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
        Jump();
        Slide();
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
        }

        else if (Input.GetKeyUp(KeyCode.LeftControl))
        {
            runCollider.enabled = true;
            slideCollider.enabled = false;
            anim.SetBool("isSlide", false);
        }
    }

    // public void Die()
    // {
    //     float animationLength = anim.GetCurrentAnimatorStateInfo(0).length;
    //     anim.SetTrigger("isDie");
    //     new WaitForSeconds(animationLength + 1f);
    //     GameManager.instance.RestartGame();
    // }

}
