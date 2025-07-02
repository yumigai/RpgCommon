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
    }

    [SerializeField]
    public TYPE Type;

    [SerializeField]
    private GameObject Effect;

    [SerializeField]
    private JemMng SymbolJem;

    [SerializeField]
    private FreeMultiAnimeSimpleMng GetAnime;

    private ParticleSystem[] Particles;

    private bool IsFinished = false;


    private void Awake() {
        Particles = GetComponentsInChildren<ParticleSystem>();
        if (SymbolJem != null && SymbolJem.Target == null) {
            SymbolJem.Target = FieldPlayerMng.Hero;
        }
        if (Effect != null) {
            //エフェクト発生時遅延を発生させないために、あらかじめ読み込む
            EffectSimpleMng.readyEffect(Effect);
        }
    }

    public void OnTriggerEnter(Collider other) {
        if (other.gameObject == FieldPlayerMng.Hero) {
            hitArea();
        }
    }

    public void OnTriggerExit(Collider other) {
        if (other.gameObject == FieldPlayerMng.Hero) {
            outArea();
        }
    }

    private void hitArea() {

        if (!IsFinished) {

            switch (Type) {
                case TYPE.GATE:
                gateArea();
                break;
                case TYPE.KEY:
                keyArea();
                break;
                case TYPE.DROP_ITEM:
                dropItemArea();
                break;
            }

            if(Effect != null) {
                EffectSimpleMng.showEffect(FieldPlayerMng.Hero.transform.position, new Quaternion(), Effect);
            }
        }
    }

    private void outArea() {
        if (!IsFinished) {

            switch (Type) {
                case TYPE.GATE:
                gateAreaOut();
                break;
                case TYPE.KEY:
                break;
                case TYPE.DROP_ITEM:
                break;
            }
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
        GetAnime.AnimeStart();
    }
}
