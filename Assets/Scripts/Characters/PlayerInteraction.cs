using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class PlayerInteraction : NetworkBehaviour
{
    void PickupItem() {

    }
    public void DropItem(Item item)
    {
        if (item != null)
        {
            item.DetachFromPlayer();
            item = null;

            Debug.Log("丢弃了物品" + item.itemName);
        }
    }
}