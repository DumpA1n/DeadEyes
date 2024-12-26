using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpeedBoost : Item
{
    public string playerTag = "Player";
    public override void Interact(GameObject owner) {
        StartCoroutine(UseSpeedBoost(owner));
    }

    IEnumerator UseSpeedBoost(GameObject owner) {
        PlayerController ctl = owner.GetComponent<PlayerController>();
        float origMoveSpeed = ctl.m_BaseMoveSpeed;
        ctl.m_BaseMoveSpeed = 20.0f;
        yield return new WaitForSeconds(3.0f);
        ctl.m_BaseMoveSpeed = origMoveSpeed;
        Destroy(this);
    }

    private void OnTriggerStay(Collider other)
    {
        if (!isPickedUp && other.CompareTag(playerTag))
        {
            Hider hider = other.GetComponent<Hider>();
            if (hider != null) {
                hider.PickupItem(this);
            }
        }
    }
}
