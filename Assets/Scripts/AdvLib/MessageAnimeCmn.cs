using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class MessageAnimeCmn : MonoBehaviour {

    private const int AUTO_WAIT_TIME = 5;

	[SerializeField]
    public Text MessageBoard;
	[SerializeField]
	public int MessageSpeed;
	[SerializeField]
	public AudioClip MessageSe;
	[SerializeField]
	public string showText;
	[SerializeField, Header("効果音は最初のみか")]
	public bool IsStartedOnlySe;

	[System.NonSerialized]
	public int textCaret;
	[System.NonSerialized]
	public int AfterEndShowCount;

    [System.NonSerialized]
    private int messageTime;

	//早送り状態
	protected bool IsFast = false;

	//自動メッセージ送り状態
	protected bool IsAuto = false;

	public static MessageAnimeCmn Singleton;


    public bool endShow { get { return showText == null ? true : (textCaret >= showText.Length); } }

    public bool IsReadyAutoNext{ get { return ( AfterEndShowCount > showText.Length * AUTO_WAIT_TIME); } }

    public void Awake(){
		Singleton = this;
	}

	public void init( string text ){
        text = text.Replace("\\n", "\n");
		showText = text;
		messageTime = 0;
		textCaret = 0;
		AfterEndShowCount = 0;
        if (IsStartedOnlySe) {
			playMessageSe();
		}
	}

	public void Update(){
		//if (Input.GetMouseButtonDown (0) && !endShow ) {
		//	textCaret = showText.Length;
		//	writeWord ();
		//}
	}



	void FixedUpdate(){

		if (showText == null || showText == "") {
			return;
		}

		messageTime++;

		if (endShow) {
			AfterEndShowCount++;
			return;
		}

		if (MessageSpeed <= 0) {
			textCaret = showText.Length;
			MessageBoard.text = showText;

		} else {
//			if (!endShow) {

				if( messageTime % MessageSpeed == 0 ){

					textCaret++;

                //					if (Input.GetMouseButton (0)) {
                //						textCaret = showText.Length;
                //					}
                if (!IsStartedOnlySe) {
					playMessageSe();
				}
					
					writeWord ();
			}
//			}
		}
		
	}

	public void ShowAllText() {
        if (!endShow) {
			textCaret = showText.Length;
			writeWord();
		}
	}

	private void writeWord(){
		//SoundMng.Instance.playSE (MessageSe);
		string txt = showText.Substring (0, textCaret);
		MessageBoard.text = txt;

	}

	private void playMessageSe() {
        if (!IsFast) {
			SoundMng.Instance.playSE(MessageSe);
		}
	}

}
