using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro; // 引入 TextMeshPro 命名空间

public class GameManager : Singleton<GameManager>
{
    [SerializeField] GameState currentGameState;
    public GameState GameState => currentGameState;

    [SerializeField] private Camera gameCamera;
    public Vector3 cameraPosition { get => gameCamera.transform.position; }
    public static event Action<GameState> OnGameStateChange;

    public int score = 0;
    public TextMeshProUGUI scoreText; // 关联 ScoreText

    public void UpdateGameState(GameState newState)
    {
        currentGameState = newState;
        switch (newState)
        {
            case GameState.GameStart:
                HandleGameStart();
                break;
            case GameState.GameOver:
                HandleGameOver();
                break;
            default:
                throw new ArgumentOutOfRangeException(nameof(newState), newState, null);

        }
        OnGameStateChange?.Invoke(newState);

    }

    // 增加分数
    public void winScore(int num)
    {
        score += num;
        UpdateScoreUI(); // 更新 UI
    }

    // 更新分数 UI
    private void UpdateScoreUI()
    {
        if (scoreText != null)
        {
            scoreText.text = "Score: " + score;
        }
    }

    // 处理游戏结束
    private void HandleGameOver()
    {
        Debug.Log("GameOver");
    }

    // 处理游戏开始或重启
    private void HandleGameStart()
    {
        score = 0; // 重新开始时分数归零
        UpdateScoreUI(); // 重置 UI
        Debug.Log("GameStart");
    }


    // Start is called before the first frame update
    void Start()
    {
        // 初始化 UI
        UpdateScoreUI();
    }

    // Update is called once per frame
    void Update()
    {
        // 可根据需要在 Update() 中添加逻辑
    }
}

public enum GameState
{
    //Summary:
    //         The state for game start and restart.
    GameStart,
    PauseMenu,
    GameOver,

}
