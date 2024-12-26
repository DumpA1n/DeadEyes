using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Weapon : Item
{
    public string playerTag = "Player";
    public GameObject bulletPrefab;
    public Transform firePoint;
    public float bulletSpeed = 100f;
    public float damage = 10f;
    public float range = 50f;
    
    public float fireInterval = 0.1f;
    private float lastFireTime = 0f;

    public override void Interact(GameObject player) {}

    private void OnTriggerStay(Collider other)
    {
        if (!isPickedUp && other.CompareTag(playerTag))
        {
            Hunter hunter = other.GetComponent<Hunter>();
            if (hunter != null) {
                hunter.PickupWeapon(this);
            }
        }
    }

    public void Shoot()
    {
        if (Time.time - lastFireTime >= fireInterval) {
            if (bulletPrefab != null) {
                GameObject bullet = Instantiate(bulletPrefab, firePoint.position, firePoint.rotation);
                StartCoroutine(DestroyBullet(bullet, 1.5f));
                Rigidbody rb = bullet.GetComponent<Rigidbody>();
                if (rb != null) {
                    rb.velocity = firePoint.forward * bulletSpeed;
                }
            }

            RaycastHit hit;
            Debug.DrawRay(firePoint.position, firePoint.forward * range, Color.red, 2.0f);
            if (Physics.Raycast(firePoint.position, firePoint.forward, out hit, range)) {
                Debug.Log("命中目标：" + hit.collider.name);

                Hider target = hit.collider.GetComponentInParent<Hider>();
                if (target != null) {
                    target.TakeDamage(damage);
                }
            }
            // else {
            //     Debug.Log("未命中任何目标");
            // }

            lastFireTime = Time.time;
        }
    }

    private IEnumerator DestroyBullet(GameObject bullet, float lifespan)
    {
        yield return new WaitForSeconds(lifespan);
        Destroy(bullet);
    }
}
