using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HelpMng : MultiUseScrollMng {

    private void Start() {
        this.GetComponent<HelpMng>().makeList(HelpMast.List);
    }

    override public void pushList(MultiUseListMng mng) {
        if (System.Array.IndexOf(HelpMast.List, mng.Id) < 0) {
            return;
        }

        CommonProcess.playClickSe();

    }

}
