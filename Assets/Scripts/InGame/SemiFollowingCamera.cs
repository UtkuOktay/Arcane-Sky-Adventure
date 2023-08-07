using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SemiFollowingCamera : MonoBehaviour
{
    private GameObject player;

    // Start is called before the first frame update
    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player");
    }

    // Update is called once per frame
    void Update()
    {
        transform.position = new Vector3(player.transform.position.x, transform.position.y, -10);
        
        float playerY = player.transform.position.y;
        if (playerY > 4)
            transform.position = new Vector3(transform.position.x, playerY - 4, -10);
    }
}
