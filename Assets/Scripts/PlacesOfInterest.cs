using System.Collections.Generic;
using UnityEngine;

public class TestTestTest : MonoBehaviour
{
    public GameObject targetObject; // The object you want to toggle rendering modes on
    private bool isWireframe = false; // Track the current rendering mode
    public Transform root;
    Material poiMat;
    Material defaultMat;
    List<GameObject> toBeDeleted = new List<GameObject>();

    void Start()
    {
        poiMat = Resources.Load("Materials/POIMaterial", typeof(Material)) as Material;
        defaultMat = Resources.Load("Materials/DefaultMaterial", typeof(Material)) as Material;
    }

    // This method will be called when the button is clicked
    public void ToggleRenderingMode()
    {
        if (targetObject == null)
        {
            Debug.LogError("Target object is not assigned.");
            return;
        }

        // Toggle the wireframe mode
        isWireframe = !isWireframe;

        if (isWireframe)
        {
            // Enable wireframe mode
            SetWireframeMode(true);
        }
        else
        {
            // Disable wireframe mode
            SetWireframeMode(false);
        }
    }

    public void SetWireframeMode22222(bool enableWireframe)
    {
        if (!enableWireframe)
        {
            foreach (var go in toBeDeleted)
                Destroy(go);
        }
        else
        {
            // Get all child colliders
            //Collider[] childColliders = GetComponentsInChildren<Collider>();
            Collider[] childColliders = targetObject.GetComponentsInChildren<Collider>();

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
                        toBeDeleted.Add(largerObject);

                        // Calculate the new size
                        Vector3 newSize = originalBounds.size * 1f;

                        // Set the position of the new GameObject to match the original Bounds center
                        largerObject.transform.position = originalBounds.center;
                        largerObject.transform.localEulerAngles = Vector3.zero;

                        // Adjust the scale of the new GameObject
                        largerObject.transform.localScale = new Vector3(largerObject.transform.parent.localScale.x *newSize.x, largerObject.transform.parent.localScale.y * newSize.y, largerObject.transform.parent.localScale.z * newSize.z);

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
    }

    Bounds GetIntersection(Bounds b1, Bounds b2)
    {
        // Calculate the min and max points of the intersection
        Vector3 min = Vector3.Max(b1.min, b2.min);
        Vector3 max = Vector3.Min(b1.max, b2.max);

        // Create a new Bounds object for the intersection
        Bounds intersection = new Bounds();
        intersection.SetMinMax(min, max);

        return intersection;
    }

    public void SetWireframeMode(bool enableWireframe)
    {
        //targetObject.getchildr.GetComponentsInChildren<Collider>();
        foreach (var go in CorePartFinder.placesOfInterest)
            go.SetActive(enableWireframe);
    }

}
