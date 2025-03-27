using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class Player : MonoBehaviour
{
    public int MaxHealth = 3;
    public int playerHealth = 3;
    [SerializeField] List<GameObject> Hearts;

    public void getDamage()
    {
        playerHealth --;
        Hearts[playerHealth].SetActive(false);
        if (playerHealth <= 0)
        {
            GameManager.Instance.UpdateGameState(GameState.GameOver);
        }
    }

    private void OnTriggerEnter(Collider other){
        Debug.Log("get hit");
        if(other.CompareTag("Bullet")){
            getDamage();
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
        if (GameManager.Instance.GameState == GameState.PauseMenu)
        {
            return; // 暂停时不更新玩家状态
        }
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
        foreach(GameObject gameObject in Hearts){
            gameObject.SetActive(true);
        }
    }
}
