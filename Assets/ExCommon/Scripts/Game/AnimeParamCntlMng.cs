using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimeParamCntlMng : StateMachineBehaviour {

	public enum PARAM
	{
		NON,
		BOOL,
		INT,
		FLOAT,
		MESSAGE,
	}

	[SerializeField]
	protected PARAM Key;

	[SerializeField]
    protected string ParamKey;

	[SerializeField]
    protected bool ChangeBool;

    [SerializeField]
    protected float Value;

    protected void setAnimeParam(Animator animator, PARAM key, string param_key, bool change = false)
    {
        switch (key)
        {
            case PARAM.BOOL:
                animator.SetBool(param_key, change);
                break;
            case PARAM.MESSAGE:
                animator.SendMessage(param_key);
                break;
            case PARAM.INT:
                animator.SetInteger(ParamKey, (int)Value);
                break;
            case PARAM.FLOAT:
                animator.SetFloat(ParamKey, Value);
                break;
        }
    }
}
