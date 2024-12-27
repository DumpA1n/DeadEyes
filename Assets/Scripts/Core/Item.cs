using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public abstract class Item : NetworkBehaviour
{
    public string itemName;
    public string itemType;
    public Sprite itemSprite;
    [SyncVar] public bool isPickedUp = false;
    [SyncVar] public bool colliderEnabled = true;

    public abstract void Interact(GameObject owner);

    public void AttachToPlayer(Transform attachPoint)
    {
        if (isServer)
        {
            transform.SetParent(attachPoint);
            transform.localPosition = Vector3.zero;
            transform.localRotation = Quaternion.identity;

            isPickedUp = true;
            colliderEnabled = false;

            RpcAttachToPlayer(attachPoint.position, attachPoint.rotation);
        }
    }

    public void DetachFromPlayer()
    {
        if (isServer)
        {
            transform.SetParent(null);
            isPickedUp = false;
            colliderEnabled = true;

            RpcDetachFromPlayer();
        }
    }

    [ClientRpc]
    private void RpcAttachToPlayer(Vector3 attachPosition, Quaternion attachRotation)
    {
        transform.SetParent(transform.parent);
        transform.position = attachPosition;
        transform.rotation = attachRotation;
    }

    [ClientRpc]
    private void RpcDetachFromPlayer()
    {
        transform.SetParent(null);
    }
}
