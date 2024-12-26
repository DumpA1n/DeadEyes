using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public abstract class Item : NetworkBehaviour
{
    public string itemName;
    public string itemType;
    public string itemImage;
    [SyncVar] public bool isPickedUp = false;
    [SyncVar] public bool colliderEnabled = true;

    public abstract void Interact(GameObject owner);  // 道具的交互方法

    // 附着到玩家的某个指定位置
    public void AttachToPlayer(Transform attachPoint)
    {
        transform.SetParent(attachPoint);
        transform.localPosition = Vector3.zero;
        transform.localRotation = Quaternion.identity;
        if (isServer) {
            isPickedUp = true;
            colliderEnabled = false;
        }
    }

    // 使道具恢复为世界物体
    public void DetachFromPlayer()
    {
        transform.SetParent(null);
        if (isServer) {
            isPickedUp = false;
            colliderEnabled = true;
        }
    }
}
