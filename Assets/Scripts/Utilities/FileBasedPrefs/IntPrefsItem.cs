using System;
using System.Linq;

[Serializable]
public class IntPrefsItem : PrefsItem
{
    public int value;
    public override Type DataType => typeof(int);

    public IntPrefsItem() { }

    public IntPrefsItem(string key, int value)
    {
        base.key = key;
        this.value = value;
    }

    public override object GetValueFromKey(string key, object defaultValue, FileBasedPrefsSaveData data)
    {
        for (int i = 0; i < data.intData.Length; i++)
        {
            if (data.intData[i].key.Equals(key)) { return data.intData[i].value; }
        }

        return defaultValue;
    }
    public override bool HasKey(string key, FileBasedPrefsSaveData data)
    {
        for (int i = 0; i < data.intData.Length; i++)
        {
            if (data.intData[i].key.Equals(key)) { return true; }
        }

        return false;
    }
    public override void SetValueForExistingKey(string key, object value, FileBasedPrefsSaveData data)
    {
        if (!(value is int)) { return; }

        for (int i = 0; i < data.intData.Length; i++)
        {
            if (data.intData[i].key.Equals(key)) { data.intData[i].value = (int)value; }
        }
    }
    public override void SetValueForNewKey(string key, object value, FileBasedPrefsSaveData data)
    {
        if (!(value is int)) { return; }

        var tempList = data.intData.ToList();
        tempList.Add(new IntPrefsItem(key, (int)value));
        data.intData = tempList.ToArray();
    }
    public override void DeleteKey(string key, FileBasedPrefsSaveData data)
    {
        for (int i = data.intData.Length - 1; i >= 0; i--)
        {
            if (data.intData[i].key.Equals(key))
            {
                var tempList = data.intData.ToList();
                tempList.RemoveAt(i);
                data.intData = tempList.ToArray();
            }
        }
    }
}
