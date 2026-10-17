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
    if (obstacle == null || obstacle.Length == 0) return;
    
    int index = Random.Range(0, obstacle.Length);
    GameObject selectedObstacle = obstacle[index];
    if (selectedObstacle == null) return;
    // Tự động kiểm tra xem con này bay hay đi bộ để chọn điểm spawn thích hợp
    Obstacle obsComponent = selectedObstacle.GetComponent<Obstacle>();
    Transform spawnPoint = (obsComponent != null && obsComponent.IsFlying) ? flyPoint : groundPoint;
    Instantiate(selectedObstacle, spawnPoint.position, Quaternion.identity);
}
}
