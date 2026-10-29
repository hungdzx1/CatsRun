using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Mainmenu : MonoBehaviour
{
    [SerializeField] private GameObject[] Ob;
    [SerializeField] private GameObject selectMapPanel;
    [SerializeField] private GameObject resetSuccessPanel; 
    public static Mainmenu Instance;

    private Coroutine successMessageCoroutine;

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

        // Ẩn bảng thông báo thành công lúc bắt đầu
        if (resetSuccessPanel != null)
        {
            resetSuccessPanel.SetActive(false);
        }

        if (Ob != null && Ob.Length > 0 && Ob[0] != null)
        {
            Ob[0].SetActive(false);
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
        if (Ob != null && Ob.Length > 0 && Ob[0] != null)
            Ob[0].SetActive(true);
    }

    public void CloseSettings()
    {
        if (Ob != null && Ob.Length > 0 && Ob[0] != null)
            Ob[0].SetActive(false);
    }

    public void OpenSelectCharacter()
    {
        if (Ob != null && Ob.Length > 1 && Ob[1] != null)
            Ob[1].SetActive(true);
    }

    public void CloseSelectCharacter()
    {
        if (Ob != null && Ob.Length > 1 && Ob[1] != null)
            Ob[1].SetActive(false);
    }

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

    public void ResetAllHighScores()
    {
        GameManager.ResetAllHighScores();
        CloseResetPanel();

        // Hiển thị thông báo thành công trong 3s rồi tự tắt
        if (resetSuccessPanel != null)
        {
            if (successMessageCoroutine != null)
            {
                StopCoroutine(successMessageCoroutine);
            }
            successMessageCoroutine = StartCoroutine(ShowSuccessNotificationRoutine());
        }
    }

    private IEnumerator ShowSuccessNotificationRoutine()
    {
        resetSuccessPanel.SetActive(true);
        yield return new WaitForSecondsRealtime(3f);
        resetSuccessPanel.SetActive(false);
        successMessageCoroutine = null;
    }

    public void OpenResetPanel()
    {
        if (Ob != null && Ob.Length > 2 && Ob[2] != null)
            Ob[2].SetActive(true);
    }

    public void CloseResetPanel()
    {
        if (Ob != null && Ob.Length > 2 && Ob[2] != null)
            Ob[2].SetActive(false);
    }
}
