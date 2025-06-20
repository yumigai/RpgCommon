using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class FreeImgAndTxtMng : MonoBehaviour {

    [SerializeField]
    public Image Img;

    [SerializeField]
    public Text Txt;

    [SerializeField]
    public string ImgPath;

    public void setText(string txt, bool is_overwrite = true) {
        if (is_overwrite || txt.Length > 0) {
            Txt.text = txt;
        }
    }
}
