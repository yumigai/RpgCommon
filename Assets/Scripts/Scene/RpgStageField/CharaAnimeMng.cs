using UnityEngine;
using System.Collections;

public class CharaAnimeMng : MonoBehaviour {

    public enum HIT_TYPE
    {
        NORMAL,
        DOWN,
    }

	[System.NonSerialized]
    public CharacterMng Chara;

    public bool MeleeHitReady;

    private void Awake() {
        Chara = this.GetComponent<CharacterMng>();
    }

    public void attackStart()
    {
        if( Chara.AttackVoice != null)
        {
            SoundMng.Instance.playSE(Chara.AttackVoice);
        }
    }

    public void playAttackSe()
    {
        SoundMng.Instance.playSE(Chara.MeleeWeapon.AttackSe);
    }

    public void Hit(HIT_TYPE type = HIT_TYPE.NORMAL)
    {
        Chara.DownAction = type == HIT_TYPE.DOWN;
        Chara.MeleeWeapon.Hit();
    }

    public void meleeHitStart()
    {
        MeleeHitReady = true;
        Chara.MeleeWeapon.showXray(true);
        Chara.lookTarget();
    }

    public void meleeHitEnd()
    {
        MeleeHitReady = false;
        if (Chara.MeleeWeapon != null && Chara.MeleeWeapon.Xray != null)
        {
    	    Chara.MeleeWeapon.showXray(false);
        }
    }

}
