using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class StandCharaMng : MonoBehaviour {

	public enum FACE
	{
		Normal,
		Smile,
		Angry,
		Sad,
		Shock,
		Shy,
		All
	}

//	[SerializeField]
//	public string SpriteName;

	[SerializeField]
	public Sprite[] FaceSprites;

	[SerializeField]
	public Image FaceImage;

    //[System.NonSerialized]
    //public string addImgPath;


    private RectTransform ThisRect;

	void Awake(){
		if (FaceImage == null) {
			FaceImage = GetComponent<Image> ();
		}
		if (FaceSprites.Length == 0) {
			FaceSprites = new Sprite[(int)FACE.All];
		}

		ThisRect = this.GetComponent<RectTransform> ();
	}

	public void setPosi( Vector2 posi, bool is_visible = true ){
		if (is_visible) {
			this.gameObject.SetActive (true);
		}
		ThisRect.anchoredPosition = new Vector2 (posi.x, ThisRect.anchoredPosition.y);
	}

	public void hideImage(){
		this.gameObject.SetActive (false);
	}

	public void setFace( string str ){

        if( string.IsNullOrEmpty(str))
        {
            return;
        }

		FACE face = (FACE)System.Enum.Parse(typeof(FACE), str);
//		FACE face = (FACE)str;
		int fi = (int)face;
		if (fi < FaceSprites.Length) {
			if (FaceImage.sprite != FaceSprites [fi]) {
				if (FaceSprites [fi] == null) {
					string path = CmnConst.Path.ADV_IMG_STAND + this.gameObject.name  + str;
					FaceSprites [fi] = Resources.Load<Sprite> (path);
				}
                if (FaceSprites[fi] != null)
                {
                    FaceImage.sprite = FaceSprites[fi];
                }
			}
		}
	}


}
