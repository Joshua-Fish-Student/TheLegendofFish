using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class Pickup : MonoBehaviour
{
    Player player;
    [SerializeField] Item_SO typeOfItem;
    private void Awake()
    {
        player = FindObjectOfType<Player>();
    }
    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject == player.gameObject) CollectItem();
    }
    void CollectItem()
    {
        player.moneys += typeOfItem.moneysAmount;
        player.UpdateHealth(typeOfItem.healAmount);
        Destroy(gameObject);
    }
}
