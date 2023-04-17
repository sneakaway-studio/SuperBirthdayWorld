using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bot_Message : MonoBehaviour
{
    public Bot bot;

    private void OnValidate()
    {
        bot = transform.parent.GetComponent<Bot>();
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        Debug.Log(bot.gameObject.name, bot.gameObject);
        Debug.Log($"{bot.gameObject.tag}.OnCollisionEnter2D() collision.transform={collision.transform.tag}");
        if (collision.transform.CompareTag("Player"))
        {
            bot.OnShowMessage();
        }
        else if (collision.transform.parent != null)
        {
            if (collision.transform.parent.CompareTag("Player"))
            {
                Debug.Log($"{bot.gameObject.tag}.OnCollisionEnter2D() collision.transform.parent={collision.transform.parent.name}");
                bot.OnShowMessage();
            }
        }
    }
    //private void OnCollisionEnter2D(Collision2D collision)
    //{
    //    Debug.Log(bot.gameObject.name, bot.gameObject);
    //    Debug.Log($"{bot.gameObject.tag}.OnCollisionEnter2D() collision.transform={collision.transform.tag}");
    //    if (collision.transform.CompareTag("Player"))
    //    {
    //        bot.OnShowMessage();
    //    }
    //    else if (collision.transform.parent != null)
    //    {
    //        if (collision.transform.parent.CompareTag("Player"))
    //        {
    //            Debug.Log($"{bot.gameObject.tag}.OnCollisionEnter2D() collision.transform.parent={collision.transform.parent.name}");
    //            bot.OnShowMessage();
    //        }
    //    }
    //}


}
