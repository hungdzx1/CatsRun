using UnityEngine;
using UnityEngine.SceneManagement;

public class Mainmenu : MonoBehaviour
{
    [SerializeField] private GameObject[] Ob;
    public static Mainmenu Instance;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    } 

   
   public void Play(){
        SceneManager.LoadScene("GamePlay");
   }

   public void OpenSettings()
   {
      Ob[0].SetActive(true);
   }

   public void CloseSettings()
   {
      Ob[0].SetActive(false);
   }

   public void OpenSelectCharacter(){
      Ob[1].SetActive(true);
   }

   public void CloseSelectCharacter(){
      Ob[1].SetActive(false);
   }
}
