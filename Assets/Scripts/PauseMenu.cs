using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class PauseMenu : MonoBehaviour
{
    public GameObject pauseMenuUI;
    public Text scoreText;
    public Button restartButton;

    private bool isPaused = false;

    void Start()
    {
        // 隐藏暂停菜单
        pauseMenuUI.SetActive(false);

        // 监听按钮点击
        restartButton.onClick.AddListener(RestartGame);
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

    // 点击暂停按钮时调用
    public void PauseGame()
    {
        pauseMenuUI.SetActive(true);
        Time.timeScale = 0f; // 暂停游戏
        isPaused = true;

        // 更新分数
        UpdateScoreText();
    }

    // 继续游戏
    public void ResumeGame()
    {
        pauseMenuUI.SetActive(false);
        Time.timeScale = 1f; // 恢复游戏
        isPaused = false;
    }

    // 更新分数显示
    void UpdateScoreText()
    {
        scoreText.text = "Score: " + GameManager.Instance.score.ToString();
    }

    // 重新开始游戏
    public void RestartGame()
    {
        Time.timeScale = 1f; // 恢复游戏速度
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }
}
