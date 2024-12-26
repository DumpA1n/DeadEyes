using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Mirror;
using TMPro;
using System;
using UnityEngine.SceneManagement;

public class GameUI : NetworkBehaviour
{
    public float gameTimer = 300f;      // 游戏总时长，单位秒
    [SyncVar] public float remainingTime;         // 剩余时间
    public Slider slider;
    public TextMeshProUGUI PlayerCount;
    [SyncVar] private int playerCount;
    private int localPlayerCount;
    public GameObject gameEndPanel;
    public GameObject timePanel;
    private TextMeshProUGUI timePanelText;

    private void Start()
    {
        remainingTime = gameTimer;
        timePanelText = timePanel.GetComponentInChildren<TextMeshProUGUI>();
        timePanel.SetActive(false);
        gameEndPanel.SetActive(false);
    }

    private void Update()
    {
        if (isServer)
        {
            int curPlayerCount = NetworkServer.connections.Count;
            if (curPlayerCount != playerCount) {
                playerCount = curPlayerCount;
            }
        }
        if (localPlayerCount != playerCount) {
            PlayerCount.text = "PLAYER COUNT : " + Convert.ToString(playerCount);
            localPlayerCount = playerCount;
        }

        if (isServer) {
            float curRemainingTime = remainingTime - Time.deltaTime;
            RpcSetValueToClient(slider.value, curRemainingTime);
        }
    
        // if (remainingTime <= 0) {
        //     gameEndPanel.SetActive(true);
        // }
        // timePanelText.text = "Remaining Time: " + Convert.ToString((int)remainingTime);
    }

    [Command(requiresAuthority = false)]
    private void CmdSetValueToServer(float value, float value2)
    {
        slider.value = value;
        remainingTime = value2;
    }

    [ClientRpc]
    private void RpcSetValueToClient(float value, float value2)
    {
        slider.value = value;
        remainingTime = value2;
    }
}