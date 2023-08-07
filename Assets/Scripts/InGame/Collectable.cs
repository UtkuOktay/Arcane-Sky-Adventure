using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Collectable : MonoBehaviour
{

    private Rigidbody2D rigidbody;

    private float angularVelocity = 50.0f;

    private float initialY;

    private int direction = -1;

    private double yRange = 0.5;

    // Start is called before the first frame update
    void Start()
    {
        rigidbody = GetComponent<Rigidbody2D>();
        initialY = transform.position.y;
    }

    // Update is called once per frame
    void Update()
    {
        float a = angularVelocity * Time.deltaTime;
        transform.Rotate(new Vector3(0, a, 0));

        if (transform.position.y < initialY - yRange)
            direction = 1;

        else if (transform.position.y > initialY + yRange)
            direction = -1;

        if (direction == 1)
        {
            rigidbody.velocity = new Vector2(0, 50 * Time.deltaTime);
            direction = 0;
        }

        else if (direction == -1)
        {
            rigidbody.velocity = new Vector3(0, -50 * Time.deltaTime);
            direction = 0;
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.tag != "Player")
            return;

        Collect(collision.gameObject);
        Destroy(gameObject);
    }

    //Specified what is to be done when the the object is collected. Since it depends on the
    // object, each object will have its own tasks to be carried out.
    protected abstract void Collect(GameObject collector);
}
