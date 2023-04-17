using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpriteAnim_Blink : MonoBehaviour
{
    public float frameRate = .2f;
    public int index = 0;
    public SpriteRenderer spriteRenderer;
    public List<Color> states;

    void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();

        states = new List<Color>()
        {
            new Color(1f, 1f, 1f, 1f),
            new Color(1f, 1f, 1f, .5f),
            new Color(1f, 1f, 1f, 0f),
            new Color(1f, 1f, 1f, .5f),
        };
        // start loop
        StartCoroutine(AnimationLoop(frameRate));
    }


    IEnumerator AnimationLoop(float _frameRate)
    {
        while (true)
        {
            //Debug.Log($"AnimationLoop() index={index}");

            spriteRenderer.color = states[index++];

            if (index >= states.Count) index = 0;

            yield return new WaitForSeconds(_frameRate);
        }
    }


}
