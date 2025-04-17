using UnityEngine;

public class PlayerSwitch : MonoBehaviour
{
    public GameObject[] playerPrefabs; // Assign different player prefabs in Inspector
    public GameObject[] tutorialBoards;
    private GameObject currentPlayer;
    private int currentIndex = 0;

    void Start()
    {
        SpawnPlayer(currentIndex);
    }

    public void SwitchPlayer()
    {
        if (currentPlayer != null)
        {
            SwitchBoards();
            // Helen to Mech SFX
            if(currentIndex == 0)
            {
                AudioManager.Instance.PlaySoundEffect("helenToMech");
                HealthDisplay.Instance.healthChange(4);
                AudioManager.Instance.StopAudio(0);
                AudioManager.Instance.PlayMusic(1, 0.2f);
                GlobalVar.Instance.helenState = 1;
            }
            // Mech to Helen SFX
            else if(currentIndex == 1)
            {
                AudioManager.Instance.PlaySoundEffect("mechToHelen");
                HealthDisplay.Instance.healthChange(3);
                AudioManager.Instance.StopAudio(1);
                AudioManager.Instance.PlayMusic(0, 0.2f);
                GlobalVar.Instance.helenState = 0;
            }
            Vector3 position = currentPlayer.transform.position; // Save player position
            Destroy(currentPlayer); // Remove old player
            currentIndex = (currentIndex + 1) % playerPrefabs.Length; // Cycle index
            SpawnPlayer(currentIndex, position);
        }
    }

    private void SpawnPlayer(int index, Vector3? position = null)
    {
        Vector3 spawnPosition = position ?? Vector3.zero; // Default to (0,0,0) if no position is given
        currentPlayer = Instantiate(playerPrefabs[index], spawnPosition, Quaternion.identity);
        CameraFollowPlayer.Instance.SetPlayer(currentPlayer);
    }

    private void SwitchBoards()
    {
        for (int i = 0; i < tutorialBoards.Length; i++)
        {
            tutorialBoards[i].SetActive(!tutorialBoards[i].activeSelf);
        }
    }
}
