using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StuffManager : MonoBehaviour {

	private static StuffManager m_Instance;
	public static StuffManager Instance {get{return m_Instance;}}
	[Header("Sound")]
	[SerializeField] AudioClip m_StartGameSound;
	[SerializeField] AudioClip m_StartMathSound;
	[SerializeField] AudioClip m_ChonCoSound;
	[SerializeField] AudioClip m_WinSound;
	[SerializeField] AudioClip m_LoseSound;
	[SerializeField] AudioClip m_DanhCoSound;
	[SerializeField] AudioClip m_AnCoSound;
	[SerializeField] AudioClip m_BiAnCoSound;
	[SerializeField] AudioClip m_LenLevelSound;
    System.Random rd = new System.Random();


    [SerializeField]private AudioSource m_AudioSound;
	[SerializeField]private AudioSource m_SoundSource;
    [Space(10)]
    [Header("Bi an")]
    [SerializeField] AudioClip[] m_BiAnCoSounds;

    [Space(10)]
    [Header("An")]
    [SerializeField] AudioClip[] m_AnCoSounds;
    [Space(10)]
    [Header("Lau The")]
    [SerializeField] AudioClip[] m_ChoLauSounds;

    [Space(10)]
    [Header("DI CO")]
    [SerializeField] AudioClip[] m_AIDiCoSound;
    /// <summary>
    /// Awake is called when the script instance is being loaded.
    /// </summary>
    void Awake()
	{
		if(m_Instance == null) m_Instance = this;
	}
	/// <summary>
	/// Start is called on the frame when a script is enabled just before
	/// any of the Update methods is called the first time.
	/// </summary>
	void Start()
	{
		if(Main.Instance.IsMusicOn) {
			m_AudioSound.clip = m_StartGameSound;
			m_AudioSound.Play();
		}
	}
	public void playGame() {
		if(Main.Instance.IsMusicOn) {
			Debug.Log("vao playGame");
			m_AudioSound.clip = m_StartMathSound;
			m_AudioSound.volume = 0.2f;
			m_AudioSound.Play();
		}
		else {
			m_AudioSound.Stop();
		}
	}
	public void playSound(SoundName name) {
		switch (name)
		{
			case SoundName.CHON_QUAN_CO:
				m_SoundSource.clip = m_ChonCoSound;
				break;
			case SoundName.DANH_QUAN_CO:
				m_SoundSource.clip = m_DanhCoSound;
				break;
			case SoundName.AN_CO:
                   m_SoundSource.clip = m_BiAnCoSounds[rd.Next(0, m_BiAnCoSounds.Length)];
               
				break;
			case SoundName.BI_AN:
                   m_SoundSource.clip = m_AnCoSounds[rd.Next(0, m_AnCoSounds.Length)];
                break;
			case SoundName.THANG:
				m_SoundSource.clip = m_WinSound;
				m_AudioSound.Stop();
				break;
			case SoundName.THUA:
				m_SoundSource.clip = m_LoseSound;
				m_AudioSound.Stop();
				break;
			case SoundName.LEVEL_UP:
				m_SoundSource.clip = m_LenLevelSound;
				m_AudioSound.Stop();
				break;
            case SoundName.AI_AN_CO:
                m_SoundSource.clip = m_AnCoSounds[rd.Next(0, m_AnCoSounds.Length)];
                //m_AudioSound.Stop();
                break;
            case SoundName.AI_BI_AN_CO:
                m_SoundSource.clip = m_BiAnCoSounds[rd.Next(0, m_BiAnCoSounds.Length )];
                //m_AudioSound.Stop();
                break;
            case SoundName.AI_CHO_LAU:
                Debug.LogError("VAo day!");
                m_SoundSource.clip = m_ChoLauSounds[rd.Next(0, m_ChoLauSounds.Length)];
                //m_AudioSound.Stop();
                break;
            case SoundName.AI_DI_CO:
                m_SoundSource.clip = m_AIDiCoSound[rd.Next(0, m_AIDiCoSound.Length)];
                //m_AudioSound.Stop();
                break;
            default:
				break;
		}
		m_SoundSource.Play();
	}
}
public enum SoundName
{
	CHON_QUAN_CO = 0,
	DANH_QUAN_CO = 1,
	AN_CO = 2,
	BI_AN = 3,
	THANG = 4, 
	THUA = 5,
	LEVEL_UP = 6,
    AI_AN_CO = 7,
    AI_BI_AN_CO = 8,
    AI_CHO_LAU = 9,
    AI_DI_CO =10,
	COUNT
}