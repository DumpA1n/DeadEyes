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
    public Scanner scanner;  // 扫描器
    public Trap trap;        // 陷阱
    private Weapon curWeapon; // 当前拾取的武器
    private Transform m_WeaponHolder;   // 武器存放的位置
    public Image HealthBar;

    void Start()
    {
        m_WeaponHolder = transform.Find("WeaponHolder");
    }

    public void PickupWeapon(Weapon weapon)
    {
        if (curWeapon == null)
        {
            curWeapon = weapon;
            weapon.AttachToPlayer(m_WeaponHolder);

            // 向服务器同步武器位置
            CmdSyncWeaponPosition(m_WeaponHolder.position, m_WeaponHolder.rotation);

            Debug.Log("拾取了武器：" + weapon.itemName);
        }
        else
        {
            Debug.Log("已有武器，无法拾取新的武器！");
        }
    }

    public void DropWeapon()
    {
        if (curWeapon != null)
        {
            curWeapon.DetachFromPlayer();
            curWeapon = null;

            // 向服务器同步武器丢弃位置
            CmdSyncWeaponPosition(Vector3.zero, Quaternion.identity);

            Debug.Log("丢弃了武器");
        }
    }

    // 服务器命令：同步武器的位置和旋转
    [Command]
    private void CmdSyncWeaponPosition(Vector3 position, Quaternion rotation)
    {
        RpcSyncWeaponPosition(position, rotation);
    }

    // 客户端RPC：同步武器的位置和旋转
    [ClientRpc]
    private void RpcSyncWeaponPosition(Vector3 position, Quaternion rotation)
    {
        if (!isLocalPlayer && curWeapon != null)
        {
            // 确保只有本地客户端更新自己的武器位置
            curWeapon.transform.position = position;
            curWeapon.transform.rotation = rotation;
        }
    }

    public void Shoot()
    {
        if (curWeapon != null)
        {
            curWeapon.Shoot();
        }
        else
        {
            Debug.Log("没有持有武器，无法射击");
        }
    }

    public void SetTrap(Item target)
    {
        Debug.Log("设置陷阱：" + target.itemName);
        trap.Activate();
    }

    public void UseScanner()
    {
        scanner.Scan();
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

    void Update()
    {
        if (isLocalPlayer)
        {
            if (Input.GetMouseButtonDown(0))
            {
                Shoot();
            }

            if (Input.GetKeyDown(KeyCode.Q))
            {

            }

            if (Input.GetKeyDown(KeyCode.E))
            {
                DropWeapon();
            }
        }

        RectTransform rectTransform = HealthBar.GetComponent<RectTransform>();
        rectTransform.sizeDelta = new Vector2(m_CurHealth / m_MaxHealth, rectTransform.sizeDelta.y);
    }
}
