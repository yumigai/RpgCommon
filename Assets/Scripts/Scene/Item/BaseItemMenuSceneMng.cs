using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BaseItemMenuSceneMng : MonoBehaviour {

	// Use this for initialization
	void Start () {
		
	}
	

    public void pushView()
    {
        BaseItemListSceneMng.OrderMode = ItemBoardMng.MODE.VIEW;
        jumpScene(CmnConst.SCENE.ItemScene);
    }

    public void pushSell()
    {
        BaseItemListSceneMng.OrderMode = ItemBoardMng.MODE.SELL;
        BaseItemListSceneMng.OrderCategory = ItemMast.CATEGORY.MATERIAL;
        jumpScene(CmnConst.SCENE.ItemScene);
    }

    public void pushShop()
    {
        jumpScene(CmnConst.SCENE.ShopScene);
    }

    public void pushEquipUp()
    {
        BaseItemListSceneMng.OrderMode = ItemBoardMng.MODE.POWER_UP;
        jumpScene(CmnConst.SCENE.ItemScene);
    }

    //void pushShopUp()
    //{
        
    //}

    void jumpScene( CmnConst.SCENE scene )
    {
        CommonProcess.playClickSe();
        SceneManagerWrap.LoadScene(scene);
    }
}
