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

    private void Start()
    {
        // Tự động tìm SelectedMapPanel nếu chưa gán trong Inspector
        if (selectMapPanel == null)
        {
            SelectedMap sm = FindFirstObjectByType<SelectedMap>(FindObjectsInactive.Include);
            if (sm != null) selectMapPanel = sm.gameObject;
        }

        // Lúc bắt đầu game ở Main Menu thì panel chọn map luôn tắt
        if (selectMapPanel != null)
        {
            selectMapPanel.SetActive(false);
        }
    }

    public void Play()
    {
        if (AudioManager.instance != null)
            AudioManager.instance.PlayButtonClickSFX();

        OpenSelectMap();
    }

    public void OpenSettings()
    {
        Ob[0].SetActive(true);
    }

    public void CloseSettings()
    {
        Ob[0].SetActive(false);
    }

    public void OpenSelectCharacter()
    {
        Ob[1].SetActive(true);
    }

    public void CloseSelectCharacter()
    {
        Ob[1].SetActive(false);
    }

    [SerializeField] private GameObject selectMapPanel;

    public void OpenSelectMap()
    {
        if (selectMapPanel == null)
        {
            SelectedMap sm = FindFirstObjectByType<SelectedMap>(FindObjectsInactive.Include);
            if (sm != null) selectMapPanel = sm.gameObject;
        }

        if (selectMapPanel != null)
        {
            selectMapPanel.SetActive(true);
        }
    }

    public void CloseSelectMap()
    {
        if (selectMapPanel != null)
        {
            selectMapPanel.SetActive(false);
        }
    }
}
