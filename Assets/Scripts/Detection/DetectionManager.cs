using UnityEngine;
using System.Collections.Generic;

public class DetectionManager : MonoBehaviour
{
    public static DetectionManager instance;

    [SerializeField] private CollisionScanner collisionScanner;
    [SerializeField] private float extraBoxMargin = 0.01f;
    public float ExtraBoxMargin => extraBoxMargin;

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
    [SerializeField] private string testTypeCore = "";
    [SerializeField] private string testTypeChild = "";

    private List<CollisionDetector> coreParts = new List<CollisionDetector>();


    private void Awake()
    {
        if (instance == null) { instance = this; }
        else if (instance != this) { Destroy(gameObject); }
    }

    private void Start()
    {
        //FindCoreParts();
    }

    [ContextMenu("FindCoreParts")]
    public void FindCoreParts()
    {
        coreParts = CorePartFinder.FindCoreParts();
        collisionScanner.ScanCoreParts(coreParts);
    }

    [ContextMenu("Disable All")]
    public void DisableAllColoring()
    {  DisableCoreParts("all", "all"); }

    [ContextMenu("Enable All")]
    public void EnableAllColoring()
    { HighlightCoreParts("all", "all"); }

    [ContextMenu("Highlight by type")]
    public void TestHighlightCoreParts() { HighlightCoreParts(testTypeCore, testTypeChild); }
    public void HighlightCoreParts(string ifcType, string linkedIfcType = "")
    {
        if (ifcType == "") { return; }
        foreach (CollisionDetector d in coreParts)
        { if (d.IfcClass == ifcType || ifcType == "all") { d.EnableColoringByType(linkedIfcType); } }
    }
    [ContextMenu("Disable by type")]
    public void TestDisableCoreParts() { DisableCoreParts(testTypeCore, testTypeChild); }
    public void DisableCoreParts(string ifcType, string linkedIfcType = "")
    {
        if (ifcType == "") { return; }
        foreach (CollisionDetector d in coreParts)
        { if (d.IfcClass == ifcType || ifcType == "all") { d.DisableAllColoring(); } }
    }
}
