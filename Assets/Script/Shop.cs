using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Shop : MonoBehaviour
{
    public RectTransform uiGroup;
    public Animator animator;

    public GameObject[] itemObj;
    public int[] itemPrice;
    public Transform spawnPos;
    public Text talkText;

    private Player enterPlayer;

    public void Enter(Player player)
    {
        enterPlayer = player;
        uiGroup.anchoredPosition = Vector3.zero;
    }

    public void Exit()
    {
        animator.SetTrigger("doHello");
        uiGroup.anchoredPosition = Vector3.down * 1000;
    }

    public void Buy(int index)
    {
        int price = itemPrice[index];

        if(enterPlayer.coin < price)
        {
            talkText.text = "You not enough coin!";
            return;
        }

        enterPlayer.coin -= price;

        GameObject item = itemObj[index];
        Instantiate(item, spawnPos.position, item.transform.rotation);
    }
}
