using UnityEngine;
using System.Collections;

public class WeaponMng : MonoBehaviour {

	[SerializeField]
    public float Damage;
	[SerializeField]
	public float MotionSpeed;

	[SerializeField]
    public AudioClip AttackSe;
	[SerializeField]
    public AudioClip HitSe;
    [SerializeField]
    public GameObject HitEffect;

    [SerializeField]
	public bool IsCounter;		//体制を崩せるか

	[SerializeField]
	public bool IsDownAttack;	//追い打ちボーナスありか

    [System.NonSerialized]
    public CharacterMng User;

    public void Awake()
    {
        User = GetComponentInParent<CharacterMng>();
    }

    //public bool isHit( Collider hit )
    //{
    //    if (User.name == FieldPlayerMng.Hero.name)
    //    {
    //        if( hit.GetComponent<FieldEnemyMng>() != null)
    //        {
    //            return true;
    //        }
    //    }
    //    else
    //    {
    //        if (hit.name == FieldPlayerMng.Hero.name)
    //        {
    //            return true;
    //        }
    //    }
    //    return false;
    //}

}
