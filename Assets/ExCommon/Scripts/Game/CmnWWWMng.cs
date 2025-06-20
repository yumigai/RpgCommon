using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine.UI;

public class CmnWwwMng : MonoBehaviour {

	private const float RETRY_TIME = 1.0f;

	protected static CmnWwwMng instance;
	protected static GameObject BaseObj{ get; set; }

	private string Responce{ get; set; }
	private string Error{ get; set; }

	static CmnWwwMng()
	{
	}

	public static CmnWwwMng Instance
	{
		get
		{
			if(instance == null)
			{
				instance = (CmnWwwMng) FindObjectOfType(typeof(CmnWwwMng));

				if( instance == null ){
					BaseObj = new GameObject();
					BaseObj.name = "CmnWWWMng";
					instance = BaseObj.AddComponent<CmnWwwMng>();
				}
			}
			
			return instance;
		}
	}
	
	public void init(){
		Responce = "";
	}


	public void access( string url, Action<string> callback, WWWForm form = null){
		StartCoroutine( accessProcess( url, callback, form ) );
	}
	
	IEnumerator accessProcess ( string url, Action<string> callback, WWWForm form ) {

		if (form == null) {
			form = new WWWForm ();
		}

		WWW www = new WWW(url, form);

		yield return www;

		if (www.error == null) {
			Responce = www.text;
			Error = null;
		}else{
			Responce = null;
			Error = www.error;
		}
		callback(Responce);
	}

	IEnumerator downloadText ( string url, string asset_name, int version, Action<string,string> callback ){

		TextAsset txt;

#if !( UNITY_WEBGL && UNITY_2022_1_OR_NEWER )
		while (!Caching.ready)
			yield return null;
#endif

		using(WWW www = WWW.LoadFromCacheOrDownload ( url, version )){
			yield return www;
			if (www.error != null){
				throw new Exception("WWWerror" + www.error);
			}
			AssetBundle bundle = www.assetBundle;
			if (asset_name == ""){
				txt = Instantiate(bundle.mainAsset) as TextAsset;
			}else{
				txt = Instantiate(bundle.LoadAsset(asset_name)) as TextAsset;
			}

			bundle.Unload(false);

			callback( asset_name, txt.text );
			
		}
	}

	public static bool chekNetwork(){
		switch ( Application.internetReachability ) {
		case NetworkReachability.NotReachable:
			return false;
		case NetworkReachability.ReachableViaCarrierDataNetwork:
		case NetworkReachability.ReachableViaLocalAreaNetwork:
			return true;
		}
		return false;
	}

	public void loadWWWImg( string url, Image img ){
		StartCoroutine (loadWWWImgProcess (url, img));
	}
	public static IEnumerator loadWWWImgProcess( string url, Image img ){
		WWW www = new WWW(url);
		Debug.Log (url);
		yield return www;
		Texture2D texture2d = www.textureNonReadable;

		img.sprite = Sprite.Create (texture2d, new Rect(0, 0, texture2d.width, texture2d.height), new Vector2(0.5f, 0.5f));
	}


}
