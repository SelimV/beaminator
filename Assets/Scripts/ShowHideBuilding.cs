using System;
using UnityEngine;

public class ShowHideBuilding : MonoBehaviour
{
    // Reference to the GameObject you want to show/hide
    public GameObject targetObject;

    // Method to toggle the visibility of the target object
    public void ToggleObjectVisibility()
    {
        if (targetObject != null)
        {
            // Toggle the active state of the target object
            targetObject.SetActive(true);
            AssignColliders();
        }
        else
        {
            Debug.LogWarning("Target object is not assigned.");
        }
    }

    private void AssignColliders()
    {
        //GameObject[] objects = targetObject.GetComponentsInChildren<GameObject>();
        //foreach (var o in objects)
        //{
        //    var co = o.AddComponent<MeshCollider>();
        //    co.convex = true;
        //    //co.transform.localScale = new Vector3(1.1f, 1.1f, 1.1f);


        //    // Calculate the new size
        //    Vector3 newSize = co.bounds.size * 1.01f;

        //    // Set the position of the new GameObject to match the original Bounds center
        //    co.transform.position = co.bounds.center;

        //    // Adjust the scale of the new GameObject
        //    co.transform.localScale = newSize;

        //}
    }
}
