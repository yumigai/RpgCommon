using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CoinNumMng : MonoBehaviour {

	[SerializeField]
	public Text CoinNum;

	[SerializeField]
	public bool IsDelay;

	[System.NonSerialized]
	public int DispCoinNum;

	public int NowCoinNum = -1;

	public static CoinNumMng Singleton;

	void Awake(){
		CoinNum = GetComponent<Text> ();
		Singleton = this;
	}

	// Update is called once per frame
	void FixedUpdate () {
		
		if ( IsDelay && DispCoinNum != NowCoinNum) {
			DispCoinNum += (int)(Mathf.Sign (NowCoinNum - DispCoinNum) * 1f);
			CoinNum.text = DispCoinNum.ToString ();
		} 
	}

	public static void setCoinNum(int num){
        if (Singleton.IsDelay) {
            Singleton.NowCoinNum = num;
        } else {
            Singleton.CoinNum.text = num.ToString();
        }
		
	}
}
