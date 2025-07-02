using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 子クラス別実装する場合のために抽象化
/// </summary>
public abstract class Base2DSceneMng : MonoBehaviour
{
    private enum BUTTON_GUID_TYPE
    {
        DEFAULT,
        CUSTOM,
        BUTTON_CLEAR,
    }

    [SerializeField]
    private string TitleJp;

    [SerializeField]
    private string TitleEn;

    [SerializeField]
    private BUTTON_GUID_TYPE ButtonGuidType;

    [SerializeField, Header("ButtonGuidTypeがCUSTOMの場合に使用")]
    private ButtonGuidMng[] GuidButtons;

    [SerializeField]
    private string[] ButtonJps;

    [SerializeField]
    private string[] ButtonEngs;

    [SerializeField, Tooltip("Xボタンを表示するか")]
    private bool IsShowClose = true;

    public System.Action pushCloseProcess;

    protected void Start() {
        BaseProcess();
        init();
    }

    private void init() {

        CommonUIMng.setVisibles(IsShowClose);

        CommonUIMng.setInitInfo(TitleJp, TitleEn);

        if (ButtonGuidSetMng.Singleton != null && ButtonGuidSetMng.Singleton.ButtonGuids != null) {
            switch (ButtonGuidType) {
                case BUTTON_GUID_TYPE.BUTTON_CLEAR:
                System.Array.ForEach(ButtonGuidSetMng.Singleton.ButtonGuids, it => it.gameObject.SetActive(false));
                break;
                case BUTTON_GUID_TYPE.CUSTOM:
                System.Array.ForEach(ButtonGuidSetMng.Singleton.ButtonGuids, it => it.gameObject.SetActive(false));
                foreach (var button in GuidButtons) {
                    button.gameObject.SetActive(true);
                }
                break;
            }

            CommonUIMng.setInitButtons(ButtonJps, ButtonEngs, GuidButtons);
        }
    }

    protected void BaseProcess() {
        if (SaveMng.Status.RestTimeLimit <= 0) {
            StoryListMast.StoryOrder("GameOver");
            BaseStorySceneMng.ReturnSceneOrder = CmnConst.SCENE.TitleScene.ToString();
            SceneManagerWrap.LoadAndFadeOut(this, CmnConst.SCENE.StoryScene, false);
        }
    }

    //public abstract void pushButtonA();

    // public abstract void pushButtonA();
    // public abstract void pushButtonA();
    // public abstract void pushButtonA();
    // public abstract void pushButtonA();
    // public abstract void pushButtonA();
    // public abstract void pushButtonA();
    // public abstract void pushButtonA();

    //public abstract void pushClose();
}
