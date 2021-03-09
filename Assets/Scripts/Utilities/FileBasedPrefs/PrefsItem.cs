using System;

[Serializable]
public abstract class PrefsItem
{
    public string key;
    public abstract Type DataType { get; }

    public PrefsItem() { }

    public abstract object GetValueFromKey(string key, object defaultValue, FileBasedPrefsSaveData data);
    public abstract bool HasKey(string key, FileBasedPrefsSaveData data);
    public abstract void SetValueForExistingKey(string key, object value, FileBasedPrefsSaveData data);
    public abstract void SetValueForNewKey(string key, object value, FileBasedPrefsSaveData data);
    public abstract void DeleteKey(string key, FileBasedPrefsSaveData data);
}
