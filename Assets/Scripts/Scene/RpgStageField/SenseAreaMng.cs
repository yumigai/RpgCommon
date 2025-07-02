using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SenseAreaMng : MonoBehaviour
{
    public enum TYPE
    {
        Shot,
        MeleeAttack,
        Skill1,
        Skill2,
        Skill3,
        All
    }

    [SerializeField]
    public TYPE Type;

    [SerializeField]
    public float CoolTime;

    [System.NonSerialized]
    public List<CharacterMng> HitTargets = new List<CharacterMng>();
    [System.NonSerialized]
    public List<BreakObstractMng> HitObstracts = new List<BreakObstractMng>();

    private float RestCoolTime;

    private CharacterMng Chara;

    private void Awake()
    {
        Chara = GetComponentInParent<CharacterMng>();
        
    }

    private void Start()
    {
        RestCoolTime = CoolTime;
    }

    private void FixedUpdate()
    {
        if( RestCoolTime > 0)
        {
            RestCoolTime -= Time.fixedDeltaTime;
        }
    }

    private void OnTriggerEnter(Collider inside) {

        bool hit = Chara.inSideSense(inside.gameObject, this);

        if (hit) {
            CharacterMng hit_chara = inside.GetComponent<CharacterMng>();
            HitTargets.Add(hit_chara);
        }

        BreakObstractMng obs = BreakObstractMng.getHitObject(inside);
        if (obs != null) {
            HitObstracts.Add(obs);
        }
    }

    private void OnTriggerExit(Collider outside) {

        CharacterMng chara = outside.GetComponent<CharacterMng>();
        if (chara != null) {
            HitTargets.Remove(chara);
        }

        BreakObstractMng obs = BreakObstractMng.getHitObject(outside);
        if (obs != null) {
            HitObstracts.Remove(obs);
        }

        Chara.outSideSense(outside.gameObject, this);

    }

    public void recool()
    {
        RestCoolTime = CoolTime;
    }
}
