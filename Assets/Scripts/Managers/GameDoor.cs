using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameDoor : MonoBehaviour
{
    public SpriteRenderer door;
    public bool doorEnabled;
    public Color enabledColor = new Color(Color.green.r, Color.green.g, Color.green.b, .5f);
    public Color disabledColor = new Color(Color.magenta.r, Color.magenta.g, Color.magenta.b, .5f);

    private void Awake()
    {
        if (door == null) door = transform.Find("Door").GetComponent<SpriteRenderer>();
    }

    void Update()
    {
        if (doorEnabled)
            door.color = enabledColor;
        else
            door.color = disabledColor;

    }
}
