using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class BaseTitleSceneMng : MonoBehaviour {

    [SerializeField]
    public GameObject CleardImage;

    public void Awake()
    {
        //GameReady.SetActive(false);

    }

    public void Start()
    {
        //if(SaveMng.GameData.StageNum < StageMast.List.Length)
        //{
        //    CleardImage.SetActive(false);
        //}
        //else
        //{
        //    CleardImage.SetActive(true);
        //}


        SoundMng.Instance.stopBGM();

    }

    public void pushStart()
    {
        CommonProcess.playClickSe();

        if (SaveMng.Status.IsFinishOpening) {
            SceneManagerWrap.LoadScene(CmnConst.SCENE.HomeScene, false);
        } else {
            SceneManagerWrap.LoadScene(CmnConst.SCENE.OpeningScene, false);
        }
        //SceneManagerWrap.LoadScene(CmnConst.SCENE.InitScene, false);
        //SceneManagerWrap.LoadScene(CmnConst.SCENE.HomeScene, false);
    }


}