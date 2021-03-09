using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

[Serializable]
public class FileBasedPrefsSaveData
{
    public StringPrefsItem[] stringData = new StringPrefsItem[0];
    public IntPrefsItem[] intData = new IntPrefsItem[0];
    public FloatPrefsItem[] floatData = new FloatPrefsItem[0];
    public BoolPrefsItem[] boolData = new BoolPrefsItem[0];

    private static Dictionary<Type, PrefsItem> _dataTypeDict = new Dictionary<Type, PrefsItem>();
    private static bool _initialized;


    public FileBasedPrefsSaveData()
    {
        Initialize();
    }

    public object GetValueFromKey(string key, object defaultValue)
    {
        if (_initialized == false) { Initialize(); }

        var dataValue = _dataTypeDict[defaultValue.GetType()];

        return dataValue.GetValueFromKey(key, defaultValue, this);
    }

    public void UpdateOrAddData(string key, object value)
    {
        if (_initialized == false) { Initialize(); }

        var dataValue = _dataTypeDict[value.GetType()];

        if (dataValue.HasKey(key, this)) { dataValue.SetValueForExistingKey(key, value, this); }
        else { dataValue.SetValueForNewKey(key, value, this); }
    }

    public bool HasKeyFromObject(string key, object value)
    {
        if (_initialized == false) { Initialize(); }

        var dataValue = _dataTypeDict[value.GetType()];

        return dataValue.HasKey(key, this);
    }
    public bool HasKey(string key)
    {
        if (_initialized == false) { Initialize(); }

        foreach (var dataValue in _dataTypeDict)
        {
            if (dataValue.Value.HasKey(key, this)) { return true; }
        }

        return false;
    }

    public void DeleteKey(string key)
    {
        if (_initialized == false) { Initialize(); }

        foreach (var dataValue in _dataTypeDict)
        {
            dataValue.Value.DeleteKey(key, this);
        }
    }
    public void DeleteString(string key)
    {
        if (_initialized == false) { Initialize(); }

        var stringData = _dataTypeDict[typeof(string)];
        stringData.DeleteKey(key, this);
    }
    public void DeleteInt(string key)
    {
        if (_initialized == false) { Initialize(); }

        var intData = _dataTypeDict[typeof(int)];
        intData.DeleteKey(key, this);
    }
    public void DeleteFloat(string key)
    {
        if (_initialized == false) { Initialize(); }

        var floatData = _dataTypeDict[typeof(float)];
        floatData.DeleteKey(key, this);
    }
    public void DeleteBool(string key)
    {
        if (_initialized == false) { Initialize(); }

        var boolData = _dataTypeDict[typeof(bool)];
        boolData.DeleteKey(key, this);
    }

    private static void Initialize()
    {
        _dataTypeDict.Clear();

        var assembly = Assembly.GetAssembly(typeof(PrefsItem));
        var allDataTypes = assembly.GetTypes().Where(t => typeof(PrefsItem).IsAssignableFrom(t) && t.IsAbstract == false);

        foreach (var dataType in allDataTypes)
        {
            PrefsItem data = Activator.CreateInstance(dataType) as PrefsItem;
            _dataTypeDict.Add(data.DataType, data);
        }

        _initialized = true;
    }
}
