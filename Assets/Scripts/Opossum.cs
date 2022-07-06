using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Opossum : Enemy
{
    Rigidbody2D rb;
    public Transform left, right;
    public float Speed;
    private float leftx, rightx;
    private bool FaceLeft = true;
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
        Movement();
    }

    void Movement()
    {

       
        if (transform.position.x < leftx && FaceLeft)
        {

            rb.velocity = new Vector2(Speed, 0);
           
            transform.localScale = new Vector3(-1, 1, 1);
            FaceLeft = false;
        }
        else if (transform.position.x > rightx && !FaceLeft)
        {
            rb.velocity = new Vector2(-Speed, 0);
            transform.localScale = new Vector3(1, 1, 1);
            FaceLeft = true;
        }
        else
        {
            if (FaceLeft)
            {
                rb.velocity = new Vector2(-Speed, 0);
            }
            else
            {
                rb.velocity = new Vector2(Speed, 0);
            }

        }
    }

    
}
