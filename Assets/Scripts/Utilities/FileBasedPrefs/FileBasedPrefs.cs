using System;
using System.IO;
using UnityEngine;

public static class FileBasedPrefs
{
    private const string _SAVE_FILE_NAME = "PlayerPrefs.prefs";
    private const string _ENCRYPTION_CODEWORD = "wr7tg#kj;p-7@$";
    private const bool _SCRAMBLE_SAVE_DATA = false;
    private const bool _AUTO_SAVE_DATA = true;
    private const string _STRING_EMPTY = "";
    private static FileBasedPrefsSaveData _latestData;

    public static void SetString(string key, string value = _STRING_EMPTY) => AddDataToSaveFile(key, value);
    public static void SetInt(string key, int value = default) => AddDataToSaveFile(key, value);
    public static void SetFloat(string key, float value = default) => AddDataToSaveFile(key, value);
    public static void SetBool(string key, bool value = default) => AddDataToSaveFile(key, value);
    public static string GetString(string key, string defaultValue = _STRING_EMPTY) => (string)GetDataFromSaveFile(key, defaultValue);
    public static int GetInt(string key, int defaultValue = default) => (int)GetDataFromSaveFile(key, defaultValue);
    public static float GetFloat(string key, float defaultValue = default) => (float)GetDataFromSaveFile(key, defaultValue);
    public static bool GetBool(string key, bool defaultValue = default) => (bool)GetDataFromSaveFile(key, defaultValue);
    public static bool HasKey(string key) => LoadSaveFile().HasKey(key);
    public static bool HasKeyForString(string key) => LoadSaveFile().HasKeyFromObject(key, string.Empty);
    public static bool HasKeyForInt(string key) => LoadSaveFile().HasKeyFromObject(key, default(int));
    public static bool HasKeyForFloat(string key) => LoadSaveFile().HasKeyFromObject(key, default(float));
    public static bool HasKeyForBool(string key) => LoadSaveFile().HasKeyFromObject(key, default(bool));
    public static void DeleteKey(string key)
    {
        LoadSaveFile().DeleteKey(key);
        SaveSaveFile();
    }
    public static void DeleteString(string key)
    {
        LoadSaveFile().DeleteString(key);
        SaveSaveFile();
    }
    public static void DeleteInt(string key)
    {
        LoadSaveFile().DeleteInt(key);
        SaveSaveFile();
    }
    public static void DeleteFloat(string key)
    {
        LoadSaveFile().DeleteFloat(key);
        SaveSaveFile();
    }
    public static void DeleteBool(string key)
    {
        LoadSaveFile().DeleteBool(key);
        SaveSaveFile();
    }
    public static void DeleteAll()
    {
        WriteToSaveFile(JsonUtility.ToJson(new FileBasedPrefsSaveData()));
        _latestData = new FileBasedPrefsSaveData();
    }
    public static void OverwriteLocalSaveFile(string data)
    {
        WriteToSaveFile(data);
        _latestData = null;
    }
    public static void ManualySave()
    {
        SaveSaveFile(true);
    }
    public static string GetSaveFilePath() => Path.Combine(Application.persistentDataPath, _SAVE_FILE_NAME);
    public static string LoadSaveFileAsJson()
    {
        CheckSaveFileExists();

        return File.ReadAllText(GetSaveFilePath());
    }

    private static FileBasedPrefsSaveData LoadSaveFile()
    {
        CheckSaveFileExists();

        if (_latestData == null)
        {
            var saveFileText = File.ReadAllText(GetSaveFilePath());

            if (_SCRAMBLE_SAVE_DATA) { saveFileText = DataScrambler(saveFileText); }

            try { _latestData = JsonUtility.FromJson<FileBasedPrefsSaveData>(saveFileText); }
            catch (ArgumentException e)
            {
                Debug.LogException(new Exception("Save File In Wrong Format, Creating New Save File: " + e.Message));
                DeleteAll();
            }
        }

        return _latestData;
    }
    private static object GetDataFromSaveFile(string key, object defaultValue) => LoadSaveFile().GetValueFromKey(key, defaultValue);
    private static void AddDataToSaveFile(string key, object value)
    {
        LoadSaveFile().UpdateOrAddData(key, value);
        SaveSaveFile();
    }
    private static void SaveSaveFile(bool manualSave = false)
    {
        if (!_AUTO_SAVE_DATA && !manualSave) { return; }

        WriteToSaveFile(JsonUtility.ToJson(LoadSaveFile()));
    }
    private static void WriteToSaveFile(string data)
    {
        var streamWriter = new StreamWriter(GetSaveFilePath());

        if (_SCRAMBLE_SAVE_DATA) { data = DataScrambler(data); }

        streamWriter.Write(data);
        streamWriter.Close();
    }
    private static void CheckSaveFileExists()
    {
        if (DoesSaveFileExist()) { return; }

        CreateNewSaveFile();
    }
    private static bool DoesSaveFileExist() => File.Exists(GetSaveFilePath());
    private static void CreateNewSaveFile()
    {
        WriteToSaveFile(JsonUtility.ToJson(new FileBasedPrefsSaveData()));
    }
    private static string DataScrambler(string data)
    {
        string res = _STRING_EMPTY;

        for (int i = 0; i < data.Length; i++)
        {
            res += (char)(data[i] ^ _ENCRYPTION_CODEWORD[i % _ENCRYPTION_CODEWORD.Length]);
        }

        return res;
    }
}
