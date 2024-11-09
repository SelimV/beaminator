using UnityEngine;
using System.Collections.Generic;

[System.Serializable]
public class ProductSuggestion
{
    public List<PeikkoProduct> columntofoundation;
    public List<PeikkoProduct> columntocolumn;
    public List<PeikkoProduct> columntobeam;
    public List<PeikkoProduct> beamtobeam;
    public List<PeikkoProduct> walltofoundation;
    public List<PeikkoProduct> walltowall;
    public List<PeikkoProduct> walltocolumn;

    public static ProductSuggestion CreateFromJSON(string jsonString = "Products_joints")
    {
        return JsonUtility.FromJson<ProductSuggestion>(Resources.Load<TextAsset>(jsonString).ToString());
    }
}

[System.Serializable]
public class PeikkoProduct 
{
    public string name;
    public string url;
}