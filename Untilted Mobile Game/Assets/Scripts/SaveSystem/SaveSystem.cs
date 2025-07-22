using System.IO;
using UnityEngine;

public static class SaveSystem
{
    private static readonly string path = $"{Application.persistentDataPath}/playerdata.json";

    public static void SaveData(PlayerData playerData)
    {
        string json = JsonUtility.ToJson(playerData);

        File.WriteAllText(path, json);
    }

    public static PlayerData LoadPlayerData()
    {
        if (File.Exists(path))
        {
            string json = File.ReadAllText(path);

            PlayerData loadedData = JsonUtility.FromJson<PlayerData>(json);
            return loadedData;
        }
        else
        {
            Debug.Log("No player data was found!");
            return null;
        }
    }
}