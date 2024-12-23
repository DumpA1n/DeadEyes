using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class GameManager : NetworkBehaviour
{
    public static GameManager instance;
    public bool isGameRunning = false;  // 游戏是否运行
    public float gameTimer = 300f;      // 游戏总时长，单位秒
    public float remainingTime;         // 剩余时间
    public GameObject hunterPrefab;     // 猎人预制体
    public GameObject hiderPrefab;      // 逃生者预制体
    public Transform hunterSpawnPoint;  // 猎人出生点
    public Transform hiderSpawnPoint;   // 逃生者出生点
    public GameObject selectionUI;  // 角色选择 UI

    private void Awake()
    {
        if (instance == null)
            instance = this;
        else
            Destroy(gameObject);
    }

    void Start()
    {
        remainingTime = gameTimer;
        selectionUI.SetActive(true); // 显示角色选择界面
    }

    public void SelectHunter()
    {
        selectionUI.SetActive(false); // 隐藏角色选择界面
    }

    public void SelectHider()
    {
        selectionUI.SetActive(false); // 隐藏角色选择界面
    }

    void Update()
    {
        if (isGameRunning)
        {
            remainingTime -= Time.deltaTime;

            if (remainingTime <= 0)
            {
                EndGame();
            }
        }
    }

    public void StartGame()
    {
        isGameRunning = true;
        remainingTime = gameTimer;
        Debug.Log("游戏开始！");
    }

    public void EndGame()
    {
        isGameRunning = false;
        Debug.Log("游戏结束！");
    }

    public void PauseGame()
    {
        isGameRunning = false;
        Debug.Log("游戏暂停");
    }

    public void ResumeGame()
    {
        isGameRunning = true;
        Debug.Log("游戏继续");
    }

    public void RestartGame()
    {
        EndGame();
        StartGame();
        Debug.Log("游戏重启");
    }
}
