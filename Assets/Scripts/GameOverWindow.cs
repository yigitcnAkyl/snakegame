using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameOverWindow : MonoBehaviour
{
    public static GameOverWindow Instance { get; private set; }

    [SerializeField] private GameObject contentPanel;
    [SerializeField] private TextMeshProUGUI currentScoreText;
    [SerializeField] private TextMeshProUGUI highScoreText;
    [SerializeField] private Button restartButton;

    private void Awake()
    {
        Instance = this;

        if (restartButton != null)
        {
            restartButton.onClick.AddListener(RestartGame);
        }

        Hide();
    }

    public void Show()
    {
        if (contentPanel != null)
        {
            contentPanel.SetActive(true);
        }

        // Skorları yazdır
        if (currentScoreText != null)
        {
            currentScoreText.text = "SCORE " + Score.GetScore().ToString();
        }

        if (highScoreText != null)
        {
            highScoreText.text = "HIGH SCORE " + Score.GetHighScore().ToString();
        }
    }

    public void Hide()
    {
        if (contentPanel != null)
        {
            contentPanel.SetActive(false);
        }
    }

    public void RestartGame()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    private void Update()
    {
        // Butona tıklamak dışında Space veya R tuşuyla da yeniden başlasın
        if (contentPanel != null && contentPanel.activeSelf)
        {
            if (Input.GetKeyDown(KeyCode.Space) || Input.GetKeyDown(KeyCode.R))
            {
                RestartGame();
            }
        }
    }
}