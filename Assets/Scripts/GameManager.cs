using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;
using System.Collections;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;
    private float gameSpeed = 5f;
    private float maxGameSpeed = 20f;
    [SerializeField] private float speedUp = 0.15f;

    [SerializeField] private TextMeshProUGUI scoreText;
    [SerializeField] private TextMeshProUGUI scoreTextGameOver;
    private float score = 0;
    [SerializeField] private TextMeshProUGUI bestScoreText;
    [SerializeField] private TextMeshProUGUI bestScoreTextGameOver;
    private int bestScore = 0;  

    [SerializeField] private GameObject[] Text;
    public bool isGameOver = false;
    public bool IsGameStarted;
    public bool isStopSfxRun;
    public bool isPaused = false;
    private Animator playerAnimator;

    
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
        HandlePauseInput();
        CheckGameOverRetry();
        best_Score();
        if (IsGameStarted && !isGameOver)
        {
             UpdateScore();
             UpdateGameSpeed();
        }
    }

    private void HandlePauseInput()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (!IsGameStarted || isGameOver) return;

            if (Text != null && Text.Length > 6 && Text[6] != null && Text[6].activeSelf)
            {
                CloseSettingsMenu();
                return;
            }

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

    private void CheckGameOverRetry()
    {
        if (isGameOver)
        {
            bool isPanelActive = Text != null && Text.Length > 2 && Text[2] != null && Text[2].activeSelf;
            if (isPanelActive && (Input.GetKeyDown(KeyCode.Return) || Input.GetKeyDown(KeyCode.KeypadEnter)))
            {
                RestartGame();
            }
        }
    }

    private void UpdateGameSpeed()
    {
        if (gameSpeed < maxGameSpeed)
        {
            gameSpeed += speedUp * Time.deltaTime;
        }
        else
        {
            gameSpeed = maxGameSpeed;
        }
    }

    private void UpdateScore()
    {
        score += Time.deltaTime * 10;
        scoreText.text =  Mathf.FloorToInt(score).ToString();
    }

    private void StartGame()
    {
        isPaused = false;
        IsGameStarted = false;
        Time.timeScale = 0;
        Text[0].SetActive(true); //score
        Text[1].SetActive(true); //press enter to start
        Text[2].SetActive(false); //game over
        Text[3].SetActive(false); //pause button (ẩn khi chưa ấn Enter)
        Text[4].SetActive(false); //pause menu
        Text[5].SetActive(true); //best score
        Text[6].SetActive(false); //settings menu
    }

    private void EnterStartGame()
    {
        if(!IsGameStarted && (Input.GetKeyDown(KeyCode.Return) || Input.GetKeyDown(KeyCode.KeypadEnter)) && !isGameOver)
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
        isPaused = false;
        gameSpeed = 0;

        if (playerAnimator == null && Player.instance != null)
        {
            playerAnimator = Player.instance.GetComponent<Animator>();
        }

        //đo thời gian chạy của anim
        float dieLength = playerAnimator.GetCurrentAnimatorStateInfo(0).length;
        yield return new WaitForSeconds(dieLength + 0.4f);

        scoreTextGameOver.text = Mathf.FloorToInt(score).ToString();
        bestScoreTextGameOver.text = bestScore.ToString();
        Text[0].SetActive(false);
        Text[1].SetActive(false);
        Text[2].SetActive(true);
        Text[3].SetActive(false);
        Text[4].SetActive(false);
        Text[5].SetActive(false);
        AudioManager.instance.StopBGM();
        AudioManager.instance.StopRunSFX();
        AudioManager.instance.StopSlideSFX();
        AudioManager.instance.PlayGameOverSFX();
        isStopSfxRun = true;

        Time.timeScale = 0;
    }

    //Pause
    public void PauseGame()
    {
    
        if (!IsGameStarted || isGameOver) return;

        isPaused = true;
        Time.timeScale = 0;
        Text[0].SetActive(false);
        Text[1].SetActive(false);
        Text[2].SetActive(false);
        Text[3].SetActive(false);
        Text[4].SetActive(true);
        Text[5].SetActive(false);
        AudioManager.instance.pauseBGM();
        isStopSfxRun = true;
        AudioManager.instance.StopRunSFX();

    }

    public void ResumeGame()
    {
        if (!IsGameStarted || isGameOver) return;

        isPaused = false;
        Time.timeScale = 1;
        Text[0].SetActive(true);
        Text[1].SetActive(false);
        Text[2].SetActive(false);
        Text[3].SetActive(true);
        Text[4].SetActive(false);
        Text[5].SetActive(true);
        AudioManager.instance.resumBGM();
        isStopSfxRun = false;
        AudioManager.instance.PlayRunSFX();
    }

    public void RestartGame()
    {
        isPaused = false;
        Time.timeScale = 1;
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        AudioManager.instance.PlayBGM();
    }

    public void BackToMenu()
    {
        isPaused = false;
        Time.timeScale = 1;
        SceneManager.LoadScene("Main Menu");
    }

    public void OpenSettingsMenu()
    {
        Text[6].SetActive(true);
    }

    public void CloseSettingsMenu()
    {
        Text[6].SetActive(false);
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
