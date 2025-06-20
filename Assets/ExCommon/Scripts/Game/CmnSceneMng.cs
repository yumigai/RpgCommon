using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class CmnSceneMng : MonoBehaviour {

	public string DefaultIntro;
	public string DefaultMusic;

	public void setImage( Image img, Sprite sp, string sp_path ){
		if (sp != null) {
			img.sprite = sp;
		} else if (sp_path != null) {
			img.sprite = Resources.Load<Sprite> (sp_path);
		}
	}

	public void setBgm(string intro, string music ){

		if (string.IsNullOrEmpty(intro) && string.IsNullOrEmpty(music) ) {
			intro = DefaultIntro;
			music = DefaultMusic;
		}

		if ( string.IsNullOrEmpty( intro ) ) {
			SoundMng.Instance.playBGM (music);
		} else {
			SoundMng.Instance.playBGM (intro, music );
		}
	}

}



