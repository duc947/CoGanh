using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class StartManager : MonoBehaviour {

	// Use this for initialization
	
	[Header("Tuy chinh khi Game bat dau")]
	[SerializeField] private GameObject m_StartBanner;
	[SerializeField] private Dropdown m_StartModeDropdown;
	[SerializeField] private Dropdown m_StartThinkTime;
	[SerializeField] private Button m_StartBtn;
	[SerializeField] private Mode m_CurrentMode = Mode.NONE;
	[SerializeField] private float m_Time = 90f;
	void Start () {
		// Debug.LogError("Start mode cua StartMangager lay tu Main: " + Main.Instance.CurrentMode);
		m_CurrentMode = Main.Instance.CurrentMode;
		m_Time = Main.Instance.Time2EachPlayer;
		switch (Main.Instance.CurrentMode)
		{
			case Mode.AI_AI:
				m_StartModeDropdown.value = 2;
				break;
			case Mode.PLAYER_AI:
				
				m_StartModeDropdown.value = 0;
				break;
			case Mode.PLAYER_PLAYER:
				m_StartModeDropdown.value = 1;

				break;
			default:
				m_StartModeDropdown.value = 0;
				break;
		}
		switch (Main.Instance.Time2EachPlayer/60)
		{
			case 5:
				m_StartThinkTime.value = 0;
				break;
			case 10:
				m_StartThinkTime.value = 1;
				break;
			case 15:
				m_StartThinkTime.value = 2;
				break;
			case 30:
				m_StartThinkTime.value = 3;
				break;
			default:
				m_StartThinkTime.value = 0;
				break;
		}
		m_StartModeDropdown.onValueChanged.AddListener(delegate {
			Mode tempMode  = Mode.NONE;
			Debug.Log("Gia tri mode thay doi la: " + m_CurrentMode + " ");
			switch (m_StartModeDropdown.value)
			{
				case 0:
					m_CurrentMode = Mode.PLAYER_AI;
					break;
				case 1: 
					m_CurrentMode = Mode.PLAYER_PLAYER;
					break;
				case 2: 
					m_CurrentMode = Mode.AI_AI;
					break;
				default:
				break;
			}
		});
		m_StartThinkTime.onValueChanged.AddListener(delegate {
			Debug.Log("Gia tri thoi gian thay doi la: " + m_Time + " ");
			switch (m_StartThinkTime.value)
			{
				case 0:
					m_Time = 5*60f;
				break;
				case 1: 
					m_Time = 10*60f;
					break;
				case 2: 
					m_Time = 15*60f;
					break;
				case 3: 
					m_Time = 30*60f;
					break;
				default:
					break;
			}
		});
		m_StartBtn.onClick.AddListener(delegate {
			this.gameObject.SetActive(false);
			Main.Instance.Env.GamePause = false;
			Main.Instance.gameRestart(m_CurrentMode, m_Time);
		});
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}
