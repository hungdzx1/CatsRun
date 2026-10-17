using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

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
            // Bắt sự kiện PointerDown 
            EventTrigger trigger = btn.gameObject.GetComponent<EventTrigger>();
            if (trigger == null)
            {
                trigger = btn.gameObject.AddComponent<EventTrigger>();
            }

            EventTrigger.Entry entry = new EventTrigger.Entry();
            entry.eventID = EventTriggerType.PointerDown;
            entry.callback.AddListener((data) =>
            {
                if (AudioManager.instance != null)
                {
                    AudioManager.instance.PlayButtonClickSFX();
                }
            });

            trigger.triggers.Add(entry);
        }
    }
}
