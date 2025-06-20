using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimeEnterCntlMng : AnimeParamCntlMng
{

    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        setAnimeParam(animator, Key, ParamKey, ChangeBool);

    }
}
