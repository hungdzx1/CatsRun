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
    }

    // Update is called once per frame
    void Update()
    {
        EnterStartGame();
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
        scoreText.text = "Score: " + Mathf.FloorToInt(score).ToString();
    }

    private void StartGame()
    {
        IsGameStarted = false;
        Time.timeScale = 0;
        Text[0].SetActive(false);
        Text[1].SetActive(true);
        Text[2].SetActive(false);
        Text[3].SetActive(false);
        Text[4].SetActive(false);
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
    }

    public void ResumeGame()
    {
        Time.timeScale = 1;
        Text[0].SetActive(true);
        Text[1].SetActive(false);
        Text[2].SetActive(false);
        Text[3].SetActive(true);
        Text[4].SetActive(false);
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
}
