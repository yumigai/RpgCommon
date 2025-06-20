using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class MultiUseScrollMng : MonoBehaviour {

    [SerializeField]
    public string IconDirectory;
	
	[SerializeField]
	public GameObject ListItem;

    [System.NonSerialized]
    public List<MultiUseListMng> ItemList = new List<MultiUseListMng>();

    //	public static MultiUseScrollMng Singleton;

    void Awake(){
        ListItem.SetActive(false);
    }

    public List<MultiUseListMng> makeList( MulitiUseListMast[] list ){

        clear();
	
		for( int i = 0; i < list.Length; i++ ){
            makeListItemMasterId( i, list[i] );
		}

        return ItemList;
	}

    /// <summary>
    /// IDを独自採番する（マスタによるIDではなくトランザクションのIDを使用）
    /// </summary>
    /// <param name="i"></param>
    /// <param name="id"></param>
    /// <param name="mst"></param>
    /// <param name="button_txt"></param>
    /// <param name="btn_state"></param>
    /// <returns></returns>
    public MultiUseListMng makeListItem(int i, int id, MulitiUseListMast mst, string button_txt = "選択", MultiUseListMng.BUTTON btn_state = MultiUseListMng.BUTTON.SHOW) {
        MultiUseListMng mng = makeListItemMasterId(i, mst, button_txt, btn_state);
        mng.Id = id;
        return mng;
    }

    /// <summary>
    /// マスターIDで採番する
    /// </summary>
    /// <param name="i"></param>
    /// <param name="mst"></param>
    /// <param name="button_txt"></param>
    /// <param name="btn_state"></param>
    /// <returns></returns>
    public MultiUseListMng makeListItemMasterId( int i, MulitiUseListMast mst, string button_txt = "選択", MultiUseListMng.BUTTON btn_state = MultiUseListMng.BUTTON.SHOW ){

        MultiUseListMng mng = makeListItem(mst.Id, mst.Name, mst.Icon, mst.Detail);
        mng.Index = i;
        //      mng.Id = mst.Id;
        //mng.Name.text = mst.Name;
        //if (mng.Detail != null) {
        //	mng.Detail.text = mst.Detail;
        //}
        //mng.setIcon(ICON_PATH + mst.Icon);

        if ( mng.ButtonTxt != null ){
			mng.ButtonTxt.text = button_txt;
		}
        
		mng.setButton( btn_state );
        mng.ImagePath = mst.ImagePath;
		mng.Callback = pushList;
        
        return mng;
	}

    public MultiUseListMng makeListItem(int id, string name, string icon = "", string detail = "", string value = "", UnityAction<int> action = null) {

        MultiUseListMng mng = makeListItem();
        mng.Id = id;

        if (mng.Name != null) {
            mng.Name.text = name;
        }
        if(icon != null && icon.Length > 0) {
            mng.setIcon(IconDirectory + icon);
        }
        if (mng.Detail != null) {
            mng.Detail.text = detail;
        }
        if(mng.Value != null) {
            mng.Value.text = value;
        }
        if(mng.Btn != null && action != null) {
            mng.SetButtonInvoke(action);
        }

        return mng;
    }

    public MultiUseListMng makeListItem(){
		GameObject item = Instantiate( ListItem ) as GameObject;
        item.SetActive(true);
        item.transform.SetParent( ListItem.transform.parent );
		item.transform.localScale = ListItem.transform.localScale;
        MultiUseListMng mng = item.GetComponent<MultiUseListMng>();
        ItemList.Add(mng);
        return mng;

	}
	
	virtual public void pushList( MultiUseListMng mng ){}

    /// <summary>
    /// リストクリア
    /// </summary>
    public void clear() {
        ListItem.SetActive(false);
        ItemList.ForEach(it => it.gameObject.SetActive(false));
        ItemList.ForEach(it => Destroy(it.gameObject));
        ItemList.Clear();
    }

    /// <summary>
    /// アクティブ状態のカウント
    /// </summary>
    /// <returns></returns>
    public int activeCount() {
        return ItemList.Where(it => it.gameObject.activeSelf && it.Btn.interactable).Count();
    }
}