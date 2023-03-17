using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/**
 *  A Perfect Jump in Unity - A Complete Guide - Press Start
 *  https://www.youtube.com/watch?v=RPdn3r_tqcM 
 */

public class PlayerControl3 : MonoBehaviour
{
    [Header("Horizontal Movement")]
    public float moveSpeed = 10f;
    public Vector2 direction;
    private bool facingRight = true;

    [Header("Vertical Movement")]
    public float jumpSpeed = 10f;
    public float jumpDelay = 0.25f;
    [Tooltip("When > 0 allows a new jump")]
    private float jumpTimer;
    public bool jumpHold;

    [Header("Components")]
    public Rigidbody2D rb;
    //public Animator animator;
    public LayerMask groundLayer;
    public GameObject characterHolder;

    [Header("Physics")]
    public float maxSpeed = 7f;
    public float linearDrag = 4f;
    public float gravity = 1f;
    public float fallMultiplier = 5f;
    public float dragConstant = .9f;

    [Header("Collision")]
    public bool wasOnGround = false;
    public bool onGround = false;
    public float groundLength = 0.6f;
    public Vector3 colliderOffset = new Vector3(0.12f, 0, 0);


    public Vector2 lastVelocity;

    // Update is called once per frame
    void Update()
    {
        CheckIfGrounded();

        // player just landed
        if (!wasOnGround && onGround)
            // play land animation
            StartCoroutine(JumpSqueeze(1.25f, 0.8f, 0.05f));

        // check if jump key / button is pressed on this loop
        if (Input.GetButtonDown("Jump") || Input.GetKeyDown(KeyCode.UpArrow))
            jumpTimer = Time.time + jumpDelay;

        // check if jump key / button is held down
        jumpHold = Input.GetButton("Jump") || Input.GetKey(KeyCode.UpArrow);

        //animator.SetBool("onGround", onGround);
        direction = new Vector2(Input.GetAxisRaw("Horizontal"), 0);

        Flip();
    }

    void FixedUpdate()
    {
        // get last velocity
        lastVelocity = rb.velocity;

        Move(direction.x);

        // add jump action
        if (jumpTimer > Time.time && onGround) Jump();

        ModifyPhysics();
    }

    void CheckIfGrounded()
    {
        wasOnGround = onGround;
        onGround = Physics2D.Raycast(transform.position + colliderOffset, Vector2.down, groundLength, groundLayer) ||
            Physics2D.Raycast(transform.position - colliderOffset, Vector2.down, groundLength, groundLayer);
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        // if landed on ground
        if (collision.transform.tag == "Ground")
            // and was just falling
            if (lastVelocity.y < -5)
                // preserve momentum on impact
                rb.velocity = new Vector2(lastVelocity.x, rb.velocity.y);
    }

    void Move(float dir)
    {
        Vector3 v0 = Vector3.zero;
        Vector2 targetVelocity = new Vector2(dir * maxSpeed, rb.velocity.y);
        rb.velocity = Vector3.SmoothDamp(rb.velocity, targetVelocity, ref v0, 0.02f);

        //animator.SetFloat("horizontal", Mathf.Abs(rb.velocity.x));
        //animator.SetFloat("vertical", rb.velocity.y);
    }

    void Jump()
    {
        rb.velocity = new Vector2(rb.velocity.x, 0);
        rb.AddForce(Vector2.up * jumpSpeed, ForceMode2D.Impulse);
        jumpTimer = 0;
        jumpHold = false;
        onGround = false;

        // play jump animation
        StartCoroutine(JumpSqueeze(0.5f, 1.2f, 0.1f));
    }

    void ModifyPhysics()
    {
        bool changingDirections = (direction.x > 0 && rb.velocity.x < 0) || (direction.x < 0 && rb.velocity.x > 0);

        if (onGround)
        {
            if (Mathf.Abs(direction.x) < 0.4f || changingDirections)
            {
                rb.drag = linearDrag;
            }
            else
            {
                rb.drag = 0.1f;
            }
            rb.gravityScale = 0;
        }
        else
        {
            // "better jump" 
            rb.gravityScale = gravity;
            rb.drag = linearDrag * 0.15f;
            if (rb.velocity.y < 0)
            {
                rb.gravityScale = gravity * fallMultiplier;
            }
            else if (rb.velocity.y > 0 && !jumpHold)
            {
                rb.gravityScale = gravity * (fallMultiplier / 2);
            }
        }
    }

    void Flip()
    {
        if ((rb.velocity.x > 0 && !facingRight) || (rb.velocity.x < 0 && facingRight))
        {
            facingRight = !facingRight;
            transform.rotation = Quaternion.Euler(0, facingRight ? 0 : 180, 0);
        }
    }

    IEnumerator JumpSqueeze(float xSqueeze, float ySqueeze, float seconds)
    {
        Vector3 originalSize = Vector3.one;
        Vector3 newSize = new Vector3(xSqueeze, ySqueeze, originalSize.z);
        float t = 0f;
        while (t <= 1.0)
        {
            t += Time.deltaTime / seconds;
            characterHolder.transform.localScale = Vector3.Lerp(originalSize, newSize, t);
            yield return null;
        }
        t = 0f;
        while (t <= 1.0)
        {
            t += Time.deltaTime / seconds;
            characterHolder.transform.localScale = Vector3.Lerp(newSize, originalSize, t);
            yield return null;
        }
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawLine(transform.position + colliderOffset, transform.position + colliderOffset + Vector3.down * groundLength);
        Gizmos.DrawLine(transform.position - colliderOffset, transform.position - colliderOffset + Vector3.down * groundLength);
    }
}