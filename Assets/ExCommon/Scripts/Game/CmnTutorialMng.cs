using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class CmnTutorialMng : MonoBehaviour {

	public enum END_TO
	{
		DESTROY,
		NEXT_SCENE,
	}


	//public CmnTutorialSheetMng[] TutorialSheets;

	[SerializeField]
	public Text MessageAnime;

	[SerializeField]
	public GameObject OkButton;

	[SerializeField]
	public float RefleshButtonTime = 0.5f;

	[SerializeField]
	public AudioClip NextSe;

	[SerializeField]
	public AudioClip Bgm;

	[SerializeField]
	public CmnTutorialSheetMng[] Sheets;

	[SerializeField]
	public END_TO EndTo;

	[SerializeField]
	public string NextSceneName;

	public static CmnTutorialMng Instance{ get; set; }

	public int NextShowIndex{ get; set; }

	void Awake(){

		Instance = this;
		NextShowIndex = 0;

		if (Sheets == null || Sheets.Length == 0 ) {
			Sheets = this.transform.GetComponentsInChildren<CmnTutorialSheetMng> ();
			setShow (false);
		}
	}

	void Start(){
		SoundMng.Instance.playBGM (Bgm);
		next ();
	}

	public void pushNext(){
		SoundMng.Instance.playSE (NextSe);
		next ();
	}

	private void next(){

		bool skip = false;
		do {
			skip = nextProcess ();
		} while(skip);
	}

	private bool nextProcess(){

		bool is_skip = false;
		bool is_end = showSheet ();

		if (is_end) {
			switch (EndTo) {
			case END_TO.DESTROY:
				Destroy (this.gameObject);
				break;
			case END_TO.NEXT_SCENE:
				if( string.IsNullOrEmpty( NextSceneName ) ){
					SceneManagerWrap.loadBefore ();
				}else{
					SceneManagerWrap.loadScene (NextSceneName);
				}
				break;
			}

		} else {

			if (Sheets [NextShowIndex].StartTo == CmnTutorialSheetMng.START_TO.SKIP) {
				is_skip = true;
			}

			NextShowIndex++;
			preventBarrage ();
		}

		return is_skip;

	}

	private bool showSheet( int index = -1 ){
		if (index < 0) {
			index = NextShowIndex;
		}
		if (index > 0) {
			switch (Sheets [index - 1].EndTo) {
			case CmnTutorialSheetMng.END_TO.DELETE:
				Sheets [index - 1].gameObject.SetActive (false);
				break;
			case CmnTutorialSheetMng.END_TO.DELETE_ALL:
				setShow (false);
				break;
			default:
				break;
			}
		}
		if (index < Sheets.Length) {
			Sheets [index].gameObject.SetActive (true);
			return false;
		}

		return true;
	}

	public static void show( int index ){
		Instance.showSheet (index);
	}

	public static void showNext(){
		Instance.next ();
	}

	public void setShow(bool isshow){
		foreach (CmnTutorialSheetMng val in Sheets) {
			val.gameObject.SetActive (isshow);
		}
	}

	//連打防止
	private void preventBarrage(){
		if (OkButton != null) {
			OkButton.SetActive (false);
			StartCoroutine( refleshButton (RefleshButtonTime) );
		}
	}

	private IEnumerator refleshButton( float time ){
		yield return new WaitForSeconds (time);
		OkButton.SetActive (true);
	}

}