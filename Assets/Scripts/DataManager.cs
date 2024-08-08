using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;

[System.Serializable]
public class SaveData
{
    public List<int> StarCountsPerMap = new();
    public int CompletedMap;
    public int TotalCoin;
    public List<ShopItemSO> ShopItemPurchaseList = new();
    public ShopItemSO CurrentShopItemSO;
    public List<Colors> HasBeenUsedColorList = new();
}

public class DataManager : Singleton<DataManager>
{
    string _path;
    const string _dbFileName = "database.json";
    void Start()
    {
        _path = Path.Combine(Application.persistentDataPath, _dbFileName);
    }

    public void JsonLoad()
    {
        SaveData saveData = new SaveData();
        _path = Path.Combine(Application.persistentDataPath, _dbFileName);

        if (!File.Exists(_path))
        {
            GameManager.Instance.CompletedMap = 0;
            JsonSave();
        }
        else
        {
            string loadJson = File.ReadAllText(_path);
            saveData = JsonUtility.FromJson<SaveData>(loadJson);

            if (saveData != null)
            {
                for (int i = 0; i < saveData.StarCountsPerMap.Count; i++)
                {
                    GameManager.Instance.StarCountsPerMap.Add(saveData.StarCountsPerMap[i]);
                }
                for (int i = 0; i < saveData.ShopItemPurchaseList.Count; i++)
                {
                    GameManager.Instance.ShopItemPurchaseList.Add(saveData.ShopItemPurchaseList[i]);
                }
                GameManager.Instance.CompletedMap = saveData.CompletedMap;
                GameManager.Instance.TotalCoin = saveData.TotalCoin;
                GameManager.Instance.CurrentShopItemSO = saveData.CurrentShopItemSO;
            }
        }
    }

    public void JsonSave()
    {
        _path = Path.Combine(Application.persistentDataPath, _dbFileName);

        SaveData saveData = new SaveData();

        // Data Load
        for (int i = 0; i < GameManager.Instance.StarCountsPerMap.Count; i++)
        {
            saveData.StarCountsPerMap.Add(GameManager.Instance.StarCountsPerMap[i]);
        }
        for (int i = 0; i < GameManager.Instance.ShopItemPurchaseList.Count; i++)
        {
            Debug.Log(GameManager.Instance.ShopItemPurchaseList.Count);
            saveData.ShopItemPurchaseList.Add(GameManager.Instance.ShopItemPurchaseList[i]);
        }
        saveData.CompletedMap = GameManager.Instance.CompletedMap;
        saveData.TotalCoin = GameManager.Instance.TotalCoin;
        saveData.CurrentShopItemSO = GameManager.Instance.CurrentShopItemSO;

        // Data Save
        string json = JsonUtility.ToJson(saveData, true);
        File.WriteAllText(_path, json);
    }

    public void JsonClear() // Clear Data
    {
        _path = Path.Combine(Application.persistentDataPath, _dbFileName);

        SaveData saveData = new SaveData();
        saveData.CompletedMap = 0;
        saveData.TotalCoin = 0;

        string json = JsonUtility.ToJson(saveData, true);
        File.WriteAllText(_path, json);
    }

}
