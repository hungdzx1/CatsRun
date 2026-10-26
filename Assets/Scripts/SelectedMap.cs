using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;

// Gắn script này lên SelectedMapPanel
public class SelectedMap : MonoBehaviour
{
    [System.Serializable]
    public class MapSlot
    {
        [Tooltip("Số thứ tự map: 1, 2, 3")]
        public int mapNumber = 1;
        [Tooltip("Tên scene sẽ load khi bấm Select")]
        public string sceneName;

        [Header("UI")]
        public Button cardButton;          // Button gắn trên Map1/Map2/Map3 (hoặc Card)
        public GameObject selectionFrame;  // Khung vàng khi được chọn
        public GameObject lockObject;      // Lớp che mờ + ổ khóa
        public GameObject scoreFrame;      // FrameScore (khung điểm cao nhất)
        public TextMeshProUGUI highScoreText;
        public TextMeshProUGUI unlockText; // (tuỳ chọn) chữ "Unlock: High score: ..." trong Lock

        [Header("Điều kiện mở khóa")]
        [Tooltip("Điểm cao nhất của map trước phải LỚN HƠN số này. Map 1 để 0.")]
        public int requiredScore = 0;

        [HideInInspector] public bool isUnlocked;
    }

    [SerializeField] private MapSlot[] maps = new MapSlot[]
    {
        new MapSlot { mapNumber = 1, sceneName = "Map 1", requiredScore = 0 },
        new MapSlot { mapNumber = 2, sceneName = "Map2",  requiredScore = 500 },
        new MapSlot { mapNumber = 3, sceneName = "Map 3", requiredScore = 700 },
    };

    [SerializeField] private Button selectButton;
    [SerializeField] private Button exitButton;

    [Header("Debug / Test")]
    [Tooltip("Tích vào đây để mở khóa tất cả các map ngay lập tức để tiện test game!")]
    [SerializeField] private bool unlockAllForTesting = false;

    private int selectedIndex = -1;

    private void Awake()
    {
        if (maps == null || maps.Length == 0) return;

        for (int i = 0; i < maps.Length; i++)
        {
            int index = i; // tránh lỗi closure
            if (maps[i].cardButton != null)
            {
                maps[i].cardButton.onClick.RemoveAllListeners();
                maps[i].cardButton.onClick.AddListener(() => OnMapClicked(index));
            }
        }

        if (selectButton != null)
        {
            selectButton.onClick.RemoveAllListeners();
            selectButton.onClick.AddListener(PlaySelectedMap);
        }

        if (exitButton != null)
        {
            exitButton.onClick.RemoveAllListeners();
            exitButton.onClick.AddListener(ClosePanel);
        }
    }

    private void OnEnable()
    {
        Refresh();
    }

    public void ClosePanel()
    {
        if (AudioManager.instance != null)
            AudioManager.instance.PlayButtonClickSFX();

        gameObject.SetActive(false);
    }

    // Cập nhật trạng thái khóa, điểm cao và khung chọn
    public void Refresh()
    {
        if (maps == null || maps.Length == 0) return;

        for (int i = 0; i < maps.Length; i++)
        {
            MapSlot m = maps[i];

            // Nếu bật unlockAllForTesting thì mở khóa toàn bộ
            if (unlockAllForTesting)
            {
                m.isUnlocked = true;
            }
            // Map đầu luôn mở; map sau mở khi map trước đã mở VÀ điểm map trước > requiredScore
            else if (i == 0)
            {
                m.isUnlocked = true;
            }
            else
            {
                int prevMapScore = GameManager.GetHighScore(maps[i - 1].mapNumber);
                m.isUnlocked = maps[i - 1].isUnlocked && prevMapScore > m.requiredScore;
            }

            // Cập nhật điểm cao nhất của map này
            int best = GameManager.GetHighScore(m.mapNumber);
            if (m.highScoreText != null) 
                m.highScoreText.text = best.ToString();

            // Khóa / mở các thành phần UI
            if (m.lockObject != null) 
                m.lockObject.SetActive(!m.isUnlocked);

            if (m.scoreFrame != null) 
                m.scoreFrame.SetActive(m.isUnlocked);

            if (m.cardButton != null)
                m.cardButton.interactable = m.isUnlocked;

            // Hiển thị điều kiện mở khóa dưới ổ khóa
            if (m.unlockText != null && i > 0)
            {
                m.unlockText.text = "Unlock:\nHigh score > " + m.requiredScore;
            }
        }

        // Chọn map hợp lệ: nếu map đang chọn bị khóa hoặc không hợp lệ, chọn map đầu tiên đã mở
        if (selectedIndex < 0 || selectedIndex >= maps.Length || !maps[selectedIndex].isUnlocked)
        {
            selectedIndex = GetFirstUnlockedMapIndex();
        }

        UpdateSelectionFrames();
    }

    private int GetFirstUnlockedMapIndex()
    {
        for (int i = 0; i < maps.Length; i++)
        {
            if (maps[i].isUnlocked) return i;
        }
        return 0;
    }

    public void OnMapClicked(int index)
    {
        if (index < 0 || index >= maps.Length) return;
        if (!maps[index].isUnlocked) return; // map khóa thì không chọn được

        if (AudioManager.instance != null) 
            AudioManager.instance.PlayButtonClickSFX();

        selectedIndex = index;
        UpdateSelectionFrames();
    }

    private void UpdateSelectionFrames()
    {
        if (maps == null) return;

        for (int i = 0; i < maps.Length; i++)
        {
            // Chỉ hiện SelectionFrame khi đúng là map đang chọn VÀ map đó đã mở khóa
            if (maps[i].selectionFrame != null)
                maps[i].selectionFrame.SetActive(i == selectedIndex && maps[i].isUnlocked);
        }

        if (selectButton != null)
        {
            bool canPlay = selectedIndex >= 0 && selectedIndex < maps.Length && maps[selectedIndex].isUnlocked;
            selectButton.interactable = canPlay;
        }
    }

    public void PlaySelectedMap()
    {
        if (selectedIndex < 0 || selectedIndex >= maps.Length) return;
        if (!maps[selectedIndex].isUnlocked) return;

        if (AudioManager.instance != null)
            AudioManager.instance.PlayButtonClickSFX();

        Time.timeScale = 1f; // Đảm bảo thời gian game chạy bình thường khi vào scene mới

        string scene = maps[selectedIndex].sceneName;
        if (!string.IsNullOrEmpty(scene))
        {
            SceneManager.LoadScene(scene);
        }
        else
        {
            Debug.LogWarning("[SelectedMap] Chưa đặt sceneName cho map số " + maps[selectedIndex].mapNumber);
        }
    }
}
