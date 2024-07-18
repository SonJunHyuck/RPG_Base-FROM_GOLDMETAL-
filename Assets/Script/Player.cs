using System;
using System.Collections;
using System.Collections.Generic;
using System.Xml;
using UnityEngine;

public class Player : MonoBehaviour
{
    [Header("Player Stat")]
    [SerializeField]
    private float speed;

    public float Speed
    {
        get
        {
            if (isDodge)
            {
                return speed * 2.0f;
            }

            if (isWalkKeyDown)
            {
                return speed * 0.5f;
            }

            return speed;
        }
        set
        {
            speed = value;
        }
    }

    public GameObject[] weapons;
    public bool[] hasWeapon;
    public GameObject[] grenades;
    public int hasGrenade;

    public int ammo;
    public int coin;
    public int health;

    public int maxAmmo;
    public int maxCoin;
    public int maxHealth;
    public int maxGrenade;

    private float axisH;
    private float axisV;

    // KeyDown
    private bool isWalkKeyDown;  // left shift
    private bool isJumpKeyDown;  // space
    private bool isAquireKeyDown;  // e
    private bool isSwapKeyDown1;
    private bool isSwapKeyDown2;
    private bool isSwapKeyDown3;
    private bool isAttackKeyDown;
    private bool isReloadKeyDown;

    private bool isJump;
    private bool isDodge;
    private bool isSwap;
    private bool isReload;

    public bool IsBorder
    {
        get
        {
            return Physics.Raycast(transform.position, transform.forward, 5, LayerMask.GetMask("Wall"));
        }
    }

    private bool IsMoving
    {
        get { return moveVec != Vector3.zero; }
    }

    private Vector3 moveVec;
    private Vector3 dodgeVec;

    private Animator animator;
    private Rigidbody rigid;

    private GameObject nearObject;
    private Weapon equipWeapon;
    private int equipWeaponIndex;
    public bool IsEquip
    {
        get 
        {
            Debug.Log("EquipWeapon is null");
            return equipWeapon != null;
        }
    }

    private float attackSpeed;
    public float AttackSpeed
    {
        get
        {
            return attackSpeed;
        }
        set
        {
            attackSpeed = value;
            animator.SetFloat("AttackSpeed", value);
        }
    }
    private bool canAttack;

    Dictionary<Weapon.Type, string> attackAnimationKeyDic;

    private void Awake()
    {
        equipWeaponIndex = -1;

        animator = GetComponentInChildren<Animator>();
        rigid = GetComponent<Rigidbody>();

        AttackSpeed = 1.0f;
        canAttack = true;
        isJump = false;
        isDodge = false;
        isSwap = false;
        isReload = false;

        attackAnimationKeyDic = new Dictionary<Weapon.Type, string>();
        attackAnimationKeyDic.Add(Weapon.Type.Melee, "doSwing");
        attackAnimationKeyDic.Add(Weapon.Type.Range, "doShot");
    }

    void Update()
    {
        GetInput();

        moveVec = new Vector3(axisH, 0, axisV).normalized;

        moveVec = isDodge ? dodgeVec : moveVec;

        Move();
        Turn();
        Jump();
        Attack();
        Reload();
        Dodge();
        Interaction();
        Swap();
    }

    // can 감지 -> nearGameObject -> e누르면 GetComponent<Item> -> null 아니면 Take
    // Take하면, Item은 스스로 소멸

    private void GetInput()
    {
        axisH = Input.GetAxisRaw("Horizontal");
        axisV = Input.GetAxisRaw("Vertical");
        isWalkKeyDown = Input.GetButton("Walk");
        isJumpKeyDown = Input.GetButtonDown("Jump");
        isAttackKeyDown = Input.GetButton("Fire1");
        isReloadKeyDown = Input.GetButtonDown("Reload");
        isAquireKeyDown = Input.GetButtonDown("Interaction");
        isSwapKeyDown1 = Input.GetButtonDown("Swap1");
        isSwapKeyDown2 = Input.GetButtonDown("Swap2");
        isSwapKeyDown3 = Input.GetButtonDown("Swap3");
    }

    private void Move()
    {
        if(!IsBorder)
        {
            rigid.position += Speed * Time.deltaTime * moveVec;
        }
        

        // .. Move의 연장선
        animator.SetBool("isRun", IsMoving);
        animator.SetBool("isWalk", isWalkKeyDown);
    }

    private void Turn()
    {
        if (!IsMoving)
            return;

        // 회전은 캐릭터만 -> 카메라의 회전에 영향 x
        // 캐릭터가 -90도로 누워있기 때문에 up Vector로 보정
        //Quaternion lookDir = Quaternion.LookRotation(moveVec);
        //rigid.rotation = Quaternion.Slerp(rigid.rotation, lookDir, Time.deltaTime * 5.0f);

        if(isAttackKeyDown)
        {
            if (!isJump && !isDodge && !isSwap && !isReload && canAttack)
            {
                Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
                RaycastHit hit;
                if (Physics.Raycast(ray, out hit, 100))
                {
                    Vector3 lookDir = hit.point - transform.position;

                    transform.LookAt(hit.point);
                }
            }
        }
        else
        {
            transform.LookAt(transform.position + moveVec);
        }
        
    }

    private void Jump()
    {
        if(isJumpKeyDown && !IsMoving && !isJump && !isDodge)
        {
            float jumpPower = 15.0f;
            rigid.AddForce(Vector3.up * jumpPower, ForceMode.Impulse);
            animator.SetBool("isJump", true);
            animator.SetTrigger("doJump");
            isJump = true;
        }
    }

    private void Attack()
    {
        if (!IsEquip)
            return;

        if (isAttackKeyDown && canAttack && !isDodge && !isSwap && equipWeapon.CanAttack && !isReload)
        {
            // dictionary<type, string> 사용
            animator.SetTrigger(attackAnimationKeyDic[equipWeapon.type]);
            StartCoroutine(AttackDelay());
        }
    }

    private IEnumerator AttackDelay()
    {
        if (!IsEquip)
            yield break;

        canAttack = false;
        yield return new WaitForSeconds(equipWeapon.AttackCooltime / AttackSpeed);
        canAttack = true;
    }

    private void Reload()
    {
        if (!isReloadKeyDown)
            return;

        if (!IsEquip)
        {
            Debug.Log("EquipWeapon is null");
            return;
        }

        if (equipWeapon.type == Weapon.Type.Melee)
        {
            Debug.Log("You have equiped melee type weapon now");
            return;
        }

        if (ammo == 0)
        {
            Debug.Log("You not enough ammo");
            return;
        }

        if(!isReload && !isDodge && !isSwap && canAttack)
        {
            isReload = true;
            animator.SetTrigger("doReload");
        }
    }
    public void ReloadOff()
    {
        ammo = equipWeapon.Reload(ammo);
        isReload = false;
    }

    private void Dodge()
    {
        if (isJumpKeyDown && IsMoving && !isJump && !isDodge && !isReload)
        {
            dodgeVec = moveVec;
            animator.SetTrigger("doDodge");
            isDodge = true;
        }
    }

    public void DodgeOff()
    {
        isDodge = false;
    }

    public void Interaction()
    {
        if (isAquireKeyDown && nearObject != null)
        {
            if(nearObject.CompareTag("Weapon"))
            {
                Item item = nearObject.GetComponent<Item>();
                int weaponIndex = item.value;
                hasWeapon[weaponIndex] = true;

                nearObject = null;
                item.Disappear();
            }
        }
    }

    public void Swap()
    {
        if (isSwap || isDodge)
            return;

        bool isSwapKeyDown = (isSwapKeyDown1 || isSwapKeyDown2 || isSwapKeyDown3);

        if (!isSwapKeyDown)
        {
            //Debug.Log("Don't input any Key");
            return;
        }

        int weaponIndex = -1;
        if (isSwapKeyDown1) 
        { 
            weaponIndex = 0; 
        }
        if (isSwapKeyDown2)
        {
            weaponIndex = 1;
        }
        if (isSwapKeyDown3)
        {
            weaponIndex = 2;
        }

        if (weaponIndex == -1)
        {
            Debug.Log("Don't have any weapon");
            return;
        }

        // 스왑 아이템 없으면 리턴
        if (!hasWeapon[weaponIndex])
        {
            Debug.Log("Don't have" + weaponIndex + "weapon");
            return;
        }

        if(weaponIndex == equipWeaponIndex)
        {
            Debug.Log("You already equiped same weapon");
            return;
        }

        if(equipWeapon != null)
        {
            equipWeapon.gameObject.SetActive(false);
        }

        equipWeapon = weapons[weaponIndex].GetComponent<Weapon>();
        equipWeaponIndex = weaponIndex;
        weapons[weaponIndex].SetActive(true);

        animator.SetTrigger("doSwap");
        isSwap = true;
    }

    public void SwapOff()
    {
        isSwap = false;
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Floor"))
        {
            isJump = false;
            animator.SetBool("isJump", isJump);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if(other.CompareTag("Item"))
        {
            Item item = other.GetComponent<Item>();
            switch(item.type)
            {
                case Item.Type.Ammo:
                    ammo += item.value;
                    ammo = Math.Min(ammo, maxAmmo);
                    break;
                case Item.Type.Coin:
                    coin += item.value;
                    coin = Math.Min(coin, maxCoin);
                    break;
                case Item.Type.Heart:
                    health += item.value;
                    health = Math.Min(health, maxHealth);
                    break;
                case Item.Type.Grenade:
                    if (hasGrenade == maxGrenade)
                        return;

                    grenades[hasGrenade].SetActive(true);
                    hasGrenade += item.value;
                    hasGrenade = Math.Min(hasGrenade, maxGrenade);
                    break;
            }

            item.Disappear();
        }
        
    }

    private void OnTriggerStay(Collider other)
    {
        if (other.CompareTag("Weapon"))
        {
            nearObject = other.gameObject;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (nearObject != null)
            nearObject = null;
    }
}