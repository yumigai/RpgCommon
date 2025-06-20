//#define USE_INPUT_SYSTEM //InputSystemを使用しない場合、削除する

using UnityEngine;
using System.Collections;

#if USE_INPUT_SYSTEM
using UnityEngine.InputSystem;
#endif

public class CmnBaseProcessMng : MonoBehaviour {

	/// <summary>
	/// 定期処理タイプ
	/// </summary>
	private enum LOOP_PROCESS
    {
		SHORT,
		MIDDLE,
		LONG,
		ALL
    }

	/// <summary>
	/// 定期処理時間
	/// </summary>
	private readonly float[] PROCESS_TIMES = new float[(int)LOOP_PROCESS.ALL] {
		5,
		20,
		60
	};

	///// <summary>
	///// 定期処理（短：5秒）
	///// </summary>
	//private const float PROCESS_SHORT = 5;

	///// <summary>
	///// 定期処理（中：20秒）
	///// </summary>
	//private const float PROCESS_MIDDLE = 20;

	///// <summary>
	///// 定期処理（長い：60秒）
	///// </summary>
	//private const float PROCESS_LONG = 60;

	private bool ShowDialog { get; set; }

	[SerializeField]
	public AudioClip ButtonClickSe;

	[SerializeField]
	public AudioClip CanselClickSe;

	[SerializeField]
	public AudioClip ErrorSe;

	[SerializeField]
	public AudioClip SpecialSe;

	[SerializeField]
	public AudioClip[] CursolSe;

	[SerializeField]
	public GameObject ConfirmWindow;

	[SerializeField]
	public GameObject MessageWindow;

	[SerializeField]
	public GameObject LongMessageWindow;

	[SerializeField]
	public EffectControllMng Common2DEffect;

	//[SerializeField]
	//public GameObject LongMessageWindow;

	private float[] RestProcessTime = new float[(int)LOOP_PROCESS.ALL];

	private System.Action[] CallbackProcess = new System.Action[(int)LOOP_PROCESS.ALL];

	public static bool IsGamePad = false;

	public static CmnBaseProcessMng Singleton;

	private static bool InitLoading { get; set; }

	/// <summary>
	/// ポーズ状態かどうか
	/// </summary>
	public static bool IsPause { get { return Time.timeScale == 0; } }

	protected void Awake() {

		Singleton = this;

		Application.targetFrameRate = 60;

		UpdateGamePadStatus();

#if USE_INPUT_SYSTEM
		InputSystem.onDeviceChange += onDeviceChange;
#endif

		DontDestroyOnLoad(this.gameObject);
	}

    protected void OnDestroy() {
#if USE_INPUT_SYSTEM
		InputSystem.onDeviceChange -= onDeviceChange;
#endif
	}

    public void Start() {
        
    }

    public void Update() {
		regularlyProcess();
	}

#if USE_INPUT_SYSTEM
	public void onDeviceChange(InputDevice device, InputDeviceChange change) {
		if(device is Gamepad) {
			UpdateGamePadStatus();
        }
	}
#endif

	/// <summary>
	/// 定期的な処理
	/// </summary>
	private void regularlyProcess() {

		var time = Time.fixedDeltaTime;

		for(int i = 0; i < (int)LOOP_PROCESS.ALL; i++) {
			RestProcessTime[i] -= time;
			if(RestProcessTime[i] <= 0) {

				//５秒ごとにコントローラー状態更新
				if(i == (int)LOOP_PROCESS.SHORT) {
					UpdateGamePadStatus();
                }

				if (CallbackProcess[i] != null) {
					CallbackProcess[i]();
				}
			}
        }
	}

	/// <summary>
	/// ゲームパッドの状態更新
	/// </summary>
	void UpdateGamePadStatus() {
		string[] controller_names = Input.GetJoystickNames();
		IsGamePad = System.Array.Exists(controller_names, it => it.Length > 0);
	}


    

    //private void intputEscape()
    //{
    //    #if UNITY_ANDROID
    //            if (!ShowDialog)
    //            {
    //                ShowDialog = true;

    //                string dialog_message = "アプリを終了しますか？";
    //                if (CmnSaveProc.Conf.SelectLang == (int)CmnSaveProc.GameConfig.LANG.JP)
    //                {
    //                    DialogManager.Instance.SetLabel("はい", "いいえ", "CLOSE");
    //                }
    //                else
    //                {
    //                    DialogManager.Instance.SetLabel("Yes", "No", "CLOSE");
    //                    dialog_message = "exit game?";
    //                }
    //                DialogManager.Instance.ShowSelectDialog(
    //                    dialog_message,
    //                    (bool result) => {
    //                        if (result)
    //                        {
    //                            Application.Quit();
    //                        }
    //                    });
    //                StartCoroutine(reShowDialog());
    //            }
    //    #endif
    //}

    IEnumerator reShowDialog()
    {
        yield return new WaitForSeconds(1f);
        ShowDialog = false;
    }

	public static void waitOneFrame(System.Action callback) {
		Singleton.StartCoroutine(Singleton.waitOneFrameProcess(callback));
	}

	public IEnumerator waitOneFrameProcess( System.Action callback ) {
		yield return new WaitForEndOfFrame();
        if (callback != null) {
			callback();
		}
	}


    public static void playClickSe()
		{
			SoundMng.Instance.playSE( Singleton.ButtonClickSe );
		}

		public static void playCanselSe()
		{
			SoundMng.Instance.playSE(Singleton.CanselClickSe);
		}

		public static void playErrorSe()
		{
			SoundMng.Instance.playSE(Singleton.ErrorSe);
		}

    public static void playSpecialSe()
    {
        SoundMng.Instance.playSE(Singleton.SpecialSe);
    }

	public static void playCursolSe(int index = 0){
		SoundMng.Instance.playSE (Singleton.CursolSe [index]);
	}

		public static void toTitle()
		{
		loadScene(CmnConst.SCENE.TitleScene);
		}

	public static void toMain(){
		loadScene(CmnConst.SCENE.MainGame);
	}

	public static void toConfig(){
		loadScene(CmnConst.SCENE.ConfigScene);
	}
	public static void toAlbum(){
		loadScene (CmnConst.SCENE.CollectionScene);
	}
	public static void toReady(){
		loadScene (CmnConst.SCENE.ReadyScene);
	}
	public static void toStory(){
		SceneManagerWrap.LoadScene (CmnConst.SCENE.StoryScene, false);
	}

	public static void loadScene( CmnConst.SCENE scene ){
		SceneManagerWrap.LoadScene (scene.ToString());
	}

	public static void selectLang(){
		SceneManagerWrap.LoadScene (CmnConst.SCENE.SelectLangScene);
	}

		public static void loadSceneWrap( string name )
		{
			SceneManagerWrap.LoadScene (name);
		}

		public static void pushToTitle()
		{
			playCanselSe();
			toTitle();
		}

	public static void pushToMain(){
		playClickSe();
		toMain();
	}

	public static void pushToReady(){
		playClickSe();
		toReady();
	}

	public static void pushToConfig(){
		playClickSe();
		toConfig();
	}

	public static void pushToAlbum(){
		playClickSe();
		toAlbum();
	}

	public static void reset()
	{
		Destroy(CmnBaseProcessMng.Singleton.gameObject);
		InitLoading = false;
		CmnBaseProcessMng.toTitle();
	}

	public static GameObject showUiWindow( GameObject prefab, Transform pare ){
		GameObject obj = Instantiate( prefab ) as GameObject;
		obj.transform.parent = pare;
		obj.transform.localPosition = new Vector3();
		obj.transform.localScale = prefab.transform.localScale;
		return obj;
	}

	public static void activeObject( GameObject obj, bool is_show ){
		if (obj.activeSelf != is_show) {
			obj.SetActive (is_show);
		}
	}

	public static ConfirmWindowCmn showConfirm(string txt, System.Action<object> callback, object param = null, bool isHold = false) {
		return showConfirm(txt, callback, null, param, isHold);
	}

	public static ConfirmWindowCmn showConfirm(string txt, System.Action<object> callback, System.Action<object> cansel, object param = null, bool isHold = false) {
		var wnd = ConfirmWindowCmn.show(Singleton.ConfirmWindow, txt, null, callback, cansel, param);
		wnd.changeButtonType(isHold);
		return wnd;

	}

	public static ConfirmWindowCmn showMessage( string txt)
    {
        return ConfirmWindowCmn.show( Singleton.MessageWindow, txt, null, null );
    }

	public static void callQuestion( string txt = "" ){
		CmnBaseProcessMng.callQuestion ();
	}

    public static ConfirmWindowCmn showMessage(string title, string txt) {
        ConfirmWindowCmn wnd = ConfirmWindowCmn.show(Singleton.MessageWindow, title, txt);
        wnd.initTextPosition();
        return wnd;
    }

    public static ConfirmWindowCmn showLongMessage(string title, string txt) {
		return ConfirmWindowCmn.show(Singleton.LongMessageWindow, title, txt);
	}

	public static void show2DEffect(string key, Vector2 posi) {
		Singleton.Common2DEffect.showEffect(key, posi);
	}


	/// <summary>
	/// レア度別カラー取得
	/// </summary>
	/// <returns></returns>
	public static Color getRareColor(int rare) {
		Color rareColor = Color.white;
		if (GameConst.RARITY_COLOR.Length > 0) {
			rare = Mathf.Clamp(rare, 0, GameConst.RARITY_COLOR.Length);
			ColorUtility.TryParseHtmlString(GameConst.RARITY_COLOR[rare], out rareColor);
		}
		return rareColor;
	}
}
