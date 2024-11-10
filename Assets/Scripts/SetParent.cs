using UnityEngine;

public class SetParent : MonoBehaviour
{
    public Transform parent;

    public void SetParentAsParent()
    {
        transform.parent = parent;
    }
}
