using Newtonsoft.Json.Linq;
using UnityEngine;

public class Localization
{
    private readonly JObject _config;
    private static Localization _instance;
    public static Localization Instance => _instance ??= new Localization();

    private Localization()
    {
        _config = JObject.Parse(Resources.Load<TextAsset>("Localization/loc_en").ToString());
    }
    
    public string GetKey(string key)
    {
        if (_config.ContainsKey(key)) return _config[key].Value<string>();
        Debug.LogError("Key not found " + key);
        return key;
    }
}
