using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class MeleeWeaponMng : WeaponMng
{
    [SerializeField]
	public XftWeapon.XWeaponTrail Xray;

    [SerializeField]
    public SenseAreaMng HitArea;

    float NeedStamina { get; set; }

    private List<CharacterMng> HitTargets { get { return HitArea.HitTargets; } }

    private List<BreakObstractMng> HitObstracts { get { return HitArea.HitObstracts; } }

    private void Awake()
    {
        base.Awake();
        showXray(false);
    }

    public void showXray(bool val)
    {
        if (Xray != null)
        {
            if (val)
            {
                Xray.Activate();
            }
            else
            {
                Xray.Deactivate();
            }
        }
    }

    public void Hit() {
        for (int i = 0; i < HitTargets.Count; i++) {
            if (HitTargets[i] == null) {
                HitTargets.RemoveAt(i);
                continue;
            }
            StageFieldSceneMng.hitEffect(HitEffect, HitTargets[i].HitEffectPoint.position, User.CharaObj.localRotation, HitSe);
            var target = HitTargets[i].GetComponentInChildren<CharacterMng>();
            if (target != null) {
                StageFieldSceneMng.Singleton.hitAndEncount(User, target );
            }
        }

        breakObstract();

    }

    public void breakObstract() {
        for (int i = 0; i < HitObstracts.Count; i++) {
            if (HitObstracts[i] == null) {
                HitObstracts.RemoveAt(i);
                continue;
            }
            StageFieldSceneMng.hitEffect(HitEffect, HitObstracts[i].transform.position, User.CharaObj.localRotation, HitSe);
            HitObstracts[i].breakObject();
        }
    }

    /// <summary>
    /// 当たり判定クリア
    /// </summary>
    public void Clear() {
        HitTargets.Clear();
        HitObstracts.Clear();
    }
}           
