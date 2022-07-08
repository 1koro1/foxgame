using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Player : MonoBehaviour
{
    //Horizontal
    public float moveSpeed = 10f;//移动速度
    public Vector2 direction; 
    private bool facingRight = true;//是否面朝右

    //Vertical 
    public float jumpSpeed = 15f;//起跳速度
    public float jumpDelay = 0.25f;//重力参数
    private float jumpTimer;//起跳间隔

    //Components
    public Rigidbody2D rb;
    public Animator animator;
    public LayerMask groundLayer;//指向图层
    public BoxCollider2D box;//盒装碰撞体
    public SpriteRenderer sr;

    //Physics
    public float maxSpeed;
    public float linearDrag = 4f;//线性阻力
    public float gravity;//重力
    public float fallMultiplier;//下将乘数
    public float lowJumpMultiplier;//长按跳跃

    //Collision
    public bool onGround = false; //是否在地面上
    public bool onTop = false; //是否头顶有地板碰撞
    public float groundLength = 0.6f;//地板指针长度 
    public Vector3 colliderOffset;//画线的参数


    //Audio
    public AudioSource jump;
    public AudioSource Hit;


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

        onTop = Physics2D.OverlapCircle(transform.position , 0.2f ,groundLayer);
        animator.SetBool("isTop",onTop);
        
        if (Input.GetButtonDown("Jump")) {
            jumpTimer = Time.time + jumpDelay;
        }

        direction = new Vector2(Input.GetAxisRaw("Horizontal"),Input.GetAxisRaw("Vertical"));
    }

    private void FixedUpdate()
    {
        moveCharacter(direction.x);
        crouch(direction.y);

        if (jumpTimer > Time.time && onGround) {
            Jump();
        }
        //调用阻力
        modifyPhysics();
        
    }
    //触碰函数
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.tag == "deathLine") {
            //调用死亡重置场景
            GetComponent<AudioSource>().enabled = false;
            Invoke("Restart",1f);
        }
    }

    //碰撞函数
    private void OnCollisionEnter2D(Collision2D collision)
    {
        //当碰撞对象为敌人单位时
        if (collision.gameObject.tag == "Enemy") {
            //实例化frog的类
            Enemy enemy = collision.gameObject.GetComponent<Enemy>();
            //第一种下落时消灭对象
            if (!onGround && !animator.GetBool("isJump"))
            {
                enemy.jumpOn();
                //触发2段跳
                rb.velocity = new Vector2(rb.velocity.x, jumpSpeed * 0.8f);
                //添加跳跃音效
                jump.Play(); 
                //rb.AddForce(Vector2.up * jumpSpeed * 0.6f, ForceMode2D.Impulse); 
            }
            else {
                //不处于下落状态，则出发受伤动画
                animator.SetTrigger("Hit");
                //播放受伤音频
                Hit.Play();
            
            }
        }
    }

    //下蹲函数
    void crouch(float vertical) {
        //进行下蹲操作
        if (vertical < 0) {
            animator.SetBool("crouch", true);
            box.enabled = false;
        } else if (!onTop) {
            box.enabled = true;
            animator.SetBool("crouch", false);
        }
    }
    //移动函数
    void moveCharacter(float horizontal) {
        if ((horizontal > 0 && !facingRight) || (horizontal < 0 && facingRight))
        {
            Flip();
        }

        rb.AddForce(Vector2.right * horizontal * moveSpeed);//赋予移动方向的力
        //赋予一个加速度
        if (horizontal>0)
        {
            rb.AddForce(Vector2.right * moveSpeed, ForceMode2D.Impulse);
        }
        else if(horizontal<0){
            rb.AddForce(Vector2.left * moveSpeed, ForceMode2D.Impulse);
        }
        
        if (Mathf.Abs(rb.velocity.x) > maxSpeed) {
            rb.velocity = new Vector2(Mathf.Sign(rb.velocity.x) * maxSpeed ,rb.velocity.y);
        }
        animator.SetFloat("Speed",Mathf.Abs(horizontal));
         
    }
    //跳跃函数
    void Jump() {
        if (!animator.GetBool("crouch")) {
            rb.velocity = new Vector2(rb.velocity.x, jumpSpeed);
            //rb.AddForce(Vector2.up * jumpSpeed,ForceMode2D.Impulse);
            jumpTimer = 0;
            //跳跃
            animator.SetBool("isJump", true);
            //添加跳跃音效
            jump.Play();
        } 
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
                rb.velocity += Vector2.up * Physics2D.gravity.y * (fallMultiplier - 1) * Time.deltaTime;
                //rb.gravityScale = gravity * fallMultiplier;
                animator.SetBool("isJump",false);
            } else if (rb.velocity.y > 0 && !Input.GetButton("Jump")) {
                rb.velocity += Vector2.up * Physics2D.gravity.y * (lowJumpMultiplier - 1) * Time.deltaTime;
                //rb.gravityScale = gravity * (fallMultiplier / 2);
                
            }
        }
    }

    //转向函数
    void Flip() {
        facingRight = !facingRight;
        if (facingRight)
        {
            sr.flipX = false;
        }
        else {
            sr.flipX = true;
        } 
        //transform.rotation = Quaternion.Euler(0,facingRight ? 0:180,0);
        //transform.localScale = new Vector3(facingRight ? 1 : -1, 1,0);
    }
    //重置当前场景
    void Restart() {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    
    }

    //内置画看不见的线的函数
    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawLine(transform.position + colliderOffset,transform.position + colliderOffset + Vector3.down * groundLength);
        Gizmos.DrawLine(transform.position - colliderOffset,transform.position - colliderOffset + Vector3.down * groundLength);

        //绘制一个头顶的线
        Gizmos.color = Color.yellow;
        Gizmos.DrawLine(transform.position, transform.position + Vector3.up * 0.2f);
    }
}
