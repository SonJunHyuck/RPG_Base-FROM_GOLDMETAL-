using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Weapon : MonoBehaviour
{
    public enum Type { Melee, Range };
    public Type type;
    public float damage;

    public int maxAmmo;
    public int curAmmo;

    public float attackSpeed;
    public float AttackSpeed
    {
        get { return attackSpeed; }
    }

    [SerializeField]
    private float attackCooltime;
    public float AttackCooltime
    {
        get
        {
            attackSpeed = Mathf.Clamp(attackSpeed, 0.1f, 2.0f);
            return attackCooltime / attackSpeed;
        }
    }

    public bool CanAttack
    {
        get 
        {
            if(type == Type.Melee && !isSwing)
            {
                return true;
            }
            else if(type == Type.Range && curAmmo > 0)
            {
                return true;
            }
            return false;
        }
    }

    public bool isSwing;

    private BoxCollider meleeArea;
    private TrailRenderer trailRenderer;

    public Transform bulletShotPos;
    public GameObject bullet;
    public Transform bulletCasePos;
    public GameObject bulletCase;

    private void Awake()
    {
        meleeArea = GetComponent<BoxCollider>();
        trailRenderer = GetComponentInChildren<TrailRenderer>();

        //canAttack = true;
        isSwing = false;
    }

    public void Use()
    {
        //if (!canAttack)
        //    return;

        //canAttack = false;
        if(type == Type.Melee)
        {
            isSwing = true;
        }
    }

    public void SwingAttack()
    {
        meleeArea.enabled = true;
        trailRenderer.enabled = true;
    }

    public void SwingAttackEnd()
    {
        meleeArea.enabled = false;
    }

    public void EndSwing()
    {
        trailRenderer.enabled = false;
        isSwing = false; 
    }

    public void ShotAttack()
    {
        // 탄
        GameObject instantBullet = Instantiate(bullet, bulletShotPos.position, bulletShotPos.rotation);
        Rigidbody bulletRigid = instantBullet.GetComponent<Rigidbody>();
        bulletRigid.velocity = bulletShotPos.forward * 50;

        // 탄피
        GameObject instantBulletCase = Instantiate(bulletCase, bulletCasePos.position, bulletCasePos.rotation);
        Rigidbody bulletCaseRigid = instantBullet.GetComponent<Rigidbody>();
        Vector3 bulletCaseDir = -bulletCasePos.forward * UnityEngine.Random.Range(2, 3) + Vector3.up * UnityEngine.Random.Range(2, 3);
        bulletCaseRigid.AddForce(bulletCaseDir, ForceMode.Impulse);
        bulletCaseRigid.AddTorque(Vector3.up * 10, ForceMode.Impulse);

        curAmmo--;
    }

    public int Reload(int inAmmo)
    {
        curAmmo += inAmmo;
        int surplusAmmo = curAmmo - maxAmmo;  // 채우고 남은 탄환 수
        curAmmo = Math.Min(curAmmo, maxAmmo);

        return Math.Max(0, surplusAmmo);
    }
}
