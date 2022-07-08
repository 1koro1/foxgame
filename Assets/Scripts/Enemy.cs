using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    protected Animator animator;
    // Start is called before the first frame update
    protected virtual void Start()
    {
        animator = GetComponent<Animator>();
    }

    //把敌人的公共的方法写在这
    //消灭地方目标
    public void death()
    {
        Destroy(gameObject);
    }
    //放地方消灭动画
    public void jumpOn()
    {
        animator.SetTrigger("death");
        GetComponent<Collider2D>().enabled = false;
    }
}
