using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Level_Reload : MonoBehaviour
{

    [SerializeField] BoxCollider2D boxCollider2D;

    void OnValidate()
    {
        boxCollider2D = GetComponent<BoxCollider2D>();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        print(collision.transform.tag);
        if (collision.tag == "Player" || collision.transform.parent.tag == "Player")
        {
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
        }
    }


}
