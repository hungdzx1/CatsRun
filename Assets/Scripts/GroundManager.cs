using UnityEngine;

public class GroundManager : MonoBehaviour
{
    [SerializeField] private float positionGround;
    [SerializeField] private float speedGround = 0.01f; //speedGround = Parallax

    private float gameSpeed;
    private float xGround;

    void Start()
    {
        xGround = positionGround;
        gameSpeed = GameManager.instance != null ? GameManager.instance.GetGameSpeed() : 5f;
    }

    void Update()
    {
        if (GameManager.instance != null)
            gameSpeed = GameManager.instance.GetGameSpeed();

        MoveGround();
    }

    private void MoveGround()
    {
        xGround -= speedGround * gameSpeed * Time.deltaTime; //Dùng Translate cũng được
        if (xGround <= -18f)
        {
            xGround = 36f;
        }
        transform.position = new Vector3(xGround, transform.position.y, transform.position.z);
        // có thể dùng Vector2
    }
}
