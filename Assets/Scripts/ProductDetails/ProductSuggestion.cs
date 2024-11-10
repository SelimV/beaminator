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

    public List<PeikkoProduct> GetProducts(string ifcTypeA, string ifcTypeB)
    {
        if (ifcTypeA == "column")
        {
            switch (ifcTypeB)
            {
                case "column": return columntocolumn;
                case "beam": return columntobeam;
                case "foundation": return columntofoundation;
                default: return new List<PeikkoProduct>();
            }
        }
        else if (ifcTypeA == "beam")
        {
            switch (ifcTypeB)
            {
                case "beam": return beamtobeam;
                default: return new List<PeikkoProduct>();
            }
        }
        else if (ifcTypeA == "wall")
        {
            switch (ifcTypeB)
            {
                case "column": return walltocolumn;
                case "wall": return walltowall;
                case "foundation": return walltofoundation;
                default: return new List<PeikkoProduct>();
            }
        }
        return new List<PeikkoProduct>();
    }
}

[System.Serializable]
public class PeikkoProduct
{
    public string name;
    public string url;
}