using UnityEngine;
using System.Collections;

public class TestTurretBehaviour : MonoBehaviour, ITurretBehaviour
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
