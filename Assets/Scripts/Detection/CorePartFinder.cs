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
            // Add collider
            MeshCollider col = c.gameObject.AddComponent<MeshCollider>();
            col.convex = true;
            col.isTrigger = false;
            col.enabled = true;

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
        IfcProductData productData = potentialPart.GetComponent<IfcProductData>();
        if (productData == null) { return false; }

        if (productData.IfcClass == "IfcColumn") { potentialPart.material = DetectionManager.instance.YellowMat; }
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
