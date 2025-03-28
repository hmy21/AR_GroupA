using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro; 

public class PauseMenu : MonoBehaviour
{
    public GameObject pauseMenuUI;
    public Button pauseButton;
    public GameObject gameOverMenuUI;
    public TextMeshProUGUI scoreText;
    public GameObject canvas;
    public Button RestartButton;

    private bool isPaused = false;

    void Start()
    {
        // 隐藏暂停菜单
        RestartButton.gameObject.SetActive(false);
        pauseMenuUI.SetActive(false);
        gameOverMenuUI.SetActive(false);

        // 监听按钮点击
        RestartButton.onClick.AddListener(RestartGame);

        // 监听 GameState 变化
        GameManager.OnGameStateChange += OnGameStateChange;
    }

    void OnDestroy()
    {
        // 取消监听，防止内存泄漏
        GameManager.OnGameStateChange -= OnGameStateChange;
    }

    void Update()
    {
        // 监听 ESC 键打开/关闭菜单
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (isPaused)
            {
                ResumeGame();
            }
            else
            {
                PauseGame();
            }
        }
    }

    // 监听 GameState 变化
    private void OnGameStateChange(GameState newState)
    {
        if (newState == GameState.PauseMenu)
        {
            PauseGame();
        }
        else if (newState == GameState.GameStart)
        {
            ResumeGame();
        }
        else if (newState == GameState.GameOver)
        {
            ShowGameOverMenu();
        }
    }

    // 点击暂停按钮时调用
    public void PauseGame()
    {
        pauseMenuUI.SetActive(true);
        if (pauseButton != null)
    {
        pauseButton.gameObject.SetActive(false);
    }
        Time.timeScale = 0f; // 暂停游戏
        isPaused = true;
        UpdateScoreText(); // 更新分数
    }

    // 继续游戏
    public void ResumeGame()
    {
        pauseMenuUI.SetActive(false);
        if (pauseButton != null)
    {
        pauseButton.gameObject.SetActive(true);
    }
        Time.timeScale = 1f; // 恢复游戏
        isPaused = false;
    }

    // 显示游戏结束菜单
    void ShowGameOverMenu()
    {
        canvas.SetActive(false);
        RestartButton.gameObject.SetActive(true);
        gameOverMenuUI.SetActive(true); // 显示菜单
        if (pauseButton != null)
    {
        pauseButton.gameObject.SetActive(false);
    }
        scoreText.text = "Final Score: " + GameManager.Instance.score.ToString();
        Time.timeScale = 0f;
    }

    // 更新分数显示
    void UpdateScoreText()
    {
        if (scoreText != null)
        {
            scoreText.text = "Score: " + GameManager.Instance.score.ToString(); 
        }
    }

    // 重新开始游戏
    public void RestartGame()
    {
        Time.timeScale = 1f; // 恢复游戏速度
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        GameManager.Instance.UpdateGameState(GameState.GameStart); // 重新开始时回到 GameStart 状态
    }
}
