using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ButtonGuidMng : MonoBehaviour
{
    [SerializeField]
    public CmnConfig.GamePadButton DealButton;

    [SerializeField]
    public Image ButtonImage;

    [SerializeField]
    public Text ButtonText;

    [SerializeField]
    public string TextJp;

    [SerializeField]
    public string TextEn;

    private void OnEnable() {
        UpdateInfo(CmnBaseProcessMng.IsGamePad);
    }

    public void UpdateInfo(bool isPad) {
        updateText();
        updateImage(isPad);
    }

    /// <summary>
    /// ボタン表示切り替え
    /// </summary>
    /// <param name="show_button"></param>
    public void updateText() {

        if( ButtonText != null) {
            if (CmnSaveProc.IsJp) {
                ButtonText.text = TextJp;
            } else {
                ButtonText.text = TextEn;
            }
        }
    }

    /// <summary>
    /// 環境別ボタンイメージ更新
    /// </summary>
    /// <param name="DealButton"></param>
    /// <param name="Img"></param>
    public void updateImage(bool isPad) {

        string path = "";
        string key = "";

        if (isPad) {
            key = CmnSaveProc.Key.Buttons[(int)DealButton];
            path = CmnConst.Path.GAMEPAD_IMG + key;
        } else {
            key = CmnSaveProc.Key.KeyBoards[(int)DealButton];
            path = CmnConst.Path.KEYBOARD_IMG + key;
        }
        if (ButtonImage != null && key != null && key.Length > 0) {
            ButtonImage.sprite = Resources.Load<Sprite>(path);
        }
    }

}
