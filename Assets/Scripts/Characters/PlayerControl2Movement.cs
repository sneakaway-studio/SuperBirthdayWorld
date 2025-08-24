using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/**
 *  How to jump like Super Mario in Unity 2D [Metroidvania Ep10] - Josh Codes
 *  https://www.youtube.com/watch?v=39ia45WU5yc
 *  https://github.com/JoshCodesStuff/Metroidvania/tree/episode-13
 */

//[RequireComponent(typeof(Animator))]
[RequireComponent(typeof(Rigidbody2D))]
public class PlayerControl2Movement : MonoBehaviour
{
    //necessary for animations and physics
    private Rigidbody2D rb2D;
    //private Animator myAnimator;

    private bool facingRight = true;

    //variables to play with
    public float speed = 2.0f;
    public float horizMovement;//= 1[OR]-1[OR]0

    private void Start()
    {
        //define the gameobjects found on the player
        rb2D = GetComponent<Rigidbody2D>();
        //myAnimator = GetComponent<Animator>();
    }

    //handles the input for physics
    private void Update()
    {
        //check direction given by player
        horizMovement = Input.GetAxisRaw("Horizontal");
    }
    //handles running the physics
    private void FixedUpdate()
    {
        //move the character left and right
        rb2D.linearVelocity = new Vector2(horizMovement * speed, rb2D.linearVelocity.y);
        //myAnimator.SetFloat("speed", Mathf.Abs(horizMovement));
        Flip(horizMovement);
    }

    //flipping function
    private void Flip(float horizontal)
    {
        //if facing left && moving left
        if (horizontal > 0 && !facingRight || horizontal < 0 && facingRight)
        {
            facingRight = !facingRight;

            Vector3 theScale = transform.localScale;
            theScale.x *= -1;
            transform.localScale = theScale;
        }
    }
}