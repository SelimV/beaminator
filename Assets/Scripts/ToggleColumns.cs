using UnityEngine;

public class ToggleColumns : MonoBehaviour
{
    public string IfcType;
    public string IfcLinkType;

    public void Toggle()
    {
        DetectionManager.instance.HighlightCoreParts(IfcType, IfcLinkType);
    }
}
