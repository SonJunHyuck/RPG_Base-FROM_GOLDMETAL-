using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAnimationHandler : MonoBehaviour
{
    private Player player;

    private void Awake()
    {
        player = transform.root.GetComponent<Player>();
    }

    // Dodge에서 Event로 호출
    private void OnDodgeEndEvent()
    {
        Debug.Log("OnEndEvent");
        player.DodgeOff();
    }
}
