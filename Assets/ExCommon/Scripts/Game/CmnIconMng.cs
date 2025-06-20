using UnityEngine;
using System.Collections;

public class CmnIconMng : MonoBehaviour {

	protected System.Action<CmnIconMng> CallBack{ get; set;}
	public object Param{ get; set; }

	// Use this for initialization
	void Start () {
	
	}

	public void setCallback( System.Action<CmnIconMng> act, object para ){
		CallBack = act;
		Param = para;
	}

	public void pushIcon(){
		CallBack(this);
	}


}
