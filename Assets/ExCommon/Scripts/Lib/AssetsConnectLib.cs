using UnityEngine;
using System.Collections;
using System;
using System.Collections.Generic;
using System.Linq;

public class AssetsConnectLib  {

	public static string ResourceHost{ get; set; }
	
	private static Dictionary<string,object[]> Cache = new Dictionary<string, object[]> ();
	private static Dictionary<string, AssetBundle> assetBundleCache = new Dictionary<string, AssetBundle>();
	public static Dictionary<string, bool> loadLock = new Dictionary<string, bool> ();

	private static Dictionary<string, Sprite[]> loadedSprite = new Dictionary<string, Sprite[]> ();
		
	public IEnumerator downloadAsset ( string url, Action< object, object[] > callback, Type type, int version = 1, object tag = null, string asset_name = "" ){

		loadLock[url] = true;

		AssetBundle bundle;
		object[] objs;

		if (assetBundleCache.ContainsKey (url) && assetBundleCache [url] != null) {
			bundle = assetBundleCache[url];
		} else {

#if !(UNITY_WEBGL && UNITY_2022_1_OR_NEWER)
			while (!Caching.ready)
				yield return null;
#endif

			using (WWW www = WWW.LoadFromCacheOrDownload ( ResourceHost + url, version )) {

				yield return www;

				if (www.error != null) {
					throw new Exception (www.error + " : path " + url);
				}
				bundle = www.assetBundle;
				assetBundleCache[url] = bundle;

				if (bundle == null) {
					throw new Exception ("bundle loassetad error");
				}
				
				if (asset_name == "") {
					objs = bundle.LoadAllAssets (type);
				} else {
					objs = new object[1];
					objs [0] = bundle.LoadAsset (asset_name);
				}
				Cache [url] = objs;
		
				
			}
		}



		if (bundle == null) {
			throw new Exception ("bundle loassetad error");
		}
		
		if (asset_name == "") {
			objs = bundle.LoadAllAssets (type);
		} else {
			objs = new object[1];
			objs [0] = bundle.LoadAsset (asset_name);
		}

		Cache [url] = objs;

		bundle.Unload (false);

		callback (tag, Cache [url]);

		loadLock[url] = false;

	}
	
	public static void clearCache( string url ){
		assetBundleCache [url].Unload (false);
		assetBundleCache [url] = null;
	}

	public static Sprite[] getSprites ( string path ){

		if( !loadedSprite.ContainsKey( path ) ){

			Sprite[] sps = new Sprite[Cache [path].Length];
			for (int i = 0; i < sps.Length; i++ ) {
				sps[i] = (Sprite)Cache [path][i];
			}
			
			loadedSprite[path] = sps;
		}

		return loadedSprite[path];

	}

	public static AudioClip[] getAudioClip ( string path ){
		return (AudioClip[])Cache [path].Cast<AudioClip>();
	}
	

	public IEnumerator waiteLoadAsset( string url, Action< object, string> callback, Type type, object tag = null, string asset_name = ""  ){
		while (loadLock[url])
			yield return null;

		callback (tag,url);
	}
}
