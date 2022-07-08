using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Player : MonoBehaviour
{
    //Horizontal
    public float moveSpeed = 10f;//�ƶ��ٶ�
    public Vector2 direction; 
    private bool facingRight = true;//�Ƿ��泯��

    //Vertical 
    public float jumpSpeed = 15f;//�����ٶ�
    public float jumpDelay = 0.25f;//��������
    private float jumpTimer;//�������

    //Components
    public Rigidbody2D rb;
    public Animator animator;
    public LayerMask groundLayer;//ָ��ͼ��
    public BoxCollider2D box;//��װ��ײ��
    public SpriteRenderer sr;

    //Physics
    public float maxSpeed;
    public float linearDrag = 4f;//��������
    public float gravity;//����
    public float fallMultiplier;//�½�����
    public float lowJumpMultiplier;//������Ծ

    //Collision
    public bool onGround = false; //�Ƿ��ڵ�����
    public bool onTop = false; //�Ƿ�ͷ���еذ���ײ
    public float groundLength = 0.6f;//�ذ�ָ�볤�� 
    public Vector3 colliderOffset;//���ߵĲ���


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
        onGround = Physics2D.Raycast(transform.position + colliderOffset,Vector2.down,groundLength,groundLayer) || Physics2D.Raycast(transform.position - colliderOffset, Vector2.down, groundLength, groundLayer);//����Ͷ��
        //�Ƿ��ڵ�����
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
        //��������
        modifyPhysics();
        
    }
    //��������
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.tag == "deathLine") {
            //�����������ó���
            GetComponent<AudioSource>().enabled = false;
            Invoke("Restart",1f);
        }
    }

    //��ײ����
    private void OnCollisionEnter2D(Collision2D collision)
    {
        //����ײ����Ϊ���˵�λʱ
        if (collision.gameObject.tag == "Enemy") {
            //ʵ����frog����
            Enemy enemy = collision.gameObject.GetComponent<Enemy>();
            //��һ������ʱ�������
            if (!onGround && !animator.GetBool("isJump"))
            {
                enemy.jumpOn();
                //����2����
                rb.velocity = new Vector2(rb.velocity.x, jumpSpeed * 0.8f);
                //�����Ծ��Ч
                jump.Play(); 
                //rb.AddForce(Vector2.up * jumpSpeed * 0.6f, ForceMode2D.Impulse); 
            }
            else {
                //����������״̬����������˶���
                animator.SetTrigger("Hit");
                //����������Ƶ
                Hit.Play();
            
            }
        }
    }

    //�¶׺���
    void crouch(float vertical) {
        //�����¶ײ���
        if (vertical < 0) {
            animator.SetBool("crouch", true);
            box.enabled = false;
        } else if (!onTop) {
            box.enabled = true;
            animator.SetBool("crouch", false);
        }
    }
    //�ƶ�����
    void moveCharacter(float horizontal) {
        if ((horizontal > 0 && !facingRight) || (horizontal < 0 && facingRight))
        {
            Flip();
        }

        rb.AddForce(Vector2.right * horizontal * moveSpeed);//�����ƶ��������
        //����һ�����ٶ�
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
    //��Ծ����
    void Jump() {
        if (!animator.GetBool("crouch")) {
            rb.velocity = new Vector2(rb.velocity.x, jumpSpeed);
            //rb.AddForce(Vector2.up * jumpSpeed,ForceMode2D.Impulse);
            jumpTimer = 0;
            //��Ծ
            animator.SetBool("isJump", true);
            //�����Ծ��Ч
            jump.Play();
        } 
    }
    //��������
    void modifyPhysics() {
        //�Ƿ�ı䷽��
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
            rb.gravityScale = 0;//�ڵ�������������
        }
        else {
            rb.gravityScale = gravity;
            rb.drag = linearDrag * 0.15f;
            if (rb.velocity.y < 0) {
                //�½�
                rb.velocity += Vector2.up * Physics2D.gravity.y * (fallMultiplier - 1) * Time.deltaTime;
                //rb.gravityScale = gravity * fallMultiplier;
                animator.SetBool("isJump",false);
            } else if (rb.velocity.y > 0 && !Input.GetButton("Jump")) {
                rb.velocity += Vector2.up * Physics2D.gravity.y * (lowJumpMultiplier - 1) * Time.deltaTime;
                //rb.gravityScale = gravity * (fallMultiplier / 2);
                
            }
        }
    }

    //ת����
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
    //���õ�ǰ����
    void Restart() {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    
    }

    //���û����������ߵĺ���
    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawLine(transform.position + colliderOffset,transform.position + colliderOffset + Vector3.down * groundLength);
        Gizmos.DrawLine(transform.position - colliderOffset,transform.position - colliderOffset + Vector3.down * groundLength);

        //����һ��ͷ������
        Gizmos.color = Color.yellow;
        Gizmos.DrawLine(transform.position, transform.position + Vector3.up * 0.2f);
    }
}
