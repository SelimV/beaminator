using UnityEngine;

public class ToggleColumns : MonoBehaviour
{
    public string IfcType;
    public string IfcLinkType;

    public void Toggle(bool state)
    {
        if (state)
        { DetectionManager.instance.HighlightCoreParts(IfcType, IfcLinkType); }
        else { DetectionManager.instance.DisableCoreParts(IfcType, IfcLinkType); }
    }
}
