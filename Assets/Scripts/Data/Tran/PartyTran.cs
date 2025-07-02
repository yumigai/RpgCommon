using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[System.Serializable]
public class PartyTran  {

	public const int MAX_PARTY = 4;
	public const int MAX_MEMBER = 4;

	public int[] Members = new int[MAX_MEMBER] { -1, -1, -1, -1 };

	public PartyTran(){
	}

    public PartyTran(int unit_tran_id) {
        Members = new int[MAX_MEMBER] { unit_tran_id, -1, -1, -1 };
    }


}

