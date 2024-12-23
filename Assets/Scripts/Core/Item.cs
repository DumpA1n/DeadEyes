using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public abstract class Item : NetworkBehaviour
{
    public string itemName;  // 道具的名字
    public string itemType;  // 道具的类型（如武器、陷阱、扫描器等）

    public abstract void Interact();  // 道具的交互方法

    // 附着到玩家的某个指定位置
    public void AttachToPlayer(Transform attachPoint)
    {
        transform.SetParent(attachPoint);
        transform.localPosition = Vector3.zero;
        transform.localRotation = Quaternion.identity;
    }

    // 使道具恢复为世界物体
    public void DetachFromPlayer()
    {
        transform.SetParent(null);
    }
}
