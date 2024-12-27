using System.Collections;
using UnityEngine;
using Mirror;
using UnityEngine.UI;
using TMPro;

public class GameManager : NetworkBehaviour
{
    public static GameManager Instance;

    public GameObject readyButtonPrefab;
    public GameObject hunterVictoryPanel;
    public GameObject hiderVictoryPanel;
    public GameObject waitingForPlayersPanel;
    public TextMeshProUGUI gameTimerText;
    public float gameDuration = 300f;
    private float remainingTime;
    
    private bool gameStarted = false;
    private bool gameEnded = false;
    private int readyPlayerCount = 0;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
    }

    private void Start()
    {
        remainingTime = gameDuration;
        waitingForPlayersPanel.SetActive(true);
        hunterVictoryPanel.SetActive(false);
        hiderVictoryPanel.SetActive(false);
    }

    private void Update()
    {
        // 只有主机才能开始游戏
        if (isServer && !gameEnded)
        {
            if (gameStarted)
            {
                remainingTime -= Time.deltaTime;
                gameTimerText.text = "Time Remaining: " + Mathf.Max(0, Mathf.CeilToInt(remainingTime)).ToString();

                // 判断游戏是否结束
                if (remainingTime <= 0)
                {
                    EndGame(HiderVictoryCondition());
                }

                // 判断Hunter是否找到了所有Hiders
                if (HunterVictoryCondition())
                {
                    EndGame(true);
                }
            }
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
        int hunterCount = GameObject.FindGameObjectsWithTag("Hunter").Length;
        return hiderCount == 0;
    }

    // 游戏结束的处理
    private void EndGame(bool hunterVictory)
    {
        gameEnded = true;
        if (hunterVictory)
        {
            hunterVictoryPanel.SetActive(true);
        }
        else
        {
            hiderVictoryPanel.SetActive(true);
        }

        // 停止计时，禁止玩家行动
        waitingForPlayersPanel.SetActive(false);

        // 给所有玩家发送游戏结束消息
        RpcNotifyGameEnd(hunterVictory);
    }

    [ClientRpc]
    private void RpcNotifyGameEnd(bool hunterVictory)
    {
        if (hunterVictory)
        {
            hunterVictoryPanel.SetActive(true);
        }
        else
        {
            hiderVictoryPanel.SetActive(true);
        }
    }

    // 开始游戏
    [Server]
    public void StartGame()
    {
        if (gameStarted) return;

        gameStarted = true;
        waitingForPlayersPanel.SetActive(false);

        // 通知所有玩家游戏已开始
        RpcNotifyGameStart();
    }

    [ClientRpc]
    private void RpcNotifyGameStart()
    {
        Debug.Log("Game Started!");
    }

    // 玩家准备
    [Server]
    public void PlayerReady(NetworkConnection conn)
    {
        readyPlayerCount++;

        // 所有玩家都准备好后，主机可以开始游戏
        if (readyPlayerCount == NetworkServer.connections.Count)
        {
            StartGame();
        }
    }

    // 玩家退出时的处理
    [Server]
    public void PlayerLeft(NetworkConnection conn)
    {
        readyPlayerCount--;

        if (gameStarted && !gameEnded)
        {
            // 如果游戏已经开始，且玩家退出，则停止游戏并显示等待界面
            EndGame(false);
        }
    }

    // 创建准备按钮
    [Client]
    public void CreateReadyButton()
    {
        if (isLocalPlayer && !gameStarted && !gameEnded)
        {
            var button = Instantiate(readyButtonPrefab);
            button.transform.SetParent(transform, false);
            button.GetComponent<Button>().onClick.AddListener(OnReadyButtonClicked);
        }
    }

    private void OnReadyButtonClicked()
    {
        CmdNotifyServerReady();
    }

    [Command]
    private void CmdNotifyServerReady()
    {
        if (isServer) return; // 主机已经可以开始游戏，不需要再通知服务器

        GameManager.Instance.PlayerReady(connectionToClient);
    }

    // 重新开始游戏
    [Server]
    public void RestartGame()
    {
        if (gameEnded)
        {
            // 清除旧的游戏状态，重新开始
            gameStarted = false;
            gameEnded = false;
            readyPlayerCount = 0;
            remainingTime = gameDuration;

            // // 清空玩家和敌人的角色
            // foreach (var conn in NetworkServer.connections.Values)
            // {
            //     NetworkServer.DestroyPlayersForConnection(conn);
            // }

            // // 通知客户端重新加载场景
            // NetworkServer.SendToAll(new SceneMessage
            // {
            //     sceneName = SceneManager.GetActiveScene().name
            // });

            RpcNotifyGameRestart();
        }
    }

    [ClientRpc]
    private void RpcNotifyGameRestart()
    {
        waitingForPlayersPanel.SetActive(true);
        hunterVictoryPanel.SetActive(false);
        hiderVictoryPanel.SetActive(false);
    }
}
