using UnityEngine;

public class CharacterSpawner : MonoBehaviour
{
    public GameObject[] characterPrefabs;
    public Transform spawnPoint; 

    void Awake()
    {
        SpawnSelectedCharacter();
    }

    private void SpawnSelectedCharacter()
    {
        int selectedIndex = PlayerPrefs.GetInt("SelectedCharacter", 0);

        if (selectedIndex >= 0 && selectedIndex < characterPrefabs.Length)
        {
            Transform targetPoint = spawnPoint != null ? spawnPoint : transform;
            
            GameObject player = Instantiate(characterPrefabs[selectedIndex], targetPoint.position, targetPoint.rotation);
        }
        else
        {
            Debug.LogError("Chỉ số nhân vật vượt quá số lượng Prefab hiện có!");
        }
    }
}