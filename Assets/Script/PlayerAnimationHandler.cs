using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAnimationHandler : MonoBehaviour
{
    private Player player;
    [SerializeField]
    private Weapon weapon;
    public Weapon GetWeapon
    {
        get
        {
            if(weapon == null)
                weapon = transform.root.GetComponentInChildren<Weapon>();

            return weapon;
        }
    }

    private void Awake()
    {
        player = transform.root.GetComponent<Player>();
        weapon = transform.root.GetComponentInChildren<Weapon>();
    }

    #region Dodge
    // Dodge Animation에서 Event로 호출
    private void OnDodgeEndEvent()
    {
        Debug.Log("DodgeEndEvent");
        player.DodgeOff();
    }
    #endregion

    #region Swap
    // Swap Animation에서 Event로 호출
    private void OnSwapEndEvent()
    {
        Debug.Log("SwapEndEvent");
        player.SwapOff();
        weapon = transform.root.GetComponentInChildren<Weapon>();
    }
    #endregion

    #region Swing
    // Swing Animation에서 Event로 호출
    private void OnSwingAttackEvent()
    {
        GetWeapon.SwingAttack();
    }
    // Swing Animation에서 Event로 호출
    private void OnSwingAttackEndEvent()
    {
        GetWeapon.SwingAttackEnd();
    }
    // Swing Animation에서 Event로 호출
    private void OnSwingEndEvent()
    {
        GetWeapon.EndSwing();
    }
    #endregion

    private void OnShotAttackEvent()
    {
        GetWeapon.ShotAttack();
    }

    private void OnReloadEndEvent()
    {
        player.ReloadOff();
    }
}
