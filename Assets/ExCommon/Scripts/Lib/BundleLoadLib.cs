using UnityEngine;
using System.Collections;
using System;
using System.Collections.Generic;
using UnityEngine.UI;

public class BundleLoadLib : MonoBehaviour {

	static GameObject g_base_obj;
	static BundleLoadLib instance;
	//static Dictionary<string,Action> g_callbacks;
	static Action g_callback;
	static object g_target;

	public static BundleLoadLib Instance
	{
		get
		{
			if(instance == null)
			{
				instance = (BundleLoadLib) FindObjectOfType(typeof(BundleLoadLib));
				if( instance == null ){
					g_base_obj = new GameObject();
					g_base_obj.name = "BundleLoader";
					instance = g_base_obj.AddComponent<BundleLoadLib>();
				}
			}
			return instance;
		}
	}

	public static void loadSprite ( string path, SpriteRenderer render, string asset_name = "" ){
		Instance.load( path, callbackLoadSprite<SpriteRenderer>, setSprite<SpriteRenderer>, typeof(Sprite), render, asset_name );
	}

	public static void loadSprite ( string path, Image render, string asset_name = "" ){
		Instance.load( path, callbackLoadSprite<Image>, setSprite<Image>, typeof(Sprite), render, asset_name );
	}

	public static void loadVoice( string path, string asset_name = "" ){
		Instance.load( path, callbackLoadVoice, setVoice, typeof(AudioClip), "", asset_name );
	}
	
	public static void loadMusic( string path, AudioSource source, string asset_name = "" ){
		Instance.load( path, callbackLoadBgm, setBgm, typeof(AudioClip), source, asset_name );
	}

	public static void setCallback( Action callback, object target ){
		g_callback = callback;
		g_target = target;
	}

	public static void finish(){
		g_callback = null;
		Destroy (g_base_obj);
	}

	private void load( string path, Action< object, object[] > callbackA, Action< object, string > callbackB, Type type, object tag = null, string asset_name = "" ){
		
		AssetsConnectLib ass = new AssetsConnectLib ();
		
		if ( !AssetsConnectLib.loadLock.ContainsKey(path) || !AssetsConnectLib.loadLock [path]) {
			StartCoroutine( ass.downloadAsset( path, callbackA, type, 1, tag ) );
		} else {
			StartCoroutine( ass.waiteLoadAsset( path, callbackB, type, tag ) );
		}
	}
	
	private static void callbackLoadSprite<T> ( object obj, object[] objs ){
		T t = (T)obj;
		if( t.GetType() == typeof(SpriteRenderer) ){
			SpriteRenderer render = (SpriteRenderer)obj;
			render.sprite = (Sprite)objs [0];
		}else if( t.GetType() == typeof(Image) ){
			Image render = (Image)obj;
			render.sprite = (Sprite)objs [0];
		}
		loadedCallback (obj);
	}

	private static void setSprite<T> ( object obj, string path ){
		T t = (T)obj;
		if( t.GetType() == typeof(SpriteRenderer) ){
			SpriteRenderer render = (SpriteRenderer)obj;
			render.sprite = AssetsConnectLib.getSprites (path)[0];
		}else if( t.GetType() == typeof(Image) ){
			Image render = (Image)obj;
			render.sprite = AssetsConnectLib.getSprites (path)[0];
		}
		loadedCallback (obj);
	}

	private static void callbackLoadVoice( object obj, object[] objs ){
		AudioClip voice = (AudioClip)objs [0];
		SoundMng.Instance.playVoice (voice);
		loadedCallback (obj);
	}
	private static void setVoice ( object obj, string path ){
		AudioClip voice = AssetsConnectLib.getAudioClip (path)[0];
		SoundMng.Instance.playVoice (voice);
		loadedCallback (obj);
	}

	private static void callbackLoadBgm( object obj, object[] objs ){
		AudioSource source = (AudioSource)obj;
		source.clip = (AudioClip)objs [0];
		loadedCallback (obj);
	}
	private static void setBgm ( object obj, string path ){
		AudioSource source = (AudioSource)obj;
		source.clip = AssetsConnectLib.getAudioClip (path)[0];
		loadedCallback (obj);
	}




	public static void loadedCallback( object target ){
		if (g_callback != null && g_target == target ) {
			g_callback ();
		}
	}


}
