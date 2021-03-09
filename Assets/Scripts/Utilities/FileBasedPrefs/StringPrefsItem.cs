using System;
using System.Linq;

[Serializable]
public class StringPrefsItem : PrefsItem
{
    public string value;
    public override Type DataType => typeof(string);

    public StringPrefsItem() { }
    public StringPrefsItem(string key, string value)
    {
        base.key = key;
        this.value = value;
    }

    public override object GetValueFromKey(string key, object defaultValue, FileBasedPrefsSaveData data)
    {
        for (int i = 0; i < data.stringData.Length; i++)
        {
            if (data.stringData[i].key.Equals(key)) { return data.stringData[i].value; }
        }

        return defaultValue;
    }
    public override bool HasKey(string key, FileBasedPrefsSaveData data)
    {
        for (int i = 0; i < data.stringData.Length; i++)
        {
            if (data.stringData[i].key.Equals(key)) { return true; }
        }

        return false;
    }
    public override void SetValueForExistingKey(string key, object value, FileBasedPrefsSaveData data)
    {
        if (!(value is string)) { return; }

        for (int i = 0; i < data.stringData.Length; i++)
        {
            if (data.stringData[i].key.Equals(key)) { data.stringData[i].value = (string)value; }
        }
    }
    public override void SetValueForNewKey(string key, object value, FileBasedPrefsSaveData data)
    {
        if (!(value is string)) { return; }

        var tempList = data.stringData.ToList();
        tempList.Add(new StringPrefsItem(key, (string)value));
        data.stringData = tempList.ToArray();
    }
    public override void DeleteKey(string key, FileBasedPrefsSaveData data)
    {
        for (int i = data.stringData.Length - 1; i >= 0; i--)
        {
            if (data.stringData[i].key.Equals(key))
            {
                var tempList = data.stringData.ToList();
                tempList.RemoveAt(i);
                data.stringData = tempList.ToArray();
            }
        }
    }
}
