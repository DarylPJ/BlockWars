using System.IO;
using UnityEngine;

public class SaveManager : MonoBehaviour
{
    private string dataLocation;
    const string Key = "Hello Hackers!! Please consider watching a few ads :)";

    private void Awake()
    {
        var saveManagers = FindObjectsOfType<SaveManager>();

        if (saveManagers.Length > 1)
        {
            gameObject.SetActive(false);
            Destroy(gameObject);
            return;
        }

        dataLocation = Application.persistentDataPath + "/a526ae48-7883-41e0-ac8e-9060da3a8118.txt";
        Debug.Log(dataLocation);
        DontDestroyOnLoad(gameObject);
    }

    public void SaveData(SaveData saveData)
    {
        var rawData = JsonUtility.ToJson(saveData);
        var encryptedData = XORCipher(rawData);
        using var writter = new StreamWriter(dataLocation);
        writter.Write(encryptedData);
    }

    public SaveData GetSaveData()
    {
        try
        {
            using var reader = new StreamReader(dataLocation);
            var data = XORCipher(reader.ReadToEnd());

            return JsonUtility.FromJson<SaveData>(data);
        }
        catch (FileNotFoundException)
        {
            var data = new SaveData();
            SaveData(data);
            return data;
        }
    }
    
    private string XORCipher(string data)
	{
		int dataLen = data.Length;
		int keyLen = Key.Length;
		char[] output = new char[dataLen];

		for (int i = 0; i < dataLen; ++i)
		{
			output[i] = (char)(data[i] ^ Key[i % keyLen]);
		}

		return new string(output);
	}
}
