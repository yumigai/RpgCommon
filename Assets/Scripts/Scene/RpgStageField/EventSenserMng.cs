using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EventSenserMng : MonoBehaviour
{
    public enum TYPE
    {
        GATE,
        KEY,
        DROP_ITEM,
        MESSAGE,
        DAMAGE,
        GIMMICK_ENTER,
        GIMMICK_STAY,
        GIMMICK_SWITCH,
    }

    [SerializeField]
    public TYPE Type;

    [SerializeField]
    public GameConst.RARITY Rarity;

    [SerializeField]
    protected GameObject Effect;

    //[SerializeField]
    //protected JemMng SymbolJem;

    [SerializeField,Header("イベント開始時に表示するオブジェクト")]
    protected GameObject ShowObject;

    [SerializeField,Header("イベント開始時に非表示にするオブジェクト")]
    protected GameObject HideObject;

    [SerializeField]
    protected CmnAnimeTraceMng EventAnime;

    protected ParticleSystem[] Particles;

    protected bool IsFinished = false;


    protected void Awake() {
        Particles = GetComponentsInChildren<ParticleSystem>();
        //if (SymbolJem != null && SymbolJem.Target == null) {
        //    SymbolJem.Target = FieldPlayerMng.Hero;
        //}
        if (Effect != null) {
            //エフェクト発生時遅延を発生させないために、あらかじめ読み込む
            EffectSimpleMng.readyEffect(Effect);
        }
    }

    public void OnTriggerEnter(Collider other) {
        if (other.gameObject == FieldPlayerMng.Hero && !IsFinished) {
            hitArea();
        }
    }

    public void OnTriggerStay(Collider other){
        if (other.gameObject == FieldPlayerMng.Hero && !IsFinished){
            stayArea();
        }
    }

    public void OnTriggerExit(Collider other) {
        if (other.gameObject == FieldPlayerMng.Hero && !IsFinished) {
            outArea();
        }
    }

    protected virtual void hitArea() {

        switch (Type)
        {
            case TYPE.GATE:
                gateArea();
                break;
            case TYPE.KEY:
                keyArea();
                break;
            case TYPE.DROP_ITEM:
                dropItemArea();
                break;
            case TYPE.GIMMICK_ENTER:
                EventAnime.AnimeStart();
                break;
            case TYPE.GIMMICK_SWITCH:
                EventAnime.AnimeSwitch();
                break;
        }

            if(Effect != null) {
                EffectSimpleMng.showEffect(FieldPlayerMng.Hero.transform.position, new Quaternion(), Effect);
            }
    }

    protected virtual void stayArea()
    {
        switch (Type)
        {
            case TYPE.DAMAGE:
                damageArea();
                break;
            case TYPE.GIMMICK_STAY:
                break;
        }
    }

    protected virtual void outArea() {
        switch (Type) {
            case TYPE.GATE:
                gateAreaOut();
                break;
            case TYPE.KEY:
                break;
            case TYPE.DROP_ITEM:
                break;
            case TYPE.GIMMICK_STAY:
                break;
        }
    }

    private void keyArea() {
        //SymbolJem.injection(getKey);
        BaseStageFieldSceneMng.Singleton.getKey();
        IsFinished = true;
        eventAnimation();
        Destroy(this.gameObject, 5f);
    }
    //public void getKey() {
    //    //BaseStageFieldSceneMng.Singleton.getKey();
    //    Destroy(this.gameObject, 5f);
    //}

    /// <summary>
    /// ゴール接触
    /// </summary>
    private void gateArea() {
        BaseStageFieldSceneMng.Singleton.readyStageClear();
    }

    /// <summary>
    /// ゴール離れ
    /// </summary>
    private void gateAreaOut() {
        BaseStageFieldSceneMng.Singleton.resetStageClear();
    }

    private void dropItemArea() {
        //何らかの情報
    }

    /// <summary>
    /// 取得時のシンボルアニメ＋エフェクト停止
    /// </summary>
    private void eventAnimation() {
        System.Array.ForEach(Particles, it => { it.Stop(); });
        EventAnime.AnimeStart();
    }

    private void damageArea(){
        //int damage = Rarity > GameConst.RARITY.N ? (int)Rarity * TRAP_DAMAGE : 1;
        //FieldPlayerMng.hero().fieldDamage(damage);
    }

    private void gimmickEnter(){
        EventAnime.AnimeStart();
    }

    private void gimmickSwitch(){
        EventAnime.AnimeSwitch();
    }

    private void gimmickStay(){

    }
}
