using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SettingManager : MonoBehaviour {

	[SerializeField] private Button[] m_MusicBtn;
	[SerializeField] private Button[] m_SoundEffectBtn;
	[SerializeField] private Dropdown m_Mode;
	[SerializeField] private Dropdown m_ThinkTime;
	[SerializeField] private GameObject m_SettingBoard;
	[SerializeField] private Mode m_CurrentMode = Mode.NONE;
	[SerializeField] private Button m_BeginBtn;
	[SerializeField] private Button m_ResumeGame;

	[SerializeField] private float m_Time = 90f;
	[SerializeField] private Sprite m_OnSprite, m_OffSprite;
	private bool m_IsSoundOn = false;
	private bool m_IsMusicOn = false;
	
	/// <summary>
	/// Start is called on the frame when a script is enabled just before
	/// any of the Update methods is called the first time.
	/// </summary>
	/// <summary>
	/// Awake is called when the script instance is being loaded.
	/// </summary>
	void Awake()
	{
		
		// m_SettingBoard.SetActive(false);
		m_Mode.onValueChanged.AddListener(delegate {
			Mode tempMode  = Mode.NONE;
			Debug.Log("Gia tri thay doi la: " + m_Mode.value + " " + m_Mode.name);
			switch (m_Mode.value)
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
		m_ThinkTime.onValueChanged.AddListener(delegate {
			switch (m_ThinkTime.value)
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
		m_BeginBtn.onClick.AddListener(delegate {
			BeginGame();
		});
		m_ResumeGame.onClick.AddListener(delegate{
			Main.Instance.Env.GamePause = false;
			m_SettingBoard.SetActive(false);
		});
		Debug.LogError("Setting Start");
		turnMusic(Main.Instance.IsMusicOn);
		turnSoundEffect(Main.Instance.IsSoundEffectOn);
	}
	public void turnMusic(bool e) {
		Main.Instance.musicOn(e);
		if(e) {
			m_MusicBtn[0].image.sprite = m_OnSprite;
			m_MusicBtn[1].image.sprite = m_OffSprite;
		}
		else {
			m_MusicBtn[0].image.sprite = m_OffSprite;
			m_MusicBtn[1].image.sprite = m_OnSprite;
		}
	}
	public void turnSoundEffect(bool e) {
		Main.Instance.soundEffectOn(e);
		if(e) {
			m_SoundEffectBtn[0].image.sprite = m_OnSprite;
			m_SoundEffectBtn[1].image.sprite = m_OffSprite;
		}
		else {
			m_SoundEffectBtn[0].image.sprite = m_OffSprite;
			m_SoundEffectBtn[1].image.sprite = m_OnSprite;
		}
	}
	public void BeginGame() {
		m_SettingBoard.SetActive(false);
		Main.Instance.Env.GamePause = false;
		Main.Instance.gameRestart(m_CurrentMode, m_Time);
	}
	public void ResumeGame() {
		Debug.LogError("no vao toi day!" + Main.Instance.CurrentMode + " "+ (Main.Instance.Time2EachPlayer/60).ToString());
		switch (Main.Instance.CurrentMode)
		{
			case Mode.AI_AI:
				m_Mode.value = 2;
				break;
			case Mode.PLAYER_AI:
				m_Mode.value = 0;
				break;
			case Mode.PLAYER_PLAYER:
			m_Mode.value = 1;
				break;
			default:
				m_Mode.value = 0;
				break;
		}
		switch (Main.Instance.Time2EachPlayer/60)
		{
			case 5:
				m_ThinkTime.value = 0;
				break;
			case 10:
				m_ThinkTime.value = 1;
				break;
			case 15:
				m_ThinkTime.value = 2;
				break;
			case 30:
				m_ThinkTime.value = 3;
				break;
			default:
				m_ThinkTime.value = 0;
				break;
		}
		m_SettingBoard.SetActive(true);
		Main.Instance.Env.GamePause = true;
	}
}
