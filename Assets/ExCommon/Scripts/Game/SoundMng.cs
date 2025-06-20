using UnityEngine;
using System;
using System.Collections;

using System.Collections.Generic;

public class SoundMng : MonoBehaviour
{
	public class SoundConfigData
	{
		private static SoundConfigData instance = null;

		public static SoundConfigData Instance
		{
			get
			{
				if(instance == null)
				{
					instance = new SoundConfigData();
				}
				return instance;
			}
		}

		public float BgmVolume;
		public float SeVolume;
		public float VoiceVolume;
		public bool IsMute;

		public SoundConfigData()
		{
			BgmVolume = 0.5f;
			SeVolume = 0.5f;
			VoiceVolume = 1.0f;
			IsMute = false;
			LoadSoundConfigData();
		}

		public void LoadSoundConfigData()
		{
			string str = PlayerPrefs.GetString(CmnConfig.SAVE_NAME.CMN_SOUND_CONFIG.ToString());
			if (str != null && str != "") {
				string[] configs = str.Split (',');
				BgmVolume = float.Parse(configs[0]);
				SeVolume = float.Parse(configs[1]);
				VoiceVolume = float.Parse(configs[2]);
				IsMute = Convert.ToBoolean (configs [3]);
			}
		}

		public void SaveSoundConfigData()
		{
			string[] configs = { BgmVolume.ToString(), SeVolume.ToString(), VoiceVolume.ToString(), IsMute.ToString() };
			string str = string.Join (",", configs);
			PlayerPrefs.SetString (CmnConfig.SAVE_NAME.CMN_SOUND_CONFIG.ToString(), str);
			PlayerPrefs.Save ();
		}
	}

	public static string MUSIC_PATH = "Sound/Music/";
	public static string SE_PATH = "Sound/Se/";
	public static string VOICE_PATH = "Sound/Voice/";

	public enum TYPE{
		MUSIC,
		VOICE,
		SE
	};

	/// <summary>
	/// フェードインとフェードアウトの実行を重ねる割合です。
	/// 0を指定すると、完全にフェードアウトしてからフェードインを開始します。
	/// 1を指定すると、フェードアウトとフェードインを同時に開始します。
	/// </summary>
	[Range (0f, 1f)]
	public float CrossFadeRatio = 1.0f;
	/// <summary>
	/// フェードイン、フェードアウトにかかる時間です。
	/// </summary>
	[System.NonSerialized]
	public float TimeToFade = 2.0f;
	
	protected static SoundMng instance;
	protected static SoundMng subInstance;
	static GameObject g_base_obj;

	private AudioSource BGMSource;
	private AudioSource SubBGMSource;

	public bool IsBGMLoop { get; private set; }
	public string BGMFileName { get; private set; }
	public string SubBGMFileName { get; private set; }

	private AudioSource g_se_sources;
	private AudioSource g_se_single_sources;
	private AudioSource g_voice_sources;
	
	protected static IDictionary<string,AudioClip> g_sound_effects;
	protected static IDictionary<string,AudioClip> g_voice;

    private List<string> PlayingSe = new List<string>();

	public float SeEndTime{ get; private set; }
	public float VoiceEndTime{ get; private set; }

	static SoundMng()
	{
		//init();
	}

	void Awake()
	{
		IsBGMLoop = false;
		SeEndTime = 0f;
		VoiceEndTime = 0f;

		if(BGMSource == null)
		{
			BGMSource = gameObject.AddComponent<AudioSource> ();
			BGMFileName = null;
		}
		if(SubBGMSource == null)
		{
			SubBGMSource = gameObject.AddComponent<AudioSource> ();
			SubBGMFileName = null;
		}
		if( g_se_sources == null )
		{
			g_se_sources = gameObject.AddComponent<AudioSource>();
		}
		if( g_se_single_sources == null)
		{
			g_se_single_sources = gameObject.AddComponent<AudioSource>();
		}
		if( g_voice_sources == null )
		{
			g_voice_sources = gameObject.AddComponent<AudioSource>();
		}

		// ボリューム設定
		setVolume (TYPE.MUSIC, SoundConfigData.Instance.BgmVolume);
		setVolume (TYPE.SE, SoundConfigData.Instance.SeVolume);
		setVolume (TYPE.VOICE, SoundConfigData.Instance.VoiceVolume);
	}

	public static SoundMng Instance
	{
		get
		{
			if(instance == null)
			{
				init();
				instance = (SoundMng) FindObjectOfType(typeof(SoundMng));
				if( instance == null ){
					g_base_obj = new GameObject();
					g_base_obj.name = "SoundMng";
					instance = g_base_obj.AddComponent<SoundMng>();
					DontDestroyOnLoad (g_base_obj);
				}
			}
			return instance;
		}
	}

	public static SoundMng SubInstance
	{
		get
		{
			if(subInstance == null)
			{
				init();
				subInstance = (SoundMng) FindObjectOfType(typeof(SoundMng));
			}
			return subInstance;
		}
	}
	

	public static void init(){

		if( g_sound_effects == null ){
			g_sound_effects = new Dictionary <string,AudioClip>();
		}

		if( g_voice == null ){
			g_voice = new Dictionary <string,AudioClip>();
		}
	}

    private void Update() {
        PlayingSe.Clear();
    }

    public void playBGM(string FileName, bool IsLoop = true)
	{
		playBGM (null, FileName, IsLoop);
	}
	public void playBGM(string IntroFileName, string LoopFileName, bool IsLoop = true)
	{
		IsBGMLoop = IsLoop;

		if(IntroFileName != null)
		{
			
#if USE_SERVER
			BGMFileName = MUSIC_PATH + IntroFileName + ".unity3d";
			BundleLoader.loadMusic( BGMFileName, BGMSource );
#else
			BGMSource.clip = Resources.Load(MUSIC_PATH + IntroFileName, typeof(AudioClip)) as AudioClip;
#endif
			BGMSource.loop = false;
		}
		if(LoopFileName != null)
		{
			
#if USE_SERVER
			SubBGMFileName = MUSIC_PATH + LoopFileName + ".unity3d";
			BundleLoader.loadMusic( SubBGMFileName, SubBGMSource );
#else
			SubBGMSource.clip = Resources.Load(MUSIC_PATH + LoopFileName, typeof(AudioClip)) as AudioClip;
#endif
			SubBGMSource.loop = IsLoop;
		}

		if( IntroFileName != null){
			playClip( BGMSource, SubBGMSource, true );
		}else{
			playClip( BGMSource, SubBGMSource, false );
		}
	}

	public void replayBGM( bool play_intro = true ){
		playClip( BGMSource, SubBGMSource, play_intro);
	}

	private void playClip( AudioSource intro, AudioSource loop, bool play_intro ){

		float delay = 0f;
		if (play_intro) {
			delay = 1f;
		}

		StartCoroutine( playClip( BGMSource, SubBGMSource, delay ) );
	}

	private IEnumerator playClip( AudioSource intro, AudioSource loop, float delay ){

		while (loop.clip == null)
		{
			yield return null;
		}

		if( delay > 0f )
		{
			intro.Play ();
			float time = intro.clip.length * delay;
			loop.PlayDelayed (time);
		}
		else
		{
			intro.Stop ();
			loop.Play ();
		}
	}

	public void stopBGM()
	{
		BGMSource.Stop ();
		SubBGMSource.Stop ();
	}

	public void playBGM( AudioClip bgm, bool IsLoop = true ){
        if (SubBGMSource.clip == bgm)
        {
            return;
        }
		SubBGMSource.clip = bgm;
		SubBGMSource.loop = IsLoop;
		playClip( BGMSource, SubBGMSource, false );
	}

	public void playBGM( AudioClip intro, AudioClip bgm, bool IsLoop = true ){
        if (SubBGMSource.clip == bgm)
        {
            return;
        }
        BGMSource.clip = intro;
		SubBGMSource.clip = bgm;
		SubBGMSource.loop = IsLoop;
		playClip( BGMSource, SubBGMSource, true );
	}

	public void seekBGM()
	{
		SubBGMSource.time = 10.0f;
	}

	public bool IsPlayBGM()
	{
		return BGMSource.isPlaying || SubBGMSource.isPlaying;
	}

	public void changeFadeBgm( string intro, string loop, bool isloop = true ){
		StartCoroutine (changeFadeBgmProcess (intro,loop,isloop));
	}

	private IEnumerator changeFadeBgmProcess( string intro, string loop, bool isloop ){

		bool is_end = false;

		StartCoroutine (fadeVolume ( TimeToFade, SoundConfigData.Instance.BgmVolume, 0f, (result)=> is_end = result ));

		while (!is_end) {
			yield return null;
		}

		playBGM (intro, loop, isloop);

		StartCoroutine (fadeVolume ( TimeToFade, 0f, SoundConfigData.Instance.BgmVolume, (result)=> is_end = result ));
	}


	private IEnumerator fadeVolume( float timeToFade, float fromVolume, float toVolume, System.Action<bool> callback ){

		float startTime = Time.time;
		for (int i = 0; i < 10000; i++ ) {
			float diff_time = Time.time - startTime;
			if (diff_time > timeToFade) {
				BGMSource.volume = toVolume;
				SubBGMSource.volume = toVolume;
				break;
			}
			float rate = diff_time / timeToFade;
			float vol = Mathf.Lerp (fromVolume, toVolume, rate);
			BGMSource.volume = vol;
			SubBGMSource.volume = vol;

			yield return null;
		}
		callback (true);
	}


	public float playSE( string tag, float delay_time = 0f )
	{
		AudioClip SE = getSE (tag);
		return playSE (SE,delay_time);
	}

	public float playSE( AudioClip se, float delay_time = 0f ){
		if (se == null) {
			return 0f;
		}
		//同時に同じSEは鳴らさない（音量がおかしくなる）
		if (PlayingSe.IndexOf(se.ToString()) >= 0) {
            return 0;
        }
		if (delay_time > 0f) {
			StartCoroutine (playSeDelay(se, delay_time));
		} else {
			g_se_sources.PlayOneShot (se);
		}
		SeEndTime = Time.time + se.length + delay_time;
        PlayingSe.Add(se.ToString());
		return se.length;
	}

	public IEnumerator playSeDelay(AudioClip se, float time){
		yield return new WaitForSeconds (time);
		g_se_sources.PlayOneShot (se);
	}

	public void stopSE()
	{
		g_se_sources.Stop ();
	}
	private AudioClip getSE( string tag )
	{
		if (g_sound_effects.ContainsKey(tag) )
		{
			return g_sound_effects[tag];
		}
		else
		{
			string file_name = SE_PATH + tag;
			return g_sound_effects[tag] = Resources.Load( file_name, typeof(AudioClip)) as AudioClip;
		}
	}

	// SEを単独で再生する。playSEとは違って再生中のSEは停止する。
	public float playSESingle( string tag, bool IsLoop )
	{
		AudioClip SE = getSE (tag);
		
		g_se_single_sources.clip = SE;
		g_se_single_sources.loop = IsLoop;
		g_se_single_sources.Play ();

		return SE.length;
	}
	
	public void stopSESingle()
	{
		g_se_single_sources.Stop ();
	}

	public void playVoice( string tag ){
		playVoice(getVoice(tag));
	}

	public void playVoice( AudioClip voice ){
		if (voice != null) {
			g_voice_sources.PlayOneShot (voice);
			VoiceEndTime = Time.time + voice.length;
		}
	}

	public bool isPlayVoice( ){
		return g_voice_sources.isPlaying;
	}
	public float getVoiceLong(){
		if (g_voice_sources.clip == null) {
			return 0f;
		} else {
			return g_voice_sources.clip.length;
		}
	}

	public void setVoiceEndTime( float val ){
		VoiceEndTime = Time.time + val;
	}

	public bool isEndVoice(){
		return Time.time > VoiceEndTime ? true : false;
	}
	public bool isEndSe(){
		return Time.time > SeEndTime ? true : false;
	}

	private AudioClip getVoice( string tag )
	{
		if (g_voice.ContainsKey(tag) ) {
			return g_voice[tag];
		}else{
			string file_name = VOICE_PATH + tag;
			return g_voice[tag] = Resources.Load( file_name, typeof(AudioClip)) as AudioClip;
		}
	}
	public void stopVoice()
	{
		g_voice_sources.Stop ();
	}

	//-----------------------------------------------------------------------------
	// ボリューム関係
	//-----------------------------------------------------------------------------
	public void setVolume( SoundMng.TYPE type, float val )
	{

		switch( type )
		{
		case TYPE.MUSIC:
			SoundConfigData.Instance.BgmVolume = val;
			if (SoundConfigData.Instance.IsMute) {
				val = 0f;
			}
			BGMSource.volume = val;
			SubBGMSource.volume = val;
			break;
		case TYPE.SE:
			SoundConfigData.Instance.SeVolume = val;
			if (SoundConfigData.Instance.IsMute) {
				val = 0f;
			}
			g_se_sources.volume = val;
			g_se_single_sources.volume = val;
			break;
		case TYPE.VOICE:
			SoundConfigData.Instance.VoiceVolume = val;
			if (SoundConfigData.Instance.IsMute) {
				val = 0f;
			}
			g_voice_sources.volume = val;
			break;
		}
	}

	public void mute(){
//		mute (SoundMng.TYPE.MUSIC);
//		mute (SoundMng.TYPE.SE);
//		mute (SoundMng.TYPE.VOICE);

		SoundConfigData.Instance.IsMute = SoundConfigData.Instance.IsMute ? false : true;
		setVolume (SoundMng.TYPE.MUSIC,SoundConfigData.Instance.BgmVolume);
		setVolume (SoundMng.TYPE.SE,SoundConfigData.Instance.SeVolume);
		setVolume (SoundMng.TYPE.VOICE,SoundConfigData.Instance.VoiceVolume);
		SaveVolume ();
	}
//	public void mute( SoundMng.TYPE type ){
//		SoundConfigData.Instance.IsMute = SoundConfigData.Instance.IsMute ? false : true;
//	}

	public void SaveVolume()
	{
		SoundConfigData.Instance.SaveSoundConfigData ();
	}

	private IEnumerator waitSe( string clip, float time ){
		yield return new WaitForSeconds (time);
		playSE (clip);
	}
	private IEnumerator waitVoice( string clip, float time ){
		yield return new WaitForSeconds (time);
		playVoice (clip);
	}
	private IEnumerator waitSe( AudioClip clip, float time ){
		yield return new WaitForSeconds (time);
		playSE (clip);
	}
	private IEnumerator waitVoice( AudioClip clip, float time ){
		yield return new WaitForSeconds (time);
		playVoice (clip);
	}

	public void orderSesAndVoices( string[] tags, TYPE[] types, float[] intervals ){

		float wait = 0f;

		for (int i = 0; i < tags.Length && i < types.Length; i++) {
			if (i < intervals.Length) {
				wait += intervals [i];
			}
			AudioClip clip = null;
			if (types [i] == TYPE.VOICE) {
				clip = getVoice (tags [i]);
				StartCoroutine (waitVoice (clip,wait));
			} else {
				clip = getSE (tags [i]);
				StartCoroutine (waitSe (clip,wait));
			}

			wait += clip.length;
		}

	}
}
