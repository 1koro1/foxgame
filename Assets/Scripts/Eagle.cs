using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Eagle : Enemy
{
    Rigidbody2D rb;
    public Transform top, down;
    public float Speed;
    private float topy, downy;
    private bool Gotop = true;//Õ˘…œ∑…

    // Start is called before the first frame update
    protected override void Start()
    {
        base.Start();
        rb = GetComponent<Rigidbody2D>();
        topy = top.position.y;
        downy = down.position.y;
        Destroy(top.gameObject);
        Destroy(down.gameObject);
    }

    // Update is called once per frame
    void Update()
    {
        Movement();
    }

    void Movement() {
        if (transform.position.y > topy && Gotop)
        {
            rb.velocity = new Vector2(0, -Speed);
            Gotop = false;
        }
        else if (transform.position.y < downy && !Gotop)
        {
            rb.velocity = new Vector2(0, Speed);
            Gotop = true;
        }
        else {
            if (Gotop)
            {
                rb.velocity = new Vector2(0, Speed);
            }
            else { 
                rb.velocity = new Vector2(0, -Speed);
            }
        }
            
    }
}
