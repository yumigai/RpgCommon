using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Linq;

/// <summary>
/// ControllGuidのprefabで使用している
/// </summary>
public class ButtonGuidSetMng : MonoBehaviour
{
    public const string PS_CONTROLLER = "Wireless Controller";

    [SerializeField]
    public ButtonGuidMng[] ButtonGuids;

    //GamePadButtonMngが必ず一対一とは限らないので、参照しない
    //（リスト化されてたり、その関係で無かったりするから）
    //[SerializeField] 
    //public GamePadButtonMng[] GamePads;

    public static bool IsPad = false;

    public static ButtonGuidSetMng Singleton;

    private void Awake() {
        Singleton = this;
    }

    // Start is called before the first frame update
    void Start()
    {
        updateButtons();
    }

    /// <summary>
    /// ボタンセットアップ
    /// </summary>
    /// <param name="deals"></param>
    /// <param name="textJp"></param>
    public static void initButton() {
        Singleton.updateButtons();
    }

    /// <summary>
    /// ボタン表示切り替え
    /// </summary>
    /// <param name="show_button"></param>
    private void updateButtons() {

        string[] controller_names = Input.GetJoystickNames();

        //bool is_ps = System.Array.IndexOf(controller_names, PS_CONTROLLER) >= 0;
        IsPad = System.Array.Exists(controller_names, it => it.Length > 0);

        for (int i = 0; i < ButtonGuids.Length; i++) {
            ButtonGuids[i].UpdateInfo(IsPad);
        }
        //StartCoroutine(updatePadButtonWait());
    }


    /// <summary>
    /// レイアウトが崩れるため１フレーム待つ
    /// </summary>
    /// <returns></returns>
    //public IEnumerator updatePadButtonWait() {

    //    if (ButtonGuids.Length > 0) {
    //        yield return new WaitForEndOfFrame();
    //        ButtonGuids[0].transform.SetAsLastSibling();
    //        yield return new WaitForEndOfFrame();
    //        ButtonGuids[0].transform.SetAsFirstSibling();
    //        System.Array.ForEach(ButtonGuids, it => it.transform.localScale = Vector3.one);
    //    }
    //}

#if UNITY_EDITOR
    //protected void OnValidate() {
    //    if (ButtonGuids.Length > 0) {
    //        ButtonGuids[0].transform.SetAsLastSibling();
    //        ButtonGuids[0].transform.SetAsFirstSibling();
    //        System.Array.ForEach(ButtonGuids, it => it.transform.localScale = Vector3.one);
    //    }
    //}
#endif

}
