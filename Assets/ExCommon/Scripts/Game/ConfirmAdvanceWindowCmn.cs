using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class ConfirmAdvanceWindowCmn : ConfirmWindowCmn {

    [SerializeField]
    public Text ConfirmTxt;

    [SerializeField]
    public Text InfoTxt;

    [SerializeField]
	public Text[] InputNumberTxt;

    [SerializeField]
    public GameObject NumberBoard;
	
	[SerializeField]
	public int[] Coefficient;

    [SerializeField]
    public int MinNum = 0;

    [SerializeField]
    public int MaxNum;

	[SerializeField]
	public Text UnitCostTxt;

	[SerializeField]
	public Text[] NowNumTxt;

    [System.NonSerialized]
	public int InputNum = 0;
	
	public static ConfirmAdvanceWindowCmn Singleton;

	new public void Awake(){
		base.Awake ();
		Singleton = this;
	}

    public void OnEnable(){
        updNumText();
    }

    public static ConfirmAdvanceWindowCmn show( GameObject prefab, string txt,  Transform pare, System.Action<object> call, System.Action<object> cansel_call ){
        ConfirmWindowCmn baseMng = ConfirmWindowCmn.show( prefab, txt, pare, call, cansel_call );
		return baseMng.GetComponent<ConfirmAdvanceWindowCmn>();
	}

    /// <summary>
    /// アイテムウインドウ表示
    /// </summary>
    /// <param name="text"></param>
    /// <param name="maxNum"></param>
    /// <param name="unitCost"></param>
    /// <param name="callback"></param>
    public void show( string text, string confirmText, int maxNum, int unitCost, int nowNum, System.Action<object> callback, System.Action<object> cansel_call = null ) {
        showNumberBoard(true);
        Coefficient = new int[] { 1, unitCost };
        MinNum = 1;
        MaxNum = maxNum;
        setNum(MinNum);
        setNowNum(nowNum);
        UnitCostTxt.text = unitCost.ToString();
        ConfirmTxt.text = confirmText;
        show(text, callback);
    }
	
	public void pushAddNum(int add = 1){
		CmnBaseProcessMng.playCursolSe ();
		setNum( InputNum + add );
	}
	
	public void pushSubNum( int sub = 1 ){
		CmnBaseProcessMng.playCursolSe ();
		setNum( InputNum - sub );
	}
	
	private void setNum( int num ){
		InputNum = Mathf.Clamp( num, MinNum, MaxNum );
        updNumText();
	}

    /// <summary>
    /// 現在の所持数設定
    /// </summary>
    /// <param name="num"></param>
    public void setNowNum( int num ) {
        for (int i = 0; i < NowNumTxt.Length; i++) {
            NowNumTxt[i].text = num.ToString();
        }
    }

    private void updNumText()
    {
        for (int i = 0; i < InputNumberTxt.Length; i++){
            int n = InputNum;
            if (i < Coefficient.Length){
                n = n * Coefficient[i];
            }
            InputNumberTxt[i].text = n.ToString();
        }
    }

    public void showNumberBoard( bool show){
        NumberBoard.SetActive(show);
        InfoTxt.gameObject.SetActive(!show);
    }



}
