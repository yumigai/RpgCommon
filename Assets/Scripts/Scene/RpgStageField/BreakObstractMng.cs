using Exploder.Utils;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class BreakObstractMng : MonoBehaviour {

    public const float BREAK_TIME = 0.5f;

    [SerializeField]
    public AudioClip BreakSe;

    [SerializeField]
    public GameObject Effect;

    //[SerializeField, Header("拡散力")]
    //private float Diffusion = 0f;

    [SerializeField, Header("エフェクトの中心")]
    protected Transform EffectPosi;

    [SerializeField, Header("破壊可能")]
    protected bool CanBreak = true;

    [SerializeField, Header("HP")]
    protected int BreakHitCount = 1;



    private Material[] Mtels;

    private float Alpha = 1f;

    private bool IsBreak = false;

    protected void Start()
    {

        Renderer[] rens = this.transform.GetComponentsInChildren<Renderer>();

        Mtels = rens.Select(it => it.material).ToArray();

    }

    virtual public bool breakObject()
    {
        if (!CanBreak) {
            return false;
        }

        BreakHitCount--;

        Vector3 eff_posi = EffectPosi == null ? this.transform.position : EffectPosi.position;

        if (Effect != null) {
            EffectSimpleMng.showEffect(eff_posi, this.transform.localRotation, Effect, 1.2f);
        } else {
            StageFieldSceneMng.hitEffect(eff_posi, this.transform.localRotation);
        }

        if (BreakHitCount > 0) {
            return false;
        }

        SoundMng.Instance.playSE(BreakSe);

        ExploderSingleton.Instance.ExplodeObject(this.gameObject);

        IsBreak = true;

        Destroy(this.gameObject, BREAK_TIME);

        return true;
    }

    public void FixedUpdate()
    {
        if (IsBreak)
        {
            Alpha -= 0.05f;
            if (Alpha > 0)
            {
                for (int i = 0; i < Mtels.Length && Mtels[i] != null; i++)
                {
                    Mtels[i].SetColor("_Color", new Color(1f, 1f, 1f, Alpha));
                }
            }
        }
    }

    /// <summary>
    /// ブレイクオブジェクト取得
    /// 一つ上の親は許容
    /// </summary>
    /// <param name="coll"></param>
    /// <returns></returns>
    public static BreakObstractMng getHitObject(Collider coll ) {
        BreakObstractMng obs = coll.GetComponent<BreakObstractMng>();
        if (obs == null) {
            // 直親のオブジェクトのみ判定する
            if (coll.transform.parent != null) {
                obs = coll.transform.parent.GetComponent<BreakObstractMng>();
            }
        }
        return obs;
    }

}
