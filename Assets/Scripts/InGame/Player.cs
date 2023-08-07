using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class Player : MonoBehaviour, Entity
{
    [SerializeField]
    private Rigidbody2D rigidbody;

    [SerializeField]
    private Animator animator;

    [SerializeField]
    private DistanceJoint2D distanceJoint;

    private Rigidbody2D rigidBodyOfConnectedRing;

    [SerializeField]
    private LineRenderer lineRenderer;

    [SerializeField]
    private GameObject groundChecker;

    [SerializeField]
    private SpriteRenderer spriteRenderer;

    [SerializeField]
    private GameObject shield;
    
    [SerializeField]
    private TextMeshProUGUI coinText;

    private int jumpCounter = 2;

    private bool onGround = false;

    private bool underTrapEffect = false;

    private bool respawned = false;

    public int respawns { get; private set; } = 0;

    public int defeatedEnemies { get; private set; } = 0;

    private int health = 100;

    public int Health
    {
        get { return health; }
        private set
        {
            health = value;
            SetHealthBar();

            if (health <= 0)
                Respawn();
        }
    }

    public int numberOfCoins { get; private set; } = 0;

    [SerializeField]
    private GameObject healthBar;

    [SerializeField]
    private GameObject shieldBar;

    [SerializeField]
    private Image shieldIconImage;

    private RectTransform rectTransformOfHealthBar;

    private RectTransform rectTransformOfShieldBar;

    private RawImage rawImageOfShieldBar;

    public Vector2 checkpoint = new Vector2(-4, -1);

    GameObject[] ringList;

    private bool connectedToARing = false;

    [SerializeField]
    private float velocity;

    [SerializeField]
    private GameObject pauseScreen;

    private PauseScreen pauseScreenScript;
    
    private int shieldState = 0;

    private float shieldPoints = 100;

    private float ShieldPoints
    {
        get { return shieldPoints; }
        set
        {
            shieldPoints = value;
            SetShieldBar();
        }
    }

    private Vector3 shieldOffset = Vector3.zero;

    private Color32 shieldBarBlue = new Color(0.47f, 0.75f, 0.98f);
    private Color32 shieldBarGreen = new Color(0.41f, 0.63f, 0.3f);

    private Color32 darkRed = new Color32(180, 0, 0, 255);


    // Start is called before the first frame update
    void Start()
    {
        rigidbody.constraints = RigidbodyConstraints2D.FreezeRotation;

        ringList = GameObject.FindGameObjectsWithTag("Ring");

        rectTransformOfHealthBar = healthBar.GetComponent<RectTransform>();

        rectTransformOfShieldBar = shieldBar.GetComponent<RectTransform>();

        rawImageOfShieldBar = shieldBar.GetComponent<RawImage>();

        pauseScreenScript = pauseScreen.GetComponent<PauseScreen>();
    }

    // Update is called once per frame
    void Update()
    {
        if (pauseScreenScript.paused)
            return;

        velocity = rigidbody.velocity.x;
        float absVelocity = Mathf.Abs(velocity);

        ApplyFriction(absVelocity);

        if ((Input.GetKeyDown(KeyCode.Space) || Input.GetKeyDown(KeyCode.W))) //Vertical Input
            Jump();


        //Horizontal Input
        if (Input.GetKey(KeyCode.A))
            MoveHorizontal(-1);

        else if (Input.GetKey(KeyCode.D))
            MoveHorizontal(1);

        //Set velocity to 0 if no horizontal input is provided and the velocity is close to zero.
        else if (absVelocity <= 0.05)
            changeXVelocity(0);

        if (Input.GetKeyDown(KeyCode.UpArrow))
            Hook();

        if (Input.GetKeyDown(KeyCode.LeftArrow))
            UseShield(-1);

        else if (Input.GetKeyDown(KeyCode.RightArrow))
            UseShield(1);

        //if (Input.GetKeyDown(KeyCode.DownArrow))
        //    ScreenCapture.CaptureScreenshot("SomeLevel.png");

        animator.SetFloat("velocity", Mathf.Abs(rigidbody.velocity.x));
        animator.SetBool("onGround", onGround);

        if (connectedToARing)
            lineRenderer.SetPositions(new Vector3[] { transform.position, rigidBodyOfConnectedRing.transform.position });

        UpdateShield();
    }

    private void ApplyFriction(float absVelocity)
    {
        if (connectedToARing || absVelocity < 0.05)
            return;

        int k = 10;

        if (onGround)
            k = 50;

        rigidbody.AddForce((velocity / absVelocity) * Vector3.left * k * Time.deltaTime, ForceMode2D.Impulse);
        LimitVelocity();
    }

    private void Jump()
    {
        if (jumpCounter <= 0)
            return;

        if (rigidbody.velocity.y < 0)
            rigidbody.velocity = rigidbody.velocity * Vector2.right;

        rigidbody.AddForce(Vector2.up * 7, ForceMode2D.Impulse);
        jumpCounter--;
    }

    private void MoveHorizontal(int direction)
    {
        Vector3 localScale = transform.localScale;
        localScale.x = direction;
        transform.localScale = localScale;
        int k = 100;

        if (connectedToARing)
            k = 10;

        rigidbody.AddForce(Vector2.right * direction * k * Time.deltaTime, ForceMode2D.Impulse);
        
    }
    
    private void LimitVelocity()
    {
        int velocityLimit = 5;

        if (velocity < -velocityLimit)
            rigidbody.velocity = new Vector2(-velocityLimit, rigidbody.velocity.y);

        else if (velocity > velocityLimit)
            rigidbody.velocity = new Vector2(velocityLimit, rigidbody.velocity.y);
    }

    //Set x velocity to the desired value.
    private void changeXVelocity(float xVelocity)
    {
        Vector2 velocity = rigidbody.velocity;
        velocity.x = xVelocity;
        rigidbody.velocity = velocity;
    }

    //When the player lands on the ground, it is called by the ground checker object.
    //Sets the jump counter to 2 to allow double jump.
    public void Landed()
    {
        onGround = true;
        jumpCounter = 2;
    }

    public void TookOff()
    {
        onGround = false;
    }

    
    //Increments the number of coins the player has collected.
    public void IncrementNumberOfCoins()
    {
        numberOfCoins++;
        coinText.text = numberOfCoins.ToString();
    }

    public void IncrementDefeatedEnemies()
    {
        defeatedEnemies++;
    }

    //If connected to any ring, disconnects the joint.
    //If not connected to any ring, finds and connects to the closest ring as long as
    // the distance between the player and the ring is lower than 5 units
    // and there is not an obstacle in between them.
    private void Hook()
    {
        if (connectedToARing) //Disconnect
        {
            lineRenderer.SetPositions(new Vector3[] { Vector3.zero, Vector3.zero });
            lineRenderer.enabled = true;
            connectedToARing = false;
            distanceJoint.connectedBody = null;
            distanceJoint.enabled = false;
        }

        else //Connect
        {
            GameObject ring = GetClosestRing();

            if (ring == null)
                return;

            rigidBodyOfConnectedRing = ring.GetComponent<Rigidbody2D>();
            lineRenderer.enabled = true;
            connectedToARing = true;
            distanceJoint.enabled = true;
            distanceJoint.connectedBody = rigidBodyOfConnectedRing;
            distanceJoint.distance = 5;
        }
    }

    private GameObject GetClosestRing()
    {
        GameObject closestRing = null;
        float minDistance = Mathf.Infinity;
        string tagOfObjectOnpath = null;

        gameObject.layer = 2; // LayerMask.NameToLayer("Ignore Raycast");
        foreach (GameObject ring in ringList)
        {
            float squaredDistance = SquaredDistanceToObject(ring);

            RaycastHit2D foundObject = Physics2D.Linecast(transform.position, ring.transform.position); //To check if there is any obstacle.

            //If there is an obstacle on the path or the ring is not the closest one so far, ignore it.
            if (foundObject.collider.tag != "Ring" || minDistance <= squaredDistance)
                continue;

            closestRing = ring;
            minDistance = squaredDistance;
            tagOfObjectOnpath = foundObject.collider.tag;
        }
        gameObject.layer = 0;

        if (closestRing == null || SquaredDistanceToObject(closestRing) > 25)
            return null;

        return closestRing;
    }

    //Calculates the squared distance to the object.
    private float SquaredDistanceToObject(GameObject gameObject)
    {
        return Mathf.Pow(transform.position.x -  gameObject.transform.position.x, 2)
            + Mathf.Pow(transform.position.y - gameObject.transform.position.y, 2);
    }

    private void UseShield(int direction)
    {
        if (direction == 0 || direction == shieldState) //Disable the shield
        {
            shield.SetActive(false);
            shieldState = 0;
            shieldPoints = 0;
        }
        else if (shieldState != 0 || ShieldPoints >= 100) //Enable the shield
        {
            shield.SetActive(true);
            shieldOffset = new Vector3(0.55f * direction, -0.2f, 0);
            shieldState = direction;
        }
    }

    private void UpdateShield()
    {
        if (shieldState == 0)
            ShieldPoints += Time.deltaTime * 50;

        if (shieldPoints > 100)
            ShieldPoints = 100;

        else if (shieldState != 0)
        {
            shield.transform.position = transform.position + shieldOffset;
            Vector3 shieldScale = shield.transform.localScale;
            shieldScale.x = transform.localScale.x * shieldState;
            shield.transform.localScale = shieldScale;
            ShieldPoints -= Time.deltaTime * 100;
        }


        if (shieldState != 0 && ShieldPoints <= 0)
            UseShield(0);
    }

    private void SetShieldBar()
    {
        //width = shieldPoints * 3.2
        rectTransformOfShieldBar.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, ShieldPoints * 3.2f);
        Color color;
        
        if (shieldState == 0)
        {
            if (shieldPoints == 100)
                color = shieldBarGreen;

            else
                color = Color.gray;
        }

        else
            color = shieldBarBlue;

        rawImageOfShieldBar.color = color;
        shieldIconImage.color = color;
    }

    private void SetHealthBar()
    {
        //width = health * 2.88
        rectTransformOfHealthBar.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, Health * 2.88f);
    }

    public void Respawn()
    {
        respawned = true;
        Health = 100;
        respawns++;
        rigidbody.velocity = Vector3.zero;
        rigidbody.angularVelocity = 0;
        transform.position = checkpoint;
        restoreFromTrapEffect();
    }

    public void Damage(int penalty)
    {
        spriteRenderer.color = darkRed;
        Health -= penalty;
        Invoke("restoreFromTrapEffect", 0.5f);
    }

    public void TrapDamage(int penalty)
    {
        if (underTrapEffect || respawned)
            return;

        underTrapEffect = true;
        jumpRandomly();
        Damage(penalty);
    }

    private void jumpRandomly()
    {
        float verticalForce = 10 - rigidbody.velocity.y;
        
        if (verticalForce < 0)
            verticalForce = 0;

        transform.position += Vector3.up / 2;
        int horizontalForce = 10 * (Random.Range(0, 2) * 2 - 1);
        rigidbody.AddForce(new Vector2(horizontalForce, verticalForce), ForceMode2D.Impulse);
    }

    private void restoreFromTrapEffect()
    {
        spriteRenderer.color = Color.white;
        underTrapEffect = false;
    }

    private void LateUpdate()
    {
        respawned = false;
    }
}
