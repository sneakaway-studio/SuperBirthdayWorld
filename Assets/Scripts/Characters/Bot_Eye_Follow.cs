using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bot_Eye_Follow : MonoBehaviour
{

    public Transform player;
    public Transform pupil;
    public float eyeRadius;

    private void OnValidate()
    {
        pupil = transform.Find("Pupil").transform;
    }

    private void Awake()
    {
        player = GameObject.Find("Player").transform;
        eyeRadius = GetComponent<SpriteRenderer>().bounds.size.x / 2;
    }

    void Update()
    {
        pupil.localPosition = eyeRadius * (player.position - pupil.position).normalized;

    }
}
