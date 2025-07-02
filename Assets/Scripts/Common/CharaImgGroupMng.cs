using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class CharaImgGroupMng : MonoBehaviour
{
    [SerializeField]
    public CharaImgGaugeMng UnitPrefab;

    [SerializeField]
    public GamePadListRecivMng InputReciv;

    [SerializeField,Tooltip("表示・非表示切り替え用のベース")]
    public GameObject GroupBase;

    [System.NonSerialized]
    public List<CharaImgGaugeMng> Members = new List<CharaImgGaugeMng>();

    public bool IsActive {
        get {
            return GroupBase != null && GroupBase.activeSelf;
        }
    }
    public static CharaImgGroupMng Singleton {get; private set;}


    private void Awake() {
        if ((typeof(CharaImgGroupMng) == this.GetType())) {
            Singleton = this;
        }
        if (InputReciv == null) {
            InputReciv = this.GetComponent<GamePadListRecivMng>();
        }
        if (GroupBase == null) {
            GroupBase = this.gameObject;
        }

    }

    public void CreateGroup(bool input = false, bool check = false) {
        CreateGroup(SaveMng.GetActiveAllUnits(), input, check);
        //allCheck(check);
    }

    // public void allCheck(bool val){
    //     Members.ForEach(it=>it.check(val));
    // }



    /// <summary>
    /// ユニットアイコングループ作成
    /// </summary>
    /// <param name=“units”></param>
    public void CreateGroup(List<UnitStatusTran> units, bool input = false, bool check = false) {

        ResetGroup();

        for (int i = Members.Count; i < units.Count; i++) {
            CharaImgGaugeMng mng = CreatePlate(UnitPrefab.gameObject);
            mng.UnitTranId = units[i].Id;
            mng.UnitType = units[i].Type;
            mng.setImage(units[i].Img);
            mng.setMax(units[i].MaxHp, units[i].Hp, 0);
            mng.setMax(units[i].MaxMp, units[i].Mp, 1);

            if (mng.Name != null) {
                mng.Name.text = units[i].Name;
            }
            mng.UnitType = units[i].Type;
            mng.check(check);

            Members.Add(mng);
        }

        Members.ForEach(it => it.setPlayerEnemyFrame());

        if (input && InputReciv != null) {
            InputReciv.initSetupWithFrameEnd(true);
        }

        //UnitPrefab.gameObject.SetActive(false);
    }

    /// <summary>
    /// グループ初期化
    /// </summary>
    public void ResetGroup() {
        Members.ForEach(it => Destroy(it.gameObject));
        Members.Clear();
        UnitPrefab.gameObject.SetActive(false);
    }

    /// <summary>
    /// ソート
    /// </summary>
    /// <param name=“units”></param>
    //public void SortGroup(List<UnitStatusTran> units) {

    //    for (int i = 0; i < units.Count; i++) {
    //        Members.Find(it => it.UnitTranId == units[i].Id).transform.SetAsLastSibling();
    //    }
    //}

    /// <summary>
    /// 戦闘不能キャラ削除
    /// </summary>
    /// <param name=“units”></param>
    public void UpdateGroup(List<UnitStatusTran> units) {
        //units.ForEach(it => Destroy(Members.Find(it2 => it.Hp <= 0 && it.Id == it2.UnitTranId)?.gameObject));
        //Members.RemoveAll(it=>it==null); //Destoryは遅れてこの時点ではnullにならない
        if (units != null) {
            foreach (var unit in units) {
                if (unit.Hp <= 0) {
                    RemoveUnit(unit.Id);
                }
            }
        }
    }

    /// <summary>
    /// キャラ削除
    /// </summary>
    /// <param name=“unitTranId”></param>
    public void RemoveUnit(int unitTranId) {
        var index = Members.FindIndex(it => unitTranId == it.UnitTranId);
        if (index >= 0) {
            Destroy(Members[index]?.gameObject);
            Members.RemoveAt(index);
        }
    }



    /// <summary>
    /// キャラプレート作成
    /// </summary>
    /// <param name=“prefab”></param>
    /// <returns></returns>
    protected CharaImgGaugeMng CreatePlate(GameObject prefab) {

        GameObject obj = Instantiate(prefab) as GameObject;
        obj.transform.SetParent(prefab.transform.parent);
        obj.transform.localScale = prefab.transform.localScale;
        obj.transform.localPosition = prefab.transform.localPosition;
        obj.SetActive(true);
        return obj.GetComponent<CharaImgGaugeMng>();
    }

    /// <summary>
    /// HP・MP更新
    /// </summary>
    /// <param name=“units”></param>
    public void RefleshStatus(List<UnitStatusTran> units) {
        for (int i = 0; i < Members.Count; i++) {
            Members[i].gameObject.SetActive(i < units.Count);
            if (i < units.Count) {
                Members[i].setMax(units[i].MaxHp, units[i].Hp, 0);
                Members[i].setMax(units[i].MaxMp, units[i].Mp, 1);
            }

        }
    }
    
    public bool closeWindow() {
        if (IsActive) {
            GroupBase.SetActive(false);
            return false;
        }
        return true;
    }

}
