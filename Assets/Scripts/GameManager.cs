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
    private float score = 0;

    [SerializeField] private GameObject[] Text;
    private bool isGameOver = false;

    
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
        if (!isGameOver)
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
        Time.timeScale = 0;
        Text[0].SetActive(false);
        Text[1].SetActive(true);
        Text[2].SetActive(false);
    }

    private void EnterStartGame()
    {
        if(Input.GetKeyDown(KeyCode.Return) || Input.GetKeyDown(KeyCode.Space))
        {
            Time.timeScale = 1;
            Text[0].SetActive(true);
            Text[1].SetActive(false);
            Text[2].SetActive(false);
        }
    }

    public void GameOver()
    {
        isGameOver = true;
        
        Text[0].SetActive(false);
        Text[1].SetActive(false);
        Text[2].SetActive(true);
        StartCoroutine(ReloadScene());
        Time.timeScale = 0;
    }

    private IEnumerator ReloadScene()
    {
        yield return new WaitForSecondsRealtime(1f);

        Time.timeScale = 1f;

        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }
}
