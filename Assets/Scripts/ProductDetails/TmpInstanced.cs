using UnityEngine;
using TMPro;

public class TmpInstanced : MonoBehaviour
{
    [SerializeField] private TMP_Text m_Text;
    public void SetText(string txt) { m_Text.text = txt; }
}
