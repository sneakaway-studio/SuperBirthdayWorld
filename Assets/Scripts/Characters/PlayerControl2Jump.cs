using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/**
 *  How to jump like Super Mario in Unity 2D [Metroidvania Ep10] - Josh Codes
 *  https://www.youtube.com/watch?v=39ia45WU5yc
 *  https://github.com/JoshCodesStuff/Metroidvania/tree/episode-13
 */

[RequireComponent(typeof(Rigidbody2D))]
//[RequireComponent(typeof(Animator))]
public class PlayerControl2Jump : MonoBehaviour
{
    [Header("Jump Details")]
    public float jumpForce;
    public float jumpTime;
    private float jumpTimeCounter;
    private bool stoppedJumping;

    [Header("Ground Details")]
    [SerializeField] private Transform groundCheck;
    [SerializeField] private float radOCircle;
    [SerializeField] private LayerMask whatIsGround;
    public bool grounded;

    [Header("Components")]
    private Rigidbody2D rb;
    //private Animator myAnimator;

    private void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        //myAnimator = GetComponent<Animator>();
        jumpTimeCounter = jumpTime;
    }

    //myAnimator.SetBool("falling", true);
    //myAnimator.SetBool("falling", false);

    //myAnimator.SetTrigger("jump");
    //myAnimator.ResetTrigger("jump");

    private void Update()
    {
        //what it means to be grounded
        grounded = Physics2D.OverlapCircle(groundCheck.position, radOCircle, whatIsGround);

        if (grounded)
        {
            jumpTimeCounter = jumpTime;
            //myAnimator.ResetTrigger("jump");
            //myAnimator.SetBool("falling", false);
        }

        //if we press the jump button
        if (Input.GetButtonDown("Jump") && grounded)
        {
            //jump!!!
            rb.linearVelocity = new Vector2(rb.linearVelocity.x, jumpForce);
            stoppedJumping = false;
            //tell the animator to play jump anim
            //myAnimator.SetTrigger("jump");
        }

        //if we hold the jump button
        if (Input.GetButton("Jump") && !stoppedJumping && (jumpTimeCounter > 0))
        {
            //jump!!!
            rb.linearVelocity = new Vector2(rb.linearVelocity.x, jumpForce);
            jumpTimeCounter -= Time.deltaTime;
            //myAnimator.SetTrigger("jump");
        }

        //if we release the jump button
        if (Input.GetButtonUp("Jump"))
        {
            jumpTimeCounter = 0;
            stoppedJumping = true;
            //myAnimator.SetBool("falling", true);
            //myAnimator.ResetTrigger("jump");
        }

        if (rb.linearVelocity.y < 0)
        {
            //myAnimator.SetBool("falling", true);
        }
    }

    private void OnDrawGizmos()
    {
        Gizmos.DrawSphere(groundCheck.position, radOCircle);
    }

    private void FixedUpdate()
    {
        HandleLayers();
    }

    private void HandleLayers()
    {
        if (!grounded)
        {
            //myAnimator.SetLayerWeight(1, 1);
        }
        else
        {
            //myAnimator.SetLayerWeight(1, 0);
        }
    }
}