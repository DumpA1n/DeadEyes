using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Weapon : Item
{
    public string playerTag = "Player"; // 玩家对象的Tag
    public GameObject bulletPrefab; // 子弹的预制件
    public Transform firePoint; // 子弹发射位置
    public float damage = 10f; // 武器的伤害
    public float range = 50f; // 射线的最大射程

    private bool isPickedUp = false;

    public override void Interact()
    {
        Debug.Log("使用武器射击...");
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag(playerTag) && !isPickedUp)
        {
            Hunter hunter = other.GetComponent<Hunter>();
            if (hunter != null)
            {
                hunter.PickupWeapon(this);
                isPickedUp = true;

                GetComponent<Collider>().enabled = false; // 隐藏碰撞器防止重复拾取
            }
        }
    }

    // 执行武器射击行为
    public void Shoot()
    {
        if (bulletPrefab != null) // 使用子弹预制件
        {
            GameObject bullet = Instantiate(bulletPrefab, firePoint.position, Quaternion.identity);
            Rigidbody rb = bullet.GetComponent<Rigidbody>();
            if (rb != null)
            {
                rb.velocity = firePoint.forward * 100.0f; // 设置子弹的速度
            }

            Debug.Log("子弹发射！");
        }
        // else // 使用射线
        // {
            RaycastHit hit;
            Debug.DrawRay(firePoint.position, firePoint.forward * range, Color.red, 1.0f); // 绘制射线
            if (Physics.Raycast(firePoint.position, firePoint.forward, out hit, range))
            {
                Debug.Log("命中目标：" + hit.collider.name);

                // 对命中的目标执行伤害逻辑
                Hunter target = hit.collider.GetComponent<Hunter>();
                if (target != null)
                {
                    target.TakeDamage(damage);
                }
            }
            else
            {
                Debug.Log("未命中任何目标");
            }
        // }
    }
}