using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BookSceneCmn : MonoBehaviour {


    public void pushList(MultiUseListMng mng)
    {
        CommonProcess.showMessage(mng.Name.text, mng.Detail.text);
    }


}
