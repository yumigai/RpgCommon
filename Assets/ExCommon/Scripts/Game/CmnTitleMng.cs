using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class CmnTitleMng : CmnSceneMng {
	
	[SerializeField]
	public Image TitleImage;
	[SerializeField]
	public GameObject OurAppliIcon;
	[SerializeField]
	public bool IsShowReviw = true;

	public static string TitleImgPath{ get; set; }
	public static Sprite TitleImgSprite{ get; set; }
	public static string IntroPath{ get; set; }
	public static string MusicPath{ get; set; }

	// Use this for initialization
	void Start () {
		setImage (TitleImage, TitleImgSprite, TitleImgPath);
		setBgm (IntroPath, MusicPath);

		int upd = PlayerPrefs.GetInt (CmnConfig.SAVE_NAME.CMN_PLAY_COUNT.ToString ());

		showInit();

		UtilToolLib.addSaveInt (CmnConfig.SAVE_NAME.CMN_PLAY_COUNT.ToString (), 1);

	}

	private void showInit(){

		//CommercialMng.changeNendPosition (gra);

		CmnReviewMng.show (false);

		if (IsShowReviw && CmnReviewMng.isREview()) {
			CmnReviewMng.show (true);
		} 
			
	}

	public void toMainGame(){
		GmCmnMng.toMaingame ();
	}

	public void toConfig(){
		GmCmnMng.toConfig ();
	}

}
