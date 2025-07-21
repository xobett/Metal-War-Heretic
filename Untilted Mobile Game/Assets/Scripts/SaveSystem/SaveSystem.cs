using System.IO;
using UnityEngine;

public static class SaveSystem
{
    private static readonly string path = $"{Application.persistentDataPath}/playerdata.json";

    public static void SaveData(PlayerData playerData)
    {
        string json = JsonUtility.ToJson(playerData);
    }

    public static void LoadPlayerData()
    {

    }
}