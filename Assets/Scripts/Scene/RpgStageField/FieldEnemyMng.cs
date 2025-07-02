using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System;

public class FieldEnemyMng : CharacterMng
{
    public enum MODE
    {
        NON,
        STOP,
        RANDOM,
        PATROL,
        CHASE,
    }

    public enum MOVE_TYPE
    {
        STOP,
        RANDOM,
        EVER_CHASE,
        PATROL,
    }

    private const float SMASH = 30f;
    private const float CHASE_TIME = 3f;
    private const float MAX_RANDOM_TIME = 6f;
    private const float MIN_REST_TIME = 3f;
    private const float MAX_REST_TIME = 5f;

    [SerializeField]
    private Material[] Variation;

    [SerializeField]
    private int VariationLv;

    [SerializeField]
    private Transform ThisMesh;

    //[SerializeField]
    //protected Collider ThisCollider;

    [SerializeField]
    public int GetCoin;

    [SerializeField]
    public MOVE_TYPE MoveType;

    [System.NonSerialized]
    protected UnityEngine.AI.NavMeshAgent Navi;

    [System.NonSerialized]
    private MODE MoveMode;

    [System.NonSerialized]
    private float ChaseMoveTime;

    [System.NonSerialized]
    private float RandomMoveTime;

    [System.NonSerialized]
    private float StopMoveTime;

    [System.NonSerialized]
    public float DistanceToHero = 100f;

    protected Renderer ThisRenderer;

    protected Vector3 WalkTarget;

    protected List<SenseAreaMng> AttackPlans = new List<SenseAreaMng>();

    protected GameObject Hero {
        get {
            return FieldPlayerMng.Hero;
        }
    }

    override public void Awake() {
        base.Awake();
        Navi = GetComponent<UnityEngine.AI.NavMeshAgent>();
        Navi.enabled = false;
        if (ThisMesh != null) {
            ThisRenderer = ThisMesh.GetComponent<Renderer>();
        }

    }

    // Use this for initialization
    public void Start() {

        float rote = UnityEngine.Random.Range(0, 360);
        CharaObj.transform.Rotate(new Vector3(0f, rote, 0f));
    }

    // Update is called once per frame
    new protected void FixedUpdate() {

        base.FixedUpdate();

        if (!Navi.enabled) {
            if (StageFieldSceneMng.Singleton.StageArea.IsBaked) {
                Navi.enabled = true;
            }
            return;
        }

        if (StageFieldSceneMng.Singleton.IsBattle) {
            Navi.isStopped = true;
            setAnimeBool(TRIGERS.Run, false);
            AttackPlans.Clear();
            return;
        }

        DistanceToHero = Vector3.Distance(this.transform.position, Hero.transform.position);

        assult();
    }

    public void Ready() {
        Navi.enabled = true;
    }

    protected void assult() {
        if (AttackPlans.Count > 0) {
            attack();
        } else {
            moveAction();
        }
    }

    protected void moveAction() {
        checkHerosRange();

        //StateInfo no use itÅfs heavy
        if (Anime.GetBool(TRIGERS.Moving.ToString())) {
            move();
        } else {
            if (Navi != null && !Navi.isStopped) {
                Navi.isStopped = true;
            }
        }

        AimAngle = FieldPlayerMng.Position;
    }

    protected void attack() {
        //for (int i = 0; i < AttackPlans.Count; i++) {

        //    SenseAreaMng plan = AttackPlans[i];
        foreach (SenseAreaMng plan in AttackPlans) {
            if (checkRemoveAttackPlan(plan)) {
                continue;
            }
            if ((int)SenseAreaMng.TYPE.Skill1 <= (int)plan.Type) {
                setAnimeTriger(TRIGERS.Skill);
                setAnimeInt(ANIME_PARAM.SkillType, (int)plan.Type - (int)SenseAreaMng.TYPE.Skill1);
            } else {
                Anime.SetTrigger(plan.Type.ToString());
            }
            Navi.isStopped = true;
            break;
        }

    }

    protected void addTrophy() {

    }

    protected void checkHerosRange() {
        chechRange(DistanceToHero);

    }

    protected void chechRange(float range) {
        if (range < InSite) {
            Anime.SetBool(TRIGERS.Run.ToString(), true);
            MoveMode = MODE.CHASE;
            ChaseMoveTime = CHASE_TIME;
        } else {
            if (MoveMode == MODE.CHASE && ChaseMoveTime <= 0f) {
                MoveMode = MODE.STOP;
                //Anime.SetBool(TRIGERS.Run.ToString(), false);
                ChaseMoveTime = CHASE_TIME;
            }
        }
    }

    protected void move() {
        switch (MoveMode) {
            case MODE.CHASE:
            ChaseMoveTime -= Time.fixedDeltaTime;
            Navi.SetDestination(Hero.transform.position);
            Navi.isStopped = false;
            break;

            case MODE.RANDOM:
            if (Vector3.Distance(WalkTarget, this.transform.position) <= Navi.stoppingDistance || RandomMoveTime <= 0) {
                MoveMode = MODE.STOP;
            } else {
                RandomMoveTime -= Time.fixedDeltaTime;
            }
            break;

            case MODE.STOP:
            Anime.SetBool(TRIGERS.Run.ToString(), false);
            Anime.SetBool(TRIGERS.Moving.ToString(), false);
            Navi.isStopped = true;
            MoveMode = MODE.NON;
            break;

            case MODE.NON:
            if (StopMoveTime <= 0f) {
                if (MoveType == MOVE_TYPE.RANDOM) {
                    MoveMode = MODE.RANDOM;
                    RandomMoveTime = MAX_RANDOM_TIME;
                    WalkTarget = RespawnMng.getShortRandom(this.transform.position);
                    Anime.SetBool(TRIGERS.Moving.ToString(), true);
                    Navi.isStopped = false;
                }
            }
            StopMoveTime -= Time.fixedDeltaTime;
            break;
        }
    }

    override public bool inSideSense(GameObject hit, SenseAreaMng sence) {
        if (hit.name == Hero.name && !AttackPlans.Contains(sence)) {
            AimTarget = hit.transform;
            AttackPlans.Add(sence);
            Navi.isStopped = true;
            return true;
        }

        return false;
    }

    private void removeHitTargets() {

    }

    private bool checkRemoveAttackPlan(SenseAreaMng sence) {
        if (sence.HitTargets.Count == 0 && sence.HitObstracts.Count == 0) {
            AttackPlans.Remove(sence);
            return true;
        }
        return false;
    }

    override public bool outSideSense(GameObject hit, SenseAreaMng sence) {
        checkRemoveAttackPlan(sence);
        return false;
    }

    protected Vector3 getTargetPosi(Vector3 posi, Vector3 tar) {
        return new Vector3(tar.x, posi.y, tar.z);
    }
}
