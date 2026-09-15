using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;
using System.Collections;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;
    private float gameSpeed = 5f;

    [SerializeField] private float speedUp = 0.15f;

    [SerializeField] private TextMeshProUGUI scoreText;
    [SerializeField] private TextMeshProUGUI scoreTextGameOver;
    private float score = 0;
    [SerializeField] private TextMeshProUGUI bestScoreText;
    private int bestScore = 0;  

    [SerializeField] private GameObject[] Text;
    private bool isGameOver = false;
    public bool IsGameStarted;
    [SerializeField] private Animator playerAnimator;

    
    void Awake()
    {
        if(instance == null) instance = this;
    }

    public float GetGameSpeed()
    {
        return gameSpeed;
    }
    void Start()
    {
        StartGame();
        getBestScore();
    }

    // Update is called once per frame
    void Update()
    {
        EnterStartGame();
        best_Score();
        if (IsGameStarted && !isGameOver)
        {
             UpdateScore();
             UpdateGameSpeed();
        }
    }

    private void UpdateGameSpeed()
    {
        gameSpeed += speedUp * Time.deltaTime;
    }

    private void UpdateScore()
    {
        score += Time.deltaTime * 10;
        scoreText.text =  Mathf.FloorToInt(score).ToString();
    }

    private void StartGame()
    {
        IsGameStarted = false;
        Time.timeScale = 0;
        Text[0].SetActive(true); //score
        Text[1].SetActive(true); //press enter to start
        Text[2].SetActive(false); //game over
        Text[3].SetActive(true); //pause button
        Text[4].SetActive(false); //pause menu
        Text[5].SetActive(true); //best score
    }

    private void EnterStartGame()
    {
        if(Input.GetKeyDown(KeyCode.Return) && !isGameOver)
        {
            Time.timeScale = 1;
            IsGameStarted = true;
            Text[0].SetActive(true);
            Text[1].SetActive(false);
            Text[2].SetActive(false);
            Text[3].SetActive(true);
            Text[4].SetActive(false);
            Text[5].SetActive(true);

        }
    }

    public void GameOver()
    {
        if(isGameOver) return;
        isGameOver = true;
        StartCoroutine(GameOverRoutine());
    }

    private IEnumerator GameOverRoutine()
    {
        gameSpeed = 0;

        //đo thời gian chạy của anim
        float dieLength = playerAnimator.GetCurrentAnimatorStateInfo(0).length;
        yield return new WaitForSeconds(dieLength + 0.4f);

        scoreTextGameOver.text = "Score: " + Mathf.FloorToInt(score).ToString();
        Text[0].SetActive(false);
        Text[1].SetActive(false);
        Text[2].SetActive(true);
        Text[3].SetActive(false);
        Text[4].SetActive(false);
        Text[5].SetActive(false);

        Time.timeScale = 0;
    }

    //Pause
    public void PauseGame()
    {
        Time.timeScale = 0;
        Text[0].SetActive(false);
        Text[1].SetActive(false);
        Text[2].SetActive(false);
        Text[3].SetActive(false);
        Text[4].SetActive(true);
        Text[5].SetActive(false);

    }

    public void ResumeGame()
    {
        Time.timeScale = 1;
        Text[0].SetActive(true);
        Text[1].SetActive(false);
        Text[2].SetActive(false);
        Text[3].SetActive(true);
        Text[4].SetActive(false);
        Text[5].SetActive(true);

    }

    public void RestartGame()
    {
        Time.timeScale = 1;
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    public void BackToMenu()
    {
        Time.timeScale = 1;
        SceneManager.LoadScene("Main Menu");
    }

    private void best_Score()
    {
        if(score > bestScore)
        {
            bestScore = Mathf.FloorToInt(score);
            PlayerPrefs.SetInt("HighScore", bestScore);
            PlayerPrefs.Save();
            bestScoreText.text = bestScore.ToString();
        }
    }

    private void getBestScore()
    {
        bestScore = PlayerPrefs.GetInt("HighScore", 0);
        if (bestScoreText != null)
        {
            bestScoreText.text = bestScore.ToString();
        }
    }
}
