using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class EventEditorMng : MonoBehaviour
{
    [SerializeField]
    public Text SearchEventTag;


    public void pushSearch(){
        var list = EventActionMast.List.Where(it => it.Tag == SearchEventTag.text);
        var judge = list.Where(it => it.Act == EventActionMast.ACTION.JUDGE);
        foreach(var ev in judge)
        {
            Debug.Log(ev.Type.ToString() + "g need" + " taisyou:" + ev.TargetTag + " atai:" + ev.Param ) ;
        }

        //CONFIRM,
        //CONFIRM_HOLD,
        //ADD,
        //REMOVE,
        //SCENARIO,
        //MESSAGE,

    }
}
