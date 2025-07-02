using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BaseOpeningSceneMng : MonoBehaviour
{

    [SerializeField]
    public InputNameMng InputName;

    [SerializeField]
    public AudioClip Bgm;


    private bool IsFinish = false;

    private void Start()
    {
        SoundMng.Instance.playBGM(Bgm);
    }

    private void Update()
    {
        if(IsFinish)
        {
            IsFinish = false;
            StartCoroutine( nextScene() );
        }
    }

    public void movieEnd()
    {
        if( InputName == null) {
            IsFinish = true;
        } else {
            InputNameMng.Callback = inputNameEnd;
            initInput(InputName.gameObject);
        }
    }

    private void initInput( GameObject prefab)
    {
        GameObject obj = Instantiate(prefab) as GameObject;
        obj.transform.parent = this.transform;
        obj.transform.localPosition = prefab.transform.localPosition;
        obj.transform.localScale = prefab.transform.localScale;
    }

    public void inputNameEnd()
    {
        IsFinish = true;
        
    }

    public IEnumerator nextScene()
    {
        yield return new WaitForSeconds(0.2f);

        //ステータス更新
        SaveMng.Status.IsFinishOpening = true;
        StoryListMast.StoryOrder("Opening");

        BaseStorySceneMng.ReturnSceneOrder = CmnConst.SCENE.HomeScene.ToString();
        SceneManagerWrap.LoadAndFadeOut(this, CmnConst.SCENE.StoryScene,false);
    }
}
