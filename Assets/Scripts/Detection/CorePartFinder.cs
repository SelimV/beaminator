using UnityEngine;
using System.Collections.Generic;
using System;
using System.Linq;

public static class CorePartFinder
{
    public static Transform root;

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

        FindIntersections(coreParts);


        return coreParts;
    }

    public static List<GameObject> placesOfInterest = new List<GameObject>();


    private static void FindIntersections(List<CollisionDetector> coreParts)
    {
        var poiMat = Resources.Load("Materials/POIMaterial", typeof(Material)) as Material;


        // Get all child colliders
        //Collider[] childColliders = GetComponentsInChildren<Collider>();
        //Collider[] childColliders = targetObject.GetComponentsInChildren<Collider>();
        Collider[] childColliders = coreParts.Select(x => x.collider).ToArray();

        // Check each pair of child colliders for collision
        for (int i = 0; i < childColliders.Length; i++)
        {
            var bounds1 = childColliders[i].bounds;

            if (childColliders[i].gameObject.tag == "IfcPart")
                continue;

            for (int j = i + 1; j < childColliders.Length; j++)
            {
                if (childColliders[j].gameObject.tag == "IfcPart")
                    continue;

                var bounds2 = childColliders[j].bounds;
                if (bounds1.Intersects(bounds2))
                {
                    //Debug.Log($"Collision detected between {childColliders[i].name} and {childColliders[j].name}");
                    // Create a small sphere at the contact point to visualize the intersection

                    Bounds originalBounds = GetIntersection(bounds1, bounds2);

                    // Create a new GameObject
                    GameObject largerObject = GameObject.CreatePrimitive(PrimitiveType.Cube);
                    largerObject.transform.parent = root;
                    largerObject.name = $"{childColliders[i].gameObject.name}+{childColliders[j].gameObject.name}";
                    placesOfInterest.Add(largerObject);

                    // Calculate the new size
                    Vector3 newSize = originalBounds.size * 1f;

                    // Set the position of the new GameObject to match the original Bounds center
                    largerObject.transform.position = originalBounds.center;
                    largerObject.transform.localEulerAngles = new Vector3(-90,0,0);

                    // Adjust the scale of the new GameObject
                    largerObject.transform.localScale = new Vector3(largerObject.transform.parent.localScale.x * newSize.x, largerObject.transform.parent.localScale.y * newSize.y, largerObject.transform.parent.localScale.z * newSize.z);

                    // Optionally, adjust the material or other properties
                    Renderer renderer = largerObject.GetComponent<Renderer>();
                    if (renderer != null)
                    {
                        //renderer.material.color = Color.red; // Just an example to change color
                        renderer.material = poiMat;
                    }


                    //GameObject highlight = GameObject.CreatePrimitive(PrimitiveType.Cube);
                    //highlight.transform.position = contact.point;
                    //highlight.transform.localScale = Vector3.one * 0.1f; // Adjust size as needed
                    //highlight.GetComponent<Renderer>().material = highlightMaterial;

                    //// Optionally, destroy the highlight after a short duration
                    //Destroy(highlight, 1.0f);
                }
            }
        }
    }

    static Bounds GetIntersection(Bounds b1, Bounds b2)
    {
        // Calculate the min and max points of the intersection
        Vector3 min = Vector3.Max(b1.min, b2.min);
        Vector3 max = Vector3.Min(b1.max, b2.max);

        // Create a new Bounds object for the intersection
        Bounds intersection = new Bounds();
        intersection.SetMinMax(min, max);

        return intersection;
    }

    private static MeshRenderer[] GetPotentialParts()
    {
        return CorePartFinder.root.GetComponentsInChildren<MeshRenderer>();
        //return (MeshRenderer[])GameObject.FindObjectsByType(typeof(MeshRenderer), FindObjectsSortMode.None);
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
