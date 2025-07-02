using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class FaceMessageMng : MessageAnimeCmn
{

    [SerializeField]
    public Image IconImage;
    [SerializeField]
    public Text NamePlate;

    [SerializeField]
    public Text KanaPlate;

    [SerializeField]
    public GameObject IconImageBase;
    [SerializeField]
    public GameObject NamePlateBase;

    new public static FaceMessageMng Singleton;

    new private void Awake() {
        Singleton = this;
    }

    public void changeIconImage(string path) {
        if (IconImage != null) {
            bool is_show = !string.IsNullOrEmpty(path);
            CmnBaseProcessMng.activeObject(IconImageBase.gameObject, is_show);
            if (is_show) {
                changeCommonImage(path, CmnConst.Path.IMG_CHARA_ICON, IconImage);
            }
        }
    }
    public void changeNamePlate(string name, string kana = "") {
        if (NamePlate != null) {
            bool is_show = !string.IsNullOrEmpty(name);
            CmnBaseProcessMng.activeObject(NamePlateBase.gameObject, is_show);
            if (is_show) {
                NamePlate.text = name;
            }
        }
        if (KanaPlate != null) {
            bool is_show = !string.IsNullOrEmpty(kana);
            KanaPlate.gameObject.SetActive(is_show);
            KanaPlate.text = kana;
        }
    }
    protected void changeCommonImage(string path, string dire, Image target) {
        if (string.IsNullOrEmpty(path)) {
            if (target != null) {
                target.gameObject.SetActive(false);
            }
        } else {
            if (!target.gameObject.activeSelf) {
                target.gameObject.SetActive(true);
            }
            if (target.sprite == null || target.sprite.name != path) {
                string img_path = dire + path;
                Sprite sp = Resources.Load<Sprite>(img_path);
                target.sprite = sp;
            }
        }
    }

}
