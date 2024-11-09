using UnityEngine;

public class XRay : MonoBehaviour
{
    public GameObject targetObject; // The object you want to toggle rendering modes on
    private bool isWireframe = false; // Track the current rendering mode
    Material xrayMat;
    Material defaultMat;

    void Start()
    {
        xrayMat = Resources.Load("Materials/XXRayMaterial", typeof(Material)) as Material;
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

    public void SetWireframeMode(bool enableWireframe)
    {
        // Get all renderers in the target object
        Renderer[] renderers = targetObject.GetComponentsInChildren<Renderer>();

        foreach (Renderer renderer in renderers)
        {
            if (enableWireframe)
            {
                renderer.material = xrayMat;
            }
            else
            {
                renderer.material = defaultMat;
            }
        }
    }
}
