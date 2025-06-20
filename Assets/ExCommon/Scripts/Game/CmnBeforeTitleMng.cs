using UnityEngine;
using System.Collections;

public class CmnBeforeTitleMng : MonoBehaviour {

	// Use this for initialization
	void Start () {
		StartCoroutine (showInit());
	}
	
	IEnumerator showInit ( ){

		bool show = false;

		//CommercialMng.CallbackClickInterstitial = toTitle;

		for (int i = 0; i < 10 && !show; i++) {
			yield return new WaitForSeconds(0.5f);
			//show = CommercialMng.showInterstitial ();
		}

	}

	public void toTitle(){
		GmCmnMng.toTitle ();
	}


}
