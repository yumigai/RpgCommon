using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EventTrapMng : EventSenserMng
{
    public const int AREA_DAMAGE = 5;

    public const int EMIT_DAMAGE = 10;

    public enum TRAP_TYPE
    {
        DAMAGE,
        EMIT,
        SHOT,
        WARP,
        ALERT,
    }

    [SerializeField]
    public TRAP_TYPE TrapType;

    override protected void hitArea() {
        switch (TrapType) {
            case TRAP_TYPE.WARP:
            RandomWarp();
            break;
            case TRAP_TYPE.ALERT:
            Alert();
            break;
        }
    }

    override protected void stayArea() {
        switch (TrapType) {
            case TRAP_TYPE.DAMAGE:
            damage(AREA_DAMAGE);
            break;
            case TRAP_TYPE.EMIT:
            damage(EMIT_DAMAGE);
            break;
        }
    }

    override protected void outArea() {

    }

    private void damage(int base_damage) {
        int damage = ((int)Rarity + 1) * base_damage;
        FieldPlayerMng.hero().fieldDamage(damage);
    }

    private void RandomWarp() {
        IsFinished = true;
        HideObject.SetActive(false);
        var posi = RespawnMng.getRandom();
        FieldPlayerMng.Hero.transform.position = posi;
        EffectSimpleMng.showEffect(posi, new Quaternion(), Effect);
    }

    private void Alert() {
        //ensyutsu
        IsFinished = true;
        ShowObject.SetActive(true);
        TimeInvokeMng.TimerAction( () => { HideObject.SetActive(false); }, 3f, this.gameObject);
        StageFieldSceneMng.foeChaseMode();
    }
}
