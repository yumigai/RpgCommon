using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class BulletMng : MonoBehaviour {

    public enum TYPE
    {
        NORMAL,
        HOMING,
        HOMING_ONE_TIME,
        HOMING_ONE_TIME_Y_ONRY,
        SELF_HEAL,
        OTHER_HEAL,
        ALL
    }

    [SerializeField]
    public TYPE Type;

    [SerializeField]
    public float RushTime;

    [System.NonSerialized]
    public ShotWeaponMng SourceWeapon;
	[System.NonSerialized]
    public Vector3 FireStartPosi;
	[System.NonSerialized]
    public int NowHitNum;
	[System.NonSerialized]
    public ParticleSystem Explode;
    [System.NonSerialized]
    public Transform Target;

    private float Damage = 0f;

    private float RestRushTime = 0f;

    private float BulletSpeed = 0f;

	public static GameObject shot(ShotWeaponMng weapon, GameObject prefab, Vector3 posi, Quaternion rotate, float random_h = 0f, float random_v = 0f)
    {
        GameObject obj = Instantiate(prefab) as GameObject;

        BulletMng[] bullets = obj.transform.GetComponentsInChildren<BulletMng>();

        if (bullets != null)
        {
            foreach (BulletMng bl in bullets)
            {
				bl.init(weapon, posi, rotate, random_h, random_v);
            }
        }

        return obj;
    }

    public void Start()
    {
        Explode = GetComponent<ParticleSystem>();
        RestRushTime = RushTime;
        //Destroy (this.gameObject, SourceWeapon.Range);
    }

	public void init(ShotWeaponMng weapon, Vector3 posi, Quaternion rotate, float random_h, float random_v)
    {
        SourceWeapon = weapon;
        BulletSpeed = SourceWeapon.BulletSpeed;
        FireStartPosi = posi;
        if (Target == null && SourceWeapon.User.AimTarget != null )
        {
            Target = SourceWeapon.User.AimTarget.transform;
        }

        this.transform.position = FireStartPosi;

        if (Type == TYPE.HOMING_ONE_TIME && Target != null)
        {
            this.transform.LookAt(Target);
        }else if (Type == TYPE.HOMING_ONE_TIME_Y_ONRY && Target != null){
            Vector3 thisposi = this.transform.position;
            thisposi.y = Target.position.y;
            this.transform.position = thisposi;
            this.transform.localRotation = rotate;
        }
        else
        {
            this.transform.localRotation = rotate;
        }

        if( Type == TYPE.SELF_HEAL && SourceWeapon != null && SourceWeapon.User != null )
        {
            int value = (int)SourceWeapon.Damage / 2;
            SourceWeapon.User.heal(value, 0f);
        }

        if ( random_h != 0f || random_v != 0f ){
			float r_h = Random.Range( -random_h, random_h );
			float r_v = Random.Range( -random_v, random_v );
			transform.Rotate( new Vector3( r_v, r_h ) );
		}

        NowHitNum = 0;
    }

    public void FixedUpdate()
    {
        if( Type == TYPE.HOMING && Target != null )
        {
            transform.LookAt(Target);
        }

        Vector3 move = this.transform.position + this.transform.forward * BulletSpeed;
        this.transform.position = move;
        if(Explode!=null && !Explode.IsAlive())
        {
            Destroy(this.gameObject);
        }
    }

    void OnTriggerEnter(Collider hit)
    {
        hitProcess(hit);
    }


    private void OnTriggerStay(Collider other)
    {
        if (RushTime > 0)
        {
            if( RestRushTime <= 0f)
            {
                RestRushTime = RushTime;
                hitProcess(other);
            }
            else
            {
                RestRushTime -= Time.fixedDeltaTime;
            }
        }
    }

    private void hitProcess(Collider hit)
    {
        bool is_hit = false;

        is_hit = SourceWeapon.isHit(hit);

        if (is_hit)
        {

            CharacterMng hit_chara = hit.GetComponent<CharacterMng>();
            hit_chara.weaponDamage(SourceWeapon, hit_chara.HitEffectPoint.position, this.transform.localRotation, SourceWeapon.HitSe);

        }
        else
        {
            BreakObstractMng obj = hit.GetComponent<BreakObstractMng>();
            if (obj != null)
            {
                obj.breakObject();
                is_hit = true;
            }
        }

        if (is_hit)
        {
            if (NowHitNum >= SourceWeapon.StingNum)
            {
                Destroy(this.gameObject);
            }
            else
            {
                NowHitNum++;
            }
        }
    }


}
