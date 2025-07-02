using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class InputNameMng : MonoBehaviour {

    [SerializeField]
    public InputField InputName;

    public static System.Action Callback;

    public void Start()
    {
        InputName.text = SaveMng.Conf.PlayerName;
    }

    public void changeName()
    {
        if(InputName.text == string.Empty)
        {
            CommonProcess.playCanselSe();
            return;
        }

        CommonProcess.playClickSe();
        SaveMng.Conf.PlayerName = InputName.text;
        SaveMng.Conf.save();
        Destroy(this.gameObject);

        if (Callback != null)
        {
            Callback();
        }
    }
}
