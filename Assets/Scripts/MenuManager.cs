using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MenuManager : Singleton<MenuManager>
{

    public GameObject gameOverMenu;
    public GameObject canvas;
    // Start is called before the first frame update
    void Start()
    {
        GameManager.OnGameStateChange+= OnGameStateChange;
    }

    protected override void OnDestroy(){
        base.OnDestroy();
        GameManager.OnGameStateChange-= OnGameStateChange;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnGameStateChange(GameState gameState)
    {
        gameOverMenu.SetActive(gameState == GameState.GameOver);
        
    }
}
