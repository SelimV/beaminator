using UnityEngine;
using System.Collections.Generic;

public static class CorePartFinder
{

    [ContextMenu("Find Core Parts")]
    public static List<CollisionDetector> FindCoreParts()
    {
        MeshRenderer[] potentialParts = GetPotentialParts();
        List<CollisionDetector> coreParts = new List<CollisionDetector>();
        foreach (MeshRenderer c in potentialParts)
        {
            IfcProductData p = c.GetComponent<IfcProductData>();
            if (p == null) { continue; }

            // Add collider
            BoxCollider col = c.gameObject.AddComponent<BoxCollider>();
            col.size = col.size + (Vector3.one * (DetectionManager.instance.ExtraBoxMargin * 2f));
            col.isTrigger = false;
            col.enabled = true;

            c.gameObject.tag = "IfcPart";

            //Validate part
            if (ValidatePotentialPart(c))
            {
                coreParts.Add(SetupCorePart(c.gameObject));
            }
            else
            {
                // Make grey
                c.material = DetectionManager.instance.GreyMat;
            }
        }
        return coreParts;
    }

    private static MeshRenderer[] GetPotentialParts()
    {
        return (MeshRenderer[])GameObject.FindObjectsByType(typeof(MeshRenderer), FindObjectsSortMode.None);
    }

    private static bool ValidatePotentialPart(MeshRenderer potentialPart)
    {
        string ifcClass = "";
        IfcPropertySetData[] properties = potentialPart.gameObject.GetComponents<IfcPropertySetData>();
        foreach (IfcPropertySetData prop in properties)
        { if (prop.PropertySetName == "beaminator_classification") { ifcClass = prop.Properties[0].PropertyValue; } }

        if (ifcClass == "") { return false; }

        if (ifcClass == "column" || ifcClass == "beam" || ifcClass == "wall") { potentialPart.material = DetectionManager.instance.YellowMat; }
        else { return false; }
        return true;
    }

    private static CollisionDetector SetupCorePart(GameObject corePart)
    {
        CollisionDetector detector = corePart.AddComponent<CollisionDetector>();
        detector.Setup();
        return detector;
    }
}
