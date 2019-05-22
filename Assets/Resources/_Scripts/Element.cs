using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
public class Element : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler, IPointerClickHandler{
	// Side = 1: player 1 (computer)
	// Side = -1: Player 2 (player)
	private PlayerTurn m_Side;
	private int Index;
	public int[] Pos{get;set;}
	Action clickAction =null;
	public void setSide(PlayerTurn _side, int[] _pos, Sprite _sprite, int _index = -1, Action action = null) {
		m_Side = _side;
		Pos = _pos;
		Index = _index;
		Debug.LogWarning(Pos[0] + " " + Pos[1]);
		gameObject.GetComponent<Image>().sprite = _sprite;
		if(action == null) clickAction = choiceObjectEvent;
		else clickAction = action;
	}
	public void updatePos(int[] _newPos) {
		Pos = _newPos;
	}
	public void OnBeginDrag(PointerEventData eventData)
    {

	}
	public void OnDrag(PointerEventData data)
    {
		
	}
	public void OnEndDrag(PointerEventData eventData)
    {
	}
	void choiceObjectEvent() {
		if((Main.Instance.Env.CurrentTurn % 2 == 0 && m_Side == PlayerTurn.UPPOS)
		|| (Main.Instance.Env.CurrentTurn % 2 == 1 && m_Side == PlayerTurn.DOWNPOS)) {
			Debug.LogError(m_Side.ToString());
			Main.Instance.sendElementChoice(m_Side, Index, Pos);
			StuffManager.Instance.playSound(SoundName.CHON_QUAN_CO);
		}
	}
	public void OnPointerClick(PointerEventData pointerEventData)
    {
		Debug.LogWarning("Clicked!");
		clickAction.Invoke();
	}
}
