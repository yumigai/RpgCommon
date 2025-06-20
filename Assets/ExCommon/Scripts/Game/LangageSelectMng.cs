using UnityEngine;
using System.Collections;

public class LangageSelectMng : MonoBehaviour {

	private bool IsScene = true;

    public static void show(GameObject prefab, Transform pare)
    {
        GameObject obj = Instantiate(prefab) as GameObject;
        obj.transform.parent = pare;
        obj.transform.localPosition = new Vector3();
        obj.transform.localScale = prefab.transform.localScale;
		LangageSelectMng mng = obj.GetComponent<LangageSelectMng> ();
		mng.IsScene = false;
    }

    public void pushJp()
    {
		changeLang(CmnSaveProc.GameConfig.LANG.JP);
    }
    public void pushEn()
    {
		changeLang(CmnSaveProc.GameConfig.LANG.ENG);
    }

	public void changeLang(CmnSaveProc.GameConfig.LANG lang)
    {
		CmnBaseProcessMng.playClickSe ();
		CmnSaveProc.Conf.SelectLang = (int)lang;
		CmnSaveProc.Conf.Standby = true;
		CmnSaveProc.saveConfig();
		if (IsScene) {
			SceneManagerWrap.loadBefore ();
		} else {
			Destroy (this.gameObject);
		}
    }
}
