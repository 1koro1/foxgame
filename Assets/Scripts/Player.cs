using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    //Horizontal
    public float moveSpeed = 10f;//�ƶ��ٶ�
    public Vector2 direction; //����
    private bool facingRight = true;//�Ƿ��泯��

    //Vertical 
    public float jumpSpeed = 15f;//�����ٶ�
    public float jumpDelay = 0.25f;//��������
    private float jumpTimer;//�������

    //Components
    public Rigidbody2D rb;
    public Animator animator;
    public LayerMask groundLayer;//ָ��ͼ��

    //Physics
    public float maxSpeed = 7f;
    public float linearDrag = 4f;//��������
    public float gravity = 1f;//����
    public float fallMultiplier = 5f;//�½�����

    //Collision
    public bool onGround = false; //�Ƿ��ڵ�����
    public float groundLength = 0.6f;//�ذ�ָ�볤�� 
    public Vector3 colliderOffset;//���ߵĲ���


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
        //��������
        modifyPhysics();
        
    }
    //�ƶ�����
    void moveCharacter(float horizontal) {
        rb.AddForce(Vector2.right * horizontal * moveSpeed);//�����ƶ��������

        if ((horizontal > 0 && !facingRight) || (horizontal < 0 && facingRight)) {
            Flip();
        }
        animator.SetFloat("Speed",Mathf.Abs(horizontal));
         
    }
    //��Ծ����
    void Jump() {
        rb.velocity = new Vector2(rb.velocity.x,0);
        rb.AddForce(Vector2.up * jumpSpeed,ForceMode2D.Impulse);
        jumpTimer = 0;
        //��Ծ
        animator.SetBool("isJump", true);
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
                rb.gravityScale = gravity * fallMultiplier;
                animator.SetBool("isJump",false);
            } else if (rb.velocity.y > 0 && !Input.GetButton("Jump")) {
                rb.gravityScale = gravity * (fallMultiplier / 2);
                
            }
        }
    }

    //ת����
    void Flip() {
        facingRight = !facingRight;
        transform.rotation = Quaternion.Euler(0,facingRight ? 0:180,0);
    }

    //���û����������ߵĺ���
    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawLine(transform.position + colliderOffset,transform.position + colliderOffset + Vector3.down * groundLength);
        Gizmos.DrawLine(transform.position - colliderOffset,transform.position - colliderOffset + Vector3.down * groundLength);
    }
}
