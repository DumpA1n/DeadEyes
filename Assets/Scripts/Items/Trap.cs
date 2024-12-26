using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Trap : Item
{
    public override void Interact(GameObject owner)
    {
        Debug.Log("设置陷阱...");
        Activate();
    }

    // 激活陷阱
    public void Activate()
    {
        Debug.Log("陷阱激活，等待触发...");
        // 陷阱触发逻辑
    }
}
