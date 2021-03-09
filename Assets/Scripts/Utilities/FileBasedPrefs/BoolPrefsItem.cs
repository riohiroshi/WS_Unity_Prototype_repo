using System;
using System.Linq;

[Serializable]
public class BoolPrefsItem : PrefsItem
{
    public bool value;
    public override Type DataType => typeof(bool);

    public BoolPrefsItem() { }

    public BoolPrefsItem(string key, bool value)
    {
        base.key = key;
        this.value = value;
    }

    public override object GetValueFromKey(string key, object defaultValue, FileBasedPrefsSaveData data)
    {
        for (int i = 0; i < data.boolData.Length; i++)
        {
            if (data.boolData[i].key.Equals(key)) { return data.boolData[i].value; }
        }

        return defaultValue;
    }
    public override bool HasKey(string key, FileBasedPrefsSaveData data)
    {
        for (int i = 0; i < data.boolData.Length; i++)
        {
            if (data.boolData[i].key.Equals(key)) { return true; }
        }

        return false;
    }
    public override void SetValueForExistingKey(string key, object value, FileBasedPrefsSaveData data)
    {
        if (!(value is bool)) { return; }

        for (int i = 0; i < data.boolData.Length; i++)
        {
            if (data.boolData[i].key.Equals(key)) { data.boolData[i].value = (bool)value; }
        }
    }
    public override void SetValueForNewKey(string key, object value, FileBasedPrefsSaveData data)
    {
        if (!(value is bool)) { return; }

        var tempList = data.boolData.ToList();
        tempList.Add(new BoolPrefsItem(key, (bool)value));
        data.boolData = tempList.ToArray();
    }
    public override void DeleteKey(string key, FileBasedPrefsSaveData data)
    {
        for (int i = data.boolData.Length - 1; i >= 0; i--)
        {
            if (data.boolData[i].key.Equals(key))
            {
                var tempList = data.boolData.ToList();
                tempList.RemoveAt(i);
                data.boolData = tempList.ToArray();
            }
        }
    }
}
