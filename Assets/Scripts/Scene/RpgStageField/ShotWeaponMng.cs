using UnityEngine;
using System.Collections;

public class ShotWeaponMng : WeaponMng
{

    [SerializeField]
    public GameObject Bullet;


    [SerializeField]
    public int StingNum;

    [SerializeField]
    public float BulletSpeed;
    [SerializeField]
    public float Range;
    [SerializeField]
    public bool IsRapid;
    [SerializeField]
    public float RandomWidth = 0f;
    [SerializeField]
    public bool IsParentBullet;
    [SerializeField]
    public Transform FirePosi;

    [SerializeField]
    public GameObject FireEffect;

    [System.NonSerialized]
    public int MaxBullet;
    [System.NonSerialized]
    public int NowBullet;

    [System.NonSerialized]
    public float CoolTime;
    [System.NonSerialized]
    public float RestCoolTime;


    [System.NonSerialized]
    public int AnimeType;

    new public void Awake()
    {
        base.Awake();


    }

    public void FixedUpdate()
    {
        if (RestCoolTime > 0 && NowBullet > 0 )
        {
            RestCoolTime -= Time.deltaTime;
            RestCoolTime = Mathf.Clamp(RestCoolTime, 0f, RestCoolTime);
        }

    }

    public void fire()
    {
        if (FireEffect != null){
            EffectMng.showEffect(FirePosi.position, User.CharaObj.transform.localRotation, FireEffect, 1f);
        }

        SoundMng.Instance.playSE(AttackSe);

        if (User.AimLine != null && User.AimLine.enabled)
        {
            Vector3 target = User.AimAngle;
            target.y = User.CharaObj.transform.position.y;
            User.CharaObj.transform.LookAt(target);
        }

        GameObject bull = BulletMng.shot(this, Bullet, FirePosi.position, User.CharaObj.transform.localRotation, RandomWidth);

        Destroy(bull, Range);

        if (IsParentBullet)
        {
            bull.transform.parent = FirePosi;
        }

        RestCoolTime = CoolTime;

        NowBullet = NowBullet > 0 ? NowBullet-1 : 0;

    }

    public void setParam( SkillMast mst )
    {
        /*
        AnimeType = mst.Anime;
        MaxBullet = mst.UseNum;
        NowBullet = MaxBullet;
        Damage = mst.Power;
        CoolTime = mst.CoolTime;
        */

        RestCoolTime = 0f;
        
    }

    public void addBullet(int base_value, float percent)
    {
        NowBullet += base_value;
        float per_value = percent / 100f;
        NowBullet += (int)(MaxBullet * per_value);
        NowBullet = Mathf.Clamp(NowBullet, 0, MaxBullet);
    }

    //public void updateBulletTxt()
    //{
    //}
}
