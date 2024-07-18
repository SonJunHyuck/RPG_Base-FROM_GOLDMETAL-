using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : MonoBehaviour
{
    public int damage;
    public bool isMelee;
    public bool isRock;

    private void OnCollisionEnter(Collision collision)
    {
        // 탄피에 사용
        if(!isRock && collision.gameObject.CompareTag("Floor"))
        {
            Destroy(gameObject, 3.0f);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        // 탄에 사용
        if (!isMelee && other.gameObject.CompareTag("Wall"))
        {
            Destroy(gameObject);
        }
    }

}
