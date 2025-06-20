using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class CmnAppliListMng : MonoBehaviour {

	public int APPLI_ICON_LIMIT = 4;

	public static CmnAppliListMng Instance{ get; set; }

	public void Awake(){
		Instance = this;
	}

	public void Start(){

//#if UNITY_EDITOR
//#else
		loadAppliIcon ();
//#endif
	
	}

	public void loadAppliIcon(){
		WWWForm form = new WWWForm ();
		form.AddField ("num", APPLI_ICON_LIMIT);
		form.AddField ("platform", GmCmnMng.PLATFORM);
		CmnWwwMng.Instance.access (CmnConst.SERVER_APPLI_ICON_DATA, callbackLoadAppliIcon, form );
	}

	public void callbackLoadAppliIcon( string value ){

		if (!string.IsNullOrEmpty (value)) {

//			IList list = JsonUtility.FromJson (value);
//
//			for (int i = 0; i < this.transform.childCount; i++) {
//
//				Transform icon = this.transform.GetChild (i);
//
//				if (i < list.Count) {
//					IDictionary val = list [i] as IDictionary;
//					Image img = icon.GetComponent<Image> ();
//					string img_url = val ["img_url"] as string;
//
//					CmnWwwMng.Instance.loadWWWImg (img_url, img);
//					icon.GetComponent<CmnIconMng> ().setCallback (linkJump, val ["link_url"] );
//
//				} else {
//					icon.gameObject.SetActive (false);
//				}
//			}
		}
	}

	public void linkJump( CmnIconMng icon ){
		string link_url = (string)icon.Param;
		Application.OpenURL (link_url);
	}

	public static void setShow(bool is_show){
		if (Instance) {
			Instance.gameObject.SetActive (is_show);
		}
	}
}
