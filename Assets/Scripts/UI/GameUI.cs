using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Mirror;
using TMPro;
using System;
using UnityEngine.SceneManagement;
using Telepathy;

public class GameUI : NetworkBehaviour
{
    public float gameTimer = 300f;      // 游戏总时长，单位秒
    [SyncVar] public float remainingTime;         // 剩余时间
    public bool isGameRunning = false;  // 游戏是否运行

    public Slider slider;
    public TextMeshProUGUI PlayerCount;
    [SyncVar] private int playerCount;
    private int localPlayerCount;

    public GameObject gameEndPanel;
    public GameObject timePanel;
    private TextMeshProUGUI timePanelText;

    public GameObject startGameButton;


    public bool isHunterWin = false;
    public bool isHiderWin = false;

    private void Start()
    {
        remainingTime = gameTimer;
        timePanelText = timePanel.GetComponentInChildren<TextMeshProUGUI>();
        timePanel.SetActive(true);
        gameEndPanel.SetActive(false);
        if (!isServer) {
            startGameButton.SetActive(false);
        }
        // if (!isLocalPlayer)
        //     this.gameObject.SetActive(false);
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

        if (isServer && playerCount > 1 && isGameRunning) {
            remainingTime -= Time.deltaTime;
            RpcSetValueToClient(slider.value, remainingTime);

            if (HunterVictoryCondition()) {
                RpcSetWinner(true, false);
                RpcSetGameStatus(false);
            }
    
            if (HiderVictoryCondition()) {
                RpcSetWinner(false, true);
                RpcSetGameStatus(false);
            }
        }

        // if (isLocalPlayer) {
            if (isHunterWin) {
                gameEndPanel.SetActive(true);
                gameEndPanel.transform.Find("GameEndText").GetComponent<TextMeshProUGUI>().text = "Hunter Win";
            }

            if (isHiderWin) {
                gameEndPanel.SetActive(true);
                gameEndPanel.transform.Find("GameEndText").GetComponent<TextMeshProUGUI>().text = "Hider Win";
            }
        // }

        // if (playerCount > 1 && isGameRunning) {
        //     if (HiderVictoryCondition()) {
        //         gameEndPanel.SetActive(true);
        //         gameEndPanel.transform.Find("GameEndText").GetComponent<TextMeshProUGUI>().text = "Hider Win";
        //         if (isServer) {
        //             RpcSetGameStatus(false);
        //             // RpcSetValueToClient(slider.value, gameTimer);
        //         }
        //     }

        //     if (HunterVictoryCondition()) {
        //         gameEndPanel.SetActive(true);
        //         gameEndPanel.transform.Find("GameEndText").GetComponent<TextMeshProUGUI>().text = "Hunter Win";
        //         if (isServer) {
        //             RpcSetGameStatus(false);
        //             // RpcSetValueToClient(slider.value, gameTimer);
        //         }
        //     }
        // }

        timePanelText.text = "Remaining Time: " + Convert.ToString((int)remainingTime);
    }

    public void StartGame() {
        if (isServer) {
            RpcSetGameStatus(true);
            startGameButton.SetActive(false);
        }
    }

    // 判断是否满足Hider胜利条件：时间归零
    private bool HiderVictoryCondition()
    {
        return remainingTime <= 0;
    }

    // 判断是否满足Hunter胜利条件：找到所有Hiders
    private bool HunterVictoryCondition()
    {
        int hiderCount = GameObject.FindGameObjectsWithTag("Hider").Length;
        return hiderCount == 0;
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

    [ClientRpc]
    private void RpcSetGameStatus(bool state)
    {
        isGameRunning = state;
    }

    [ClientRpc]
    private void RpcSetWinner(bool Hunter, bool Hider)
    {
        isHunterWin = Hunter;
        isHiderWin  = Hider;
    }
}