using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FollowingCamera : MonoBehaviour
{
    private GameObject player;

    private Vector3 playerPosition;

    // Start is called before the first frame update
    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player");
        playerPosition = player.transform.position - new Vector3(0, 0, 10);
    }

    // Update is called once per frame
    void Update()
    {
        playerPosition.x = player.transform.position.x;
        playerPosition.y = player.transform.position.y;

        transform.position = playerPosition;
    }
}
