using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CommonUIMng : MonoBehaviour
{

    [SerializeField]
    private Text Title;

    [SerializeField]
    private Button CloseButton;

    private Base2DSceneMng MainProcess;

    public static CommonUIMng CmmonUiSingleton;

    private void Awake() {
        CmmonUiSingleton = this;
        MainProcess = GameObject.FindObjectOfType<Base2DSceneMng>();
        
    }

    protected void Start() {
    }

    public static void setVisibles( bool close ) {
        CmmonUiSingleton.CloseButton.gameObject.SetActive(close);
    }

    /// <summary>
    /// タイトル設定
    /// </summary>
    /// <param name="jp"></param>
    /// <param name="en"></param>
    public void setTitle(string jp, string en) {
        if (Title != null) {
            Title.text = SaveMng.IsJp ? jp : en;
        }
    }
    /// <summary>
    /// タイトル・ボタン文字設定
    /// </summary>
    /// <param name="jp"></param>
    /// <param name="en"></param>
    public void setButtonNames(string[] btnJp, string[] btnEn, ButtonGuidMng[] guids) {
        if (guids != null) {
            for( int i = 0; i < guids.Length; i++) {

                // Text.textを直接更新しない（ButtonGuidsの更新とバッティングする可能性があるので）
                if ( i < btnJp.Length) {
                    guids[i].TextJp = btnJp[i];
                }
                if( i < btnEn.Length) {
                    guids[i].TextEn = btnEn[i];
                }
                guids[i].updateText();
                
            }
        }
    }

    /// <summary>
    /// 初期表示
    /// </summary>
    /// <param name="title"></param>
    /// <param name="coin"></param>
    /// <param name="coin_show"></param>
    public static void setInitInfo( string jp, string en) {
        if(CmmonUiSingleton!=null) {
            CmmonUiSingleton.setTitle(jp, en);
        }
    }

    /// <summary>
    /// 初期表示(ボタン）
    /// </summary>
    /// <param name="title"></param>
    /// <param name="coin"></param>
    /// <param name="coin_show"></param>
    public static void setInitButtons(string[] btnJp, string[] btnEn, ButtonGuidMng[] guids) {
        if (CmmonUiSingleton != null) {
            CmmonUiSingleton.setButtonNames(btnJp, btnEn, guids);
        }
    }

    /// <summary>
    /// 初期表示
    /// </summary>
    /// <param name="title"></param>
    /// <param name="coin"></param>
    /// <param name="coin_show"></param>
    public static void setInitInfo(string jp, string en, string[] btnJp, string[] btnEn, ButtonGuidMng[] guids ) {
        if (CmmonUiSingleton != null) {
            CmmonUiSingleton.setTitle(jp, en);
            CmmonUiSingleton.setButtonNames(btnJp, btnEn, guids);
        }
    }


    //private void updButtons() {

    //    var controller_names = Input.GetJoystickNames();

    //    bool is_ps = System.Array.IndexOf(controller_names, PS_CONTROLLER) >= 0;

    //    if (ButtonGuids != null && ButtonGuids.Length > 0) {
    //        Transform button_base = ButtonGuids[0].transform.parent;
    //        //検索するより速い
    //        for (int i = 0; i < button_base.childCount; i++) {
    //            button_base.GetChild(i).gameObject.SetActive(false);
    //        }
    //        for (int i = 0; i < ButtonGuids.Length; i++) {
    //            ButtonGuids[i].gameObject.SetActive(true);
    //        }
    //    }
    //}


    /// <summary>
    /// キャンセル共通
    /// </summary>
    public void pushClose() {

        if (MainProcess == null || MainProcess.pushCloseProcess == null) {
            SceneManagerWrap.loadBefore();
        } else {
            MainProcess.pushCloseProcess();
        }
        
    }

}
