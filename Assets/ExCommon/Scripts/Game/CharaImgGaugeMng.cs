using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class CharaImgGaugeMng : ListItemMng
{
    public enum IMG_TYPE
    {
        ICON,
        LINE,
        BUSTUP,
        STAND,
        OTHER
    }

    [SerializeField]
    public Text Name;

    [SerializeField]
    public UnitMast.TYPE UnitType;

    [SerializeField]
    private IMG_TYPE ImgType;

    [SerializeField, Tooltip("画像を枠内に収める場合")]
    private FrameImageMng CharaFrame;

    [SerializeField]
    public Image CharaImg;

    [SerializeField]
    public GaugeBarMng[] Gauges;

    [SerializeField, Header("Gaugesを使用しない数値のみの場合、こちらを使用")]
    public Text[] Params;

    [SerializeField]
    public Color DeadColor;

    [SerializeField]
    private GameObject[] UnitFrames; //ユニットの所属フレーム（敵・味方など）

    [System.NonSerialized]
    public int UnitTranId;

    public GaugeBarMng Gauge {
        get {
            return Gauges.Length > 0 ? Gauges[0] : null;
        }
    }
    public GaugeBarMng Gauge2 {
        get {
            return Gauges.Length > 1 ? Gauges[1] : null;
        }
    }
    public GaugeBarMng Gauge3 {
        get {
            return Gauges.Length > 2 ? Gauges[2] : null;
        }
    }

    public void Init(string img, int max, int val, int index = 0) {
        setImage(img);
        setMax(max, val, index);
    }

    /// <summary>
    /// 画像設定
    /// </summary>
    /// <param name="img"></param>
    public void setImage(string path) {

        string img_path = "";
        if (UnitType == UnitMast.TYPE.PLAYER) {
            switch (ImgType) {
                case IMG_TYPE.ICON:
                img_path = CmnConst.Path.IMG_CHARA_ICON + path;
                break;
                case IMG_TYPE.LINE:
                img_path = CmnConst.Path.IMG_CHARA_LINE + path;
                break;
                case IMG_TYPE.BUSTUP:
                img_path = CmnConst.Path.IMG_CHARA_BUSTUP + path;
                break;
                case IMG_TYPE.STAND:
                img_path = CmnConst.Path.IMG_CHARA_STAND + path;
                break;
                case IMG_TYPE.OTHER:
                img_path = path;
                break;
            }
        } else {
            img_path = CmnConst.Path.IMG_ENEMY + path;
        }
        if (path.Length == 0) {
            SetImage(CharaImg, path);
        } else {
            SetImage(CharaImg, img_path);
            CharaFrame?.updImageSize();
        }
        

        //img.sprite = Resources.Load<Sprite>(img_path);
    }

    /// <summary>
    /// キャライメージ設定
    /// </summary>
    /// <param name="img"></param>
    /// <param name="path"></param>
    public static void SetImage(Image img, string path) {
        if (img == null) {
            return;
        }
        img.gameObject.SetActive(path != string.Empty);

        img.sprite = Resources.Load<Sprite>(path);
    }

    //public void setListItemParams( UnitStatusTran unit ) {
    //    Name.text = unit.Name;
    //    setImage(unit.Img);
    //    Params[(int)LIST_STATUS.HP].text = unit.MaxHp.ToString();
    //    Params[(int)LIST_STATUS.MP].text = unit.MaxMp.ToString();
    //    Params[(int)LIST_STATUS.ATK].text = unit.TotalAttack.ToString();
    //    Params[(int)LIST_STATUS.DEF].text = unit.TotalDefence.ToString();
    //    Params[(int)LIST_STATUS.MAG].text = unit.TotalMagic.ToString();
    //    Params[(int)LIST_STATUS.REG].text = unit.TotalRegister.ToString();
    //    Params[(int)LIST_STATUS.LV].text = unit.Lv.ToString();
    //}

    public void setMax(int max, int val, int index = 0) {
        if (Gauges.Length > index && Gauges[index] != null) {
            Gauges[index].init(max, val);
        }
    }

    public void setValue(int val, int index = 0) {
        if (Gauges.Length > index && Gauges[index] != null) {
            Gauges[index].setValue(val);
        }
    }

    /// <summary>
    /// 戦闘不能状態設定
    /// </summary>
    /// <param name="is_dead"></param>
    /// <param name="fade_time"></param>
    public void setDeadOrAlive(bool is_dead, float fade_time = 1) {
        if (is_dead) {
            CharaImg.CrossFadeColor(DeadColor, fade_time, false, false);
        } else {
            CharaImg.CrossFadeColor(Color.white, fade_time, false, false);
        }
    }

    /// <summary>
    /// ユニットサイドのフレーム設定
    /// </summary>
    public void setPlayerEnemyFrame() {
        if (UnitFrames != null && UnitFrames.Length >= (int)UnitMast.TYPE.ALL) {
            System.Array.ForEach(UnitFrames, it => it.SetActive(false));
            UnitFrames[(int)UnitType].SetActive(true);
        }
    }

    /// <summary>
    /// パラメータ表示・非表示切り替え
    /// </summary>
    /// <param name=“val”></param>
    public void showParameter(bool val) {
        foreach (var g in Gauges) {
            if (g != null) {
                g.gameObject.SetActive(val);
            }
        }
        foreach (var p in Params) {
            if (p != null) {
                p.gameObject.SetActive(val);
            }
        }
    }

    public UnitStatusTran getStatus() {
        return SaveMng.Units.Where(it => it.Id == UnitTranId).FirstOrDefault();
    }

}