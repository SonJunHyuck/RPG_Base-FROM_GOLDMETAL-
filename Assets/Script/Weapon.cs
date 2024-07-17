using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Weapon : MonoBehaviour
{
    public enum Type { Melee, Range };
    public Type type;
    public float damage;
    public float rate;

    private bool canAttack;
    public bool CanAttack
    {
        get { return canAttack; }
    } 

    [SerializeField]
    private float attackSpeed;
    public float AttackSpeed
    {
        get { return attackSpeed; }
    }

    private BoxCollider meleeArea;
    private TrailRenderer trailRenderer;

    private void Awake()
    {
        meleeArea = GetComponent<BoxCollider>();
        trailRenderer = GetComponentInChildren<TrailRenderer>();

        canAttack = true;
    }

    public void Use()
    {
        if (!canAttack)
            return;

        canAttack = false;
        if(type == Type.Melee)
        {
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
        canAttack = true;
    }
}
