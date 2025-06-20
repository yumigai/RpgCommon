using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;
using System.Collections.Generic;

public class SceneManagerWrap : UnityEngine.SceneManagement.SceneManager {

	private const int MAX_BEFORE = 4;
    private const float DEFAULT_FADE_TIME = 1f;
    private const float FADE_SUB_ALPHA = 0.01f;

	public static List<string> BeforeScene = new List<string>();

    public static System.Action LoadedCallback;
	
	public static string NowScene{ get{ return SceneManager.GetActiveScene ().name; }}
    public static string BeforeNearScene { get { return BeforeScene.Count == 0 ? CmnConst.SCENE.HomeScene.ToString() : BeforeScene[BeforeScene.Count - 1]; } }


	public static void loadScene (string sceneName, bool rec_before = true){
	
		string now_scene = NowScene;
		
		if( now_scene.Equals( sceneName ) ){
			return;
		}
		
		Time.timeScale = 1.0f;
		
		if( rec_before && BeforeScene.IndexOf(now_scene) < 0 ){
			BeforeScene.Add(now_scene);
			if( BeforeScene.Count > MAX_BEFORE ){
				BeforeScene.RemoveAt(0);
			}
		}
		
        SceneManager.LoadScene (sceneName);
		
	}

    public static void loadScene(CmnConst.SCENE scene, bool rec_before = true) {
        LoadScene(scene);
    }

    public static void LoadScene( CmnConst.SCENE scene, bool rec_before = true ){
		loadScene (scene.ToString (),rec_before);
	}


	new public static void LoadScene(string sceneName) {
		loadScene(sceneName);
	}

	public static void LoadScene(string sceneName, bool rec_before) {
		loadScene(sceneName, rec_before);
	}

	/// <summary>
	/// ナウローディング付きシーンロード
	/// </summary>
	/// <param name="scene"></param>
	public static void LoadAndNowLoading(CmnConst.SCENE scene) {
		BaseNowLoadingSceneMng.NextScene = scene;
		SceneManagerWrap.LoadScene(CmnConst.SCENE.NowLoadingScene);
	}

	public static void LoadAndFadeOut( MonoBehaviour mono, CmnConst.SCENE scene, bool rec_before = true, float time = DEFAULT_FADE_TIME)
    {
        GameObject canvs = GameObject.FindObjectOfType<Canvas>().gameObject;
        CanvasGroup grp = canvs.GetComponent<CanvasGroup>();
        grp = grp == null ? canvs.AddComponent<CanvasGroup>() : grp;

        mono.StartCoroutine(FadeOut(grp,scene, rec_before,time));
    }

    public static IEnumerator FadeOut(CanvasGroup grp, CmnConst.SCENE scene, bool rec_before, float time)
    {
        for (int i = 0; i < 1000000 && grp.alpha > 0; i++)
        {
            grp.alpha -= FADE_SUB_ALPHA;
            yield return new WaitForSeconds(0.0001f); //描画待たせる
        }
        
        yield return new WaitForSeconds(time);
        LoadScene(scene, rec_before);
       
    }

    //public static void LoadScene(CmnConst.SCENE scene, bool rec_before = true)
    //{
    //    loadScene(scene, rec_before);
    //}

    public static void loadBefore(){
		if( BeforeScene.Count > 0 ){
			string before = BeforeScene[BeforeScene.Count-1];
			BeforeScene.RemoveAt(BeforeScene.Count-1);
			if (before == CmnConst.SCENE.HomeScene.ToString()) {
				//ホームに戻る場合は一旦履歴をリセット
				BeforeScene.Clear();
			}
			if (!NowScheneIs(before)) {
				loadScene(before, false);
			}
		}else{
			LoadScene(CmnConst.SCENE.HomeScene);
		}
	}
	
	public static void reload(){
		Time.timeScale = 1.0f;
		SceneManager.LoadScene (NowScene);
	}

    public static void clearBefore()
    {
        BeforeScene.Clear();
    }

	public static bool NowScheneIs(CmnConst.SCENE scene) {
		return NowScheneIs(scene.ToString());
	}
	public static bool NowScheneIs(string scene) {
		return SceneManager.GetActiveScene().name == scene;
	}
}
