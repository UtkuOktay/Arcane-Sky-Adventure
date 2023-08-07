using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GroundChecker : MonoBehaviour
{

    private GameObject entity;
    private Entity entityScript;
    private Type entityType;

    public GameObject movingPlatformBelow = null;
    public Rigidbody2D movingPlatformBelowRigidBody = null;

    // Start is called before the first frame update
    void Start()
    {
        entity = transform.parent.gameObject;
        entityScript = entity.GetComponent<Entity>();
        entityType = entity.GetType();
    }

    //TODO: Move movingPlatformBelow and movingPlatformBelow to the Player's script.
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.tag == "Ground" || collision.gameObject.tag == "Trap" || collision.gameObject.tag == "MovingPlatform")
            entityScript.Landed();

        if (collision.gameObject.tag != "MovingPlatform" && entityType != typeof(Player))
            return;
        
        movingPlatformBelow = collision.gameObject;
        movingPlatformBelowRigidBody = movingPlatformBelow.GetComponent<Rigidbody2D>();
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        entityScript.TookOff();

        if (collision.gameObject.tag == "MovingPlatform")
        {
            movingPlatformBelow = null;
            movingPlatformBelowRigidBody = null;
        }
    }

    private void OnTriggerStay2D(Collider2D collision)
    {
        OnTriggerEnter2D(collision);
    }
}
