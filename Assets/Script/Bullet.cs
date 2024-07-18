using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : MonoBehaviour
{
    public int damage;

    private void OnCollisionEnter(Collision collision)
    {
        // 탄피에 사용
        if(collision.gameObject.CompareTag("Floor"))
        {
            Destroy(gameObject, 3.0f);
        }

        // 탄에 사용
        if(collision.gameObject.CompareTag("Wall"))
        {
            Destroy(gameObject);
        }
    }

}
