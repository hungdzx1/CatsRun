using UnityEngine;

public class Obstacle : MonoBehaviour
{
   
    public float leftRange = -18f;
    
    void Start()
    {
        
    }

    void Update()
    {
        moveObstacle();
    }

    private void moveObstacle()
    {
        transform.Translate(Vector2.left * GameManager.instance.GetGameSpeed() * Time.deltaTime);
        if (transform.position.x < leftRange)
        {
            Destroy(gameObject);
        }
    }

    private void OnTriggerEnter2D(Collider2D collider2D)
    {
        if (collider2D.CompareTag("Player"))
        {
            GameManager.instance.GameOver();
        }
    }

}
