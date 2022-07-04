using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    //Horizontal
    public float moveSpeed = 10f;//移动速度
    public Vector2 direction; //方向
    private bool facingRight = true;//是否面朝右

    //Vertical 
    public float jumpSpeed = 15f;//起跳速度
    public float jumpDelay = 0.25f;//重力参数
    private float jumpTimer;//起跳间隔

    //Components
    public Rigidbody2D rb;
    public Animator animator;
    public LayerMask groundLayer;//指向图层

    //Physics
    public float maxSpeed = 7f;
    public float linearDrag = 4f;//线性阻力
    public float gravity = 1f;//重力
    public float fallMultiplier = 5f;//下将乘数

    //Collision
    public bool onGround = false; //是否在地面上
    public float groundLength = 0.6f;//地板指针长度 
    public Vector3 colliderOffset;//画线的参数


    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        onGround = Physics2D.Raycast(transform.position + colliderOffset,Vector2.down,groundLength,groundLayer) || Physics2D.Raycast(transform.position - colliderOffset, Vector2.down, groundLength, groundLayer);//线性投射
        //是否在地面上
        animator.SetBool("isGround",onGround);
        if (Input.GetButtonDown("Jump")) {
            jumpTimer = Time.time + jumpDelay;
        }

        direction = new Vector2(Input.GetAxisRaw("Horizontal"),Input.GetAxisRaw("Vertical"));
    }

    private void FixedUpdate()
    {
        moveCharacter(direction.x);

        if (jumpTimer > Time.time && onGround) {
            Jump();
        }
        //调用阻力
        modifyPhysics();
        
    }
    //移动函数
    void moveCharacter(float horizontal) {
        rb.AddForce(Vector2.right * horizontal * moveSpeed);//赋予移动方向的力

        if ((horizontal > 0 && !facingRight) || (horizontal < 0 && facingRight)) {
            Flip();
        }
        animator.SetFloat("Speed",Mathf.Abs(horizontal));
         
    }
    //跳跃函数
    void Jump() {
        rb.velocity = new Vector2(rb.velocity.x,0);
        rb.AddForce(Vector2.up * jumpSpeed,ForceMode2D.Impulse);
        jumpTimer = 0;
        //跳跃
        animator.SetBool("isJump", true);
    }
    //阻力函数
    void modifyPhysics() {
        //是否改变方向
        bool changingDirections = (direction.x > 0 && rb.velocity.x < 0) || (direction.x < 0 && rb.velocity.x > 0);

        if (onGround)
        {
            if (Mathf.Abs(direction.x) < 0.4f || changingDirections)
            {
                rb.drag = linearDrag;
            }
            else
            {
                rb.drag = 0f;
            }
            rb.gravityScale = 0;//在地面上重力归零
        }
        else {
            rb.gravityScale = gravity;
            rb.drag = linearDrag * 0.15f;
            if (rb.velocity.y < 0) {
                //下降
                rb.gravityScale = gravity * fallMultiplier;
                animator.SetBool("isJump",false);
            } else if (rb.velocity.y > 0 && !Input.GetButton("Jump")) {
                rb.gravityScale = gravity * (fallMultiplier / 2);
                
            }
        }
    }

    //转向函数
    void Flip() {
        facingRight = !facingRight;
        transform.rotation = Quaternion.Euler(0,facingRight ? 0:180,0);
    }

    //内置画看不见的线的函数
    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawLine(transform.position + colliderOffset,transform.position + colliderOffset + Vector3.down * groundLength);
        Gizmos.DrawLine(transform.position - colliderOffset,transform.position - colliderOffset + Vector3.down * groundLength);
    }
}
