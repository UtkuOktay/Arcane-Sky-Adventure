using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FallDetector : MonoBehaviour
{

    private GameObject entity;

    private Entity entityScript;

    private Vector2 checkPoint = new Vector2(-4, -1);

    // Start is called before the first frame update
    void Start()
    {
        entity = GameObject.FindGameObjectWithTag("Player");
        entityScript = entity.GetComponent<Player>();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.tag == "Player")
            ((Player)entityScript).Respawn();

        else if (collision.gameObject.tag == "Enemy")
            Destroy(collision.gameObject);
    }
}
