using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class PartyPlatesMng : MonoBehaviour {

    [SerializeField]
    public CharaPlateMng UnitPrefab;

    //[SerializeField]
    //public GameObject TargetPrefab;

    //[SerializeField]
    //public GameObject TargetParent;

    [System.NonSerialized]
    public List<CharaPlateMng> Members = new List<CharaPlateMng>();

    //[System.NonSerialized]
    //public List<GameObject> Targets = new List<GameObject>();

    public static PartyPlatesMng Singleton { get; private set; }

    virtual public List<UnitStatusTran> Units { get { return SaveMng.Quest.ActiveParty; } }

    private void Awake() {
       if( (typeof(PartyPlatesMng) == this.GetType())) {
            Singleton = this;
        }
    }

    public void CreatePlate( string img_pth ) {

        for (int i = Members.Count; i < Units.Count; i++) {
            CharaPlateMng mng = CreatePlate(UnitPrefab.gameObject);
            mng.setUnit(Units[i]);
            mng.CharaImg.sprite = Units[i].getImage(img_pth);
            mng.Index = i;
            Members.Add(mng);
        }

        UnitPrefab.gameObject.SetActive(false);
    }

    /// <summary>
    /// キャラプレート作成
    /// </summary>
    /// <param name="prefab"></param>
    /// <returns></returns>
    protected CharaPlateMng CreatePlate(GameObject prefab) {

        GameObject obj = Instantiate(prefab) as GameObject;
        obj.transform.SetParent(prefab.transform.parent);
        obj.transform.localScale = prefab.transform.localScale;
        obj.transform.localPosition = prefab.transform.localPosition;

        //GameObject tag = Instantiate(TargetPrefab) as GameObject;
        //tag.transform.SetParent(TargetPrefab.transform.parent);
        //tag.transform.localScale = TargetPrefab.transform.localScale;
        //tag.GetComponent<RectTransform>().position = obj.GetComponent<RectTransform>().position;
        //Targets.Add(tag);

        //obj.SetActive(true);

        return obj.GetComponent<CharaPlateMng>();
    }
	
	public void setData(){
	
		for( int i = 0; i < Members.Count; i++ ){
            if ( i < Units.Count ){
                Members[i].setUnit(Units[i]);
                Members[i].gameObject.SetActive(true);
			}else{
                Members[i].gameObject.SetActive(false);
            }
        }
	}
	
	public void updatePlate(){
		
	}
	
    public CharaPlateMng getMember( int id )
    {
        return Members.Find(it => it.Unit.Id == id);
    }

    public void setCallback(Action<int, CharaPlateMng> act ) {
        Members.ForEach(it => it.Callback = act);
    }
    /// <summary>
    /// ボタン押下・ポインターエンターコールバック
    /// </summary>
    /// <param name="act"></param>
    /// <param name="enter"></param>
    public void setCallback(Action<int, CharaPlateMng> act, Action<int, CharaPlateMng> enter) {
        Members.ForEach(it => it.Callback = act);
        Members.ForEach(it => it.PointEnterCallback = enter);
    }

}