using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Shooter : MonoBehaviour
{

    [SerializeField]
    private GameObject bullet;

    [ContextMenu("Shoot")]
    public void Shoot(/*Vector3 targetPosition*/)
    {
        Vector3 velocity = /*targetPosition*/ GameObject.FindGameObjectWithTag("Player").transform.position - transform.position;

        velocity.Normalize();

        GameObject instantiatedBullet = Instantiate(bullet, transform.position, transform.rotation);
        instantiatedBullet.SetActive(true);
        instantiatedBullet.GetComponent<Bullet>().SetDirection(velocity);
    }
}
