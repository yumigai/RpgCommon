using UnityEngine;
using System.Collections;

public class PauseWindowMng : MonoBehaviour {

    public static PauseWindowMng Singleton;

    private void Awake()
    {
        Singleton = this;
    }

    public void Start()
    {
        //NendAdBanner banner = NendUtils.GetBannerComponent("NendAdBanner");
        //if (banner != null)
        //{
        //    NendAdDefaultLayoutBuilder builder = new NendAdDefaultLayoutBuilder();
        //    builder.Gravity(posi);
        //    banner.Layout(builder);
        //}
    }

    protected void Update()
    {
        if (Input.GetKey(KeyCode.Escape))
        {
            pushPause();
        }
    }

    public void pushContinue()
    {
        unPause();
    }

    public void pushGiveUp()
    {
        string txt = LanguageStaticTextMng.getLangText("ギブアップしますか？", "Are you Sure?");
        CommonProcess.showConfirm(txt, giveup);
    }
    public void giveup(object obj)
    {
        //MainProcess.Singleton.giveup();
    }

    public void pushPause()
    {
        if (this.gameObject.activeSelf)
        {
            unPause();
        }
        else
        {
            pause();
        }
    }

    public void pause()
    {
        this.gameObject.SetActive(true);
        Time.timeScale = 0f;

    }

    public void unPause()
    {
        this.gameObject.SetActive(false);
        Time.timeScale = 1f;
    }

}
