#if UNITY_EDITOR
using UnityEngine;
using UnityEditor;
using System.Collections;

/// <summary>
/// InputManagerを自動的に設定してくれるクラス
/// </summary>
public class InputManagerSetter {


    /// <summary>
    /// インプットマネージャーを再設定します。
    /// </summary>
    [MenuItem("Util/Reset InputManager")]
    public static void ResetInputManager() {
        Debug.Log("インプットマネージャーの設定を開始します。");
        InputManagerGenerator inputManagerGenerator = new InputManagerGenerator();

        Debug.Log("設定を全てクリアします。");
        inputManagerGenerator.Clear();

        //Debug.Log("プレイヤーごとの設定を追加します。");
        //AddPlayerInputSettings(inputManagerGenerator);
        //for (int i = 0; i < 1; i++) {
        //    AddPlayerInputSettings(inputManagerGenerator, i);
        //}

        Debug.Log("グローバル設定を追加します。");
        AddGlobalInputSettings(inputManagerGenerator);

        Debug.Log("インプットマネージャーの設定が完了しました。");
    }

    /// <summary>
    /// グローバルな入力設定を追加する（OK、キャンセルなど）
    /// </summary>
    /// <param name="inputManagerGenerator">Input manager generator.</param>
    private static void AddGlobalInputSettings(InputManagerGenerator inputManagerGenerator) {

        int joystickNum = 0;

        // 横方向
        {
            var name = CmnConfig.GamePadButton.Horizontal.ToString();
            inputManagerGenerator.AddAxis(InputAxis.CreatePadAxis(name, joystickNum, 1));
            inputManagerGenerator.AddAxis(InputAxis.CreateKeyAxis(name, "a", "d", "left", "right"));
            inputManagerGenerator.AddAxis(InputAxis.CreatePadAxis(name, joystickNum, 6));
        }
        //inputManagerGenerator.AddAxis(InputAxis.CreatePadAxis(CmnConfig.GamePadButton.HorizontalRaw.ToString(), joystickNum, 6)); //十字キーはアナログパッドとは別物として扱う

        // 縦方向
        {
            var name = CmnConfig.GamePadButton.Vertical.ToString();
            inputManagerGenerator.AddAxis(InputAxis.CreatePadAxis(name, joystickNum, 2, true));
            inputManagerGenerator.AddAxis(InputAxis.CreateKeyAxis(name, "s", "w", "down", "up"));
            inputManagerGenerator.AddAxis(InputAxis.CreatePadAxis(name, joystickNum, 7));
        }
        //inputManagerGenerator.AddAxis(InputAxis.CreatePadAxis(CmnConfig.GamePadButton.VerticalRaw.ToString(), joystickNum, 7)); //十字キーはアナログパッドとは別物として扱う

        // ポーズ
        {
            inputManagerGenerator.AddAxis(InputAxis.CreateButton("Pause", "Escape", "joystick button 7"));
        }
        // 決定
        {
            inputManagerGenerator.AddAxis(InputAxis.CreateButton("Submit", "z", "joystick button 0"));
        }

        // ボタン
        {
            inputManagerGenerator.AddAxis(InputAxis.CreateButton(CmnConfig.GamePadButton.Decision.ToString(), "joystick button 0", "z"));
            inputManagerGenerator.AddAxis(InputAxis.CreateButton(CmnConfig.GamePadButton.Cancel.ToString(), "joystick button 1", "x"));
            inputManagerGenerator.AddAxis(InputAxis.CreateButton(CmnConfig.GamePadButton.A.ToString(), "joystick button 0", "z"));
            inputManagerGenerator.AddAxis(InputAxis.CreateButton(CmnConfig.GamePadButton.B.ToString(), "joystick button 1", "x"));
            inputManagerGenerator.AddAxis(InputAxis.CreateButton(CmnConfig.GamePadButton.X.ToString(), "joystick button 2", "c"));
            inputManagerGenerator.AddAxis(InputAxis.CreateButton(CmnConfig.GamePadButton.Y.ToString(), "joystick button 3", "v"));
            inputManagerGenerator.AddAxis(InputAxis.CreateButton(CmnConfig.GamePadButton.L1.ToString(), "joystick button 4", "a"));
            inputManagerGenerator.AddAxis(InputAxis.CreateButton(CmnConfig.GamePadButton.R1.ToString(), "joystick button 5", "s"));
            inputManagerGenerator.AddAxis(InputAxis.CreateButton(CmnConfig.GamePadButton.Back.ToString(), "joystick button 6", "d"));
            inputManagerGenerator.AddAxis(InputAxis.CreateButton(CmnConfig.GamePadButton.Start.ToString(), "joystick button 7", "f"));
            inputManagerGenerator.AddAxis(InputAxis.CreateButton(CmnConfig.GamePadButton.L3.ToString(), "joystick button 8", "q"));
            inputManagerGenerator.AddAxis(InputAxis.CreateButton(CmnConfig.GamePadButton.R3.ToString(), "joystick button 9", "w"));
        }
    }

    /// <summary>
    /// キーボードでプレイした場合、割り当たっているキーを取得する
    /// </summary>
    /// <param name="upKey">Up key.</param>
    /// <param name="downKey">Down key.</param>
    /// <param name="leftKey">Left key.</param>
    /// <param name="rightKey">Right key.</param>
    /// <param name="attackKey">Attack key.</param>
    /// <param name="playerIndex">Player index.</param>
    private static void GetAxisKey(out string upKey, out string downKey, out string leftKey, out string rightKey, out string attackKey, int playerIndex) {
        upKey = "";
        downKey = "";
        leftKey = "";
        rightKey = "";
        attackKey = "";

        switch (playerIndex) {
            case 0:
                upKey = "w";
                downKey = "s";
                leftKey = "a";
                rightKey = "d";
                attackKey = "e";
                break;
            case 1:
                upKey = "i";
                downKey = "k";
                leftKey = "j";
                rightKey = "l";
                attackKey = "o";
                break;
            case 2:
                upKey = "up";
                downKey = "down";
                leftKey = "left";
                rightKey = "right";
                attackKey = "[0]";
                break;
            case 3:
                upKey = "[8]";
                downKey = "[5]";
                leftKey = "[4]";
                rightKey = "[6]";
                attackKey = "[9]";
                break;
            default:
                Debug.LogError("プレイヤーインデックスの値が不正です。");
                upKey = "";
                downKey = "";
                leftKey = "";
                rightKey = "";
                attackKey = "";
                break;
        }
    }
}

#endif