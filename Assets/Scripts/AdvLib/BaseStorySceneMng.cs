using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
using System.Linq;

public class BaseStorySceneMng : FaceMessageMng
{

	private enum MODE
	{
		MESSAGE,
		HIDE,
		LOG
	}

	private const string DebugPath = "Dummy";
	//	private const string ANIME_OFF = "Off";
	public const string StoryDataDirectory = "Text/Story/";
	public const string PutImageDirectory = "Prefab/Adv/Chara/";

	[SerializeField]
	public GameObject[] SelectionButton;
	[SerializeField]
	public string StoryName;
	[SerializeField]
	public string ReturnScene = null;
	[SerializeField]
	public Animator FrontAnime;

	[SerializeField]
	public Image EventImage;
	//	[SerializeField]
	//	public Image AnimeImage;
	[SerializeField]
	public Image BackImage;
	[SerializeField]
	public Transform PutImageBase;
	[SerializeField]
	public Transform PutImagePointBase;

	//メッセージボードとスキップなどの各種ボタンは別にしないといけない（子要素にしない）
	//メッセージ隠しで画面全体入力ができなくなるとまずいし、ボタンは画面全体入力より前面にないといけない
	[SerializeField]
	public GameObject MessageBase;

	[SerializeField, Header("通常時の入力")]
	public GameObject InputButtonsBase;

	[SerializeField, Header("文字隠し時の入力")]
	public GameObject InputHideBase;

	[SerializeField]
	public ScrollMessageMng BackLogWindow;

	[SerializeField]
	public MultiUseScrollMng BackLogScroll;

	[SerializeField]
	public GameObject CheckMarkFast;
	[SerializeField]
	public GameObject CheckMarkAuto;

	//次のページへの遷移猶予
	[System.NonSerialized]
	public const int WaiteNextPage = 25;
	[System.NonSerialized]
	public AdvStatusTran NowSave;
	[System.NonSerialized]
	public List<StandCharaMng> PutImages;
	[System.NonSerialized]
	public Dictionary<string, Vector2> PutImagePoints;

	private StoryMst[] Story;
	private int Nowindex;
	private StoryMst NowData {
		get {
			return Story[Nowindex];
		}
	}
	private string NowBgm = "";

	private int BaseSpeed;

	private MODE NowMode = MODE.MESSAGE;

	public static string StoryNameOrder;
	public static string ReturnSceneOrder;
	public static System.Action InitFinishCallback;
	new public static BaseStorySceneMng Singleton;

	/// <summary>
	/// 次回テキスト表示までのマージン
	/// </summary>
	public bool IsReadyNext {
		get {
			return (AfterEndShowCount > WaiteNextPage);
		}
	}

	public bool IsFinish {
		get {
			return Nowindex >= Story.Length;
		}
	}

	new void Awake() {

		Singleton = this;

		PutImages = new List<StandCharaMng>();
		PutImagePoints = new Dictionary<string, Vector2>();

		if (PutImagePointBase != null) {
			foreach (Transform val in PutImagePointBase) {
				Vector3 posi = val.GetComponent<RectTransform>().anchoredPosition;
				PutImagePoints.Add(val.name, posi);
			}
		}

		init();
	}

	void Start() {
		SoundMng.Instance.stopBGM();
		showStory();
	}

	public void changeBackImage(string path) {
		changeCommonImage(path, CmnConst.Path.ADV_IMG_BACK, BackImage);
	}

	//public void changeIconImage( string path ){
	//	if (IconImage != null) {
	//		bool is_show = !string.IsNullOrEmpty (path);
	//		CmnBaseProcessMng.activeObject (IconImageBase.gameObject, is_show);
	//		if (is_show ){
	//			changeCommonImage (path, CmnConst.Path.ADV_IMG_ICON, IconImage);
	//		}
	//	}
	//}
	//public void changeNamePlate( string name ){
	//	if (NamePlate != null) {
	//		bool is_show = !string.IsNullOrEmpty (name);
	//		CmnBaseProcessMng.activeObject (NamePlateBase.gameObject, is_show);
	//		if (is_show ){
	//			NamePlate.text = name;
	//		}
	//	}
	//}


	public void changeEventImage(string path) {
		changeCommonImage(path, CmnConst.Path.ADV_IMG_EVENT, EventImage);
	}

	public static StandCharaMng putStandChara(GameObject prefab, string sp, Vector2 posi, Transform parent) {
		GameObject stand = Instantiate(prefab) as GameObject;
		stand.transform.parent = parent;
		stand.transform.localScale = prefab.transform.localScale;
		Vector2 stand_posi = new Vector2(posi.x, prefab.transform.localPosition.y);
		stand.transform.localPosition = stand_posi;
		StandCharaMng mng = stand.GetComponent<StandCharaMng>();
		stand.name = prefab.name;
		mng.setFace(sp);
		//       stand.transform.SetSiblingIndex(0);
		return mng;
	}

	public void putImage(GameObject prefab, string sp, Vector2 posi) {

		StandCharaMng mng = putStandChara(prefab, sp, posi, PutImageBase);
		PutImages.Add(mng);

	}

	public void changeStandImage(string name, string face, Vector2 posi, bool turn) {
		int index = PutImages.FindIndex(img => img.name == name);
		if (index < 0) {
			string img_path = PutImageDirectory + name;
			GameObject obj = Resources.Load<GameObject>(img_path);
			string f_path = CmnConst.Path.ADV_IMG_ICON + name + face;
			Sprite sp = Resources.Load<Sprite>(f_path);
			putImage(obj, face, posi);
			index = PutImages.Count - 1;
		} else {
			PutImages[index].setPosi(posi);
			PutImages[index].setFace(face);
		}

		PutImages[index].transform.SetSiblingIndex(0);
		Vector2 scale = PutImages[index].transform.localScale;
		scale.x = turn ? -Mathf.Abs(scale.x) : Mathf.Abs(scale.x);
		PutImages[index].transform.localScale = scale;
	}

	public void changeStandImage(string path, string face, string point, bool turn) {
		if (!string.IsNullOrEmpty(path) && !string.IsNullOrEmpty(point)) {
			Vector2 posi = PutImagePoints[point];
			changeStandImage(path, face, posi, turn);
		}
	}
	public void changeStandImage(StoryMst.StandCharaData stand) {
		changeStandImage(stand.Image, stand.Face, stand.Position, stand.isTurn);
	}

	public void eraseImage(string name) {
		int index = PutImages.FindIndex(img => img.name == name);
		PutImages.RemoveAt(index);
	}

	public void clearImage() {
		Singleton.PutImages.Clear();
	}

	public void messageAnime(string text) {
		init(text);
	}

	public void pushSelect(int index) {
	}

	public void init() {
		loadData();
		Nowindex = 0;
		BaseSpeed = MessageSpeed;

		InputHideBase.SetActive(false);
		BackLogWindow.gameObject.SetActive(false);

		updateCheckMark();

		if (InitFinishCallback != null) {
			InitFinishCallback();
		}
	}

	public void loadData() {
		if (!string.IsNullOrEmpty(StoryNameOrder)) {
			StoryName = StoryNameOrder;
		}
		if (string.IsNullOrEmpty(StoryName)) {
			StoryName = DebugPath;
		}
		if (!string.IsNullOrEmpty(ReturnSceneOrder)) {
			ReturnScene = ReturnSceneOrder;
		}
		string path = StoryDataDirectory + StoryName;
		//string txt = UtilToolLib.readResourceText(path);
		Story = StoryMst.load<StoryMst>(path);

	}

	new public void Update() {
		base.Update();

		if (IsFast || IsAuto) {
			NextMessage();
		}
	}

	public void pushNext() {

		if (!IsFast && !IsAuto && MessageBase.activeSelf && InputButtonsBase.activeSelf) {
			NextMessage();
		}
		if (!MessageBase.activeSelf || !InputButtonsBase.activeSelf) {
			MessageBase.SetActive(true);
			InputButtonsBase.SetActive(true);
		}

		if (IsFast || IsAuto) {
			IsFast = false;
			IsAuto = false;
			updateCheckMark();
		}
	}

	private void NextMessage() {
		if (IsFinish) {
			return;
		}
		if (IsReadyNext) {
			if (IsAuto && !IsReadyAutoNext) {
				return;
			}

			Nowindex++;
			showStory();

			// if (NowData.TagJump == null || NowData.TagJump.Length == 0) {
			// 	Nowindex++;
			// 	showStory();
			// } else {
			// 	int num = 0;
			// 	for (int i = 0; i < NowData.TagJump.Length; i++) {
			// 		if (judgeFlg(NowData.TagJump[i].Flg)) {
			// 			if (NowData.TagJump[i].SelectWord == string.Empty) {
			// 				tagJump(NowData.TagJump[i].TargetTag);
			// 				break;
			// 			} else {
			// 				makeSelectWords(i);
			// 			}
			// 		}
			// 		num++;
			// 	}
			// 	if (num == 0) {
			// 		Nowindex++;
			// 		showStory();
			// 	}
			// }
		} else if (!IsAuto) {
			ShowAllText();
			if (IsFast) {
				AfterEndShowCount = WaiteNextPage;
			}
		}
	}

	private void makeSelectWords(int i) {
		SelectionButton[i].SetActive(true);
		ListButtonMng mng = SelectionButton[i].GetComponent<ListButtonMng>();
		mng.Index = i;
		//mng.BaseText.text = Story[Nowindex].TagJump[i].SelectWord;
		mng.Callback = pushSelect;
	}

	public void pushSelect(int index, object dummy) {

		CmnBaseProcessMng.playClickSe();

		// string tag = Story[Nowindex].TagJump[index].TargetTag;

		// tagJump(tag);

	}

	public void showStory() {
		showStory(Nowindex);
	}

	public void showStory(int index) {

		if (index >= Story.Length) {
			endStory();
			return;
		}

		hideSelectButton();

		Nowindex = index;

		if (!judgeFlg(NowData.NeedFlag, NowData.NeedFlagValue)) {
			return;
		}

		changeBackImage(NowData.BackImage);

		for (int i = 0; i < NowData.Stand.Length; i++) {
			changeStandImage(NowData.Stand[i]);
		}
		for (int i = 0; i < PutImages.Count; i++) {
			int find_img = System.Array.FindIndex<StoryMst.StandCharaData>(NowData.Stand, (st => st.Image == PutImages[i].name));
			if (find_img < 0) {
				PutImages[i].hideImage();
			}
		}

		changeIconImage(NowData.Icon);

		changeEventImage(NowData.EventImage);

		//addFlag(NowData.Flg,1);

		changeNamePlate(NowData.Name, NowData.Kana);

		if (NowData.Bgm != NowBgm) {
			NowBgm = NowData.Bgm;
			if (NowData.Bgm == string.Empty) {
				SoundMng.Instance.stopBGM();
			} else {
				SoundMng.Instance.playBGM(NowData.Bgm);
			}
		}
		if (!string.IsNullOrEmpty(NowData.Se)) {
			SoundMng.Instance.playSE(NowData.Se);
		}
		if (FrontAnime != null && !string.IsNullOrEmpty(NowData.Anime)) {
			//			AnimeImage.gameObject.SetActive (true);
			FrontAnime.SetTrigger(NowData.Anime);
		}
		//		else {
		//			FrontAnime.SetTrigger (ANIME_OFF);
		//			AnimeImage.gameObject.SetActive (false);
		//		}

		effect();

		messageAnime(NowData.StoryText);

	}

	// public void tagJump(string tag) {

	// 	StoryMst[] datas = System.Array.FindAll(Story, line => line.Tag == tag);

	// 	System.Array.Sort(datas, (a, b) => b.Priority - a.Priority);

	// 	if (datas.Length == 0) {
	// 		Nowindex++;
	// 	} else {
	// 		for (int i = 0; i < datas.Length; i++) {
	// 			if (judgeFlg(datas[i].Flg)) {
	// 				Nowindex = datas[i].Id;
	// 				break;
	// 			}
	// 		}
	// 	}

	// 	showStory();
	// }

	// public static void addFlag(string key, int val){
	// 	AdvStatusTran.AdvData.Flags[key] += val;
	// }

	// public static void setFlag(string key, int val){
	//     AdvStatusTran.AdvData.Flags[key] = val;
	// }

	// public static int getFlag(string key){
	//     if(AdvStatusTran.AdvData.Flags.ContainsKey(key)){
	//         return AdvStatusTran.AdvData.Flags[key];
	//     }
	//     return 0;
	// }


	// public void getFlug(string flg) {
	// 	if (AdvSaveProc.AdvData != null) {
	// 		List<string> flgs = AdvSaveProc.AdvData.Flgs.ToList();
	// 		flgs.Add(flg);
	// 		AdvSaveProc.AdvData.Flgs = flgs.ToArray();
	// 	}
	// }

	// public void removeFlug(string flg) {
	// 	List<string> flgs = AdvSaveProc.AdvData.Flgs.ToList();
	// 	flgs.Remove(flg);
	// 	AdvSaveProc.AdvData.Flgs = flgs.ToArray();
	// }

	private void hideSelectButton() {
		for (int i = 0; i < SelectionButton.Length; i++) {
			if (SelectionButton[i].activeSelf) {
				SelectionButton[i].SetActive(false);
			}
		}
	}

	private bool judgeFlg(string flg, int value) {
		if (string.IsNullOrEmpty(flg) || NowSave.getFlag(flg) > value) {
			return true;
		}
		return false;
	}

	public static void StoryOrder(string name, CmnConst.SCENE return_scene) {
		StoryOrder(name, return_scene.ToString());
	}
	public static void StoryOrder(string name, string return_scene) {
		StoryNameOrder = name;
		ReturnSceneOrder = return_scene;
		CmnBaseProcessMng.toStory();
	}

	/// <summary>
	/// 自動メッセージ送り
	/// </summary>
	public void PushAutoMessage() {
		IsFast = false;
		IsAuto = !IsAuto;
		updateCheckMark();
	}

	/// <summary>
	/// 早送り
	/// </summary>
	public void PushFastStory() {
		IsAuto = false;
		IsFast = !IsFast;
		updateCheckMark();
	}

	/// <summary>
	/// メッセージウインドウクローズ
	/// </summary>
	public void PushHideMessageWindow() {
		UpdateState(MODE.HIDE);
	}

	/// <summary>
	/// バックログ
	/// </summary>
	public void PushShowBackLog() {

		UpdateState(MODE.LOG);

		if (NowMode == MODE.LOG) {
			BackLogScroll.clear();
			for (int i = 0; i < Nowindex; i++) {
				var log = BackLogScroll.makeListItem();
				log.Detail.text = Story[i].StoryText;
			}
			BackLogWindow.SetScrollValue(0); //スクロールが一瞬ガタつくのを低減するために初回にも行う
											 //生成時に設定が間に合わない場合があるので１フレーム待つ
			CommonProcess.waitOneFrame(delegate {
				BackLogWindow.SetScrollValue(0);
			});

		}
	}

	private void UpdateState(MODE mode) {

		if (mode == NowMode) {
			if (mode == MODE.MESSAGE) {
				return;
			} else {
				mode = MODE.MESSAGE;
			}
		}

		if (mode == MODE.HIDE || mode == MODE.LOG) {
			IsFast = false;
			IsAuto = false;
		}

		MessageBase.SetActive(mode == MODE.MESSAGE);
		InputButtonsBase.SetActive(MessageBase.activeSelf);
		InputHideBase.SetActive(mode == MODE.HIDE);
		BackLogWindow.gameObject.SetActive(mode == MODE.LOG);

		updateCheckMark();

		NowMode = mode;
	}

	private void updateCheckMark() {
		CheckMarkFast.SetActive(IsFast);
		CheckMarkAuto.SetActive(IsAuto);
	}

	public void skipStory() {
		CmnBaseProcessMng.playClickSe();
		string txt = LanguageStaticTextMng.getLangText("ストーリーを\nスキップしますか？", "Do you want to skip the story ?");
		CmnBaseProcessMng.showConfirm(txt, endStory);
		//endStory ();
	}
	public void endStory(object obj) {
		endStory();
	}

	public void endStory() {
		SceneManagerWrap.LoadScene(ReturnScene, false);
	}

	public void storyCharaChange(string before, string after) {
		for (int i = 0; i < Story.Length; i++) {
			int index = System.Array.FindIndex(Story[i].Stand, it => it.Image == before);
			if (index >= 0) {
				Story[i].Stand[index].Image = after;
			}

		}
	}

	public void replaceText(string before, string after) {
		for (int i = 0; i < Story.Length; i++) {
			Story[i].StoryText = Story[i].StoryText.Replace(before, after);
		}
	}

	public void effect() {

		if (NowData.Effect == "Slow") {
			MessageSpeed = BaseSpeed * 2;
		} else if (BaseSpeed != MessageSpeed) {
			MessageSpeed = BaseSpeed;
		}
	}

}