using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    public float speed
    {
        get
        {
            return speed;
        }
        set
        {
            speed = Mathf.Clamp(value, 1, 2);
        }
    }

    private float axisH;
    private float axisV;

    Vector3 moveVec;

    void Update()
    {
        axisH = Input.GetAxisRaw("Horizontal");
        axisV = Input.GetAxisRaw("Vertical");

        moveVec = new Vector3(axisH, 0, axisV).normalized * speed * Time.deltaTime;

        transform.position += moveVec;
    }
}
