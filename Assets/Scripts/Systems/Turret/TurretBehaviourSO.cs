using UnityEngine;
using System.Collections;

[CreateAssetMenu(fileName = "TurretBehaviorSO", menuName = "TurretBehaviorSO", order = 1)]
public class TurretBehaviourSO : ScriptableObject, ITurretBehaviour
{
	public void Act(Turret turret)
	{
		if (turret.HasBattery && turret.IsOn)
		{
			// Charge and fire
			if (!turret.IsFiring)
			{
				turret.TurnToTarget();
				if (!turret.IsCoolingDown && !turret.IsCharging)
				{
                    turret.StartChargingRoutine();
                }
            }
		}
	}
}
