using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
public class UIManager : MonoBehaviour {
    public GameObject Gird;
    public Transform[] m_PlayerPos;
    private List<List<Transform>> mapMatrix;
    private List<List<Transform>> m_Elements;
    public Sprite m_PlayerDownSprite, m_PlayerUpSprite;
    private List<GameObject> m_SuggestNextMovesObj;
    [SerializeField] GameObject m_MatchResultPanel, m_LosePrefab, m_WinPrefab;
    private GameObject m_MainUIResult;

    [SerializeField]
    private Transform m_PlayerUpIcon, m_PlayerDownIcon, m_PlayerUpScore,  m_PlayerDownScore;
    
    public GameObject m_Element, m_SuggestDownElement, m_SuggestUpElement, m_ScoreElement;
    [SerializeField]
    private Vector3 minPoint = new Vector3(500, 300, 0);
    /// <summary>
    /// Start is called on the frame when a script is enabled just before
    /// any of the Update methods is called the first time.
    /// </summary>
    /// <summary>
    /// Awake is called when the script instance is being loaded.
    /// </summary>
    void Awake()
    {
        
    }
    IEnumerator hieuUngCongDiem(Transform quanCo, Transform endPoint) {
        float count = 0.0f;
        int dem = 0;
        Vector3 startPoint = quanCo.position;
        Vector3 endPointVector3 = endPoint.position;
        var image = quanCo.GetComponent<Image>();
        while (count < 1.0f) {
            iTween.RotateBy(quanCo.gameObject, iTween.Hash(
                     "z", 13f,
                     "time", 3f,
                     "easetype", "linear",
                     "looptype", iTween.LoopType.loop
                 ));
            yield return new WaitForFixedUpdate();
            dem++;
            Vector3 m1 = Vector3.Lerp(startPoint, startPoint + minPoint, count );
            Vector3 m2 = Vector3.Lerp(startPoint + minPoint, endPointVector3, count );
            image.color = new Color(image.color.r, image.color.g, image.color.b, 1.3f - count > 1f ? 1f : 1.3f - count);
            quanCo.localScale = new Vector3(1.3f - count, 1.3f - count, 1 - count);
            count += 1.0f *Time.deltaTime*dem/10;
            quanCo.position = Vector3.Lerp(m1, m2, count);
        }
        Destroy(quanCo.gameObject);
        endPoint.GetComponent<Text>().text = (int.Parse(endPoint.GetComponent<Text>().text) + 1).ToString();
        endPoint.GetComponent<Animator>().Play("TextOut");
    }
    void Start()
    {
        m_MatchResultPanel.transform.parent.gameObject.SetActive(false);
        //test();
        addHandle();
        int dem = -1;
        mapMatrix = new List<List<Transform>>();
        m_Elements = new List<List<Transform>>();
        foreach (Transform item in Gird.transform)
        {
            dem++;
            if(dem % Main.Instance.Env.mapSize[1] == 0) {
                mapMatrix.Add(new List<Transform>());
            }
            mapMatrix[mapMatrix.Count - 1].Add(item);
        }
        Debug.Log(dem + " " + mapMatrix[0].Count + "x" + mapMatrix.Count);
        initMap();
    }
                                                            
    public void gameSestart() {
        foreach (var item in m_Elements)
        {
            if(item != null)
                foreach (var i in item)
                {
                    if(i != null) Destroy(i.gameObject);
                }
        }
        m_Elements.Clear();
        m_Elements = new List<List<Transform>>();
        initMap();
    }
    void addHandle() {
        Main.Instance.Env.destroyElementHandle += destroyElement;
        Main.Instance.Env.dichuyenHandle += diChuyenQuanCo;
        Main.Instance.Env.suggestNextMovesHandle += suggestNextMoves;
        Main.Instance.Env.playerWinHanle += PlayerWin;
    }
    void PlayerWin(object sender, EventArgs e) {
        Debug.LogError("Vao player Win!");
        PlayerTurnArgs args = (PlayerTurnArgs) e;
        var playerTurn = args.PlayerTurn;
        m_MatchResultPanel.transform.parent.gameObject.SetActive(true);
        if(Main.Instance.CurrentMode == Mode.PLAYER_AI) {
            if(playerTurn == PlayerTurn.DOWNPOS) {
                m_MainUIResult = GameObject.Instantiate(m_WinPrefab, m_MatchResultPanel.transform.position, Quaternion.identity, m_MatchResultPanel.transform);
                m_MainUIResult.GetComponent<Animator>().Play("WinIdle");
                StuffManager.Instance.playSound(SoundName.THANG);
            }
            else {
                m_MainUIResult = GameObject.Instantiate(m_LosePrefab, m_MatchResultPanel.transform.position, Quaternion.identity, m_MatchResultPanel.transform);
                m_MainUIResult.GetComponent<Animator>().Play("YouLoseAnimation");
                StuffManager.Instance.playSound(SoundName.THUA);
            }
        }
        else {
            m_MainUIResult = GameObject.Instantiate(m_WinPrefab, m_MatchResultPanel.transform.position, Quaternion.identity, m_MatchResultPanel.transform);
            switch (playerTurn)
            {
                case PlayerTurn.DOWNPOS:
                    m_MainUIResult.GetComponent<WinLoseText>().setMainText("PLAYER DOWN WIN");
                    break;
                case PlayerTurn.UPPOS:
                    m_MainUIResult.GetComponent<WinLoseText>().setMainText("PLAYER UP WIN");
                    break;
                default:
                    break;
            }
            
            m_MainUIResult.GetComponent<Animator>().Play("WinIdle");
            StuffManager.Instance.playSound(SoundName.THANG);
        }
    }
    void removeHandle() {
        Main.Instance.Env.destroyElementHandle -= destroyElement;
        Main.Instance.Env.dichuyenHandle -= diChuyenQuanCo;
        Main.Instance.Env.suggestNextMovesHandle -= suggestNextMoves;
    }
    public void initMap() {
        m_PlayerUpScore.GetComponent<Text>().text = "0";
        m_PlayerDownScore.GetComponent<Text>().text = "0";
        m_MatchResultPanel.transform.parent.gameObject.SetActive(false);
        if(m_MainUIResult != null) Destroy(m_MainUIResult);
        GameObject canvas = GameObject.Find("Content");
        if(canvas == null) {Debug.LogError("not Found Canvas!"); return;}
        int[] temp;
        int index = -1;
        for (int i = 0; i < Main.Instance.Env.mapSize[0]; i++)
        {
            m_Elements.Add(new List<Transform>());
            for (int j = 0; j < Main.Instance.Env.mapSize[1]; j++)
            {
                temp = new int[]{i, j};
                index = Array.FindIndex(Main.Instance.Env.DownPlayerPos, item =>  Enumerable.SequenceEqual(item, temp));
                if( index != -1) {
                    var newGame = GameObject.Instantiate(m_Element, mapMatrix[2][2].position,Quaternion.identity, mapMatrix[i][j]);
                    iTween.MoveTo(newGame, mapMatrix[i][j].position, Const.timeMoveElement /2);
                    newGame.GetComponent<Element>().setSide(PlayerTurn.DOWNPOS, temp, m_PlayerDownSprite, index);
                    newGame.transform.SetAsLastSibling();
                    m_Elements[m_Elements.Count - 1].Add(newGame.transform);
                    continue;
                }
                index = Array.FindIndex(Main.Instance.Env.UpPlayerPos, item =>  Enumerable.SequenceEqual(item, temp));
                if(index != -1) {
                    var newGame = GameObject.Instantiate(m_Element, mapMatrix[2][2].position,
                    Quaternion.identity, 
                    mapMatrix[i][j]);
                    iTween.MoveTo(newGame, mapMatrix[i][j].position, Const.timeMoveElement /2);
                    newGame.GetComponent<Element>().setSide(PlayerTurn.UPPOS, temp, m_PlayerUpSprite, index);
                    newGame.transform.SetAsLastSibling();

                    m_Elements[m_Elements.Count - 1].Add(newGame.transform);
                }
                else m_Elements[m_Elements.Count - 1].Add(null);
            }
        }
        
    }
    void OnScaleBack(GameObject obj)
    {
        iTween.ScaleBy(obj, iTween.Hash("x", 95.2380f, "z", 95.2380f, "default", .1));
        Debug.LogError("It still here!");
    }
    private void diChuyenQuanCo(object sender, EventArgs e) {
        var args =e as DichuyenEventArgs;
        var playerTurn = args.PlayerTurn;
		var sourcePos = args.SourcePos;
		var directionPos = args.DirectionPos;
        if(playerTurn == 1)
        {
            StuffManager.Instance.playSound(SoundName.AI_DI_CO);
        }
        if (sourcePos != null && directionPos != null) {
            // Debug.LogError("Dichuyen!");
            m_Elements[directionPos[0]][directionPos[1]] = m_Elements[sourcePos[0]][sourcePos[1]];
            m_Elements[sourcePos[0]][sourcePos[1]] = null;
            m_Elements[directionPos[0]][directionPos[1]].GetComponent<Element>().updatePos(directionPos.ToArray());
            iTween.MoveTo(m_Elements[directionPos[0]][directionPos[1]].gameObject, mapMatrix[directionPos[0]][directionPos[1]].position, Const.timeMoveElement);
            var anim = m_Elements[directionPos[0]][directionPos[1]].gameObject.GetComponent<Animator>();
            if(anim != null)
            {
                anim.SetTrigger("zoom");
                Debug.Log("anim exist!");
            }
            Debug.LogError("It still here!");
        }
        // int[] playerPosChange = 
    }
    private void destroyElement(object sender, EventArgs e) {
        var args = (DestroyElementArgs) e;
        var element = args.Element;
        var p = args.P;
        if(element != null) {
            if(Main.Instance.CurrentMode == Mode.PLAYER_AI) {
			    if(p == -1) {//Player Down
                    Debug.LogError("Ai player 1 an!");

                    // var newScore = GameObject.Instantiate(m_ScoreElement, 
                    // mapMatrix[element[0]][element[1]].position,
                    // Quaternion.identity, mapMatrix[element[0]][element[1]]);
                    // newScore.GetComponent<AddScore>().FadeOut(Main.Instance.Env.ScorePerKill);
                    StuffManager.Instance.playSound(SoundName.AN_CO);

                }
                else {
                    StuffManager.Instance.playSound(SoundName.BI_AN);
                }
            }
            else {
                StuffManager.Instance.playSound(SoundName.AN_CO);
            }
            var canvs = GameObject.Find("Canvas");
            var quancoTemp = GameObject.Instantiate(m_Elements[element[0]][element[1]].gameObject, 
            m_Elements[element[0]][element[1]].position, Quaternion.identity,
            canvs.transform).transform;
            if(p == 1) {
                
                StartCoroutine(hieuUngCongDiem(quancoTemp, m_PlayerUpScore));
            }
            else {
                // m_PlayerUpScore.GetComponent<Text>().text = (int.Parse(m_PlayerUpScore.GetComponent<Text>().text) + 1).ToString();
                // m_PlayerUpScore.GetComponent<Animator>().Play("TextOut");
                StartCoroutine(hieuUngCongDiem(quancoTemp, m_PlayerDownScore));
            }
            Destroy(m_Elements[element[0]][element[1]].gameObject);
            m_Elements[element[0]][element[1]] = null;
            
        }
    }
    private void suggestNextMoves(object sender, EventArgs e) {
        removeAllSuggestNextMovesItem();
        m_SuggestNextMovesObj = new List<GameObject>();
        var args = (SuggestNextMoveArgs) e;
        var playerTurn = args.PlayerTurn;
        // Debug.LogError(playerTurn);
        var nextMoves = args.NextMoves;
        if(nextMoves == null) {
            Debug.LogError("Next Moves wrong!");
            return;
        }
        Debug.Log(nextMoves.Count);
        foreach (var item in nextMoves)
        {
            Debug.Log(item[1] + " "+ item[2]);
            Debug.Log(mapMatrix[item[1]][item[2]].position);
            GameObject newObject;
            if(playerTurn == PlayerTurn.DOWNPOS) {
                newObject = GameObject.Instantiate(m_SuggestDownElement, mapMatrix[item[1]][item[2]].position,Quaternion.identity,  mapMatrix[item[1]][item[2]]);
                newObject.GetComponent<Element>().setSide(playerTurn, item.ToArray(), m_PlayerDownSprite, -1, () => {
                    choicedNextMove(playerTurn, item.ToArray());
                    StuffManager.Instance.playSound(SoundName.DANH_QUAN_CO);
                });
            }
            else {
                newObject = GameObject.Instantiate(m_SuggestUpElement, mapMatrix[item[1]][item[2]].position,Quaternion.identity,  mapMatrix[item[1]][item[2]]);
                newObject.GetComponent<Element>().setSide(playerTurn, item.ToArray(), m_PlayerUpSprite, -1, () => {
                    choicedNextMove(playerTurn, item.ToArray());
                    StuffManager.Instance.playSound(SoundName.DANH_QUAN_CO);
                });
            }
            
            m_SuggestNextMovesObj.Add(newObject);
        }
    }
    void removeAllSuggestNextMovesItem() {
        if(m_SuggestNextMovesObj!=null) {
            foreach (var item in m_SuggestNextMovesObj)
            {
                Destroy(item.gameObject);
            }
            m_SuggestNextMovesObj.Clear();
        }
    }
    public event ChoicedNextMoveHandle choicedNextMoveHandle;
	private void choicedNextMove(PlayerTurn playerTurn, int[] move) {
        Debug.LogWarning("Go to Choiced!");
		if(choicedNextMoveHandle != null) {
			ChoicedNextMoveArgs e = new  ChoicedNextMoveArgs(playerTurn, move);
			choicedNextMoveHandle(this, e);
            removeAllSuggestNextMovesItem();
		}
	}
    public delegate void ChoicedNextMoveHandle(object sender, EventArgs e);
}

