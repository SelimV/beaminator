using UnityEngine;
using System.Collections.Generic;

public class DetectionManager : MonoBehaviour
{
    public static DetectionManager instance;

    [SerializeField] private CollisionScanner collisionScanner;

    [Space(30)]
    [SerializeField] private Material greyMat;
    public Material GreyMat => greyMat;
    [SerializeField] private Material greenMat;
    public Material GreenMat => greenMat;
    [SerializeField] private Material yellowMat;
    public Material YellowMat => yellowMat;
    [SerializeField] private Material redMat;
    public Material RedMat => redMat;

    [Space(30)]
    [SerializeField] private string testTypeCore = "-";
    [SerializeField] private string testTypeChild = "-";
    [SerializeField] private ProductSuggestion testSuggestion;

    private List<CollisionDetector> coreParts = new List<CollisionDetector>();


    private void Awake()
    {
        if (instance == null) { instance = this; }
        else if (instance != this) { Destroy(gameObject); }
    }

    [ContextMenu("FindCoreParts")]
    public void FindCoreParts()
    {
        coreParts = CorePartFinder.FindCoreParts();
        collisionScanner.ScanCoreParts(coreParts);
        testSuggestion = ProductSuggestion.CreateFromJSON();
    }

    public void DisableColoring()
    {
        foreach(CollisionDetector d in coreParts)
        { d.DisableAllColoring(); }
    }

    [ContextMenu("Highlight by type")]
    public void TestHighlightCoreParts() { HighlightCoreParts(testTypeCore, testTypeChild); }
    public void HighlightCoreParts(string ifcType, string linkedIfcType = "")
    {
        DisableColoring();
        if(ifcType == "") { return; }
        foreach (CollisionDetector d in coreParts)
        { if (d.IfcProductData.IfcClass == ifcType) { d.EnableColoringByType(linkedIfcType); } }
    }
}
