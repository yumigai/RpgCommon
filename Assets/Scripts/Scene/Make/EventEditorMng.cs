using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class EventEditorMng : MonoBehaviour
{
    [SerializeField]
    public Text SearchEventTag;
    [SerializeField]
    public Text ListPrefab;

    private List<GameObject> listItems = new List<GameObject>();

    private void Start() {
        EventActionMast.load();
        ListPrefab.gameObject.SetActive(false);
    }

    public void pushSearch(){
        var list = EventActionMast.List.Where(it => it.Tag == SearchEventTag.text);
        var judge = list.Where(it => it.Act == EventActionMast.ACTION.JUDGE);
        var act = list.Where(it => it.Act != EventActionMast.ACTION.JUDGE);
        int count = 1;
        string text = "";

        foreach(var i in listItems) {
            Destroy(i);
        }
        listItems.Clear();

        foreach(var j in judge) {
            if(j.Param != string.Empty) {
                text = string.Format("条件{0}: {1}として{2}が({3})必要", count, j.Type, j.TargetTag, j.Param);
            } else {
                text = string.Format("条件{0}: {1}として{2}が必要", count, j.Type, j.TargetTag);
            }
            count++;
            
            Debug.Log(text);
            setListItem(text);
        }

        count = 1;
        foreach (var a in act) {
            if (a.Param != string.Empty) {
                text = string.Format("動作{0}: {1}を実行。{2}({3})", count, a.Act, a.TargetTag , a.Param);
            } else if(a.TargetTag != string.Empty) {
                text = string.Format("動作{0}: {1}を実行。{2}", count, a. Act, a.TargetTag);
            } else {
                text = string.Format("動作{0}: {1}を実行", count, a.Act);
            }
            count++;
            Debug.Log(text);
            setListItem(text);
        }

        //ADD,
        //REMOVE,
        //SCENARIO,
        //MESSAGE,

    }

    void setListItem(string text) {
        var item = Instantiate(ListPrefab) as Text;
        item.text = text;
        item.transform.SetParent(ListPrefab.transform.parent);
        item.gameObject.SetActive(true);
        listItems.Add(item.gameObject);
    }
}
