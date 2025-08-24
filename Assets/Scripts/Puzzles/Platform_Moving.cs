using System.Collections;
using System.Collections.Generic;
using UnityEngine;


/**
 *  Moving Platform - Horizontal / Vertical / Diagonal
 *  ├── Platform - Collider, RigidBody, Graphic
 *  └── Start / End - Starting and finishing location for movement
 */

public class Platform_Moving : MonoBehaviour
{
    // the platform move back and forth between two positions
    public Transform start, end, platform;
    public float _speed = 3.0f;
    public bool _switch = false;


    /**
     *  EDITOR ONLY
     */
    void OnValidate()
    {
        start = transform.parent.Find("Start").transform;
        end = transform.parent.Find("End").transform;
        platform = transform;
    }
    public void OnDrawGizmos()
    {
        Gizmos.color = Color.magenta;
        Gizmos.DrawLine(start.position, end.position);
        Gizmos.DrawSphere(start.position, .2f);
        Gizmos.DrawSphere(end.position, .2f);
    }


    /**
     *  Use FixedUpdate to move @ same frequency as physics system (and thus physics-controlled objects which interact with it)
     *  Reference: https://thiscodedoesthis.com/moving-platform-unity
     */
    void FixedUpdate()
    {
        // determine direction
        if (!_switch)
            platform.position = Vector3.MoveTowards(platform.position, start.position, _speed * Time.deltaTime);
        else
            platform.position = Vector3.MoveTowards(platform.position, end.position, _speed * Time.deltaTime);

        // check that it reached the start || end position
        if (platform.position == start.position)
            _switch = true;
        else if (platform.position == end.position)
            _switch = false;

    }

    /**
     *  Keep player on top 
     */
    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.transform.CompareTag("Player"))
        {
            collision.transform.SetParent(transform);
        }
    }
    private void OnCollisionExit2D(Collision2D collision)
    {
        if (collision.transform.CompareTag("Player"))
        {
            collision.transform.parent = null;
        }
    }


}
