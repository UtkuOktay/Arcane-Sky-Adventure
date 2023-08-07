using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : MonoBehaviour
{

    [SerializeField]
    private Rigidbody2D rigidbody;

    private GameObject player;

    private Player playerScript;

    private bool reflected = false;


    // Start is called before the first frame update
    void Start()
    {
        rigidbody = GetComponent<Rigidbody2D>();
        player = GameObject.FindGameObjectWithTag("Player");
        playerScript = player.GetComponent<Player>();
    }

    // Update is called once per frame
    void Update()
    {
        if (Mathf.Abs(transform.position.x) > 300 || Mathf.Abs(transform.position.y) > 50)
            Destroy(gameObject);
    }

    public void SetDirection(Vector3 direction)
    {
        //Since the length of the provided vector may vary, we need to normalize the vector to always have the same speed.
        direction.Normalize();
        rigidbody.velocity = direction * 5;
        float angle = Mathf.Atan(direction.y / direction.x);
        transform.Rotate(new Vector3(0, 0, Mathf.Rad2Deg * angle));
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (!reflected && collision.tag == "Shield")
        {
            rigidbody.velocity *= -1;
            reflected = true;
        }

        if (collision.tag == "Enemy")
            if (reflected)
                collision.gameObject.GetComponent<Enemy>().Die();
            else
                return;

        else if (collision.GetComponent<Collider2D>().isTrigger)
            return;

        else if (collision.tag == "Player")
            playerScript.Damage(10);

        Destroy(gameObject);
    }
}
