using UnityEngine;

public class DeleteColliders : MonoBehaviour
{
    [ContextMenu("Delete Colliders")]
    public void DeleteAllColliders()
    {
        DeleteCollidersFromChildrens(transform);
        DestroyImmediate(this);
    }

    private void DeleteCollidersFromChildrens(Transform t)
    {
        Collider col = t.GetComponent<Collider>();
        if (col != null) { DestroyImmediate(col); }

        for (int i = 0; i < t.childCount; i++)
        {
            DeleteCollidersFromChildrens(t.GetChild(i));
        }
    }
}
