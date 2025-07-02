using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class StageObjectMng : BreakObstractMng
{

    private const int DROP_ITEM_PERCENT = 5;

    private const int TRAP_DAMAGE = 5;

    public enum TYPE
    {
        OBSTACLES,
        TRAP,
        TREASURE,
    }

    [SerializeField]
    public BreakObstractMng[] childObjects;

    [SerializeField]
    public TYPE Type;

    [SerializeField, Tooltip("takara nara iimono, trap nara iryoku ookii")]
    public GameConst.RARITY Rarity;

    new protected void Start() {
        base.Start();
        if (childObjects != null && childObjects.Length > 0) {
            int index = UnityEngine.Random.Range(0, childObjects.Length);
            childObjects[index] = null;
            System.Array.ForEach(childObjects, it => Destroy(it?.gameObject));
        }
        if (EffectPosi == null) {
            EffectPosi = this.transform;
        }
    }

    /// <summary>
    /// 破壊した場合の挙動
    /// </summary>
    /// <returns></returns>
    public override bool breakObject() {
        if (base.breakObject()) {
            //if (Jem != null) {
            //    switch (Jem.Type) {
            //        case JemMng.TYPE.MONEY:
            //            Jem.DirectValue = MainProcess.StageData.TreasureCoin;
            //            break;
            //    }

            //    JemMng.jemSplash(Jem, EffectPosi.transform.position, MinJem, MaxJem);
            //}
            return true;
        }
        return false;
    }

    protected void hitPlayer() {
        switch (Type) {
            case TYPE.TRAP:
            hitTrap();
            break;
            case TYPE.TREASURE:
            break;
        }
    }

    protected void hitTrap() {
        int damage = Rarity > GameConst.RARITY.N ? (int)Rarity * TRAP_DAMAGE : 1;
        FieldPlayerMng.hero().fieldDamage(damage);
    }

    void OnCollisionEnter(Collision collision) {
        if (collision.gameObject == FieldPlayerMng.Hero) {
            hitPlayer();
        }
    }

    void OnTriggerEnter(Collider other) {
        if (other.gameObject == FieldPlayerMng.Hero) {
            hitPlayer();
        }
    }

}
