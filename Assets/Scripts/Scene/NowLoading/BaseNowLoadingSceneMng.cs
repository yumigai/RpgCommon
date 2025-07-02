using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BaseNowLoadingSceneMng : RandomMessageTextMng {

    public static CmnConst.SCENE NextScene;

	// Use this for initialization
	new protected void Start () {
        base.Start();
        SoundMng.Instance.stopBGM();
        StartCoroutine(jumpNextScene(0.5f));
    }

    IEnumerator jumpNextScene( float time)
    {
        yield return new WaitForSeconds(time);
        
        SceneManagerWrap.LoadScene(NextScene, false);
    }
	
}
