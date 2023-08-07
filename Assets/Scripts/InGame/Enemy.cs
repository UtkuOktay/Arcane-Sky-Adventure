using System.Collections;
using System.Collections.Generic;
using System.Net.NetworkInformation;
using UnityEngine;
using UnityEngine.Tilemaps;

public class Enemy : MonoBehaviour, Entity
{

    private float initialX;

    [SerializeField]
    private int direction = 1; //-1 means move to left, 1 means move to right.

    //The range is initialX - xRange to initialX + xRange. Thus the total distance in one cycle will be 2 times xRange.
    private double xRange = 10;

    [SerializeField]
    private Rigidbody2D rigidbody;

    [SerializeField]
    private BoxCollider2D collider;

    [SerializeField]
    private Animator animator;

    private GameObject player;

    private Player playerScript;

    private GameObject shooter;

    private Shooter shooterScript;

    [SerializeField]
    private SpriteRenderer spriteRenderer;

    [SerializeField]
    private GameObject coin;

    private Vector3 verticalOffset;

    private bool onGround = false;

    private bool jumping = false;

    private bool coolingDown = false;

    private bool alive = true;

    // Start is called before the first frame update
    void Start()
    {
        initialX = transform.position.x;
        player = GameObject.FindGameObjectWithTag("Player");
        playerScript = player.GetComponent<Player>();
        verticalOffset = new Vector3(0, 0.25f, 0);
        shooter = transform.Find("Shooter").gameObject;
        shooterScript = shooter.GetComponent<Shooter>();

        rigidbody.velocity = Vector2.right * 10 * direction;
    }

    // Update is called once per frame
    void Update()
    {
        if (!alive)
            return;

        if (squaredDistanceTo(player) < 100 && canSeeThePlayer())
            Attack();
        else
            Idle();
    }

    private float squaredDistanceTo(GameObject gameObject)
    {
        return Mathf.Pow(transform.position.x -  gameObject.transform.position.x, 2) + Mathf.Pow(transform.position.y - gameObject.transform.position.y, 2);
    }

    private bool canSeeThePlayer()
    {
        return Physics2D.Linecast(transform.position + verticalOffset, player.transform.position).collider.tag == "Player";
    }

    void Idle()
    {
        if (transform.position.x < initialX - xRange)
            direction = 1;

        else if (transform.position.x > initialX + xRange)
            direction = -1;

        Move();
    }

    private void Move()
    {
        RaycastHit2D climbCheck = Physics2D.Linecast(transform.position, transform.position + new Vector3(direction, 0, 0));
        Debug.DrawLine(transform.position, transform.position + new Vector3(direction, 0, 0), Color.red);

        Vector2 nextBlock = transform.position;
        nextBlock.x += direction;
        LayerMask groundLayer = LayerMask.GetMask("Ground");

        RaycastHit2D gapCheck = Physics2D.Linecast(nextBlock + Vector2.up, nextBlock + Vector2.down * 10, groundLayer);
        Debug.DrawLine(nextBlock + Vector2.up, nextBlock + Vector2.down * 15);

        if (climbCheck.collider != null && climbCheck.collider.tag == "Ground")
            Jump(1);

        else if ((gapCheck.collider == null || gapCheck.collider.tag != "Ground"))
            direction *= -1;
        
        rigidbody.velocity = new Vector2(direction, rigidbody.velocity.y);

        transform.localScale = new Vector3(-direction, 1, 1);
    }

    void Attack()
    {
        FollowPlayer();

        if (coolingDown)
            return;

        shooterScript.Shoot();
        coolingDown = true;
        Invoke("resetCooldown", 2);
        
    }

    private void FollowPlayer()
    {
        if (transform.position.x < player.transform.position.x - 0.5)
            direction = 1;
        else if (transform.position.x > player.transform.position.x + 0.5)
            direction = -1;

        Move();
    }

    private void Jump(int k)
    {
        if (jumping || !onGround)
            return;

        jumping = true;
        rigidbody.AddForce(Vector3.up * k * 7, ForceMode2D.Impulse);
        Invoke("ResetJumping", 0.1f);
    }

    private void ResetJumping()
    {
        jumping = false;
    }

    public void Landed()
    {
        onGround = true;
        animator.SetBool("onGround", true); 
    }

    public void TookOff()
    {
        onGround = false;
        animator.SetBool("onGround", false);
    }

    private void resetCooldown()
    {
        coolingDown = false;
    }

    public void Die()
    {
        spriteRenderer.color = Color.red;
        alive = false;
        rigidbody.velocity = Vector3.zero;
        transform.Rotate(new Vector3(0, 0, 90));

        playerScript.IncrementDefeatedEnemies();

        rigidbody.isKinematic = true;
        collider.enabled = false;

        Invoke("Disappear", 1);
    }

    private void Disappear()
    {
        Destroy(gameObject);
        Instantiate(coin, transform.position + Vector3.up, Quaternion.identity);
    }

    private void OnCollisionStay2D(Collision2D collision)
    {
        if (collision.gameObject.tag == "Trap")
        {
            onGround = true;
            Jump(1);
        }
    }
}
