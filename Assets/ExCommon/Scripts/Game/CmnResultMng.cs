using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using UnityEngine.Advertisements;

public class CmnResultMng : CmnSceneMng {

	public Image MainImage;
	public Text ResultText;

	public Image CenterImage;

	public GameObject RewardBoard;
	public Image RewardImage;
	public Image RewardEffect;
	public Text RewardText;

	public Sprite DefaultRewardSprite;

	public static string ImgPath{ get; set; }
	public static Sprite ImgSprite{ get; set; }
	public static string IntroPath{ get; set; }
	public static string MusicPath{ get; set; }

	//public static CommercialMng.CMTYPE CmType{ get; set; }
	public static string Message{ get; set; }

	public static Sprite ChangeRewardSprite;
	public static string RewardMessage;

	// Use this for initialization
	void Start () {
		
		setImage (MainImage, ImgSprite, ImgPath);
		setBgm (IntroPath, MusicPath);

		//CommercialMng.CMTYPE show_type = CommercialMng.showCm (CmType);

		ResultText.text = Message;

//		int gra = (int)NendUnityPlugin.Common.Gravity.BOTTOM | (int)NendUnityPlugin.Common.Gravity.CENTER_HORIZONTAL;
//		CommercialMng.changeNendPosition ( gra );

		showReward ();
	}

	public void showReward(){
		//if (CommercialMng.RewardInfos != null && CommercialMng.RewardInfos.Length > 0) {
		//	if (ChangeRewardSprite == null) {
		//		if (RewardImage.sprite != DefaultRewardSprite) {
		//			RewardImage.sprite = DefaultRewardSprite;
		//		}
		//	} else {
		//		RewardImage.sprite = ChangeRewardSprite;
		//	}
		//	CmnAppliListMng.setShow (false);
		//	RewardBoard.SetActive (true);
		//} else {
		//	RewardBoard.SetActive (false);
		//	CmnAppliListMng.setShow (true);
		//}
	}

	public void tapRewardButton(){
		this.GetComponent<Animator> ().SetBool ("IsTap", true);
	}

	public void showMovieAndgetReward(){
        //CommercialMng.showMovie ();
		RewardBoard.SetActive (false);
	}


	public void twitterButton(){
	}

	public void lineButton(){
	}

	public void facebookButton(){
	}

	public void toAftrResult(){
		SceneManagerWrap.loadScene (CmnConst.SCENE.ResultAfterScene);
	}

	public void toTitle(){
		GmCmnMng.toTitle ();
	}

	public void toMainGame(){
		GmCmnMng.toMaingame ();
	}

	public void showOption(){
	}


}
