using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Scanner : Item
{
    public override void Interact()
    {
        Debug.Log("使用扫描器扫描区域...");
        Scan();
    }

    // 扫描操作
    public void Scan()
    {
        // 扫描逻辑：查找伪装的物品
        Debug.Log("扫描中...");
    }
}
