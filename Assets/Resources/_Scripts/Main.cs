using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class Main : MonoBehaviour {
	private static Main m_Instance;
	public static Main Instance {
		get{return m_Instance;}
	}
	// Use this for initialization
	private Environment m_Env;
	[SerializeField] private int firstPlayer = 1;
	[SerializeField] private UIManager uIManager;
	[SerializeField] private StuffManager m_StuffManager;


	public int FirstPlayer{get{return firstPlayer;}}
	public bool _NguoiChoiBangAI = false;

	
	private bool m_IsMusicOn = true;
	public bool IsMusicOn {get{return m_IsMusicOn;}}
	private bool m_IsSoundEffectOn = true;
	public bool IsSoundEffectOn {get{return m_IsSoundEffectOn;}}

	[SerializeField] private bool m_IsUpAI;
	public bool IsUpAI{get{return m_IsUpAI;}}
	[SerializeField] private bool m_IsDownAI;
	public bool IsDownAI{get{return m_IsDownAI;}}
	[SerializeField] private Text m_PlayerUpTimeText, m_PlayerDownTimeText, m_StepText, m_CurrentModeText;
	private float m_PlayerUpTime, m_PlayerDownTime;
	[SerializeField] private float m_Time2EachPlayer = 5*60f; 
	public int Time2EachPlayer{get{return (int)m_Time2EachPlayer;}}
	[SerializeField] private Mode m_Mode = Mode.PLAYER_AI;
	public Mode CurrentMode {get{return m_Mode;}}
	[SerializeField] private PlayerTurn m_PlayerTurn;
	public bool NguoiChoiThayBangAI {get {return _NguoiChoiBangAI;}}
	
	public Environment Env {
		get{return m_Env;}
	}
	/// <summary>
	/// Awake is called when the script instance is being loaded.
	/// </summary>
	void Awake()
	{
		Screen.orientation = ScreenOrientation.Portrait;
		if(m_Instance == null) m_Instance = this;
		m_Env = new Environment();
		
	}
	void Start () {
		addHandle();
		m_Mode = Mode.PLAYER_AI;
		m_Time2EachPlayer = 10 * 60f;	
		//init() ;
		//StartCoroutine(m_Env.begin());
	}
	void init() {
		switch (m_Mode)
		{
			case Mode.AI_AI:
				m_IsDownAI = true;
				m_IsUpAI = true;
				break;
			case Mode.PLAYER_AI:
				m_IsDownAI = false;
				m_IsUpAI = true;
				break;
			case Mode.PLAYER_PLAYER:
				m_IsDownAI = false;
				m_IsUpAI = false;
				break;
			default:
				break;
		}
		m_PlayerUpTime = m_Time2EachPlayer;
		m_PlayerDownTime = m_Time2EachPlayer;
		
	}
	public void musicOn(bool e) {
		m_IsMusicOn = e;
	}
	public void soundEffectOn(bool e) {
		m_IsSoundEffectOn = e;
	}
	/// <summary>
	/// Update is called every frame, if the MonoBehaviour is enabled.
	/// </summary>
	void Update()
	{
		// Debug.LogError(m_Env.GamePause);
		// Debug.LogError(m_Env.Winner);
		if(m_Env.Winner == PlayerTurn.NULL && !m_Env.GamePause) {
			switch (m_PlayerTurn)
			{
				case PlayerTurn.UPPOS:
					m_PlayerUpTime -= Time.deltaTime;
					break;
				case  PlayerTurn.DOWNPOS:
					m_PlayerDownTime -= Time.deltaTime;
					break;
				case  PlayerTurn.NULL:
				default:
					break;
			}
			updateTime();
		}
		// if(!m_Env.GamePause) {
		// 	updateUI ();
		// }
	}
	void updateUI () {
		//m_StepText.text = m_Env.PlayerScore.ToString();
	}
	public void updateStep(int step){
		m_StepText.text = step.ToString();
	}
	void updateTime() {
		// Debug.LogError("thoi gian cho nguoi choi 1 la: " + m_PlayerUpTime);
		m_PlayerUpTimeText.text = ((int)(m_PlayerUpTime /60)).ToString() + " : " + ((int)(m_PlayerUpTime % 60)).ToString() ;
		m_PlayerDownTimeText.text = ((int)(m_PlayerDownTime /60)).ToString() + " : " + ((int)(m_PlayerDownTime % 60)).ToString();
		if(m_PlayerUpTime <= 0f) {
			sendPlayerWinnerSignal(PlayerTurn.DOWNPOS);
		}
		else if(m_PlayerDownTime <= 0f) {
			sendPlayerWinnerSignal(PlayerTurn.UPPOS);
		}
	}
	void addHandle() {
		uIManager.choicedNextMoveHandle += playerChoicedNextMove;
		m_Env.playerCountDownTurnHandle += playerCountDownTurn;
	}
	void removeHanle() {
		uIManager.choicedNextMoveHandle -= playerChoicedNextMove;
		m_Env.playerCountDownTurnHandle -= playerCountDownTurn;
	}
	
	public void gameRestart(Mode _mode, float _time) {
		m_Mode = _mode;
		m_Time2EachPlayer = _time;
		m_PlayerUpTimeText.text = ((int)(m_Time2EachPlayer /60)).ToString() + " : " + ((int)(m_Time2EachPlayer % 60)).ToString() ;
		m_PlayerDownTimeText.text = ((int)(m_Time2EachPlayer /60)).ToString() + " : " + ((int)(m_Time2EachPlayer % 60)).ToString();
		init();
		m_Env.gameRestart();
		uIManager.gameSestart();
		switch(m_Mode){
			case Mode.AI_AI:
				m_CurrentModeText.text = "AI vs AI";
				break;
			case Mode.PLAYER_AI:
				m_CurrentModeText.text = "Player vs AI";
				break;
			case Mode.PLAYER_PLAYER:
				m_CurrentModeText.text = "Player vs Player";
				break;
			default:
				m_CurrentModeText.text = "Player vs Player";
				break;
		}
		StartCoroutine(m_Env.begin());
		m_StuffManager.playGame();
	}
	public void gameStart() {

	}
	public event SendElementChoice sendElementHandle;
	public event SendPlayerWinnerHandle sendPlayerWinnerHandle;
	public void sendPlayerWinnerSignal(PlayerTurn p) {
		if(sendPlayerWinnerHandle != null) {
			PlayerTurnArgs e = new PlayerTurnArgs(p);
			sendPlayerWinnerHandle(this, e);
		}
	}
	public void sendElementChoice(PlayerTurn playerTurn, int _index, int[] move) {
		
		if(sendElementHandle != null) {
			Debug.Log(move[0] + " ---  " + move[1]);
			SendElementChoiceArgs e = new SendElementChoiceArgs(playerTurn, _index, move);
			sendElementHandle(this, e);
		}
	}

	private void playerChoicedNextMove(object sender, EventArgs e) {
		ChoicedNextMoveArgs args = (ChoicedNextMoveArgs) e;
		var playerTurn = args.PlayerTurn;
		var move = args.Move;
		Debug.LogError("Vao day nha!");
		choicedNextMove(playerTurn, move);
	}
	private void playerCountDownTurn(object sender, EventArgs e) {
		PlayerTurnArgs args = (PlayerTurnArgs) e;
		m_PlayerTurn = args.PlayerTurn;
		// Debug.LogError("Vao day nha!");

	}
	
    public event PlayerChoicedNextMoveHandle playerChoicedNextMoveHandle;
	private void choicedNextMove(PlayerTurn playerTurn, int[] move) {
		if(playerChoicedNextMoveHandle != null) {
			ChoicedNextMoveArgs e = new  ChoicedNextMoveArgs(playerTurn, move);
			playerChoicedNextMoveHandle(this, e);
		}
	}
	
	public delegate void SendElementChoice(object sender, EventArgs e);
	public delegate void PlayerChoicedNextMoveHandle(object sender, EventArgs e);
	public delegate void SendPlayerWinnerHandle(object sender, EventArgs e);
}
public enum Mode
{
	PLAYER_PLAYER = 0,
	PLAYER_AI = 1,
	AI_AI = 2,
	NONE = 3
}
public enum PlayerTurn
{
	UPPOS = 0,
	DOWNPOS = 1,
	NULL = 2
}
