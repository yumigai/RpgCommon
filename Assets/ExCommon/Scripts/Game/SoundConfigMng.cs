using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class SoundConfigMng : MonoBehaviour {

	//スライダーの値
	public Slider sliderBgm;
	public Slider sliderSe;
	public Slider sliderVoice;

	public AudioClip SeCheck;
	public AudioClip voiceCheck;
    public bool AutoSave;

    
    public bool ReadyMusicCheck { get; set; }
    public bool ReadySeCheck{ get; set; }
	public bool ReadyVoiceCheck{ get; set; }

	// Use this for initialization
	protected void Start () {

		sliderBgm.value = SoundMng.SoundConfigData.Instance.BgmVolume;
		sliderSe.value = SoundMng.SoundConfigData.Instance.SeVolume;
		if (sliderVoice) {
			sliderVoice.value = SoundMng.SoundConfigData.Instance.VoiceVolume;
		}

		ReadySeCheck = false;
		ReadyVoiceCheck = false;

	}
	
	// Update is called once per frame
	protected void Update () {
		
		VolumeSet ();

		if (Input.GetMouseButtonUp (0)) {
			if (ReadySeCheck) {
				SoundMng.Instance.playSE (SeCheck);
				ReadySeCheck = false;
			}
			if (ReadyVoiceCheck) {
				SoundMng.Instance.playVoice (voiceCheck);
				ReadyVoiceCheck = false;
			}
            autoSave();

        }
	}

	//スライダーの値を音にセット
	protected void VolumeSet(){

		SoundMng.Instance.setVolume (SoundMng.TYPE.MUSIC, sliderBgm.value);
		SoundMng.Instance.setVolume (SoundMng.TYPE.SE, sliderSe.value);

		if (sliderVoice) {
			SoundMng.Instance.setVolume (SoundMng.TYPE.VOICE, sliderVoice.value);
		}
	}

    public void autoSave()
    {
        if (ReadyMusicCheck || ReadySeCheck || ReadyVoiceCheck)
        {
            if (AutoSave)
            {
                saveSound();
            }
        }
    }

	//ウィンドウを閉じる
	public void CloseWindow(){
        //設定した値をセーブする
        saveSound();
		Destroy (gameObject);
	}

    public static void saveSound()
    {
        SoundMng.SoundConfigData.Instance.SaveSoundConfigData();
    }

    public void changeBgm()
    {
        ReadyMusicCheck = true;
    }

	//サンプルSE再生
	public void changeSe(){
		ReadySeCheck = true;
	}

	//サンプルVOICE再生
	public void changeVoice(){
		ReadyVoiceCheck = true;
	}
		
}
