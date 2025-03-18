using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    public int MaxHealth = 3;
    public int playerHealth;

    public void getDamage()
    {
        playerHealth -= 1;
        if (playerHealth <= 0)
        {
            GameManager.Instance.UpdateGameState(GameState.GameOver);
        }
    }
    // Start is called before the first frame update
    void Start()
    {
        GameManager.OnGameStateChange += OnGameStateChange;
        resetPlayer();
    }

    // Update is called once per frame
    void Update()
    {

    }

    private void OnGameStateChange(GameState gameState)
    {
        if (gameState == GameState.GameStart)
        {
            resetPlayer();
        }
    }

    private void resetPlayer()
    {
        playerHealth = MaxHealth;
    }
}
