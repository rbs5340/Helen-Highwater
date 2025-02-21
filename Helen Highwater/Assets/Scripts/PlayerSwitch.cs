using UnityEngine;

public class PlayerSwitcher : MonoBehaviour
{
    public GameObject[] playerPrefabs; // assign player prefabs in inspector
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
            Vector3 position = currentPlayer.transform.position; // save position
            Destroy(currentPlayer); // remove old player
            currentIndex = (currentIndex + 1) % playerPrefabs.Length; // cycle index
            SpawnPlayer(currentIndex, position);
        }
    }

    private void SpawnPlayer(int index, Vector3? position = null)
    {
        Vector3 spawnPosition = position ?? Vector3.zero; // debug: default to (0,0,0) if no position
        currentPlayer = Instantiate(playerPrefabs[index], spawnPosition, Quaternion.identity);
    }
}
