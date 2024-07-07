using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class UIController : MonoBehaviour
{
    #region "Singleton"
    public static UIController Instance { get; private set; } // Singleton instance

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this; // If not, set it to this instance
            DontDestroyOnLoad(gameObject); // Make this instance persist across scenes
        }
        else if (Instance != this)
        {
            Destroy(gameObject); // Destroy this instance if another instance already exists
        }
    }
    #endregion
    public GameObject UI;
    public GameObject EndUI;
    public GameObject StatsInfo;

    public GameObject FadeUI;

    public GameObject FadeUINextLevel;

    public TextMeshProUGUI playerScore;

    public TextMeshProUGUI playerLife;
    public TextMeshProUGUI gameTime;

    public TextMeshProUGUI playerScoreEnd;
    public TextMeshProUGUI gameTimeEnd;



    void Start()
    {
        if (StatsInfo.activeSelf)
        {
            StatsInfo.SetActive(false);
        }
        if (EndUI.activeSelf)
        {
            EndUI.SetActive(false);
        }
        UI.SetActive(true);
    }
    public void UpdateScore(int score)
    {
        playerScore.text = score.ToString();
    }

    public void UpdateTime(float time)
    {
        gameTime.text = time.ToString();
    }

    public void UpdateLife(int life)
    {
        playerLife.text = life.ToString();
    }
    public void StartGame()
    {
        if (GameManager.Instance != null)
            GameManager.Instance.StartGame();
        UI.SetActive(false);
        StatsInfo.SetActive(true);
        if (AudioManager.Instance != null)
            AudioManager.Instance.PlayButton();
    }

    public void PlayAgain()
    {
        UI.SetActive(true);
        StatsInfo.SetActive(false);
        EndUI.SetActive(false);
        if (AudioManager.Instance != null)
            AudioManager.Instance.PlayButton();
        Scene currentScene = SceneManager.GetActiveScene();
        SceneManager.LoadScene(currentScene.name);
        AudioManager.Instance.currentTrackIndex = 0;
    }

    public void ShowFinalStats(int score, int time)
    {
        EndUI.SetActive(true);
        playerScoreEnd.text = score.ToString();
        gameTimeEnd.text = time.ToString();
    }

    public void Fade(bool NextLevel)
    {
        StartCoroutine(Fading(NextLevel));
    }

    IEnumerator Fading(bool NextLevel)
    {
        Player.Instance.canLoseLife = false;
        FadeUI.SetActive(true);
        if (NextLevel)
            FadeUINextLevel.SetActive(true);
        yield return new WaitForSeconds(2f);
        FadeUI.SetActive(false);
        if (NextLevel)
            FadeUINextLevel.SetActive(false);
        yield return new WaitForSeconds(0.5f);
        Player.Instance.canLoseLife = true;
    }
}
