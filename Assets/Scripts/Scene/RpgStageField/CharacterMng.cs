using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class CharacterMng : MonoBehaviour {

    public enum TRIGERS
    {
        Idle,
        Run,
        MeleeAttack,
        Shot,
        Damage,
        Death,
        Jump,
        Stun,
        Moving,
        Attacking,
        Skill,
        Idling,
        Dash,
        Stop,
        Roll,
        Rolling,
        Down,
        FinishAttack,
        ChargeAttack,
        Air,
    }

	public enum ANIME_PARAM
	{
		MeleeSpeed,
		GunSpeed,
        SkillType,
	}

	public const float COUNTER_BONUS = 1.5f;
	public const float BREAK_DAMAGE_BONUS = 3.0f;

    protected const float RESET_HIT_COUNT = 1;

    [SerializeField]
    public Transform CharaObj;
    [SerializeField]
	public Transform HitEffectPoint;
    [SerializeField]
	public Transform FireEffectPoint;

    protected Animator Anime;
    public AnimatorStateInfo StateInfo { get; set; }

    [SerializeField]
    public MeleeWeaponMng MeleeWeapon;

    [SerializeField]
    public AudioClip AttackVoice;

    [SerializeField]
    public AudioClip DownSe;

    [SerializeField]
    public float InSite = 30;

    [SerializeField]
    public LineRenderer AimLine { get; set; }
    public Vector3 AimAngle { get; set; }

    [SerializeField]
    public AudioClip FootStepSe;

    [System.NonSerialized]
    public int NowStan;

    [SerializeField]
    public bool SuperArmor;

    [SerializeField]
    public float Near = 6f;

    [System.NonSerialized]
    public float PoisonDamage;

    [System.NonSerialized]
    public float HealBonus;

    [SerializeField]
    public float MoveSpeed;

    //[System.NonSerialized]
    //public bool IsAttackNow;
    //[System.NonSerialized]
    //public bool IsMove;
    [System.NonSerialized]
    public Transform AimTarget;

    [System.NonSerialized]
    public int Lv;

    protected float RestMuteki = 0;

    protected float RestResetHitCount = 0;

    protected int HitCount = 0;

    [System.NonSerialized]
    public StageRoomMng LastTouchRoom;

    [System.NonSerialized]
    public bool DownAction;

    virtual public void Awake()
    {
        Anime = CharaObj.GetComponent<Animator>();
        AimLine = CharaObj.GetComponent<LineRenderer>();
    }

    protected void FixedUpdate()
    {
        IsUnderStageBottom();
        updateStatus();
    }

    protected void updateStatus()
    {
        if (RestMuteki > 0)
        {
            RestMuteki -= Time.fixedDeltaTime;
        }

        if (RestResetHitCount > 0)
        {
            RestResetHitCount -= Time.fixedDeltaTime;
        }
        else
        {
            HitCount = 0;
        }
    }

    public void Move(float x, float y)
    {
        //Vector2 diff = new Vector2(x, y).normalized;

        StateInfo = Anime.GetCurrentAnimatorStateInfo(0);

        if (Mathf.Abs(x) > 0f || Mathf.Abs(y) > 0f)
        {

            Anime.SetBool(TRIGERS.Run.ToString(), true);

            if ( StateInfo.IsName(TRIGERS.Run.ToString()) || StateInfo.IsName(TRIGERS.Idle.ToString()) || StateInfo.IsName(TRIGERS.Jump.ToString()))
            {
                CharaObj.rotation
                    = Quaternion.LookRotation(
                    transform.position +
                (Vector3.right * x) + (Vector3.forward * y)
                - transform.position);
                CharaObj.Rotate(0f, Camera.main.transform.eulerAngles.y, 0f); //カメラの回転に合わせて方向を調整
                transform.Translate(CharaObj.forward * MoveSpeed, Space.World); //正面

            }
        }
        else
        {
			Anime.SetBool(TRIGERS.Run.ToString(), false);
        }
    }

    public void stanEnd()
    {
        NowStan--;
        if (NowStan <= 0)
        {
            Anime.SetBool(TRIGERS.Stun.ToString(), false);
        }
    }

    public void playDownSe()
    {
        SoundMng.Instance.playSE(DownSe);
    }

    public void setAnimeBool( TRIGERS tri, bool val)
    {
        Anime.SetBool(tri.ToString(), val);
    }
    public void setAnimeTriger(TRIGERS tri)
    {
        Anime.SetTrigger(tri.ToString());
    }
    public void setAnimeInt( ANIME_PARAM key, int val)
    {
        Anime.SetInteger(key.ToString(), val);
    }

    public void footStep()
    {
        SoundMng.Instance.playSE(FootStepSe);
    }

    public void lookTarget()
    {
        if (AimTarget != null ){
            Vector3 target = AimTarget.position;
            target.y = this.CharaObj.transform.position.y;
            CharaObj.transform.LookAt(target);
        }
    }

    public void MeleeAttack() {
        if (!StateInfo.IsName(TRIGERS.MeleeAttack.ToString())) {
            Anime.SetTrigger(TRIGERS.MeleeAttack.ToString());
            lookTarget();
        }
    }

    virtual public bool inSideSense(GameObject hit, SenseAreaMng sence) {
        return false;
    }
    virtual public bool outSideSense(GameObject hit, SenseAreaMng sence) {
        return false;
    }

    public void OllisionEnter(Collision collision) {
        if (collision.gameObject.CompareTag(StageFieldSceneMng.Tag.StageRoom.ToString())) {
            LastTouchRoom = collision.transform.GetComponent<StageRoomMng>();
        }
    }

    protected void IsUnderStageBottom() {
        if (transform.position.y < BaseStageFieldSceneMng.Singleton.StageArea.AreaMin.y) {
            if (LastTouchRoom == null) {
                transform.position = RespawnMng.getNearPosi(transform.position);
            } else {
                transform.position = LastTouchRoom.RewpawnCenter.position;
            }
        }
    }
}
