using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class KeyConfigSceneMng : MonoBehaviour {

    [SerializeField]
    private Text[] PadInfos;

    [SerializeField]
    private Text[] KeyboardInfos;

    private string[] BackupKey = new string[SaveMng.Key.KeyBoards.Length];
    private string[] BakupPad = new string[SaveMng.Key.Buttons.Length];

    private readonly CmnConfig.GamePadButton[] InputTargetKeys = new CmnConfig.GamePadButton[] {
        CmnConfig.GamePadButton.A,
        CmnConfig.GamePadButton.B,
        CmnConfig.GamePadButton.X,
        CmnConfig.GamePadButton.Y,
        CmnConfig.GamePadButton.L1,
        CmnConfig.GamePadButton.R1,
    };

    private readonly Dictionary<string, CmnConfig.GamePadButton> JoyKeyOriginal = new Dictionary<string, CmnConfig.GamePadButton>() {
        {"JoystickButton0", CmnConfig.GamePadButton.A},
        {"JoystickButton1", CmnConfig.GamePadButton.B},
        {"JoystickButton2", CmnConfig.GamePadButton.X},
        {"JoystickButton3", CmnConfig.GamePadButton.Y},
        {"JoystickButton4", CmnConfig.GamePadButton.L1},
        {"JoystickButton5", CmnConfig.GamePadButton.R1},
        {"JoystickButton6", CmnConfig.GamePadButton.Back},
        {"JoystickButton7", CmnConfig.GamePadButton.Start},
        {"JoystickButton8", CmnConfig.GamePadButton.L3},
        {"JoystickButton9", CmnConfig.GamePadButton.R3},
    };

    // Use this for initialization
    void Start () {
        initSetting();
    }

    void initSetting() {
        SaveMng.Key.KeyBoards.CopyTo(BackupKey,0);
        SaveMng.Key.Buttons.CopyTo(BakupPad,0);
        updateKeyInfo();
    }

    void updateKeyInfo() {

        for (int i = 0; i < InputTargetKeys.Length; i++) {
            PadInfos[i].text = SaveMng.Key.Buttons[(int)InputTargetKeys[i]];
            KeyboardInfos[i].text = SaveMng.Key.KeyBoards[(int)InputTargetKeys[i]];
        }
    }
	
	// Update is called once per frame
	void Update () {
        keyChange();
    }

    /// <summary>
    /// キー変更
    /// </summary>
    public void keyChange() {
        if (Input.anyKeyDown) {

            if (GamePadListRecivMng.ActiveGamePadList.NowSelector >= InputTargetKeys.Length) {
                return;
            }

            int index = (int)InputTargetKeys[GamePadListRecivMng.ActiveGamePadList.NowSelector];

            foreach (KeyCode code in System.Enum.GetValues(typeof(KeyCode))) {
                if (Input.GetKeyDown(code)) {
                    string code_str = code.ToString();
                    if (KeyCode.JoystickButton0 <= code && code <= KeyCode.Joystick1Button19) {
                        if (JoyKeyOriginal.ContainsKey(code_str)) {
                            SaveMng.Key.Buttons[index] = JoyKeyOriginal[code_str].ToString();
                        }
                    }
                    if (KeyCode.None <= code && code <= KeyCode.KeypadEquals) {
                        SaveMng.Key.KeyBoards[index] = code_str;
                    }
                }
            }
            updateKeyInfo();
        }
    }

    /// <summary>
    /// 押下キーコード検索
    /// </summary>
    /// <returns></returns>
    private string getKeyCode() {

        foreach (KeyCode code in System.Enum.GetValues(typeof(KeyCode))) {

            if (Input.GetKeyDown(code)) {
                if (KeyCode.JoystickButton0 <= code && code <= KeyCode.Joystick1Button19) {
                    return code.ToString();
                }
                if (KeyCode.None <= code && code <= KeyCode.Mouse6) {
                    return code.ToString();
                }
            }
        }
        return "";
    }

    public void KeyConfigUpdate(int key_index) {
        SaveMng.Key.KeyBoards[key_index] = getKeyCode();
    }

    public void pushSave() {
        SaveMng.Conf.save();
        SceneManagerWrap.loadBefore();
    }

    public void pushCansel() {
        SaveMng.Key.KeyBoards = BackupKey;
        SaveMng.Key.Buttons = BakupPad;
        SceneManagerWrap.loadBefore();
    }

}
