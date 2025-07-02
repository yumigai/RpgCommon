using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BaseEndingSceneMng : MonoBehaviour {

    [SerializeField]
    public AudioClip Bgm;

    [System.NonSerialized]
    public bool IsFinsih = false;

    public void Start()
    {
        SoundMng.Instance.playBGM(Bgm, false);
    }

    public void endScroll()
    {
        IsFinsih = true;
    }

    public void pushScroll()
    {
        if (IsFinsih)
        {
            CommonProcess.playClickSe();
            SceneManagerWrap.LoadScene(CmnConst.SCENE.TitleScene);
        }
    }
}
