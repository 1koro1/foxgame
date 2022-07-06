using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Frog : Enemy
{
    Rigidbody2D rb;
    public Transform left, right;
    public float Speed;
    private float leftx, rightx;
    private bool FaceLeft = true;

    public LayerMask groundLayer;//指向图层
    public bool onGround = false; //是否在地面上
    public float jumpForce;//跳跃力

    // Start is called before the first frame update
    protected override void Start()
    {
        base.Start();
        rb = GetComponent<Rigidbody2D>();
        leftx = left.position.x;
        rightx = right.position.x;
        Destroy(left.gameObject);
        Destroy(right.gameObject);
    }

    // Update is called once per frame
    void Update()
    {
        onGround = Physics2D.Raycast(transform.position, Vector2.down, 1f, groundLayer);
        //是否在地面上
        animator.SetBool("isGround", onGround);
        SwitchAnim();
    }

    void Movement() {
        
        //判断是否在地面上
        if (onGround) {
            if (transform.position.x < leftx && FaceLeft)
            {
                    
                rb.velocity = new Vector2(Speed, 0);
                rb.AddForce(Vector2.up * jumpForce, ForceMode2D.Impulse);
                transform.localScale = new Vector3(-1, 1, 1);
                FaceLeft = false;
            }
            else if (transform.position.x > rightx && !FaceLeft) {
                rb.velocity = new Vector2(-Speed, 0);
                rb.AddForce(Vector2.up * jumpForce, ForceMode2D.Impulse);
                transform.localScale = new Vector3(1, 1, 1);
                FaceLeft = true;
            }
            else {
                if (FaceLeft)
                {
                    rb.velocity = new Vector2(-Speed, 0);
                    rb.AddForce(Vector2.up * jumpForce, ForceMode2D.Impulse);
                }
                else {
                    rb.velocity = new Vector2(Speed, 0);
                    rb.AddForce(Vector2.up * jumpForce, ForceMode2D.Impulse);
                }
                    
            }
            animator.SetBool("isJump", true);
        }

    }
    //切换动画
    void SwitchAnim() {
        if (animator.GetBool("isJump")) {
            if (rb.velocity.y < 0) {
                animator.SetBool("isJump",false);
            }
        }
    
    }

    
}
