using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MovingPlatform : MonoBehaviour
{

    [SerializeField]
    private Rigidbody2D rigidbody;

    private float initialX;

    public int direction = -1; //-1 means move to left, 1 means move to right.

    //The range is initialX - xRange to initialX + xRange. Thus the total distance in one cycle will be 2 times xRange.
    private double xRange = 4; 

    // Start is called before the first frame update
    void Start()
    {
        initialX = transform.position.x;
    }

    // Update is called once per frame
    void Update()
    {
        if (transform.position.x < initialX - xRange)
            direction = 1;

        else if (transform.position.x > initialX + xRange)
            direction = -1;

        transform.position += direction * Vector3.right * Time.deltaTime;

    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.tag == "Player")
            collision.transform.SetParent(transform);
    }

    private void OnCollisionExit2D(Collision2D collision)
    {
        if (collision.gameObject.tag == "Player")
            collision.transform.SetParent(null);
    }
}
