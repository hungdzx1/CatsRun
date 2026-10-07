using UnityEngine;
using UnityEngine.UI;

public class UIButton : MonoBehaviour
{
    
    void Start()
{
    AssignSoundToAllButtons();
}

private void AssignSoundToAllButtons()
{
    // Lấy tất cả Button con nằm trong Canvas/Panel này (kể cả nút đang ẩn)
    Button[] allButtons = GetComponentsInChildren<Button>(true);

    foreach (Button btn in allButtons)
    {
        btn.onClick.AddListener(() => AudioManager.instance.PlayButtonClickSFX());
    }
}
}

