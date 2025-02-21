using UnityEngine;

public class PlayerSwitcher : MonoBehaviour
{
    public GameObject mechPrefab; // assign in inspector
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

