using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Mirror;
using UnityEditor;

public class Hider : NetworkBehaviour
{
    [SyncVar] public float m_CurHealth = 100.0f;
    public float m_MinHealth = 0.0f;
    public float m_MaxHealth = 100.0f;
    private GameObject m_HUD;
    private GameObject m_FloatingInfo;
    private Canvas HealthBar;
    private Image HealthBarFront;
    [SyncVar] public bool bShowHealthBar = false;
    private Item[] Inventory;
    private int curChooseItem = 0;
    void Start() {
        m_HUD = transform.Find("HUD").gameObject;
        m_FloatingInfo = transform.Find("FloatingInfo").gameObject;
        HealthBar = transform.Find("FloatingInfo/HealthBar").GetComponent<Canvas>();
        HealthBarFront = transform.Find("FloatingInfo/HealthBar/HealthBarFront").GetComponent<Image>();
        if (!isLocalPlayer) {
            m_FloatingInfo.SetActive(false);
            m_HUD.SetActive(false);
        }
        Inventory = new Item[3];
    }

    public void PickupItem(Item item)
    {
        // if (Input.GetKeyDown(KeyCode.E)) {
            for (int i = 0; i < Inventory.Length; i++) {
                if (Inventory[i] == null) {
                    Inventory[i] = item;
                    Debug.Log("拾取了道具：" + item.itemName);
                    break;
                }
            }
        // }
    }

    public void UseItem() {
        Inventory[curChooseItem].Interact(this.gameObject);
        Inventory[curChooseItem] = null;
    }

    public void TakeDamage(float damage)
    {
        m_CurHealth -= damage;
        StopCoroutine(ShowHealthBar());
        StartCoroutine(ShowHealthBar());
        Debug.Log($"目标受到伤害，剩余生命值：{m_CurHealth}");

        if (m_CurHealth <= m_MinHealth) {
            Die();
        }
    }

    IEnumerator ShowHealthBar() {
        bShowHealthBar = true;
        yield return new WaitForSeconds(1.0f);
        bShowHealthBar = false;
    }

    private void Die()
    {
        Destroy(gameObject);
    }

    void Update()
    {
        if (isLocalPlayer)
        {
            if (Input.GetKeyDown(KeyCode.Q))
            {
                curChooseItem = (curChooseItem + 1) % Inventory.Length;
            }

            if (Input.GetKeyDown(KeyCode.F))
            {
                if (Inventory[curChooseItem] != null)
                    UseItem();
                else Debug.Log("没有此物体");
            }

            for (int i = 0; i < Inventory.Length; i++) {
                GameObject frame = m_HUD.transform.Find($"ItemCell{i}/Frame").gameObject;
                if (i == curChooseItem) {
                    frame.SetActive(true);
                } else {
                    frame.SetActive(false);
                }
                Image image = m_HUD.transform.Find($"ItemCell{i}/Item").GetComponent<Image>();
                if (Inventory[i] != null) {
                    image.sprite = Inventory[i].itemSprite;
                    image.color = new Color(1, 1, 1, 1);
                } else {
                    image.sprite = null;
                    image.color = new Color(1, 1, 1, 0);
                }
            }
        }

        RectTransform rectTransform = HealthBarFront.GetComponent<RectTransform>();
        rectTransform.sizeDelta = new Vector2(m_CurHealth / m_MaxHealth, rectTransform.sizeDelta.y);
        if (!isLocalPlayer)
            HealthBar.enabled = bShowHealthBar;
    }
}
