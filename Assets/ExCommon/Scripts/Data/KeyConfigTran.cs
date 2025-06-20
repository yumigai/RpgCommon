using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KeyConfigTran : CmnSaveProc.SaveClass {

    public string[] Buttons = new string[(int)CmnConfig.GamePadButton.KeyAll];
    public string[] KeyBoards = new string[(int)CmnConfig.GamePadButton.KeyAll];

    public bool GetKeyDown(CmnConfig.GamePadButton button) {

        if (Input.GetKeyDown(Buttons[(int)button])) {
            return true;
        }
        if (Input.GetKeyDown(KeyBoards[(int)button])) {
            return true;
        }
        return false;
    }
}