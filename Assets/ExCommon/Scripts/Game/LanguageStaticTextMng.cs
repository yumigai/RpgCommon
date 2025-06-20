using UnityEngine;
using System.Collections;
using UnityEngine.UI;

[DisallowMultipleComponent]
public class LanguageStaticTextMng : MonoBehaviour {

    [SerializeField]
    public Text TargetTxt;

	[SerializeField][Multiline]
    public string JpText;

	[SerializeField][Multiline]
    public string EngText;

    public static bool IsJp { get { return CmnSaveProc.IsJp; } }


    // Use this for initialization
    void Start () {

		setTextComponent ();
        if (TargetTxt != null)
        {
            TargetTxt.text = getLangText();
        }
	}

	protected void setTextComponent(){
		if (CmnSaveProc.Conf != null) {
			if (TargetTxt == null) {
				TargetTxt = this.GetComponent<Text> ();
			}
		}
	}

	public string getLangText(){
		if( CmnSaveProc.Conf.SelectLang == (int)CmnSaveProc.GameConfig.LANG.JP){
			return JpText;
		}else{
			return EngText;
		}
	}

	public static string getLangText( string jp, string eng ){
		if( CmnSaveProc.Conf.SelectLang == (int)CmnSaveProc.GameConfig.LANG.JP){
			return jp;
		}else{
			return eng;
		}
	}
	
}
