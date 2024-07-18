using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;

public class Boss : Enemy
{
    public GameObject missile;
    public Transform missilePortA;
    public Transform missilePortB;

    private Vector3 tauntVec;
    private bool isLook;

    public override void Awake()
    {
        base.Awake();

        isLook = true;
        StartCoroutine(Think());
        navAgent.isStopped = true;
    }

    private void Update()
    {
        if(IsDead)
        {
            return;
        }

        if (isLook)
        {
            transform.LookAt(target.position + target.forward * 5.0f);
        }
        else
            navAgent.SetDestination(tauntVec);
    }

    IEnumerator Think()
    {
        yield return new WaitForSeconds(0.1f);

        int randomAction = Random.Range(0, 5);

        switch (randomAction)
        {
            case 0:
            case 1:
                StartCoroutine(MissileShot());
                break;
            case 2:
            case 3:
                StartCoroutine(RockShot());
                break;
            case 4:
                StartCoroutine(Taunt());
                break;
        }

    }

    IEnumerator MissileShot()
    {
        animator.SetTrigger("doShot");
        yield return new WaitForSeconds(0.2f);
        GameObject instantMissileA = Instantiate(missile, missilePortA.position, missilePortA.rotation);
        BossBullet bossMissileA = instantMissileA.GetComponent<BossBullet>();
        bossMissileA.target = target;

        yield return new WaitForSeconds(0.3f);
        GameObject instantMissileB = Instantiate(missile, missilePortB.position, missilePortB.rotation);
        BossBullet bossMissileB = instantMissileB.GetComponent<BossBullet>();
        bossMissileB.target = target;

        yield return new WaitForSeconds(2.5f);

        StartCoroutine(Think());
    }
    
    IEnumerator RockShot()
    {
        isLook = false;
        animator.SetTrigger("doBigShot");
        Instantiate(bullet, transform.position, transform.rotation);
        
        yield return new WaitForSeconds(3.0f);
        isLook = true;
        StartCoroutine(Think());
    }

    IEnumerator Taunt()
    {
        tauntVec = target.position + target.forward * 5.0f;

        isLook = false;
        navAgent.isStopped = false;  // nav가 정상적 실행되면서 점프공격
        boxCollider.enabled = false;
        animator.SetTrigger("doTaunt");

        yield return new WaitForSeconds(1.5f);
        meleeArea.enabled = true;

        yield return new WaitForSeconds(0.5f);
        meleeArea.enabled = false;

        yield return new WaitForSeconds(1.0f);
        isLook = true;
        boxCollider.enabled = true;
        StartCoroutine(Think());
    }
}
