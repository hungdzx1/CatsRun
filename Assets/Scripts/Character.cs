using UnityEngine;
using UnityEngine.UI;

public class Character : MonoBehaviour
{
    [SerializeField] private Image characterImage;
    [SerializeField] private Sprite[] characterSprites;
    [SerializeField] private Image[] boxImages;

    private int currentCharacterIndex = 0;

    void OnEnable()
    {
        currentCharacterIndex = PlayerPrefs.GetInt("SelectedCharacter", 0);
        UpdateCharacterImage();

    }

    public void NextCharacter()
    {   
        if(currentCharacterIndex != characterSprites.Length - 1)
        {
            currentCharacterIndex++;
            UpdateCharacterImage();
        }
        
    }

    public void PreviousCharacter()
    {
        currentCharacterIndex--;
        if(currentCharacterIndex < 0)
        {
            currentCharacterIndex = 0;
        }
        UpdateCharacterImage();
    }

    public void SelectCharacter()
    {

        PlayerPrefs.SetInt("SelectedCharacter", currentCharacterIndex);
        PlayerPrefs.Save();

         if (Mainmenu.Instance != null)
        {
            Mainmenu.Instance.CloseSelectCharacter();
        }
    }

    private void UpdateCharacterImage()
    {
        characterImage.sprite = characterSprites[currentCharacterIndex];
        //tắt đi các box khi không có nhân vật -> tránh lỗi UI
        if(currentCharacterIndex == 0)
        {
            boxImages[0].enabled = false;
            boxImages[2].enabled = true;
            boxImages[1].sprite = characterSprites[currentCharacterIndex];
            boxImages[2].sprite = characterSprites[currentCharacterIndex + 1];
        }
        else if(currentCharacterIndex == characterSprites.Length - 1)
        {
            boxImages[2].enabled = false;
            boxImages[0].enabled = true;
            boxImages[0].sprite = characterSprites[currentCharacterIndex - 1];
            boxImages[1].sprite = characterSprites[currentCharacterIndex];
            
        }
        else
        {
            boxImages[0].enabled = true;
            boxImages[2].enabled = true;
            boxImages[0].sprite = characterSprites[currentCharacterIndex - 1];
            boxImages[1].sprite = characterSprites[currentCharacterIndex];
            boxImages[2].sprite = characterSprites[currentCharacterIndex + 1];
            
        }
    }

}
