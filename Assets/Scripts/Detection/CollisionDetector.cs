using UnityEngine;
using System.Collections.Generic;

public class CollisionDetector : MonoBehaviour
{
    [SerializeField] private BoxCollider collider;
    [SerializeField] private MeshRenderer rend;
    [SerializeField] private List<IfcLink> linkedObjects = new List<IfcLink>();
    public List<IfcLink> LinkedObjects => linkedObjects;
    [SerializeField] private IfcProductData productData;
    public IfcProductData IfcProductData => productData;
    public string IfcClass { get; private set; } = "none";
    private Rigidbody rb;

    public void Setup()
    {
        gameObject.tag = "CorePart";
        collider = gameObject.GetComponent<BoxCollider>();
        rend = gameObject.GetComponent<MeshRenderer>();
        productData = gameObject.GetComponent<IfcProductData>();

        IfcPropertySetData[] properties = gameObject.GetComponents<IfcPropertySetData>();
        foreach (IfcPropertySetData prop in properties)
        { if (prop.PropertySetName == "beaminator_classification") { IfcClass = prop.Properties[0].PropertyValue; } }

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
            string collisionIfcClass = "";
            IfcPropertySetData[] properties = collision.gameObject.GetComponents<IfcPropertySetData>();
            foreach (IfcPropertySetData prop in properties)
            { if (prop.PropertySetName == "beaminator_classification") { collisionIfcClass = prop.Properties[0].PropertyValue; } }

            if (collisionIfcClass == "" || collisionIfcClass == "other")
            {
                IfcProductData prod = collision.gameObject.GetComponent<IfcProductData>();
                if (prod != null) { collisionIfcClass = prod.IfcClass.ToLower().Substring(3); }
                if (collisionIfcClass != "footing" && collisionIfcClass != "wall" && collisionIfcClass != "beam" && collisionIfcClass != "slab") { collisionIfcClass = ""; }
                else if(collisionIfcClass == "footing") { collisionIfcClass = "foundation"; }

            }


            if (collisionIfcClass != "")
            {
                IfcLink oldLink = FindMatchingIfcLink(m);
                if (oldLink != null) { linkedObjects.Remove(oldLink); Destroy(oldLink.TmpInstance); }
                IfcLink newLink = new IfcLink(collisionIfcClass, m, AverageContactPoint(collision.contacts));
                linkedObjects.Add(newLink);

                SetNonCoreColor(m, DetectionManager.instance.RedMat);
            }
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
        foreach (IfcLink l in linkedObjects)
        {
            l.Rend.material = DetectionManager.instance.GreyMat;
            if (l.TmpInstance != null) { Destroy(l.TmpInstance.gameObject); }
        }
    }
    public void EnableColoring()
    {
        rend.material = DetectionManager.instance.YellowMat;
    }
    public void EnableAllColoring()
    {
        rend.material = DetectionManager.instance.YellowMat;
        foreach (IfcLink l in linkedObjects)
        {
            l.Rend.material = DetectionManager.instance.RedMat;
            l.CreateTmpInstance();
            l.SetProductsAsText(ProductManager.instance.GetSuggestedProducts(IfcClass, l.IfcClass));
        }
    }
    public void EnableColoringByType(string ifcType)
    {
        rend.material = DetectionManager.instance.YellowMat;
        if (ifcType == "") { return; }
        foreach (IfcLink l in linkedObjects)
        {
            if (l.IfcClass == ifcType || ifcType == "all")
            { SetNonCoreColor(l.Rend, DetectionManager.instance.RedMat); }
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

    public IfcLink FindMatchingIfcLink(MeshRenderer comparedMesh)
    {
        foreach (IfcLink i in linkedObjects)
        {
            if (i.Rend == comparedMesh) { return i; }
        }
        return null;
    }

    public Vector3 AverageContactPoint(ContactPoint[] contactPoints)
    {
        if (contactPoints == null || contactPoints.Length == 0) { return Vector3.zero; }
        Vector3 totalContactValue = Vector3.zero;
        foreach (ContactPoint c in contactPoints)
        { totalContactValue += c.point; }
        totalContactValue /= contactPoints.Length;
        return totalContactValue;
    }
}

[System.Serializable]
public class IfcLink
{
    public IfcLink(string _ifcClass, MeshRenderer linkRendrer, Vector3 _collisionPoint)
    {
        ifcClass = _ifcClass;
        rend = linkRendrer;
        collisionPoint = _collisionPoint;
    }

    [SerializeField] private string ifcClass;
    public string IfcClass => ifcClass;
    [SerializeField] private MeshRenderer rend;
    public MeshRenderer Rend => rend;

    [SerializeField] private Vector3 collisionPoint;
    public Vector3 CollisionPoint => collisionPoint;

    public TmpInstanced TmpInstance;

    public void CreateTmpInstance()
    {
        if (TmpInstance != null) { GameObject.Destroy(TmpInstance.gameObject); }
        TmpInstance = GameObject.Instantiate(ProductManager.instance.TmpInstancePrefab, CollisionPoint, Quaternion.identity);
    }
    public void SetProductsAsText(List<PeikkoProduct> products)
    {
        string txt = "";
        for (int i = 0; i < products.Count; i++)
        {
            txt += products[i].name;
            if (i != products.Count - 1) { txt += ", "; }
        }
        TmpInstance.SetText(txt);
        TmpInstance.gameObject.name = Rend.gameObject.name;
    }
}