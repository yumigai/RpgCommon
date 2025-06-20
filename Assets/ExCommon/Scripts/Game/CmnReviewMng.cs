using UnityEngine;
using System.Collections;

public class CmnReviewMng : MonoBehaviour {

	public enum REVIEW_STATE
	{
		NON,
		REVIEWED,
		LATER,
		NEVER,
	}

	public static CmnReviewMng _instance;

	public static CmnReviewMng Instance{ 
		get { 
			if (_instance == null) {
				_instance = GameObject.FindObjectOfType<CmnReviewMng>(); 
			}
			return _instance;
		} 
	}

	public void AWake(){
		_instance = this;
	}

	public static bool isREview(){

		REVIEW_STATE state = (REVIEW_STATE)PlayerPrefs.GetInt (CmnConfig.SAVE_NAME.CMN_IS_REVIEW.ToString());

		if (state == REVIEW_STATE.NON) {
			return true;
		}else if( state == REVIEW_STATE.LATER ){
			int play_count = PlayerPrefs.GetInt (CmnConfig.SAVE_NAME.CMN_PLAY_COUNT.ToString());
			if (play_count % CmnConfig.REVIEW_LATER_NEXT_PLAYCOUNT == 0) {
				return true;
			}
		}

		return false;
	}

	// Use this for initialization
	void Start () {
	
	}


	public void pushReviwNow(){
		GmCmnMng.openStorePage ();
		PlayerPrefs.SetInt (CmnConfig.SAVE_NAME.CMN_IS_REVIEW.ToString(), (int)REVIEW_STATE.REVIEWED );
		PlayerPrefs.Save ();
		Destroy (this.gameObject);
	}

	public void pushLaterReviw(){
		PlayerPrefs.SetInt (CmnConfig.SAVE_NAME.CMN_IS_REVIEW.ToString(), (int)REVIEW_STATE.LATER );
		PlayerPrefs.Save ();
		Destroy (this.gameObject);
	}
	public void pushNeverReviw(){
		PlayerPrefs.SetInt (CmnConfig.SAVE_NAME.CMN_IS_REVIEW.ToString(), (int)REVIEW_STATE.NEVER );
		PlayerPrefs.Save ();
		Destroy (this.gameObject);
	}

	public static void show(bool view){
		Instance.gameObject.SetActive( view );
	}

	public static void hide(){
		Destroy (Instance.gameObject);
	}
}
