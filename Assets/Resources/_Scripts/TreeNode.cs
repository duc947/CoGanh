using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

public class TreeNode {
	static System.Random r = new System.Random();
	//static int nActions = 5;
	static double epsilon = 1e-6;
    public int m_ScorePerKill = 10;
    public int m_PlayerScore = 0;
	static double Cp = 2*(1/Math.Sqrt(2));
	public int nodePlayerTurn;
	public TreeNode[] children;
	public float nVisits, totValue;
    //public Environment gameState;
    public int[][] nodeUpPlayerPos/* player 1*/, nodeDownPlayerPos/* player 2*/,
			 moveDiagonalRightDown, moveDiagonalRightUp, 
			 moveDiagonalLeftDown, moveDiagonalLeftUp;
    public int[][][] totalWayToMove;
    public int[] mapSize = new int[2]{5,5};//(Y, X)
	//public int[][] UpPlayerPos {get; set;}
	//public int[][] DownPlayerPos {get; set;}
	public int[] lastMove = null;
	public int x = 0;
	public int y = 0;

	public TreeNode(int playerTurn, int[][] upPlayerPos, int[][] downPlayerPos) {
        nodePlayerTurn = playerTurn;
        nodeUpPlayerPos = upPlayerPos;
        nodeDownPlayerPos = downPlayerPos;
	}

    public PlayerTurn gameComplete(int[][] nodeUpPlayerPos, int[][] nodeDownPlayerPos) {
		if(nodeUpPlayerPos.Length ==0) {
			return PlayerTurn.DOWNPOS;
		}
		if(nodeDownPlayerPos.Length == 0) {
			return PlayerTurn.UPPOS;
		}
		return PlayerTurn.NULL;
	}

	public void selectAction() {
		LinkedList<TreeNode> visited = new LinkedList<TreeNode>();
		TreeNode cur = this;
        //int dem = 0;
		visited.AddLast(this);   //visited.add(this);
		while (!cur.isLeaf()) {
			cur = cur.select();
			// System.out.println("Adding: " + cur);
			visited.AddLast(cur); //visited.add(cur);
            //Debug.Log ("keep playing" + dem);
            //dem++;
		}
		if (cur.gameComplete(cur.nodeUpPlayerPos, cur.nodeDownPlayerPos) == PlayerTurn.NULL)  // non-terminal state
		{
			cur.expand();
			TreeNode newNode = cur.select();
			if(newNode != null) {
				visited.AddLast(newNode); //visited.add(newNode);
				float value = rollOut(newNode);
                Debug.Log ("value" + value);
				foreach (TreeNode node in visited) {
					node.updateStats(value);
				}
			}
			else Debug.LogError ("select action selected a null node");
		}
		else {
			float value = 0f;
			switch(cur.gameComplete(cur.nodeUpPlayerPos, cur.nodeDownPlayerPos)){ 
			case PlayerTurn.UPPOS : // pc have a winner
				value = 1.0f; 
				break;
			case PlayerTurn.DOWNPOS : // player win
				value = 0.0f; 
				break;  
			default : /* keep playing */ 
				//Debug.Log ("keep playing");

				break; 
			}
			foreach (TreeNode node in visited) {
				node.updateStats(value);
				//value = value * -1;
			}
		}
        // TreeNode cur = this;
        // var moves = simpleMoves(cur.nodePlayerTurn, cur.nodeUpPlayerPos, cur.nodeDownPlayerPos);
        // int[][] am = moves.Select(item => item.ToArray()).ToArray();
        // cur.lastMove = am[1];
	}

	

	public void expand(/*int[][] upPlayerPos, int[][] downPlayerPos */) {
		Debug.Log("expand()");
		//children = new TreeNode[nActions];

        TreeNode cure = this;
        var moves = simpleMoves(cure.nodePlayerTurn, cure.nodeUpPlayerPos, cure.nodeDownPlayerPos);
        int[][] am = moves.Select(item => item.ToArray()).ToArray();

			Debug.LogWarning(showInfor(am));
			// int[][] upPlayerPosTemp = (int[][]) cure.nodeUpPlayerPos.Select(i => i.ToArray()).ToArray();
			// int[][] downPlayerPosTemp = (int[][]) cure.nodeDownPlayerPos.Select(i => i.ToArray()).ToArray();
			// Debug.Log("nodePlayerTurn " + (cure.nodePlayerTurn));
			// Debug.Log("upPlayerPosTemp " + Flatten(upPlayerPosTemp));
			// Debug.Log("downPlayerPosTemp " + Flatten(downPlayerPosTemp));
			// if (cure.lastMove != null)
			// {
			// 	Debug.Log("cure.lastMove[0] " + cure.lastMove[0] +"cure.lastMove[1] " + cure.lastMove[1] + "cure.lastMove[2] " + cure.lastMove[2]);
			// }

		if (moves.Count > 0) {
            Debug.Log("moves.Count" + moves.Count);
			children = new TreeNode[moves.Count];

				// int tempturn = cure.nodePlayerTurn;
				// int [][] tempDownpos = (int[][]) cure.nodeDownPlayerPos.Clone();
				// int [][] tempUppos = (int[][]) cure.nodeUpPlayerPos.Clone();
				// children[0] = new TreeNode(-tempturn, tempUppos, tempDownpos);
				// changeBoard(true, am[0], -children[0].nodePlayerTurn,ref children[0].nodeUpPlayerPos,ref children[0].nodeDownPlayerPos);
				// children[0].lastMove = am[0];


				// int[][] upPlayerPosTemptaa = (int[][]) cure.nodeDownPlayerPos.Select(k => k.ToArray()).ToArray();
				// int[][] downPlayerPosTemptaa = (int[][]) cure.nodeUpPlayerPos.Select(k => k.ToArray()).ToArray();
				// Debug.Log("upPlayerPosTemptchild " + Flatten(upPlayerPosTemptaa));
				// Debug.Log("downPlayerPosTemptchild " + Flatten(downPlayerPosTemptaa));



			int[][][] tempUpposar = new int[moves.Count][][];
			int[][][] tempDownposar = new int[moves.Count][][];
			for (int i = 0; i < moves.Count; i++)
			{
				tempUpposar[i] = (int[][]) cure.nodeUpPlayerPos.Clone();
				tempDownposar[i] = (int[][]) cure.nodeDownPlayerPos.Clone();
				// int[][] upPlayerPosTemptaa = (int[][]) tempUpposar[i].Select(j => j.ToArray()).ToArray();
				// int[][] downPlayerPosTemptaa = (int[][]) tempDownposar[i].Select(j => j.ToArray()).ToArray();
				//int[][] lastPlayerPosTemp = (int[]) cure.lastMove).ToArray();
				// //Debug.Log("nodePlayerTurnchild " + (children[i].nodePlayerTurn));
				// Debug.Log("upPlayerPosTemptaa " + Flatten(upPlayerPosTemptaa));
				// Debug.Log("downPlayerPosTemptaa " + Flatten(downPlayerPosTemptaa));
			}
				
			
			for(int i=0; i < moves.Count; i++) {
				// var tempturn = cure.nodePlayerTurn;
				// var tempUppos = tempUpposar[i];
				// var tempDownpos = tempDownposar[i];
				
				// for (int j = 0; j < moves.Count; j++)
				// {
				// 	int[][] upPlayerPosTemptaa = (int[][]) tempUpposar[j].Select(k => k.ToArray()).ToArray();
				// 	int[][] downPlayerPosTemptaa = (int[][]) tempDownposar[j].Select(k => k.ToArray()).ToArray();
				// 	Debug.Log("upPlayerPosTemptchild " + Flatten(upPlayerPosTemptaa));
				// 	Debug.Log("downPlayerPosTemptchild " + Flatten(downPlayerPosTemptaa));
				// }

				children[i] = new TreeNode(-cure.nodePlayerTurn, tempUpposar[i], tempDownposar[i]);
                changeBoard(true, am[i], -children[i].nodePlayerTurn,ref tempUpposar[i],ref tempDownposar[i]);
				children[i].lastMove = am[i];
			}
		}
	}
	
	private TreeNode select() {
		Debug.Log("select()");
        TreeNode cur = this;
		TreeNode selected = null;
		double bestValue = Double.MinValue;  
		foreach (TreeNode c in cur.children) {
			double uctValue =
				c.totValue / (c.nVisits + epsilon ) +
					Cp * Math.Sqrt(2 * Math.Log(cur.nVisits + 1) / (c.nVisits + epsilon) ) +
					r.NextDouble() * epsilon;


//			c.totValue / (c.nVisits + epsilon) +
//				Math.Sqrt(Math.Log(nVisits+1) / (c.nVisits + epsilon)) +
//					r.NextDouble() * epsilon;

			// small random number to break ties randomly in unexpanded nodes
//			Debug.Log("UCT value = " + uctValue);
			if (uctValue > bestValue) {
				selected = c;
				bestValue = uctValue;
			}
		}
		// System.out.println("Returning: " + selected);
		return selected;
	}

	public bool isLeaf() {
		//Debug.Log("isLeaf()");
        TreeNode cur = this;
		return cur.children == null;

	}
	
	public float rollOut(TreeNode tn) {
		Debug.Log("rollOut()");
		TreeNode rollGS = new TreeNode(tn.nodePlayerTurn, tn.nodeUpPlayerPos, tn.nodeDownPlayerPos);
		bool stillPlaying = true;
		float rc = 0;
		int move;
        System.Random rr = new System.Random();
		//Debug.Log ("Start rollout .............................");
        var moves = simpleMoves(rollGS.nodePlayerTurn, rollGS.nodeUpPlayerPos, rollGS.nodeDownPlayerPos);
        int[][] am = moves.Select(item => item.ToArray()).ToArray();
        move = rr.Next(moves.Count);
		while ((moves.Count > 0) && stillPlaying) {
			//rollGS.makeMove(am[move]);  // make a random move
            //Debug.Log ("game move = " + am[move][0]);
            changeBoard(true, am[move], rollGS.nodePlayerTurn,ref rollGS.nodeUpPlayerPos,ref rollGS.nodeDownPlayerPos);
			//Debug.Log ("game = " + rollGS.ToString());
			switch(gameComplete(rollGS.nodeUpPlayerPos, rollGS.nodeDownPlayerPos)){ 
			case PlayerTurn.UPPOS : // pc have a winner
				//Debug.Log("we have a winner");
				stillPlaying = false; 
				rc = 1.0f; 
				break;
			case PlayerTurn.DOWNPOS : // player have a winner
				//Debug.Log ("lose");
				stillPlaying = false;
				rc = 0.0f; 
				break;  
			default : /* keep playing */ 
				//Debug.Log ("keep playing");
                var newmoves = rollGS.simpleMoves(-rollGS.nodePlayerTurn, rollGS.nodeUpPlayerPos, rollGS.nodeDownPlayerPos);
				am = newmoves.Select(item => item.ToArray()).ToArray();
                move = rr.Next(newmoves.Count);
                rollGS.nodePlayerTurn = -rollGS.nodePlayerTurn;
				break; 
			}
			//Debug.Log ("after switch, time to test while");
		}
		Debug.Log ("end of rollOut");
		return rc; //return r.nextInt(2);
	}
	
	public void updateStats(float value) {
		//Debug.Log("updateStats()");
		nVisits++;
		totValue += value;
	}
	
	public TreeNode bestChild() {
        TreeNode cur = this;
		float smallestScore = float.MinValue;
        TreeNode chosenNode = null;

        foreach (TreeNode child in cur.children)
        {
            //Choose the child with the best score, if multiple children have the best score, then choose the one with the least children
            if ((child.totValue / child.nVisits + epsilon)/*child.AverageScore */ > smallestScore || (child.totValue / child.nVisits + epsilon)/*child.AverageScore */ == smallestScore /*&& chosenNode.children.Length > child.children.Length */)
            {
                smallestScore = child.AverageScore;
                chosenNode = child;
            }
        }
        return chosenNode;
	}

	public float AverageScore
        {
            get { return (nVisits == 0? 0 : totValue / nVisits); }
        }

    public List<List<int>> simpleMoves(int playerTurn, int[][] upPlayerPos, int[][] downPlayerPos){

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
		var result = new List<List<int>>();
		// Result item: 0 => chọn cách đi nào
					//  1, 2 => [x, y] đi đến đâu
					//  3 => chỉ di chuyển hay ăn đối phương, 0: chỉ di chuyển, 1 là ăn đối phương.
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

    protected bool freeSpace(int[] check, int[][] upPlayerPos, int[][] downPlayerPos) {
		if(Array.FindAll(check, item => item > -1).Length == check.Length && Array.FindAll(check, item => item < 5).Length == check.Length) {
			if(Array.FindIndex(upPlayerPos, item =>  Enumerable.SequenceEqual(item, check)) == -1 && 
			Array.FindIndex(downPlayerPos, item =>  Enumerable.SequenceEqual(item, check)) == -1) {
				return true;
			}
		}
		return false;
	}
    public static int[] SubArray(int[] data, int index, int length)
	{
		int[] result = new int[length];
		Array.Copy(data, index, result, 0, length);
		return result;
	}
	public void checkKill(bool isTest,int[] x, ref int[][] upPlayerPos,ref int[][] downPlayerPos, ref int[][] sourceChange, int p) {
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
	public void checkKill(bool isTest, int[] x, int playerTurn,ref int[][] upPlayerPos,ref int[][] downPlayerPos) {
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
	public void changeBoard(bool isTest, int[] move, int playerTurn,ref int[][] upPlayerPos,ref int[][] downPlayerPos) {
        //Debug.Log("Change Board !" + move[0]);
        //Debug.Log("Change Board 0: " + upPlayerPos[move[0]]);
        
        //Debug.Log("Change Board 1: " + upPlayerPos[move[1]]);
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
			if(index != -1) {
				// Debug.LogError(itemRemove[0] + " " + itemRemove[1]);
			}
		}
		else {
			checkKill(isTest, activePlayerNextMove, playerTurn,ref upPlayerPos,ref downPlayerPos);
		}
	}
	public void destroyElement(int p, int[] _element) {
		if(Main.Instance.CurrentMode == Mode.PLAYER_AI) {
			if(p == -1) {
				m_PlayerScore += m_ScorePerKill;
			}
		}
		// if(destroyElementHandle != null) {
		// 	DestroyElementArgs e = new DestroyElementArgs(p, _element);
		// 	destroyElementHandle(this, e);
		// }
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

    public event DestroyElementHandle destroyElementHandle;

    public delegate void DestroyElementHandle(object sender, EventArgs e);
	
}