using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    private const string playerPrefix = "Player_";

    private static Dictionary<uint, PlayerProperties> currentPlayerList = new Dictionary<uint, PlayerProperties>();

    public static void RegisterPlayer(uint playerIDValue, PlayerProperties _playerProperties)
    {
        currentPlayerList.Add(playerIDValue, _playerProperties);
        _playerProperties.transform.name = playerPrefix + playerIDValue;
    }

    public static void UnregisterPlayer(uint playerIDValue)
    {
        currentPlayerList.Remove(playerIDValue);
    }

    public static PlayerProperties GetPlayerProperties(uint playerIDValue)
    {
        return currentPlayerList[playerIDValue];
    }
}
