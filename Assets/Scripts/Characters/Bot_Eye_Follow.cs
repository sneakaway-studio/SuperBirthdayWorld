using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bot_Eye_Follow : MonoBehaviour
{

    [SerializeField] Transform player;
    [SerializeField] Transform pupil;
    [SerializeField] float eyeRadius;

    private void OnValidate()
    {
        pupil = transform.Find("Pupil").transform;
        eyeRadius = GetComponent<SpriteRenderer>().bounds.size.x / 2;
    }

    void Update()
    {
        if (player == null)
            player = GameObject.Find("Player").transform;
        if (pupil == null)
            pupil = transform.Find("Pupil").transform;

        pupil.localPosition = eyeRadius * (player.position - pupil.position).normalized;
    }

}
