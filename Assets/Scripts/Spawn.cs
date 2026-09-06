using UnityEngine;

public class Spawn : MonoBehaviour
{
     [SerializeField] private GameObject[] obstacle;
    [SerializeField] private Transform flyPoint;
    [SerializeField] private Transform groundPoint;
    private float timer = 2f;
    [SerializeField] private float timeSpawn = 2f;

    
    void Start()
    {
        
    }

    void Update()
    {
        timer += Time.deltaTime;
        if(timer >= timeSpawn)
        {
            renderObstacle();
            timer = 0f;
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
