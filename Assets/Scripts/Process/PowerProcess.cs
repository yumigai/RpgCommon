using System.Collections;
using System.Collections.Generic;
using System.Linq;

public static class PowerProcess
{
	public static void execPower(PowerMast pow, List<UnitStatusTran> trans) {
		execPower(pow, null, trans);
	}
	public static void execPower(PowerMast pow, UnitStatusTran tran) {
		execPower(pow, null, tran);
	}
	public static void execPower(PowerMast pow, UnitStatusTran user, List<UnitStatusTran> trans) {
		foreach (UnitStatusTran t in trans) {
			execPower(pow, user, t);
		}
	}

	public static void execPower(PowerMast pow, UnitStatusTran user, UnitStatusTran tran) {
		switch (pow.Spec) {
			case PowerMast.SPEC.ATTACK:
			break;
			case PowerMast.SPEC.HEAL:
			heal(pow, tran);
			break;
			case PowerMast.SPEC.CURE_POISON:
			case PowerMast.SPEC.CURE_STAN:
			case PowerMast.SPEC.CURE_BAD:
			case PowerMast.SPEC.CURE_PANIC:
			cure(pow, tran);
			break;
			case PowerMast.SPEC.HEAL_CURE:
			cure(pow, tran);
			heal(pow, tran);
			break;
			case PowerMast.SPEC.RESURRECT:
			tran.Status.Hp = 1;
			heal(pow, tran);
			break;
		}
	}

	private static void heal(PowerMast pow, UnitStatusTran tran) {
		if (tran.Hp > 0) {
			tran.Status.heal(pow.PhysicsPower);
			tran.Status.Mp += pow.MagicPower;
		}
	}

	private static void cure(PowerMast pow, UnitStatusTran tran) {
		if (tran.Hp > 0) {
			switch (pow.Spec) {
				case PowerMast.SPEC.CURE_BAD:
				case PowerMast.SPEC.HEAL_CURE:
				tran.removeBuff(BuffTran.TYPE.PANIC);
				tran.removeBuff(BuffTran.TYPE.CURSE);
				tran.removeBuff(BuffTran.TYPE.POISON);
				tran.removeBuff(BuffTran.TYPE.STAN);
				break;
				case PowerMast.SPEC.CURE_PANIC:
				tran.removeBuff(BuffTran.TYPE.PANIC);
				break;
				case PowerMast.SPEC.CURE_CURSE:
				tran.removeBuff(BuffTran.TYPE.CURSE);
				break;
				case PowerMast.SPEC.CURE_POISON:
				tran.removeBuff(BuffTran.TYPE.POISON);
				break;
				case PowerMast.SPEC.CURE_STAN:
				tran.removeBuff(BuffTran.TYPE.STAN);
				break;
			}
		}
	}

}