using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CharaPlateMng : MonoBehaviour {

    private const float DAMEGE_SHOW_TIME = 1.5f;

    public enum ANIME
    {
        Action,
        ActionEnd,
        Damage,
        Miss,
        Guard,
        ALL
    }

    [SerializeField]
    public Image ShadowImg;

    [SerializeField]
    public CharaImgGaugeMng CharaStatus;

    [SerializeField]
    public Animator Anime;

    [SerializeField]
    public Transform EffectPoint;

    [SerializeField]
    public Image ElementIcon;

    [SerializeField]
    public Text TacticsText;

    [SerializeField]
    public GameObject Target;

    [SerializeField]
    public Text DamageNum;

    [SerializeField]
    public GameObject RegistBoard;

    [SerializeField]
    public GameObject WeakBoard;

    [SerializeField]
    public GameObject GuardIcon;

    [SerializeField]
    public GameObject BreakBurst;

    [SerializeField]
    public FreeTransformMng Shaper;

    [SerializeField]
    public GameObject[] BuffUps = new GameObject[(int)BuffTran.TYPE.ALL];

    [SerializeField]
    public GameObject[] BuffDowns = new GameObject[(int)BuffTran.TYPE.ALL];

    [System.NonSerialized]
    public UnitStatusTran Unit;

	[System.NonSerialized]
	public int Index;

    [System.NonSerialized]
    public System.Action<int,CharaPlateMng> Callback;

    [System.NonSerialized]
    public System.Action<int, CharaPlateMng> PointEnterCallback;

    public Image CharaImg { get { return CharaStatus.CharaImg; } }

    //public Image CharaHpBar { get { return CharaStatus.Gauge?.MainGauge; } }

    //public Text CharaLv{ get { return CharaStatus.Gauge3?.NumTxt; } }

    //public Text CharaHp{ get { return CharaStatus.Gauge?.NumTxt; } }

    //public Text CharaMp{ get { return CharaStatus.Gauge2?.NumTxt; } }


    private void Start() {
        Standby();
    }

    private void OnEnable() {
        Standby();
    }

    private void Standby() {
        //if (Target != null) {
        //    Target.SetActive(false);
        //}
        showChange(Target, false);
        //if (DamageNum != null) {
        //    DamageNum.gameObject.SetActive(false);
        //}
        showChange(DamageNum.gameObject, false);

        //if (GuardIcon != null) {
        //    GuardIcon.SetActive(false);
        //}
        showChange(GuardIcon, false);

        showWeak(false);
        showRegist(false);
        showBreak(false);
        UtilToolLib.AllObjectActive(BuffUps, false);
        UtilToolLib.AllObjectActive(BuffDowns, false);
    }

    public void setUnit(UnitStatusTran unit) {
        Unit = unit;
        //ElementIcon.sprite = CommonProcess.getElementImage(Unit.Element);
        setData();
        if (TacticsText != null) {
            TacticsText.text = AiProc.TacticsJp[(int)unit.Tactics];
        }
    }

    public void setData( )
    {
        if (Unit == null) {
            CharaImg.gameObject.SetActive(false);
            UtilToolLib.AllObjectActive(CharaStatus.Gauges, false);
        } else {
            CharaStatus.setMax(Unit.MaxHp, Unit.Hp, 0);
            CharaStatus.setMax(Unit.MaxMp, Unit.Mp, 1);
            CharaStatus.setMax(Unit.Mst.MaxCrash, Unit.CrashPower, 2);

            if (Unit.Hp <= 0) {
                CharaStatus.setDeadOrAlive(true);
            }
        }
        
    }

    public void resetData() {
        Unit = null;
        setData();
    }

    /// <summary>
    /// ターン開始
    /// </summary>
    public void turnStart() {
       showChange(GuardIcon, false);
    }

    /// <summary>
    /// ターン終了
    /// </summary>
    public void turnEnd() {
    }

    /// <summary>
    /// ダメージ演出
    /// </summary>
    /// <param name="val"></param>
    /// <param name="critical"></param>
    public void damage(int val, bool critical = false)
    {
        DamageNum.color = Color.black;
        setDamageNum(val, critical);
        playAnime(ANIME.Damage);
    }

    /// <summary>
    /// 回復演出
    /// </summary>
    /// <param name="val"></param>
    public void heal(int val) {
        DamageNum.color = Color.green;
        setDamageNum(val);
    }

    /// <summary>
    /// ダメージテキスト表示
    /// </summary>
    /// <param name="val"></param>
    /// <param name="critical"></param>
    public void setDamageNum(int val, bool critical = false) {
        DamageNum.text = val.ToString();
        DamageNum.gameObject.SetActive(true);
        TimeInvokeMng.TimerHide(DamageNum.gameObject, DAMEGE_SHOW_TIME); //秒後非表示
    }

    public void action()
    {
        playAnime(ANIME.Action);
    }

    public void actionEnd() {
        playAnime(ANIME.ActionEnd);
    }

    public void miss() {
        playAnime(ANIME.Miss);
    }

    public void guard() {
        playAnime(ANIME.Guard);
        showChange(GuardIcon, true);
    }

    private void playAnime( ANIME trriger ) {
        if (Anime != null) {
            Anime.SetTrigger(trriger.ToString());
        }
    }

    public void pushButton( int btn_index )
    {
        Callback( btn_index, this );
    }
    public void pushButton() {
        pushButton(Index);
    }

    public void changeTarget() {
        PointEnterCallback(Index, this);
    }

    public void showParameter(bool show) {
        CharaStatus.showParameter(show);
    }

    /// <summary>
    /// 抵抗表示・非表示
    /// </summary>
    /// <param name="show"></param>
    public void showRegist(bool show) {
        showChange(RegistBoard, show);
    }
    /// <summary>
    /// 弱点表示・非表示
    /// </summary>
    /// <param name="show"></param>
    public void showWeak(bool show) {
        showChange(WeakBoard, show);
    }
    /// <summary>
    /// ブレイクエフェクト表示・非表示
    /// </summary>
    /// <param name="show"></param>
    public void showBreak(bool show) {
        showChange(BreakBurst, show);
    }

    /// <summary>
    /// 表示切り替え
    /// </summary>
    /// <param name="obj"></param>
    /// <param name="show"></param>
    private void showChange(GameObject obj, bool show) {
        if (obj != null) {
            obj.SetActive(show);
        }
    }

}
