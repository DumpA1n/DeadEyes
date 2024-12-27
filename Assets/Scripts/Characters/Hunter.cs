using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Mirror;

public class Hunter : NetworkBehaviour
{
    [SyncVar] public float m_CurHealth = 100.0f;
    public float m_MinHealth = 0.0f;
    public float m_MaxHealth = 100.0f;
    private Weapon curWeapon; // 当前拾取的武器
    private Transform m_WeaponHolder;   // 武器存放的位置
    private GameObject m_FloatingInfo;
    private Canvas HealthBar;
    private Image HealthBarFront;

    void Start()
    {
        m_WeaponHolder = transform.Find("WeaponHolder");
        m_FloatingInfo = transform.Find("FloatingInfo").gameObject;
        HealthBar = transform.Find("FloatingInfo/HealthBar").GetComponent<Canvas>();
        HealthBarFront = transform.Find("FloatingInfo/HealthBar/HealthBarFront").GetComponent<Image>();
        if (!isLocalPlayer) {
            m_FloatingInfo.SetActive(false);
        }
    }

    public void PickupWeapon(Weapon weapon)
    {
        if (Input.GetKeyDown(KeyCode.E)) {
            if (curWeapon == null) {
                curWeapon = weapon;
                weapon.AttachToPlayer(m_WeaponHolder);

                Debug.Log("拾取了武器：" + weapon.itemName);
            } else {
                Debug.Log("已有武器，无法拾取新的武器！");
            }
        }
    }

    public void DropWeapon()
    {
        if (curWeapon != null) {
            curWeapon.DetachFromPlayer();
            curWeapon = null;

            Debug.Log("丢弃了武器");
        }
    }

    public void Shoot()
    {
        if (curWeapon != null) {
            curWeapon.Shoot();
        }
    }

    public void TakeDamage(float damage)
    {
        m_CurHealth -= damage;
        Debug.Log($"{this.name} 受到伤害，剩余生命值：{m_CurHealth}");
        if (m_CurHealth <= m_MinHealth) {
            Die();
        }
    }

    private void Die()
    {
        Debug.Log($"{this.name} 已被消灭！");
        Destroy(gameObject);
    }

    void Update()
    {
        if (isLocalPlayer)
        {
            if (Input.GetMouseButton(0))
            {
                Shoot();
            }

            if (Input.GetKeyDown(KeyCode.G))
            {
                DropWeapon();
            }
        }

        RectTransform rectTransform = HealthBarFront.GetComponent<RectTransform>();
        rectTransform.sizeDelta = new Vector2(m_CurHealth / m_MaxHealth, rectTransform.sizeDelta.y);
    }
}
