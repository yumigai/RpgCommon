using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimeInOutStateMng : AnimeParamCntlMng
{

    override public void OnStateEnter(Animator animator, AnimatorStateInfo animatorStateInfo, int layerIndex)
    {
        setAnimeParam(animator, Key, ParamKey, ChangeBool);

    }

    override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        setAnimeParam(animator, Key, ParamKey, !ChangeBool);

    }
}
