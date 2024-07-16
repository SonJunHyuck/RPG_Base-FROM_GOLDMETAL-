using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    [SerializeField]
    private float speed;

    public float Speed
    {
        get
        {
            if (isDodge)
                return speed * 2.0f;

            if (isWalkMode)
                return speed * 0.5f;

            return speed;
        }
        set
        {
            speed = value;
        }
    }

    private float axisH;
    private float axisV;
    private bool isWalkMode;
    private bool isJumpMode;
    private bool isJump;
    private bool isDodge;

    private bool IsMoving
    {
        get { return moveVec != Vector3.zero; }
    }

    private Vector3 moveVec;
    private Vector3 dodgeVec;

    private Animator animator;
    private Rigidbody rigid;

    private void Awake()
    {
        animator = GetComponentInChildren<Animator>();
        rigid = GetComponent<Rigidbody>();
    }

    void Update()
    {
        GetInput();

        moveVec = new Vector3(axisH, 0, axisV).normalized;

        moveVec = isDodge ? dodgeVec : moveVec;

        Move();
        Turn();
        Jump();
        Dodge();
    }

    private void GetInput()
    {
        axisH = Input.GetAxisRaw("Horizontal");
        axisV = Input.GetAxisRaw("Vertical");
        isWalkMode = Input.GetButton("Walk");
        isJumpMode = Input.GetButtonDown("Jump");
    }

    private void Move()
    {
        rigid.position += Speed * Time.deltaTime * moveVec;

        // .. Move의 연장선
        animator.SetBool("isRun", IsMoving);
        animator.SetBool("isWalk", isWalkMode);
    }

    private void Turn()
    {
        if (!IsMoving)
            return;

        // 회전은 캐릭터만 -> 카메라의 회전에 영향 x
        // 캐릭터가 -90도로 누워있기 때문에 up Vector로 보정
        Quaternion lookDir = Quaternion.LookRotation(moveVec);
        rigid.rotation = Quaternion.Slerp(rigid.rotation, lookDir, Time.deltaTime * 5.0f);
    }

    private void Jump()
    {
        if(isJumpMode && !IsMoving && !isJump && !isDodge)
        {
            float jumpPower = 15.0f;
            rigid.AddForce(Vector3.up * jumpPower, ForceMode.Impulse);
            animator.SetBool("isJump", true);
            animator.SetTrigger("doJump");
            isJump = true;
        }
    }

    private void Dodge()
    {
        if (isJumpMode && IsMoving && !isJump && !isDodge)
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

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.tag == "Floor")
        {
            isJump = false;
            animator.SetBool("isJump", isJump);
        }
    }
}
