using UnityEngine;
using System.Collections;

public class LanguageStaticTextLineMng : LanguageStaticTextMng {

	public enum END_TO{
		REPEAT,
		DESTROY,
		STOP,
	}

	[SerializeField]
	public END_TO _EndTo;

	[SerializeField]
	public float _AnimationTime;

	[SerializeField]
	public float _NextShowTime;

	[SerializeField]
	public string MessageSe;

	[SerializeField]
	public bool IsAutoStart;

	public System.Action _Callback;
	public System.Action _FinishCallback;
	public int _NowShowIndex;
	public bool SkipAnime;

	private bool _NextReady;

	[System.NonSerialized]
	public string[] _TextLines;

	public LanguageStaticTextLineMng Singleton;

	void Awake(){
		Singleton = this;
	}

	void Start () {
		
		setTextComponent ();
		init ();

		if (IsAutoStart) {
			restart ();
		}
	}

	public void init(){
		_NowShowIndex = 0;
		_NextReady = false;
	}

	public void restart(){
		
		init ();

		string text = getLangText ();
		_TextLines = UtilToolLib.purgeStringLine (text);

		showText ();
	}

	void FixedUpdate(){
		if (_NextReady) {
			_NextReady = false;
			StartCoroutine (waitToWord ());
			if (_Callback != null) {
				_Callback ();
			}
		}
	}

	IEnumerator waitToWord( ){

		yield return new WaitForSeconds (_NextShowTime);

		_NowShowIndex++;

		if (_NowShowIndex < _TextLines.Length) {
			
			showText ();
		} else {

			if (_FinishCallback != null) {
				_FinishCallback ();
			}

			switch (_EndTo) {
			case END_TO.DESTROY:
				Destroy (this.gameObject);
				break;
			case END_TO.REPEAT:
				restart ();
				break;
			}
		}
	}

	IEnumerator textAnime(){

		yield return new WaitForSeconds (_AnimationTime);

		if (_NowShowIndex >= _TextLines.Length) { //異常系
			_NextReady = true;
			SkipAnime = false;
			yield return null;
		}
			
		for (int i = 0; i < _TextLines [_NowShowIndex].Length; i++) {
			if (SkipAnime) {
				TargetTxt.text = _TextLines [_NowShowIndex];
				break;
			} else {
				TargetTxt.text = _TextLines [_NowShowIndex].Substring (0, i+1);
				if (!string.IsNullOrEmpty(MessageSe)) {
					SoundMng.Instance.playSE (MessageSe);
				}
				yield return new WaitForSeconds (_AnimationTime);
			}
		}

		SkipAnime = false;
		_NextReady = true;

	}

	private void showText( ){
		if (_AnimationTime == 0f) {
			TargetTxt.text = _TextLines [_NowShowIndex];
			_NextReady = true;
		} else {
			StartCoroutine (textAnime ());
		}
	}
	

}
