using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using UnityEngine;

/// <typeparam name="T">Data type</typeparam>
public static class LocalDataSaver<T> where T : class, new()
{
	private const string AES_KEY = "Ze+H8bqe7apcTYz4RGvsKA==";
	private const string SAVE_EXTENSION = ".save";
	private const bool DEBUG_ENABLED = true;

    private readonly static string saveName = "periplum" + typeof(T).Name;

    private static string saveFullPath;

    public static T CurrentData
    {
        get
        {
            if (_currentData == null)
                LoadData();

            return _currentData;
        }

        private set
        {
            _currentData = value;
        }
    }
    private static T _currentData;

    /// <summary>
    /// Must be called from the main thread
    /// </summary>
    public static void InitPath()
    {
        saveFullPath = Application.persistentDataPath + "/" + saveName + SAVE_EXTENSION;
    }

    /// <typeparam name="T">Data type</typeparam>
    private static void LoadData()
    {
        if (saveFullPath == null)
            InitPath();

        T lSave = new T();
        BinaryFormatter lFormatter = new BinaryFormatter();
        FileStream lFileStream;

        if (CheckIfSaveExists())
        {
            lFileStream = File.Open(saveFullPath, FileMode.Open);

            try
            {
                lSave = SimpleAES.DecipherToObject<T>((string)lFormatter.Deserialize(lFileStream), AES_KEY);
                lFileStream.Close();

                if (DEBUG_ENABLED)
                    Debug.Log("Data loaded successfully!");
            }
            catch
            {
                lFileStream.Close();
                File.Delete(saveFullPath);

                if (DEBUG_ENABLED)
                    Debug.LogWarning("Data was corrupted, and has been deleted!");
            }
        }
        else
        {
            if (DEBUG_ENABLED)
                Debug.Log("Resquested data not found on local storage! Will use an empty one");
        }

        CurrentData = lSave;
    }

	public static void SaveCurrentData()
	{
		if (CurrentData == null)
			throw new System.Exception("No save loaded!");

        //Saving CurrentData on device

        BinaryFormatter lBf = new BinaryFormatter();
        FileStream lFile = File.Create(saveFullPath);

        string lSaveToStore = SimpleAES.Cipher(SimpleAES.ObjectToByteArray(CurrentData), AES_KEY);

        lBf.Serialize(lFile, lSaveToStore);
        lFile.Close();

        if (DEBUG_ENABLED)
            Debug.Log("Local Save Updated!");
    }

	private static bool CheckIfSaveExists()
	{
		return File.Exists(saveFullPath);
	}

	public static void ResetSave()
    {
		if (CheckIfSaveExists())
		{
			File.Delete(saveFullPath);
			LoadData();
		}
	}
}
