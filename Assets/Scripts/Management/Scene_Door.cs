using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Scene_Door : MonoBehaviour
{
    [SerializeField] bool ENTER;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.transform.tag == "Player")
        {
            if (ENTER)
                SceneFunctions.LoadPrevScene();
            else
                SceneFunctions.LoadNextScene();
        }
    }




}
