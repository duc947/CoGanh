using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class AddScore : MonoBehaviour {

	[SerializeField] private Text m_Text;
	public float m_TimeFade = 1.5f;
	/// <summary>
	/// Start is called on the frame when a script is enabled just before
	/// any of the Update methods is called the first time.
	/// </summary>
	// void Start()
	// {
	// }
	public void FadeOut(int _score) {
		// Debug.Log("Vao Day");
		m_Text.text = _score.ToString();
		iTween.MoveAdd(gameObject, new Vector3(0, 30, 0), m_TimeFade);
        iTween.ValueTo(gameObject, iTween.Hash(
             "from", 1.0f, "to", 0.0f,
             "time", m_TimeFade, "easetype", "linear",
             "onupdate", "setAlpha"));
		Destroy(this, m_TimeFade);
     }
     public void FadeIn() {
         iTween.ValueTo(gameObject, iTween.Hash(
             "from", 0f, "to", 1f,
             "time", m_TimeFade, "easetype", "linear",
             "onupdate", "setAlpha"));
     }
	 public void setAlpha(float newAlpha) {
         var a = m_Text.color;
		 m_Text.color = new Color(a.r, a.g, a.b, newAlpha);
     }
}
