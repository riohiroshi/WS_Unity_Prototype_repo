using System;
using System.Linq;

[Serializable]
public class FloatPrefsItem : PrefsItem
{
    public float value;
    public override Type DataType => typeof(float);

    public FloatPrefsItem() { }

    public FloatPrefsItem(string key, float value)
    {
        base.key = key;
        this.value = value;
    }

    public override object GetValueFromKey(string key, object defaultValue, FileBasedPrefsSaveData data)
    {
        for (int i = 0; i < data.floatData.Length; i++)
        {
            if (data.floatData[i].key.Equals(key)) { return data.floatData[i].value; }
        }

        return defaultValue;
    }
    public override bool HasKey(string key, FileBasedPrefsSaveData data)
    {
        for (int i = 0; i < data.floatData.Length; i++)
        {
            if (data.floatData[i].key.Equals(key)) { return true; }
        }

        return false;
    }
    public override void SetValueForExistingKey(string key, object value, FileBasedPrefsSaveData data)
    {
        if (!(value is float)) { return; }

        for (int i = 0; i < data.floatData.Length; i++)
        {
            if (data.floatData[i].key.Equals(key)) { data.floatData[i].value = (float)value; }
        }
    }
    public override void SetValueForNewKey(string key, object value, FileBasedPrefsSaveData data)
    {
        if (!(value is float)) { return; }

        var tempList = data.floatData.ToList();
        tempList.Add(new FloatPrefsItem(key, (float)value));
        data.floatData = tempList.ToArray();
    }
    public override void DeleteKey(string key, FileBasedPrefsSaveData data)
    {
        for (int i = data.floatData.Length - 1; i >= 0; i--)
        {
            if (data.floatData[i].key.Equals(key))
            {
                var tempList = data.floatData.ToList();
                tempList.RemoveAt(i);
                data.floatData = tempList.ToArray();
            }
        }
    }
}
