using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UserInfoTran : CmnSaveProc.SaveClass {

    public int SelectedGuid;
    public string PlayerName = "イロハ";
    public bool IsFinishMovie = false;

    public string[] KeyConfig = new string[]{
        "Enter",
        "Space",
        "Z",
        "X",
        "C",
        "V",
        "A",
        "S",
        "D",
        "F",
        "Q",
        "W",
    };
    public string[] KeyConfigPad = new string[] {
                CmnConfig.GamePadButton.A.ToString(), //Decision
                CmnConfig.GamePadButton.B.ToString(), //Cancel
                CmnConfig.GamePadButton.A.ToString(),
                CmnConfig.GamePadButton.B.ToString(),
                CmnConfig.GamePadButton.X.ToString(),
                CmnConfig.GamePadButton.Y.ToString(),
                CmnConfig.GamePadButton.L1.ToString(),
                CmnConfig.GamePadButton.L2.ToString(),
                CmnConfig.GamePadButton.L3.ToString(),
                CmnConfig.GamePadButton.R1.ToString(),
                CmnConfig.GamePadButton.R2.ToString(),
                CmnConfig.GamePadButton.R3.ToString(),
                CmnConfig.GamePadButton.Start.ToString(),
                CmnConfig.GamePadButton.Back.ToString(),
    };
}
