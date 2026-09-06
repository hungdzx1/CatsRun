using UnityEngine;

public class Parallax : MonoBehaviour
{
    private Material material;
    [SerializeField] float speedParallax = 0.01f;
    private float offset; 
    
    void Start()
    {
        material = GetComponent<Renderer>().material;
    }

    void Update()
    {
        ParallaxScroll();
    }

    private void ParallaxScroll()
    {
        float speed = GameManager.instance.GetGameSpeed() * speedParallax;
        offset += speed * Time.deltaTime;
        material.mainTextureOffset = new Vector2(offset, 0);
    }
}
