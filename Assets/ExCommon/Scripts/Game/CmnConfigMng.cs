using UnityEngine;
using System.Collections;

public class CmnConfigMng : SoundConfigMng {

	[SerializeField]
	public GameObject CONFIRM_PREFAB;

	[SerializeField]
	public GameObject CreditPrefab;

	private int SecretDebugCount = 0;

    // Use this for initialization
    new void Start () {
		base.Start ();
        if (CreditPrefab)
        {
            CreditPrefab.SetActive(false);
        }
	}

    // Update is called once per frame
    new void Update () {
		base.Update ();
	}

	public void pushToTitle()
	{
        CmnBaseProcessMng.playCanselSe();
        SceneManagerWrap.LoadScene(CmnConst.SCENE.TitleScene);
    }

	public void changeLang( )
	{
		CmnBaseProcessMng.playClickSe();
		CmnBaseProcessMng.selectLang();
	}

	public void pushTutorial(){
		CmnBaseProcessMng.loadSceneWrap (CmnConfig.TUTORIAL_SCENE); 
	}

    public void pushHelp()
    {
        CmnBaseProcessMng.playClickSe();
        SceneManagerWrap.LoadScene(CmnConst.SCENE.HelpScene);
        //CmnBaseProcessMng.loadSceneWrap(CmnConfig.TITLE_SCENE);
    }

	public void deleteSave()
	{
		CmnBaseProcessMng.playClickSe();

        string txt = LanguageStaticTextMng.getLangText("セーブデータを削除しますか？", "May I delete save data?");

		ConfirmWindowCmn.show(CONFIRM_PREFAB, txt, null, deleteSave2);
	}
	public void deleteSave2(object obj)
	{
		CmnBaseProcessMng.playClickSe();

		ConfirmWindowCmn.close();
		string txt = LanguageStaticTextMng.getLangText("本当によろしいですか？", "are you serious?");

		ConfirmWindowCmn.show(CONFIRM_PREFAB, txt, null, deleteSaveExec);
	}

	public static void deleteSaveExec(object obj)
	{
		CmnBaseProcessMng.playErrorSe();

		ConfirmWindowCmn.close();
		CmnSaveProc.resetSave();

		CmnBaseProcessMng.reset();
	}

	public void pushDebug()
	{
		SecretDebugCount++;
		if(SecretDebugCount > 20)
		{
			CmnBaseProcessMng.loadSceneWrap("DebugMode");
			SecretDebugCount = 0;
		}
	}

	public void openCredit(){
		CmnBaseProcessMng.playClickSe();
		CreditPrefab.SetActive (true);
	}
	public void closeCredit(){
		CmnBaseProcessMng.playCanselSe();
		CreditPrefab.SetActive (false);
	}
}
