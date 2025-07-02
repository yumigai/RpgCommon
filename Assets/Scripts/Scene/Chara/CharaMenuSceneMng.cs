using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CharaMenuSceneMng : Base2DSceneMng
{
    //[SerializeField]
    //private Text Money;

    new void Start() {
        base.Start();
        //Money.text = SaveMng.Status.Money.ToString();
    }

    public void pushCharaManager() {
        SceneManagerWrap.LoadScene(CmnConst.SCENE.CharaScene);
    }

    public void pushTeamEdit() {
        SceneManagerWrap.LoadScene(CmnConst.SCENE.TeamEditScene);
    }

    public void pushItemManager() {
        ItemBoardMng.OrderMode = ItemBoardMng.MODE.VIEW;
        SceneManagerWrap.LoadScene(CmnConst.SCENE.ItemScene);
    }

    

}
