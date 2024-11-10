using UnityEngine;

public class RaycastToHighlight : MonoBehaviour
{
    [SerializeField] private LayerMask rayMask;

    [ContextMenu("Ray")]
    public void TryRayCast()
    {
        Debug.DrawRay(transform.position, transform.forward * 10f, Color.red, 10f);
        RaycastHit hitInfo;
        if (Physics.Raycast(transform.position, transform.forward, out hitInfo, 9999f, rayMask))
        {
            CollisionDetector d = hitInfo.collider.GetComponent<CollisionDetector>();
            if (d != null)
            {
                DetectionManager.instance.DisableAllColoring();
                d.Highlight();
            }
        }
    }
}
