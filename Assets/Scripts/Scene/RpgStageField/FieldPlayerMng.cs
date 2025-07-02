using UnityEngine;
using System.Collections;
using UnityStandardAssets.CrossPlatformInput;
using System.Collections.Generic;
using System.Linq;

public class FieldPlayerMng : CharacterMng
{

	private const string WEAPON_PREFAB_PATH = CmnConfig.RESOURCE_PREFAB + "Weapon/";

    private const string SKILL_PREFAB_PATH = CmnConfig.RESOURCE_PREFAB + "Skill/";

    private readonly string[] ANIME_STATE_ATTACKS = { "", "Attack1", "Attack2", "Attack3" };

    private const float JUMP_POWER = 10f;
    private const float IS_GROUND_JUDGE = 2f;
    private const int ADJ_BUTTON_RAG = 12;
    private const int ADJ_SKILL_RAG = 25;

    private const int DOWN_HIT_NUM = 3;
    
    private const float MUTEKI_TIME = 3;

    public static FieldPlayerMng Instance;
    public static GameObject Hero;
    public static Vector3 Position;

    [SerializeField]
    public AudioClip JumpSe;

    [SerializeField]
    public AudioClip LandingSe;

    //[SerializeField]
    //public Transform SkillObjectBase;

    [SerializeField,Header("アニメーションタイプ毎のエフェクト発生元")]
    private Transform[] FirePoints;


    //private int UsingSkill = -1;

    private int ButtonAdj;
    //private int SkillAdj;

    //private bool BeforeIdling;

    //Sprivate bool IsAir { get { return Anime.GetBool(TRIGERS.Air.ToString()); } set { Anime.SetBool(TRIGERS.Air.ToString(),value); } }

    new public void Awake()
    {
        Instance = this;
        Hero = this.gameObject;
        base.Awake();
    }

    // Update is called once per frame
    new void FixedUpdate () {

        base.FixedUpdate();

        // 右・左
        float x = CrossPlatformInputManager.GetAxisRaw("Horizontal");

        // 上・下
        float y = CrossPlatformInputManager.GetAxisRaw("Vertical");

        if (Mathf.Abs (x) < 0.1f && Mathf.Abs(y) < 0.05f) {
			x = 0f;
			y = 0f;
        }

        Move(x, y);


        //if (!BeforeIdling) {
        //    BeforeIdling = Anime.GetBool(TRIGERS.Idling.ToString());
        //    if (BeforeIdling) {

        //    }
        //}

        if (ButtonAdj > 0){
            ButtonAdj--;
        }
        //if (SkillAdj > 0)
        //{
        //    SkillAdj--;
        //}
        

        Position = this.transform.position;

    }

    /// <summary>
    /// バーチャルデジタルパッド用
    /// </summary>
    /// <param name="add"></param>
    public void pushHoldMoveX( float add ) {
        Move(add, 0);
    }

    /// <summary>
    /// バーチャルデジタルパッド用
    /// </summary>
    /// <param name="add"></param>
    public void pushHoldMoveY(float add) {
        Move(0, add);
    }


    public static FieldPlayerMng hero()
    {
        return Instance;
    }

    //void OnCollisionEnter(Collision col){
    //       if(IsAir)
    //       {
    //           SoundMng.Instance.playSE(LandingSe);
    //       }
    //	IsAir = false;

    //}

    //void OnCollisionExit(Collision col){
    //	//IsTouchGround = false;
    //}

    /// <summary>
    /// 近接攻撃
    /// </summary>
    public void pushMeleeAttack() {
        if (!Anime.GetBool(TRIGERS.FinishAttack.ToString())) {
            aim(InSite);
            ButtonAdj = ADJ_BUTTON_RAG;
            MeleeAttack();
        }
    }

    public void aim(float range) {
        if (StageFieldSceneMng.Singleton.Enemys == null) {
            return;
        }

        List<FieldEnemyMng> enes = StageFieldSceneMng.Singleton.Enemys.FindAll(it => it != null && range > it.DistanceToHero);
        if (enes.Count > 0) {
            List<float> angles = new List<float>();
            for (int i = 0; i < enes.Count; i++) {
                Vector3 direction = enes[i].transform.position - CharaObj.transform.position;
                float angle = Vector3.Angle(direction, CharaObj.transform.forward);
                angles.Add(angle);
            }
            float min = angles.Min();
            int index = angles.IndexOf(min);
            AimTarget = enes[index].transform;

        } else {
            AimTarget = null;
        }

    }

    /// <summary>
    /// 攻撃当たり判定
    /// </summary>
    /// <param name="hit"></param>
    /// <param name="type"></param>
    /// <returns></returns>
    override public bool inSideSense(GameObject hit, SenseAreaMng sence) {

        FieldEnemyMng enemy = hit.GetComponent<FieldEnemyMng>();
        return enemy != null;
    }

    public void fieldDamage(int num, bool isPercent = true) {
        if (RestMuteki <= 0) {
            UnitProcess.memberAllDamage(num, isPercent);
            RestMuteki = MUTEKI_TIME;
        }
    }





}
