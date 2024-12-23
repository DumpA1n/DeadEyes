using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Hider : PlayerInteraction
{
    public float m_CurHealth = 100.0f;
    public float m_MinHealth = 0.0f;
    public float m_MaxHealth = 100.0f;
    public void Disguise(Item item)
    {
        Debug.Log("伪装成物品：" + item.itemName);
        // 伪装成物品的逻辑
    }

    public void UseSmokeBomb()
    {
        Debug.Log("使用烟雾弹");
        // 使用烟雾弹的逻辑
    }

    public void UseSpeedBoost()
    {
        Debug.Log("使用加速道具");
        // 使用加速道具的逻辑
    }

    public void TakeDamage(float damage)
    {
        m_CurHealth -= damage;
        Debug.Log($"目标受到伤害，剩余生命值：{m_CurHealth}");

        if (m_CurHealth <= m_MinHealth)
        {
            Die();
        }
    }

    private void Die()
    {
        Debug.Log("目标已被消灭！");
        Destroy(gameObject);
    }
}
