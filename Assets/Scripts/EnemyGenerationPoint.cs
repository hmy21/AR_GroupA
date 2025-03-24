using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class EnemyGenerationPoint : MonoBehaviour
{
    [SerializeField] float GenerateTime;
    [SerializeField] float currentTime;
    [SerializeField] GameObject EnemyPrefabs;
    [SerializeField] bool isActive;

    void Start()
    {
        currentTime = 0;
        isActive = true;
        GameManager.OnGameStateChange += OnGameStateChange;
    }

    void OnDestroy()
    {
        GameManager.OnGameStateChange -= OnGameStateChange;
    }
    void Update()
    {
        if (isActive)
        {
            currentTime += Time.deltaTime;
        }
        if (currentTime >= GenerateTime)
        {
            Instantiate(EnemyPrefabs, this.transform.position, Quaternion.identity);
            currentTime = 0;
        }
    }

    private void OnGameStateChange(GameState gameState)
    {
        if (gameState == GameState.GameStart)
        {
            isActive = true;
        }
        else if (gameState == GameState.GameOver)
        {
            isActive = false;
        }
    }

}
