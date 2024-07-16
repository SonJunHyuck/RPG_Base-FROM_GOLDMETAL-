using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Item : MonoBehaviour
{
    public enum Type
    {
        Ammo, Coin, Greade, Heart, Weapon
    };

    public Type type;
    public int value;

    private bool isUp;
    private Transform meshObject;

    private void Awake()
    {
        meshObject = transform.GetComponentInChildren<MeshRenderer>().transform;
    }

    private void Update()
    {
        Hovering();
    }

    private void Hovering()
    {
        if (isUp && meshObject.localPosition.y > 1.0f)
        {
            isUp = false;
        }
        else if(!isUp && meshObject.localPosition.y <= 0.0f)
        {
            isUp = true;
        }

        Vector3 hoverDir = isUp ? Vector3.up : Vector3.down;

        meshObject.Translate(hoverDir * Time.deltaTime);
    }
}
