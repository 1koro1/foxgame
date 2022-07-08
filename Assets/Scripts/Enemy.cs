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

    //�ѵ��˵Ĺ����ķ���д����
    //����ط�Ŀ��
    public void death()
    {
        Destroy(gameObject);
    }
    //�ŵط����𶯻�
    public void jumpOn()
    {
        animator.SetTrigger("death");
        GetComponent<Collider2D>().enabled = false;
    }
}
