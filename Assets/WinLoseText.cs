using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class WinLoseText : MonoBehaviour
{
    [SerializeField]private Text m_MainText;
    public void setMainText (string text){m_MainText.text = text;}
}
