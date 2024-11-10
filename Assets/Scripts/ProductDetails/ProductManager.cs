using UnityEngine;
using System.Collections.Generic;

public class ProductManager : MonoBehaviour
{
    public static ProductManager instance;

    [SerializeField] private TmpInstanced tmpInstancePrefab;
    public TmpInstanced TmpInstancePrefab => tmpInstancePrefab;
    [SerializeField] private ProductSuggestion suggestions;
    public ProductSuggestion Suggestions => suggestions;

    private void Awake()
    {
        if (instance == null) { instance = this; }
        else if (instance != this) { Destroy(gameObject); }

        suggestions = ProductSuggestion.CreateFromJSON();
    }

    public List<PeikkoProduct> GetSuggestedProducts(string ifcTypeA, string ifcTypeB)
    {
        return Suggestions.GetProducts(ifcTypeA, ifcTypeB);
    }

}