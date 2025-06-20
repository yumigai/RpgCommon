using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

//ver 1.00

public class GmCmnMng : MonoBehaviour
{

	#if UNITY_ANDROID
	public const int PLATFORM = 1;
	#elif UNITY_IOS
	public const int PLATFORM = 2;
#else
	public const int PLATFORM = 0;
#endif

	[SerializeField]
	public string GooglePlayUrl;
	[SerializeField]
	public string AppStoreUrl;

	private static bool InitLoading{ get; set; }

	protected static GmCmnMng instance;
	protected static GameObject g_base_obj;


	public static GmCmnMng Instance {
		get {
			makeInstance<GmCmnMng> ( ref instance, ref g_base_obj);
			return instance;
		}
	}

	public static void makeInstance<T>( ref T insta, ref GameObject obj ) where T: MonoBehaviour {
		if (insta == null) {
			insta = (T)FindObjectOfType (typeof(T));
			if (insta == null) {
				obj = new GameObject ();
				obj.name = typeof(T).Name;
				insta = g_base_obj.AddComponent<T> ();
			}
		}
	}

	public void Awake ()
	{
		if (InitLoading) {
			Destroy (this.gameObject);
		}else{
			InitLoading = true;
		}
	}

	public void Start ()
	{

		DontDestroyOnLoad (this);

	}

	private bool ShowDialog{ get; set; }

	public void Update ()
	{
		if (Input.GetKey (KeyCode.Escape)) {
			intputEscape ();
		}
	}

	private void intputEscape ()
	{
    }

	public static void inputPause ()
	{
		Instance.intputEscape ();
	}

	IEnumerator reShowDialog ()
	{
		yield return new WaitForSeconds (1f);
		ShowDialog = false; 
	}

	public void SoundMute ()
	{
		SoundMng.Instance.mute ();
	}


	public static void toBeforeTitle ()
	{
		SceneManagerWrap.loadScene (CmnConst.BEFORE_TITLE_SCENE_NAME);
	}

	public static void toTitle (string path, string music)
	{
		Sprite sp = Resources.Load<Sprite> (path);
		toTitle (sp, music);
	}

	public static void toTitle (Sprite sp, string music)
	{
		CmnTitleMng.MusicPath = music;
		CmnTitleMng.TitleImgSprite = sp;
		toTitle ();
	}

	public static void toTitle ()
	{
		SceneManagerWrap.loadScene (CmnConst.SCENE.TitleScene);
	}

	public static void toResult (string path, string music, string result_message = "")
	{
		Sprite sp = Resources.Load<Sprite> (path);
		toResult (sp, music, result_message );
	}

	public static void toResult (Sprite sp, string music, string result_message = "" )
	{
		CmnResultMng.MusicPath = music;
		CmnResultMng.ImgSprite = sp;
		toResult (result_message);
	}

	public static void toResult (string result_message = "" )
	{
		CmnResultMng.Message = result_message;
		SceneManagerWrap.loadScene (CmnConst.SCENE.ResultScene);
	}

	

	public static void toConfig ()
	{
		//CommercialMng.hideBanner ();
		SceneManagerWrap.loadScene (CmnConst.SCENE.ConfigScene);
	}

	public static void toMaingame ()
	{
		//CommercialMng.hideBanner ();
		SceneManagerWrap.loadScene (CmnConst.SCENE.MainGame);
	}

	public static void screenshot ()
	{
		string img_path =  "screenshot.png";
		ScreenCapture.CaptureScreenshot (img_path);
	}
	


	public static bool chekNetwork ()
	{
		return CmnWwwMng.chekNetwork ();
	}

	public static void openStorePage ()
	{
	}





		

}


