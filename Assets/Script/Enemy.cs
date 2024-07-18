using System.Collections;
using System.Collections.Generic;
using System.ComponentModel.Design;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AI;

public class Enemy : MonoBehaviour
{
    public enum Type { A, B, C, D };
    public Type type;

    public float maxHealth;

    [SerializeField]
    protected float curHealth;

    public Transform target;
    public BoxCollider meleeArea;
    public GameObject bullet;
    protected bool isChase;
    private bool isAttack;
    
    public bool IsDead
    {
        get { return curHealth <= 0; }
    }

    public bool IsBoss
    {
        get { return type == Type.D; }
    }

    protected Rigidbody rigid;
    protected Animator animator;
    protected BoxCollider boxCollider;
    protected NavMeshAgent navAgent;

    MeshRenderer[] meshRenderers;

    public virtual void Awake()
    {
        rigid = GetComponent<Rigidbody>();
        boxCollider = GetComponent<BoxCollider>();
        meshRenderers = GetComponentsInChildren<MeshRenderer>();
        navAgent = GetComponent<NavMeshAgent>();
        animator = GetComponentInChildren<Animator>();

        curHealth = maxHealth;

        if(!IsBoss)
        {
            Invoke("ChaseStart", 2.0f);
        }
    }

    void ChaseStart()
    {
        isChase = true;
        animator.SetBool("isWalk", true);
    }

    void FreezeVelocity()
    {
        if(isChase)
        {
            rigid.velocity = Vector3.zero;
            rigid.angularVelocity = Vector3.zero;
        }
    }

    private void Targeting()
    {
        if (IsBoss || IsDead)
            return;

        float targetRadius = 1.5f;
        float targetRange = 3f;

        switch (type)
        {
            case Type.A:
                targetRadius = 1.5f;
                targetRange = 3f;
                break;
            case Type.B:
                targetRadius = 1f;
                targetRange = 12f;
                break;
            case Type.C:
                targetRadius = 0.5f;
                targetRange = 25f;
                break;
        }

        RaycastHit[] rayHits = Physics.SphereCastAll(transform.position, targetRadius, transform.forward, targetRange, LayerMask.GetMask("Player"));

        if (rayHits.Length > 0 && !isAttack)
        {
            StartCoroutine(Attack());
        }
    }

    private IEnumerator Attack()
    {
        isChase = false;
        isAttack = true;
        animator.SetBool("isAttack", true);

        switch (type)
        {
            case Type.A:
                yield return new WaitForSeconds(0.2f);
                meleeArea.enabled = true;

                yield return new WaitForSeconds(1f);
                meleeArea.enabled = false;

                yield return new WaitForSeconds(1f);
                break;
            case Type.B:
                yield return new WaitForSeconds(0.1f);
                rigid.AddForce(Vector3.forward * 20f, ForceMode.Impulse);
                meleeArea.enabled = true;

                yield return new WaitForSeconds(0.5f);
                rigid.velocity = Vector3.zero;
                meleeArea.enabled = false;

                yield return new WaitForSeconds(2f);
                break;
            case Type.C:
                yield return new WaitForSeconds(0.5f);
                GameObject instantBullet = Instantiate(bullet, transform.position, transform.rotation);
                Rigidbody rigidBullet = instantBullet.GetComponent<Rigidbody>();
                rigidBullet.velocity = transform.forward * 50f;

                yield return new WaitForSeconds(2f);
                break;
        }

        isChase = true;
        isAttack = false;
        animator.SetBool("isAttack", false);
    }

    private void FixedUpdate()
    {
        Targeting();
        FreezeVelocity();
    }

    private void Update()
    {
        if (navAgent.enabled && !IsBoss)
        {
            navAgent.SetDestination(target.position);
            navAgent.isStopped = !isChase;
        }
    }

    protected void OnTriggerEnter(Collider other)
    {
        if(other.CompareTag("Melee"))
        {
            Weapon weapon = other.GetComponent<Weapon>();
            curHealth -= weapon.damage;
            StartCoroutine(OnDamage());
        }
        else if(other.CompareTag("Bullet"))
        {
            StartCoroutine(OnDamage());
            Bullet bullet = other.GetComponent<Bullet>();
            curHealth -= bullet.damage;
            Destroy(other.gameObject);
        }
    }

    public void HitByGrenade(Vector3 explosionPos)
    {
        curHealth -= 100;
        StartCoroutine(OnDamage());
    }

    protected void ColorChange(Color inColor)
    {
        foreach (MeshRenderer meshRenderer in meshRenderers)
        {
            meshRenderer.material.color = inColor;
        }
    }

    protected IEnumerator OnDamage()
    {
        ColorChange(Color.red);
        yield return new WaitForSeconds(0.1f);

        if(curHealth > 0)
        {
            ColorChange(Color.white);
        }
        else
        {
            ColorChange(Color.gray);

            gameObject.layer = 11;

            isChase = false;

            navAgent.enabled = false;
            animator.SetTrigger("doDie");
            rigid.AddForce(Vector3.up * 10, ForceMode.Impulse);
            StopAllCoroutines();

            Destroy(gameObject, 2.0f);
        }
    }
}