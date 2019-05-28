using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using System;
public class Environment {
	///

	/*------------------------- 
	//		X
	// 		________________
	// 		|
	// Y 	|
	// 		|
	// 		|
	// 		|
	// 		|
	//=> (Y, X)
	*/
	// Use this for initialization
	int mapX = 5;
	int mapY = 5;
    System.Random rd = new System.Random();
	public int[] mapSize = new int[2]{5,5};//(Y, X)
	private int[][] upPlayerPos/* player 1*/, downPlayerPos/* player 2*/,
			 moveDiagonalRightDown, moveDiagonalRightUp, 
			 moveDiagonalLeftDown, moveDiagonalLeftUp;
	// public int[][] UpPlayerPos {get; set;}
	// public int[][] DownPlayerPos {get; set;}
	public int[][] UpPlayerPos {get{return upPlayerPos;}}
	public int[][] DownPlayerPos {get{return downPlayerPos;}}
	private int[][][] totalWayToMove;
	private int[][] playerTotalMoves;
	public int demStack = 0;
	public bool showWrong = false;
	private PlayerTurn m_Winner = PlayerTurn.NULL;
	public bool m_GameRestart = false;
	public PlayerTurn Winner {get{return m_Winner;}}
	private int m_ScorePerKill = 10;
	public int ScorePerKill {get{return m_ScorePerKill;}}
	private int m_PlayerScore = 0;
	public int PlayerScore{get{return m_PlayerScore;}}
	
	public Environment() {
		
		moveDiagonalRightDown = new int[][]{new int[]{0, 0}, new int[]{1, 1}, new int[]{2, 2}, new int[]{3, 3}, new int[]{0, 2}, new int[]{1, 3}, new int[]{2, 0}, new int[]{3, 1}};
		moveDiagonalRightUp = new int[][]{new int[]{4, 0}, new int[]{3, 1}, new int[]{2, 2}, new int[]{1, 3}, 
		new int[]{2, 0}, new int[]{1, 1}, new int[]{4, 2}, new int[]{3, 3}};
		moveDiagonalLeftDown = new int[][]{new int[]{0, 4}, new int[]{1, 3}, new int[]{2, 2}, new int[]{3, 1}, 
		new int[]{0, 2}, new int[]{1, 1}, new int[]{2, 4}, new int[]{3, 3}};
		moveDiagonalLeftUp = new int[][]{new int[]{4, 4}, new int[]{3, 3}, new int[]{2, 2}, new int[]{1, 1}, 
		new int[]{2, 4}, new int[]{1, 3}, new int[]{4, 2}, new int[]{3, 1}};
		totalWayToMove = new int[][][]{
			moveDiagonalLeftDown,
			moveDiagonalLeftUp,
			moveDiagonalRightDown,
			moveDiagonalRightUp,
		};
		init();
		addHandle();
	}
	void addHandle() {
		Main.Instance.sendElementHandle += suggestNextMoves;
		Main.Instance.playerChoicedNextMoveHandle += setMove;
		Main.Instance.sendPlayerWinnerHandle += ReceivePlayerWinSingal;
	}
	void removeHandle() {
		Main.Instance.sendElementHandle -= suggestNextMoves;
		Main.Instance.playerChoicedNextMoveHandle -= setMove;
		Main.Instance.sendPlayerWinnerHandle -= ReceivePlayerWinSingal;
	}
	public void init() {
		upPlayerPos = new int[][] {new int[]{0, 0}, new int[]{0, 1}, new int[]{0, 2}, new int[]{0, 3}, new int[]{0, 4}, new int[]{1, 0},new int[] {1, 4}, new int[]{2, 4}};
		downPlayerPos = new int[][] {new int[]{4, 0}, new int[]{4, 1}, new int[]{4, 2}, new int[]{4, 3}, new int[]{4, 4}, new int[]{3, 0}, new int[]{2, 0}, new int[]{3, 4}};
		// upPlayerPos = new int[][] {new int[]{0, 0}, new int[]{0, 1}, new int[]{2, 2}, new int[]{0, 3}, new int[]{0, 4}, new int[]{1, 0},new int[] {1, 4}, new int[]{2, 4}};
		// downPlayerPos = new int[][] {new int[]{3, 1}, new int[]{4, 1}, new int[]{4, 2}, new int[]{4, 3}, new int[]{3, 3}, new int[]{3, 0}, new int[]{2, 0}, new int[]{3, 4}};
		m_Winner = PlayerTurn.NULL;
		m_GamePause = true;
	}
	public void CheckChangeValue(ref int[][] computer) {
		var temp = new int[][]{
			new int[]{9,9},
			new int[]{8,8},
			new int[]{8,8}
		};
		computer = temp;
		computer[0] = new  int[]{9,9};
	}
	//*************
	//Function: GamePlete
	// Return:
	//***** -1: game completed Player 1 win
	//***** 0: Game not complete
	//***** 1: Player 2 win
	protected PlayerTurn gameComplete(int[][] upPlayerPos, int[][] downPlayerPos) {
		if(upPlayerPos.Length ==0) {
			return PlayerTurn.DOWNPOS;
		}
		if(downPlayerPos.Length == 0) {
			return PlayerTurn.UPPOS;
		}
		return PlayerTurn.NULL;
	}
	protected bool freeSpace(int[] check, int[][] upPlayerPos, int[][] downPlayerPos) {
		if(Array.FindAll(check, item => item > -1).Length == check.Length && Array.FindAll(check, item => item < 5).Length == check.Length) {
			if(Array.FindIndex(upPlayerPos, item =>  Enumerable.SequenceEqual(item, check)) == -1 && 
			Array.FindIndex(downPlayerPos, item =>  Enumerable.SequenceEqual(item, check)) == -1) {
				return true;
			}
		}
		return false;
	}
	protected List<List<int>> suggestNextMoves(PlayerTurn playerTurn, int[] move, int[][] upPlayerPos, int[][] downPlayerPos, int index = -1) {
		var result = new List<List<int>>();
		// Result item: 0 => chọn cách đi nào
					//  1, 2 => [x, y] đi đến đâu
					//  3 => chỉ di chuyển hay ăn đối phương, 0: chỉ di chuyển, 1 là ăn đối phương.
					//  4, 5 => nếu [3] == 0: thì [-1, -1], nếu [3] == 1: thì [4, 5]
		int[][] active, passive;
		switch(playerTurn) {
			case PlayerTurn.UPPOS:
				active = upPlayerPos;
				passive = downPlayerPos;
				break;
			case PlayerTurn.DOWNPOS:
				active = downPlayerPos;
				passive = upPlayerPos;
				break;
			default:
			return null;
		}
		var _index = Array.FindIndex(active, item =>  Enumerable.SequenceEqual(item, move));
		if(_index == -1) return null;
		int[][] tempCompareOneStep;
		int[][] tempCompareTwoStep;
			//	# neu x nam trong truong hop 1, neu vi tri di cheo 1 don vi cua x khong co dong doi chiem cho,
			//	# ------------------va neu doi phuong chiem cho + vi tri cheo 2 don vi cua x trong, ta cho x di cheo 2 don v
			//	# ------------------va neu doi phong khong chiem cho thi cho x toi vi tri cheo 1 don vi
			//	# 1
			// List<List<int>> space = new List<List<int>>() {
			// 	new List<int>(){1, -1},
			// 	new List<int>(){-1, -1},
			// 	new List<int>(){-1, -1},
			// 	new List<int>(){-1, -1}
			// };
		tempCompareOneStep = new int[][]{
			new int[]{move[0] + 1, move[1] - 1}, 
			new int[]{move[0] - 1, move[1] - 1},
			new int[]{move[0] + 1, move[1] + 1},
			new int[]{move[0] - 1, move[1] + 1},
			new int[]{move[0] - 1,move[1]}, 
			new int[]{move[0] + 1,move[1]},
			new int[]{move[0],move[1] - 1},
			new int[]{move[0],move[1] + 1},
		};
		tempCompareTwoStep = new int[][]{
			new int[]{move[0] + 2, move[1] - 2}, 
			new int[]{move[0] - 2, move[1] - 2},
			new int[]{move[0] + 2, move[1] + 2},
			new int[]{move[0] - 2, move[1] + 2},
			new int[]{move[0] - 2,move[1]}, 
			new int[]{move[0] + 2,move[1]},
			new int[]{move[0],move[1] - 2},
			new int[]{move[0],move[1] + 2}
		};

		for (int toTalMoveIndex = 0; toTalMoveIndex < totalWayToMove.Length; toTalMoveIndex++)
		{
			if(Array.FindIndex(totalWayToMove[toTalMoveIndex], item =>  Enumerable.SequenceEqual(item, move)) != -1 && 
				Array.FindIndex(active, item =>  Enumerable.SequenceEqual(item, tempCompareOneStep[toTalMoveIndex])) == -1) {
				if(Array.FindIndex(passive, item =>  Enumerable.SequenceEqual(item, tempCompareOneStep[toTalMoveIndex])) != -1) {
					if(freeSpace(tempCompareTwoStep[toTalMoveIndex], upPlayerPos, downPlayerPos)) {
						result.Add(new List<int>(){_index, 
						tempCompareTwoStep[toTalMoveIndex][0], tempCompareTwoStep[toTalMoveIndex][1], 
						1, 
						tempCompareOneStep[toTalMoveIndex][0], tempCompareOneStep[toTalMoveIndex][1]}) ;
					}
				}
				else {
					result.Add(new List<int>(){_index, 
					tempCompareOneStep[toTalMoveIndex][0], tempCompareOneStep[toTalMoveIndex][1], 
					0});
				}
			}
		}
		int indexTemp =0;
		for (int fourBasicDirection = 0; fourBasicDirection < 2; fourBasicDirection++)
		{
			indexTemp = 4 + fourBasicDirection*2;
			if(move[fourBasicDirection] != 0 && Array.FindIndex(active, item =>  Enumerable.SequenceEqual(item, tempCompareOneStep[indexTemp])) == -1) {
				if(Array.FindIndex(passive, item =>  Enumerable.SequenceEqual(item, tempCompareOneStep[indexTemp])) != -1) {
					if(freeSpace(tempCompareTwoStep[indexTemp], passive, active)) {
						result.Add(new List<int>(){_index, 
						tempCompareTwoStep[indexTemp][0], tempCompareTwoStep[indexTemp][1], 
						1, 
						tempCompareOneStep[indexTemp][0], tempCompareOneStep[indexTemp][1]}) ;
					}
				}
				else {
					result.Add(new List<int>(){_index, 
					tempCompareOneStep[indexTemp][0], tempCompareOneStep[indexTemp][1], 
					0}) ;
				}
			}

			indexTemp = 4 + fourBasicDirection*2 + 1;
			if(move[fourBasicDirection] != (mapSize[fourBasicDirection] - 1) && Array.FindIndex(active, item =>  Enumerable.SequenceEqual(item, tempCompareOneStep[indexTemp])) == -1) {
				if(Array.FindIndex(passive, item =>  Enumerable.SequenceEqual(item, tempCompareOneStep[indexTemp])) != -1) {
					if(freeSpace(tempCompareTwoStep[indexTemp], passive, active)) {
						result.Add(new List<int>(){_index, 
						tempCompareTwoStep[indexTemp][0], tempCompareTwoStep[indexTemp][1], 
						1, 
						tempCompareOneStep[indexTemp][0], tempCompareOneStep[indexTemp][1]}) ;
					}
				}
				else {
					result.Add(new List<int>(){_index, 
					tempCompareOneStep[indexTemp][0], tempCompareOneStep[indexTemp][1], 
						0}) ;
				}
			}
		}
		return result;
	}
	//if player 1 move: playerTurn =1
	// Else Player = -1
	public List<List<int>> simpleMoves(int playerTurn, int[][] upPlayerPos, int[][] downPlayerPos){
		var result = new List<List<int>>();
		// Result item: 0 => chọn quan co
					//  1, 2 => [x, y] đi đến đâu
					//  3 =>  ăn đối phương, 0: chỉ di chuyển, 1 là ăn đối phương.
					//  4, 5 => nếu [3] == 0: thì [-1, -1], nếu [3] == 1: thì [4, 5]
		int[][] active, passive;
		switch(playerTurn) {
			case 1:
				active = upPlayerPos;
				passive = downPlayerPos;
				break;
			case -1:
				active = downPlayerPos;
				passive = upPlayerPos;
				break;
			default:
			return null;
		}
		int[][] tempCompareOneStep;
		int[][] tempCompareTwoStep;
		for (int index = 0; index < active.Length; index++)
		{
			//	# neu x nam trong truong hop 1, neu vi tri di cheo 1 don vi cua x khong co dong doi chiem cho,
			//	# ------------------va neu doi phuong chiem cho + vi tri cheo 2 don vi cua x trong, ta cho x di cheo 2 don v
			//	# ------------------va neu doi phong khong chiem cho thi cho x toi vi tri cheo 1 don vi
			//	# 1
			// List<List<int>> space = new List<List<int>>() {
			// 	new List<int>(){1, -1},
			// 	new List<int>(){-1, -1},
			// 	new List<int>(){-1, -1},
			// 	new List<int>(){-1, -1}
			// };
			tempCompareOneStep = new int[][]{
				new int[]{active[index][0] + 1, active[index][1] - 1}, 
				new int[]{active[index][0] - 1, active[index][1] - 1},
				new int[]{active[index][0] + 1, active[index][1] + 1},
				new int[]{active[index][0] - 1, active[index][1] + 1},
				new int[]{active[index][0] - 1,active[index][1]}, 
				new int[]{active[index][0] + 1,active[index][1]},
				new int[]{active[index][0],active[index][1] - 1},
				new int[]{active[index][0],active[index][1] + 1},
			};
			tempCompareTwoStep = new int[][]{
				new int[]{active[index][0] + 2, active[index][1] - 2}, 
				new int[]{active[index][0] - 2, active[index][1] - 2},
				new int[]{active[index][0] + 2, active[index][1] + 2},
				new int[]{active[index][0] - 2, active[index][1] + 2},
				new int[]{active[index][0] - 2,active[index][1]}, 
				new int[]{active[index][0] + 2,active[index][1]},
				new int[]{active[index][0],active[index][1] - 2},
				new int[]{active[index][0],active[index][1] + 2}
			};

			for (int toTalMoveIndex = 0; toTalMoveIndex < totalWayToMove.Length; toTalMoveIndex++)
			{	
				if(Array.FindIndex(totalWayToMove[toTalMoveIndex], item =>  Enumerable.SequenceEqual(item, active[index])) != -1 && 
					Array.FindIndex(active, item =>  Enumerable.SequenceEqual(item, tempCompareOneStep[toTalMoveIndex])) == -1) {
					if(Array.FindIndex(passive, item =>  Enumerable.SequenceEqual(item, tempCompareOneStep[toTalMoveIndex])) != -1) {
						if(freeSpace(tempCompareTwoStep[toTalMoveIndex], upPlayerPos, downPlayerPos)) {
							result.Add(new List<int>(){index, 
							tempCompareTwoStep[toTalMoveIndex][0], tempCompareTwoStep[toTalMoveIndex][1], 
							1, 
							tempCompareOneStep[toTalMoveIndex][0], tempCompareOneStep[toTalMoveIndex][1]}) ;
						}
					}
					else {
						result.Add(new List<int>(){index, 
						tempCompareOneStep[toTalMoveIndex][0], tempCompareOneStep[toTalMoveIndex][1], 
						0});
					}
				}
			}
			int indexTemp =0;
			for (int fourBasicDirection = 0; fourBasicDirection < 2; fourBasicDirection++)
			{
				indexTemp = 4 + fourBasicDirection*2;
				if(active[index][fourBasicDirection] != 0 && Array.FindIndex(active, item =>  Enumerable.SequenceEqual(item, tempCompareOneStep[indexTemp])) == -1) {
					if(Array.FindIndex(passive, item =>  Enumerable.SequenceEqual(item, tempCompareOneStep[indexTemp])) != -1) {
						if(freeSpace(tempCompareTwoStep[indexTemp], passive, active)) {
							result.Add(new List<int>(){index, 
							tempCompareTwoStep[indexTemp][0], tempCompareTwoStep[indexTemp][1], 
							1, 
							tempCompareOneStep[indexTemp][0], tempCompareOneStep[indexTemp][1]}) ;
						}
					}
					else {
						result.Add(new List<int>(){index, 
						tempCompareOneStep[indexTemp][0], tempCompareOneStep[indexTemp][1], 
						0}) ;
					}
				}

				indexTemp = 4 + fourBasicDirection*2 + 1;
				if(active[index][fourBasicDirection] != (mapSize[fourBasicDirection] - 1) && Array.FindIndex(active, item =>  Enumerable.SequenceEqual(item, tempCompareOneStep[indexTemp])) == -1) {
					if(Array.FindIndex(passive, item =>  Enumerable.SequenceEqual(item, tempCompareOneStep[indexTemp])) != -1) {
						if(freeSpace(tempCompareTwoStep[indexTemp], passive, active)) {
							result.Add(new List<int>(){index, 
							tempCompareTwoStep[indexTemp][0], tempCompareTwoStep[indexTemp][1], 
							1, 
							tempCompareOneStep[indexTemp][0], tempCompareOneStep[indexTemp][1]}) ;
						}
					}
					else {
						result.Add(new List<int>(){index, 
						tempCompareOneStep[indexTemp][0], tempCompareOneStep[indexTemp][1], 
							0}) ;
					}
				}
			}
		}
		return result;
	}
	public static int[] SubArray(int[] data, int index, int length)
	{
		int[] result = new int[length];
		Array.Copy(data, index, result, 0, length);
		return result;
	}
	protected void checkKill(bool isTest,int[] x, ref int[][] upPlayerPos,ref int[][] downPlayerPos, ref int[][] sourceChange, int p) {
		int[][] tempCheckPos = new int[][] {
			new int[]{x[0] - 1, x[1] + 1},
			new int[]{x[0] + 1, x[1] - 1}, 

			new int[]{x[0] + 1, x[1] + 1}, 
			new int[]{x[0] - 1, x[1] - 1}, 

			new int[]{x[0] - 1, x[1]}, 
			new int[]{x[0] + 1, x[1]}, 

			new int[]{x[0], x[1] - 1}, 
			new int[]{x[0], x[1] + 1}, 
		};
		int itemFirst, itemSecond;
		if(Array.FindIndex(moveDiagonalLeftDown, item =>  Enumerable.SequenceEqual(item, x)) != -1 ||
			Array.FindIndex(moveDiagonalRightUp, item =>  Enumerable.SequenceEqual(item, x)) != -1) {
				//If Temp check Pos first and second item exist in passive
				itemFirst = Array.FindIndex(sourceChange, item =>  Enumerable.SequenceEqual(item, tempCheckPos[0]));
				itemSecond = Array.FindIndex(sourceChange, item =>  Enumerable.SequenceEqual(item, tempCheckPos[1]));
				if( itemFirst != -1 &&
					itemSecond != -1) {
						changeBoardValue(isTest, itemFirst, itemSecond,ref upPlayerPos, ref downPlayerPos, ref sourceChange, p);
						// Debug.LogError("");
				}
		}
		if(Array.FindIndex(moveDiagonalLeftUp, item =>  Enumerable.SequenceEqual(item, x)) != -1 ||
			Array.FindIndex(moveDiagonalRightDown, item =>  Enumerable.SequenceEqual(item, x)) != -1) {
				//If Temp check Pos first and second item exist in passive
				itemFirst = Array.FindIndex(sourceChange, item =>  Enumerable.SequenceEqual(item, tempCheckPos[2]));
				itemSecond = Array.FindIndex(sourceChange, item =>  Enumerable.SequenceEqual(item, tempCheckPos[3]));
				if( itemFirst != -1 &&
					itemSecond != -1) {
						changeBoardValue(isTest, itemFirst, itemSecond,ref upPlayerPos, ref downPlayerPos, ref sourceChange, p);
						// var tempList = passive.ToList();
						// var tempItem = tempList[itemSecond];
						// tempList.RemoveAt(itemFirst);
						// tempList.Remove(tempItem);
						// passive = tempList.ToArray();
						// Debug.LogError("");
				}
		}
		itemFirst = Array.FindIndex(sourceChange, item =>  Enumerable.SequenceEqual(item, tempCheckPos[4]));
		itemSecond = Array.FindIndex(sourceChange, item =>  Enumerable.SequenceEqual(item, tempCheckPos[5]));
		if( itemFirst != -1 &&
			itemSecond != -1) {
				changeBoardValue(isTest, itemFirst, itemSecond,ref upPlayerPos, ref downPlayerPos, ref sourceChange, p);
				// var tempList = passive.ToList();
				// var tempItem = tempList[itemSecond];
				// tempList.RemoveAt(itemFirst);
				// tempList.Remove(tempItem);
				// passive = tempList.ToArray();
				// Debug.LogError("");
		}
		itemFirst = Array.FindIndex(sourceChange, item =>  Enumerable.SequenceEqual(item, tempCheckPos[6]));
		itemSecond = Array.FindIndex(sourceChange, item =>  Enumerable.SequenceEqual(item, tempCheckPos[7]));
		if( itemFirst != -1 &&
			itemSecond != -1) {
				changeBoardValue(isTest, itemFirst, itemSecond,ref upPlayerPos, ref downPlayerPos, ref sourceChange, p);
				// var tempList = passive.ToList();
				// var tempItem = tempList[itemSecond];
				// tempList.RemoveAt(itemFirst);
				// tempList.Remove(tempItem);
				// passive = tempList.ToArray();
				// Debug.LogError("");
		}
	}
	protected void checkKill(bool isTest, int[] x, int playerTurn,ref int[][] upPlayerPos,ref int[][] downPlayerPos) {
		// int[][] passive;
		

		switch (playerTurn)
		{
			case 1:
				checkKill(isTest, x,ref upPlayerPos, ref downPlayerPos, ref downPlayerPos, playerTurn);
				// passive = player;
				break;
			case -1:
				checkKill(isTest, x,ref upPlayerPos, ref downPlayerPos, ref upPlayerPos, playerTurn);
				// passive = computer;
				break;
			default:
				return;
		}
	}
	public void changeBoardValue(bool isTest, int itemFirst, int itemSecond, ref int[][] upPlayerPos, ref int[][] downPlayerPos, ref int[][] sourceChange, int p) {
		List<int[]> tempList ;
		tempList = sourceChange.ToList();
		if(isTest) {
			destroyElement(p, (int[]) sourceChange[itemFirst]);
			destroyElement(p, (int[]) sourceChange[itemSecond]);
		}
		// tempList = passive.ToList();
		var tempItem = tempList[itemSecond];
		// Debug.LogError("Temp List: " + tempList.Count +" "+  itemFirst);
		tempList.RemoveAt(itemFirst);
		tempList.Remove(tempItem);
		sourceChange = tempList.ToArray();
	}
	protected void changeBoard(bool isTest, int[] move, int playerTurn,ref int[][] upPlayerPos,ref int[][] downPlayerPos) {
		int[][] active, passive;
		var activePlayerNextMove = SubArray(move, 1, 2);
		switch (playerTurn)
		{
			case 1:
				active = upPlayerPos;
				passive = downPlayerPos;
				upPlayerPos[move[0]] = SubArray(move, 1, 2);
				break;
			case -1:
				active = downPlayerPos;
				passive = upPlayerPos;
				downPlayerPos[move[0]] = SubArray(move, 1, 2);
				break;
			default:
				Debug.LogError("Change Board be Wrong!");
				return;
		}
		

		// active[move[0]] = SubArray(move, 1, 2);
		if(move[3] == 1) {
			
			// var tempList = passive.ToList();
			// tempList.RemoveAt(index);
			// passive = tempList.ToArray();
			// if(showWrong) {
			// 	Debug.LogError(showInfor(passive));
			// 	showWrong = false;
			// }
			checkKill(isTest, activePlayerNextMove, playerTurn,ref upPlayerPos,ref downPlayerPos);
			List<int[]> tempList;
			int[]itemRemove;
			int index;
			switch (playerTurn)
			{
				case 1:
					itemRemove = SubArray(move, 4, 2);
					index = Array.FindIndex(downPlayerPos, item =>  Enumerable.SequenceEqual(item, itemRemove));
					if(index != -1) {
						tempList = downPlayerPos.ToList();
						if(isTest)
							destroyElement(playerTurn ,(int[])downPlayerPos[index].Clone());
						tempList.RemoveAt(index);
						downPlayerPos = tempList.ToArray();
					}
					// active = computer;
					// passive = player;
					break;
				case -1:
					itemRemove = SubArray(move, 4, 2);
					index = Array.FindIndex(upPlayerPos, item =>  Enumerable.SequenceEqual(item, itemRemove));
					if(index != -1) {
						tempList = upPlayerPos.ToList();
						if(isTest)
							destroyElement(playerTurn, (int[])upPlayerPos[index].Clone());
						tempList.RemoveAt(index);
						upPlayerPos = tempList.ToArray();
					}
					// active = player;
					// passive = computer;
					break;
				default:
					Debug.LogError("Change Board be Wrong!");
					return;
			}
			
			if(showWrong) {
				Debug.LogError("Before clean: " + showInfor(passive) + " item Clean: [" + itemRemove[0] + ";" + itemRemove[1]+"]" + " Index " + index);
			}
			if(index != -1) {
				// Debug.LogError(itemRemove[0] + " " + itemRemove[1]);
				
				
			}
		}
		else {
			checkKill(isTest, activePlayerNextMove, playerTurn,ref upPlayerPos,ref downPlayerPos);
		}
	}
	public string showInfor(int[][] infor) {
		string a = "[";
		foreach (var item in infor)
		{
			a += "[";
			foreach (var i in item)
			{
				a += i+";";
			}
			 a+= "] ";
		}
		a +="]";
		return a;
	}
	public string showInfor(List<List<int>> infor) {
		string a = "[";
		foreach (var item in infor)
		{
			a += "[";
			foreach (var i in item)
			{
				a += i+";";
			}
			 a+= "] ";
		}
		a +="]";
		return a;
	}
	protected int minimax(int playerTurn, ref int[][] upPlayerPos,ref int[][] downPlayerPos,ref List<List<string>> closeStack,int floor,int maxFloor) {

		int result;
		switch(gameComplete(upPlayerPos, downPlayerPos)){
			case PlayerTurn.DOWNPOS:
				result = -1;
				break;
			case PlayerTurn.UPPOS:
				result = 1;
				break;
			default:
				result = 0;
				break;
		}
		if(result != 0) { return 18 * result * playerTurn;}
		if(floor > maxFloor) {
			// # print abs(len(computer) - len(player))
			// # if len(computer) > len(player): return abs(len(computer) - len(player))* 1 *playerTurn
        	// # else: return abs(len(computer) - len(player))* -1 * playerTurn
			// Debug.LogError("end over floor!");
			return (upPlayerPos.Length - downPlayerPos.Length) * playerTurn;
		}
		floor += 1;
		int[] move = new int[]{};
		var score = -18;
		// Debug.LogError(showInfor(player));
		// Debug.LogError(showInfor(computer));
		// Debug.LogError(" -------------- ");
		var moves = simpleMoves(playerTurn, upPlayerPos, downPlayerPos);
		var movesArray  = moves.Select(item => item.ToArray()).ToArray();
		// if(demStack == 9) {
		// 	Debug.LogError(playerTurn);
		// 	Debug.LogError(showInfor(computer));
		// 	Debug.LogError(showInfor(player));
		// 	Debug.LogError(showInfor(movesArray));
		// 	showWrong = true;
		// }
		demStack++;
		// Debug.LogWarning(showInfor(moves));
		// var movesArray = Shuffle(moves.Select(item => item.ToArray()).ToArray());
		// Debug.LogWarning(showInfor(movesArray));
		// Debug.LogError(movesArray.Length);
		bool checkExist = false;
		string computerEncode = string.Empty;
		string playerEncode = string.Empty;
		foreach (var item in movesArray)
		{
			int[][] upPlayerPosTemp = (int[][]) upPlayerPos.Select(i => i.ToArray()).ToArray();
			int[][] downPlayerPosTemp = (int[][]) downPlayerPos.Select(i => i.ToArray()).ToArray();
			
			changeBoard(false, item, playerTurn,ref upPlayerPosTemp,ref downPlayerPosTemp);
			checkExist = false;
			computerEncode = Flatten(upPlayerPosTemp);
			playerEncode = Flatten(downPlayerPosTemp);
			// Debug.LogError(computerEncode + " " + playerEncode);
			foreach (var stackItem in closeStack)
			{
				if(computerEncode == stackItem[0] && playerEncode == stackItem[1]) {
					checkExist = true;
					break;
				}
			}
			if(checkExist) continue;
			else {
				closeStack.Add(new List<string> () {computerEncode, playerEncode});
			}
			var thisScore = -minimax(playerTurn * -1,ref upPlayerPosTemp,ref downPlayerPosTemp,ref closeStack,floor,maxFloor);
			if(thisScore > score) {
				score = thisScore;
				move = item;
				//  Debug.LogError("Update Score!");
			}
		}
		if(move.Length == 0) { return 0;}
		// Debug.LogError("end perfect!");
		return score;
	}
	// closeStack;
	// movesArray;
	// moves;
	// int score = -18;
	protected int[] computerMoves(int AINumb) {
		List<List<string>>  closeStack = new List<List<string>>();
		var score = -18;
		int[] move = null;
		bool checkExist = false;
		List<List<int>>  moves = simpleMoves(AINumb, upPlayerPos, downPlayerPos);
		int[][] movesArray  = moves.Select(item => item.ToArray()).ToArray();
		movesArray = Shuffle(movesArray);
		int maxFloor = 3;
		int floor = 0;
		string computerEncode = string.Empty;
		string playerEncode = string.Empty;
		foreach (var item in movesArray)
		{
			checkExist = false;
			int[][] upPlayerPosTemp = (int[][]) upPlayerPos.Select(i => i.ToArray()).ToArray();
			int[][] downPlayerPosTemp = (int[][]) downPlayerPos.Select(i => i.ToArray()).ToArray();
			changeBoard(false, item, AINumb, ref upPlayerPosTemp, ref downPlayerPosTemp);
			computerEncode = Flatten(upPlayerPosTemp);
			playerEncode = Flatten(downPlayerPosTemp);
			closeStack.Add(new List<string> () {computerEncode, playerEncode});
			var tempScore = - minimax(-1*AINumb, ref upPlayerPosTemp, ref downPlayerPosTemp, ref closeStack, floor, maxFloor);
			if(tempScore > score) {
				score = tempScore;
				move = item;
			}
		}
		if(move != null) {
			return move;
		}
		return null;
	}

	protected int[] computerMovesMCST(int AINumb) {
		

		// /////MCTS
		int[] lastMove = null;
		int simulationCount = 100;
		TreeNode tn = new TreeNode(AINumb, upPlayerPos, downPlayerPos);
		for (int i = 0; i < simulationCount; i++) {
			Debug.LogError("so lan!" + i);
			tn.selectAction();
			
		}
		//tn.selectAction();
		TreeNode BC = tn.bestChild();
		lastMove = BC.lastMove;
		return lastMove;		
	}
	

	protected int[] allWaysPlayerCanMoves() {
		List<List<int>>  moves = simpleMoves(-1, upPlayerPos, downPlayerPos);
		int[][] movesArray  = moves.Select(item => item.ToArray()).ToArray();
		movesArray = Shuffle(movesArray);
		return movesArray[0];
	}
	protected void playerMoves(int[] move){
		changeBoard(false, move, -1, ref upPlayerPos, ref downPlayerPos);
	}
	private int[] m_Move;
	public void setMove(object sender, EventArgs e) {
		var args = (ChoicedNextMoveArgs) e;
        Debug.LogError((object)("Nhan duoc next Choiced: " + args.PlayerTurn));
		var playerTurn = args.PlayerTurn;
		var move = args.Move;
		if((playerTurn == PlayerTurn.UPPOS && _currentTurn %2 == 0) ||
			(playerTurn == PlayerTurn.DOWNPOS && _currentTurn %2 == 1)) {
			m_Move = move;
		}
	}
	private int _currentTurn = 0;
	//CurrentTurn %2 == 0: player 2 go
	//CurrentTurn %2 == 1: player 1 go
	public int CurrentTurn {get{return _currentTurn;}}
	private bool m_GamePause = false;
	public bool GamePause{get{return m_GamePause;} set{m_GamePause = value;}}
	public void gameRestart() {
		m_GameRestart = true;
		init();
	}
	private int step = -1;
	public IEnumerator begin() {
		 step = -1;
		_currentTurn =Main.Instance.FirstPlayer;
		yield return new WaitForSeconds(1f);
		m_GameRestart = false;
		// _currentTurn = 
		PlayerTurn ketqua;
		
		m_GamePause = false;
		while(true) {
			m_Move = null;
			step++;
			Debug.Log(_currentTurn);
			Main.Instance.updateStep(step);
			if(step == 100) { 
				if(upPlayerPos.Length > downPlayerPos.Length) sendPlayerWin(PlayerTurn.UPPOS);
				else if(upPlayerPos.Length < downPlayerPos.Length) sendPlayerWin(PlayerTurn.DOWNPOS);
				else sendPlayerWin(PlayerTurn.NULL);
				break;
			}
			ketqua = gameComplete(upPlayerPos, downPlayerPos);
			if( ketqua != PlayerTurn.NULL) {
				sendPlayerWin(ketqua);
				Debug.Log(ketqua + " win roi nhe!"); break;
			}
			if((_currentTurn) % 2 ==0) {
				sendPlayerCountDownTurn(PlayerTurn.UPPOS);

                if(Main.Instance.CurrentMode == Mode.PLAYER_AI)
                {

                    StuffManager.Instance.StartCoroutine(choLauThe());
                }

				yield return new WaitUntil(() => ((m_Move != null || Main.Instance.IsUpAI || m_Winner != PlayerTurn.NULL)&& !m_GamePause )|| m_GameRestart);
				if(m_Winner != PlayerTurn.NULL || m_GameRestart) break;
				if(Main.Instance.IsUpAI) {
					Debug.Log("-------------- Computer 1 Turn --------------");
					m_Move = computerMoves(1);
					Debug.Log("m_Move[0]" + m_Move[0] + " m_Move[1]" + m_Move[1] +" m_Move[2]" + m_Move[2] +" m_Move[3]" + m_Move[3]);
				} 
				else {
					Debug.Log("-------------- Player 1 Turn --------------");
				}
				// m_Move = computerMoves(1);
				
				dichuyen(1, upPlayerPos[m_Move[0]].ToList(), new List<int>(){m_Move[1], m_Move[2]});
				changeBoard(true, m_Move, 1,ref upPlayerPos, ref downPlayerPos);
				sendPlayerCountDownTurn(PlayerTurn.NULL);
				yield return new WaitForSeconds(Const.TimeAI2Waiting);
			}
			else {
				sendPlayerCountDownTurn(PlayerTurn.DOWNPOS);
				yield return new WaitUntil(() => ((m_Move != null || Main.Instance.IsDownAI || m_Winner != PlayerTurn.NULL)&& !m_GamePause) || m_GameRestart);
				if(m_Winner != PlayerTurn.NULL || m_GameRestart) break;
				if(Main.Instance.IsDownAI) {
					Debug.Log("-------------- Computer 2 Turn --------------");
					//m_Move = computerMovesMCST(-1);
					m_Move = computerMoves(-1);
				} 
				else {
					Debug.Log("-------------- Player Turn --------------");
				}
				if(m_Move == null)
				Debug.LogWarning(m_Move[0] );
				Debug.LogWarning(m_Move[1] );
				Debug.LogWarning(m_Move[2] );
				// Debug.LogError(m_Move[0] + " " + m_Move[1] + " "+ m_Move[2]);
				dichuyen(-1, downPlayerPos[m_Move[0]].ToList(), new List<int>(){m_Move[1], m_Move[2]});
				changeBoard(true, m_Move, -1,ref upPlayerPos, ref downPlayerPos);
				sendPlayerCountDownTurn(PlayerTurn.NULL);
				yield return new WaitForSeconds(Const.TimeAI2Waiting);
			}
			_currentTurn++;
		}
	}
	public string Flatten(int[][] e) {
		string result = string.Empty;
		foreach (var pos in e)
		{
			foreach (var value in pos)
			{
				result +=value.ToString();
			}
		}
		return result;
	}
    IEnumerator choLauThe()
    {
        float time = 0;
        yield return new WaitForSeconds(2f);
        while ((_currentTurn) % 2 != 0)
        {
            Debug.Log(Main.Instance.CurrentMode);
            time += 0.2f;
            if ((_currentTurn) % 2 == 0) break;
            else
            {
                if(time > rd.Next(5,8))
                {
                    time = 0f;
                    StuffManager.Instance.playSound(SoundName.AI_CHO_LAU);
                }
                yield return new WaitForSeconds(0.2f);
            }
        }
    }
    public int[][] Shuffle (int[][] array)
    {
		System.Random rng = new System.Random();
        int n = array.Length;
        while (n > 1) 
        {
            int k = rng.Next(n--);
            int[] temp = array[n];
            array[n] = array[k];
            array[k] = temp;
        }
		return array;
    }
	public List<int[]> Shuffle (List<int[]> array)
    {
		System.Random rng = new System.Random();
        int n = array.Count;
        while (n > 1) 
        {
            int k = rng.Next(n--);
            int[] temp = array[n];
            array[n] = array[k];
            array[k] = temp;
        }
		return array;
    }
	private void sendPlayerWin(PlayerTurn player) {
		if(playerWinHanle != null) {
			PlayerTurnArgs e = new PlayerTurnArgs(player);
			playerWinHanle(this, e);
		}
	}
	public event DiChuyenHandle dichuyenHandle;
	public event DestroyElementHandle destroyElementHandle;
	public event SuggestNextMovesHandle suggestNextMovesHandle;
	public event PlayerCountDownTurn playerCountDownTurnHandle;
	public event PlayerWinHanle playerWinHanle; 
	private void sendPlayerCountDownTurn(PlayerTurn player) {
		if(playerCountDownTurnHandle != null) {
			PlayerTurnArgs e = new PlayerTurnArgs(player);
			playerCountDownTurnHandle(this, e);
		}
	}
	private void ReceivePlayerWinSingal(object sender, EventArgs e) {
		var args = (PlayerTurnArgs) e;
		m_Winner = args.PlayerTurn;
		sendPlayerWin(m_Winner);
		UpdateScore();
	}
	private void UpdateScore() {
		if(Main.Instance.CurrentMode == Mode.PLAYER_AI) {
			m_PlayerScore += 10 * (8 - upPlayerPos.Length);
			Debug.LogWarning("Update Score! ");
			if(m_Winner == PlayerTurn.DOWNPOS) m_PlayerScore += 100;
		} 
	}
	private void suggestNextMoves(object sender, EventArgs e) {
		var args = (SendElementChoiceArgs) e;
		if(suggestNextMovesHandle != null) {
            Debug.LogError((object)args.PlayerTurn);
			SuggestNextMoveArgs ev = new  SuggestNextMoveArgs(args.PlayerTurn, suggestNextMoves(args.PlayerTurn, args.Move, upPlayerPos, downPlayerPos, args.Index));
			suggestNextMovesHandle(this, ev);
		}
	}
	private void dichuyen(int _playerTurn, List<int> _sourcePos, List<int> _directionPos) {
		if(dichuyenHandle != null) {
			// Debug.LogError(_playerTurn);
			DichuyenEventArgs e = new DichuyenEventArgs(_playerTurn, _sourcePos, _directionPos);
			dichuyenHandle(this, e);
		}
	}
	
	private void destroyElement(int p, int[] _element) {
		if(Main.Instance.CurrentMode == Mode.PLAYER_AI) {
			if(p == -1) {
				m_PlayerScore += m_ScorePerKill;
			}
		}
		if(destroyElementHandle != null) {
			DestroyElementArgs e = new DestroyElementArgs(p, _element);
			destroyElementHandle(this, e);
		}
	}

	public delegate void DiChuyenHandle(object sender, EventArgs e);
	public delegate void DestroyElementHandle(object sender, EventArgs e);
	public delegate void SuggestNextMovesHandle(object sender, EventArgs e);
	public delegate void PlayerCountDownTurn(object sende, EventArgs e);
	public delegate void PlayerWinHanle(object sende, EventArgs e);

}


public class SendElementChoiceArgs : EventArgs{
	private int[] move;
	private PlayerTurn playerTurn;
	private int index;
	public SendElementChoiceArgs(PlayerTurn _playerTurn, int _index, int[] _move){
		playerTurn = _playerTurn;
		index = _index;
		move = _move;
	}
	public int[] Move{get{return move;}}
	public PlayerTurn PlayerTurn{get{return playerTurn;}}
	public int Index {get{return index;}}
}
public class DichuyenEventArgs: EventArgs {
	private int playerTurn;
	private List<int> sourcePos;
	private List<int> directionPos;
	public DichuyenEventArgs(int _playerTurn, List<int> _sourcePos, List<int> _directionPos) {
		playerTurn = _playerTurn;
		sourcePos = _sourcePos;
		directionPos = _directionPos;
	}
	public int PlayerTurn {get{return playerTurn;}}
	public List<int> SourcePos {get{return sourcePos;}}
	public List<int> DirectionPos {get{return directionPos;}}
}
public class SuggestNextMoveArgs : EventArgs {
	private PlayerTurn playerTurn;
	private List<List<int>> nextMoves;
	public SuggestNextMoveArgs(PlayerTurn _playerTurn, List<List<int>> _nextMoves){
		playerTurn = _playerTurn;
		nextMoves = _nextMoves;
	}
	public PlayerTurn PlayerTurn{get{return playerTurn;}}
	public List<List<int>> NextMoves{get{return nextMoves;}}
}
public class ChoicedNextMoveArgs: EventArgs {
	private PlayerTurn playerTurn;
	private int[] move;
	public ChoicedNextMoveArgs(PlayerTurn _playerTurn, int[] _move) {
		move = _move;
		playerTurn = _playerTurn;
	}
	public PlayerTurn PlayerTurn {get{return playerTurn;}}
	public int[] Move{get{return move;}}
}
public class DestroyElementArgs:EventArgs {
	private int[] element;
	private int p;
	public DestroyElementArgs(int _p, int[] _element){
		element = _element;
		p = _p;
	}
	public int[] Element{get{return element;}}
	public int P {get{return p;}}
}

public class PlayerTurnArgs: EventArgs {
	private PlayerTurn playerTurn;
	private int[] move;
	public PlayerTurnArgs(PlayerTurn _playerTurn) {
		playerTurn = _playerTurn;
	}
	public PlayerTurn PlayerTurn {get{return playerTurn;}}
}



static class RandomExtensions
{
    public static void Shuffle<T> (this System.Random rng, T[] array)
    {
        int n = array.Length;
        while (n > 1) 
        {
            int k = rng.Next(n--);
            T temp = array[n];
            array[n] = array[k];
            array[k] = temp;
        }
    }
}
