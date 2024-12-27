using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpeedBoost : Item
{
    public string playerTag = "Hider";
    public override void Interact(GameObject owner) {
        StartCoroutine(UseSpeedBoost(owner));
    }

    IEnumerator UseSpeedBoost(GameObject owner) {
        PlayerController ctl = owner.GetComponent<PlayerController>();
        ctl.m_SprintSpeed = 20.0f;
        yield return new WaitForSeconds(3.0f);
        ctl.m_SprintSpeed = 0.0f;
        // Destroy(this);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!isPickedUp && other.CompareTag(playerTag)) {
            Hider hider = other.GetComponent<Hider>();
            if (hider != null) {
                hider.PickupItem(this);
            }
        }
    }
}
