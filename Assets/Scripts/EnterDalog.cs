using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnterDalog : MonoBehaviour
{
    public GameObject dalog;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.tag == "Player") {
            dalog.SetActive(true);
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.gameObject.tag == "Player")
        {
            dalog.SetActive(false);
        }
    }
}
