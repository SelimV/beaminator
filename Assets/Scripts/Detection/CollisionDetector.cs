using UnityEngine;
using System.Collections.Generic;

public class CollisionDetector : MonoBehaviour
{
    [SerializeField] private BoxCollider collider;
    [SerializeField] private MeshRenderer rend;
    [SerializeField] private List<MeshRenderer> linkedObjects = new List<MeshRenderer>();
    [SerializeField] private IfcProductData productData;
    public IfcProductData IfcProductData => productData;
    private Rigidbody rb;

    public void Setup()
    {
        gameObject.tag = "CorePart";
        collider = gameObject.GetComponent<BoxCollider>();
        rend = gameObject.GetComponent<MeshRenderer>();
        productData = gameObject.GetComponent<IfcProductData>();
        this.enabled = false;
        collider.isTrigger = false;
        collider.enabled = true;
    }

    [ContextMenu("Collide")]
    public void Collide()
    {
        rb = gameObject.AddComponent<Rigidbody>();
        rb.constraints = RigidbodyConstraints.FreezeAll;
        rb.useGravity = false;
        rb.isKinematic = false;
        this.enabled = true;
        collider.isTrigger = true;
        collider.isTrigger = false;
    }

    private void OnCollisionEnter(Collision collision)
    {
        MeshRenderer m = collision.gameObject.GetComponent<MeshRenderer>();
        if (m != null)
        {
            linkedObjects.Remove(m);
            linkedObjects.Add(m);
            SetNonCoreColor(m, DetectionManager.instance.RedMat);
        }
    }
    private void LateUpdate()
    {
        Destroy(rb);
        this.enabled = false;
    }

    public void DisableAllColoring()
    {
        rend.material = DetectionManager.instance.GreyMat;
        foreach (MeshRenderer m in linkedObjects)
        { m.material = DetectionManager.instance.GreyMat; }
    }
    public void EnableColoring()
    {
        rend.material = DetectionManager.instance.YellowMat;
    }
    public void EnableAllColoring()
    {
        rend.material = DetectionManager.instance.YellowMat;
        foreach (MeshRenderer m in linkedObjects)
        { SetNonCoreColor(m, DetectionManager.instance.RedMat); }
    }
    public void EnableColoringByType(string ifcType)
    {
        rend.material = DetectionManager.instance.YellowMat;
        if (ifcType == "") { return; }
        foreach (MeshRenderer m in linkedObjects)
        {
            IfcProductData d = m.gameObject.GetComponent<IfcProductData>();
            if (d.IfcClass == ifcType || ifcType == "all")
            { SetNonCoreColor(m, DetectionManager.instance.RedMat); }
        }
    }

    [ContextMenu("Highlight this object")]
    public void Highlight()
    {
        DetectionManager.instance.DisableColoring();
        EnableAllColoring();
    }

    private void SetNonCoreColor(MeshRenderer r, Material m)
    {
        if (r.gameObject.tag == "Ifcpart")
        {
            r.material = m;
        }
    }
}
