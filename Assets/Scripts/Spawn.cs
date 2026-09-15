using UnityEngine;

public class Spawn : MonoBehaviour
{
     [SerializeField] private GameObject[] obstacle;
    [SerializeField] private Transform flyPoint;
    [SerializeField] private Transform groundPoint;
    private float timer = 2f;
    [SerializeField] private float timeSpawn = 2f;
    [SerializeField] private float timeSpawnBySpeed = 0.0015f;
    [SerializeField] private float minTimeSpawn = 0.75f;
    


    
    void Start()
    {
        
    }

    void Update()
    {
        UpdateTimeSpawn();
        timer += Time.deltaTime;
        if(timer >= timeSpawn)
        {
            renderObstacle();
            timer = 0f;
        }
    }

    void UpdateTimeSpawn()
    {
        timeSpawn -= timeSpawnBySpeed * GameManager.instance.GetGameSpeed() * Time.deltaTime;
        if(timeSpawn <= minTimeSpawn)
        {
            timeSpawn = minTimeSpawn;
        }
    }
    private void renderObstacle()
    {
        int index = Random.Range(0, obstacle.Length);
        if(index == 0 || index == 1)
        {
            GameObject newObstacle = Instantiate(obstacle[index], flyPoint.position, Quaternion.identity);
        }
        else if (index == 2)
        {
            GameObject newObstacle = Instantiate(obstacle[index], groundPoint.position, Quaternion.identity);
        }
    }
}
