using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : Singleton<GameManager>
{
    [SerializeField] GameState currentGameState;
    public GameState GameState => currentGameState;
    [SerializeField] private Camera gameCamera;
    public Vector3 cameraPosition { get => gameCamera.transform.position; }
    public static event Action<GameState> OnGameStateChange;

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

    private void HandleGameOver()
    {
        
    }

    private void HandleGameStart()
    {
        
    }


    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }
}

public enum GameState
{
    GameStart,
    GameOver,
}
