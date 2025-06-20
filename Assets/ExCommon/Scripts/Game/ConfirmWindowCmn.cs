using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class ConfirmWindowCmn : MonoBehaviour {

    private const float WAIT_BUTTON = 0.5f;

    [SerializeField]
    public Text UiTitle;

	[SerializeField]
	public Text UiText;

    [SerializeField]
    public Image Icon;

    [SerializeField]
    public Text IconText;

    [SerializeField]
    public bool CloseToHide;

	[SerializeField]
    public GamePadButtonMng OkBtn;

    [SerializeField]
    public GamePadButtonMng CanselBtn;

    [SerializeField]
    public ScrollRect Scroll;

    public System.Action<object> CallBack{ get; set; }

	public System.Action<object> CallBackCansel{ get; private set; }

    public float verticalNormalized {  set { if (Scroll != null) { Scroll.verticalNormalizedPosition = value; } } }

    public object CallbackParam;

    public static ConfirmWindowCmn Instance{ get; private set; }

	public void Awake(){
        if( Instance != null)
        {
            close(Instance);
        }

		Instance = this;
	}

	public static ConfirmWindowCmn show( GameObject prefab, string txt, Transform pare = null, System.Action<object> call = null, object param = null) {
		GameObject obj = Instantiate( prefab ) as GameObject;
        obj.transform.parent = pare;
        obj.transform.localPosition = new Vector3();
        obj.transform.localScale = prefab.transform.localScale;
        ConfirmWindowCmn mng = obj.GetComponent<ConfirmWindowCmn> ();
        mng.show(txt, call, param);
        if( mng.transform.GetComponentInChildren<GamePadListRecivMng>() == null) {
            GamePadListRecivMng.ActiveGamePadList = null;
        }
        return mng;
	}

    public static ConfirmWindowCmn show( GameObject prefab, string txt, Transform pare, System.Action<object> call, System.Action<object> cansel_call, object param = null) {
		ConfirmWindowCmn mng = show( prefab, txt, pare, call, param );
		mng.CallBackCansel = cansel_call;
		return mng;
	}

    /// <summary>
    /// インスタンスからの表示
    /// </summary>
    /// <param name="txt"></param>
    /// <param name="call"></param>
    public void show(string txt, System.Action<object> call = null, object param = null) {
        CallbackParam = param;
        Init(txt, call);
        initButton(OkBtn);
        initButton(CanselBtn);
        this.gameObject.SetActive(true);
        EventSystem.current.SetSelectedGameObject(this.gameObject);
    }

    /// <summary>
    /// タイトル付きウインドウ表示
    /// </summary>
    /// <param name="prefab"></param>
    /// <param name="title"></param>
    /// <param name="txt"></param>
    /// <returns></returns>
    public static ConfirmWindowCmn show(GameObject prefab, string title, string txt) {
        ConfirmWindowCmn wnd = show(prefab, txt);
        wnd.setText(title, txt);
        return wnd;
    } 

    /// <summary>
    /// 初期設定
    /// </summary>
    public void Init(string txt,System.Action<object> call) {
        setText("", txt);
        CallBack = call;
        //ButtonGuidSetMng.updateImage(CmnConfig.GamePadButton.Decision, OkImgTxt);
        //ButtonGuidSetMng.updateImage(CmnConfig.GamePadButton.Cancel, CanselImgTxt);
        gameObject.SetActive(true);
    }

    public void setText( string title, string detail) {
        if (UiTitle != null) {
            UiTitle.text = title;
        }
        if (UiText != null) {
            UiText.text = detail;
        }
    }

    public void setFullInfo(string title, string detail, string icon, string icon_txt = "") {
        setText(title, detail);
        setIcon(icon);
        setIconText(icon_txt);
    }


    //public void showCancel( bool show ){

        //	Vector2 posi = CanselButton.transform.localPosition;
        //	CanselButton.SetActive (show);
        //	if (show) {
        //		posi.x = -CanselButton.transform.localPosition.x;
        //	}else{
        //		posi.x = 0f;
        //	}

        //	OkButton.transform.localPosition = posi;
        //}

    public void confirmOk(){
        if (CallbackParam == null) {
            confirmOk(this);
        }else{
            confirmOk(CallbackParam);
        }
	}

	public void confirmOk(object obj){

		CmnBaseProcessMng.playClickSe ();

        if (CallBack != null)
        {
            CallBack(obj);
            CallBack = null;
        }
		closeThis ();
	}

	public void confirmCancel(){
		confirmCancel (this);
	}
	public void confirmCancel(object obj){
		
		CmnBaseProcessMng.playCanselSe();

		if (CallBackCansel != null) {
			CallBackCansel (obj);
		}
		CallBackCansel = null;
		closeThis ();
	}


	public void closeThis(  ){
		close (this);
	}

	public static void close(  ){
		close (Instance);
	}

	public static void close( ConfirmWindowCmn mng ){
        if (mng.CloseToHide){
			mng.hideThis();
        }else{
            mng.destroyThis();
        }

        //GamePadListRecivMng.ActiveGamePadList = BackupGamePadReciv;
        //BackupGamePadReciv = null;

        mng.CallBack = null;
	}

	public static void hide(){
		Instance.hideThis ();
	}

    public void hideThis()
    {
        GamePadListRecivMng.returnJustBeforeGamePad();
        this.gameObject.SetActive(false);
    }

    public void destroyThis() {
        GamePadListRecivMng.returnJustBeforeGamePad();
        Destroy(this.gameObject);
    }

    public Sprite setIcon(string path){
        Sprite sp = Resources.Load<Sprite>(path);
        if (Icon != null)
        {
            Icon.sprite = sp;
        }
        return sp;
    }
    public void setIconText( string txt ) {
        
        if (IconText != null) {
            IconText.text = txt;
        }
    }

    /// <summary>
    /// テキストポジションリセット
    /// </summary>
    public void initTextPosition() {
        StartCoroutine(initTextPositionProgress());
    }

    public void changeButtonType(bool isHold) {
        if (OkBtn != null) {
            OkBtn.changeHoldType(isHold);
        }
        if (CanselBtn != null) {
            CanselBtn.changeHoldType(isHold);
        }
    }

    private void initButton(GamePadButtonMng btn) {
        if (btn != null) {
            btn.gameObject.SetActive(false);
            TimeInvokeMng.TimerAction(this.gameObject, () => { btn.gameObject.SetActive(true); }, WAIT_BUTTON);
        }
    }

    private IEnumerator initTextPositionProgress() {
        yield return new WaitForEndOfFrame();
        UiText.transform.localPosition = Vector2.zero;
    }

}
