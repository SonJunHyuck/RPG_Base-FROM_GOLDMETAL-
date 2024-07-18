using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossRock : Bullet
{
    private float angularPower;
    private float scaleValue;
    private bool isShot;

    private Rigidbody rigid;

    private void Awake()
    {
        rigid = GetComponent<Rigidbody>();

        angularPower = 2;
        scaleValue = 0.1f;
        isShot = false;
    }

    private void Start()
    {
        StartCoroutine(GainPowerTimer());
        StartCoroutine(GainPower());
    }

    IEnumerator GainPowerTimer()
    {
        yield return new WaitForSeconds(2.2f);
        isShot = true;
    }

    IEnumerator GainPower()
    {
        while(!isShot)
        {
            angularPower += 0.02f;
            scaleValue += 0.005f;
            transform.localScale = Vector3.one * scaleValue;
            rigid.AddTorque(transform.right * angularPower, ForceMode.Acceleration);

            yield return null;
        }
    }
}
