using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;
    public sc_MiniGameSettings minigameSettings;

    private const string playerPrefix = "Player_";
    private static Dictionary<uint, sc_PlayerProperties> currentPlayerList = new Dictionary<uint, sc_PlayerProperties>();

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            return;
        }

        Debug.LogError("More than one instance of GameManager");
    }

    public static void RegisterPlayer(uint playerIDValue, sc_PlayerProperties _playerProperties)
    {
        currentPlayerList.Add(playerIDValue, _playerProperties);
        _playerProperties.transform.name = playerPrefix + playerIDValue.ToString();
    }

    public static void UnregisterPlayer(uint playerIDValue)
    {
        currentPlayerList.Remove(playerIDValue);
    }

    public static sc_PlayerProperties GetPlayerProperties(uint playerIDValue)
    {
        return currentPlayerList[playerIDValue];
    }
}
