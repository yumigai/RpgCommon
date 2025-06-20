using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameSceneCmn : MonoBehaviour {

	[SerializeField]
	public AudioClip Music;

	protected void Start(){
		SoundMng.Instance.playBGM (Music);
	}

}
