using UnityEngine;
using System.Collections;

public class TestTurretBehaviour : MonoBehaviour, ITurretBehaviour
{

	public void Act(Turret turret)
	{
		if (turret.HasBattery && (turret.IsOn || turret.type == Turret.TurretType.Laser))
		{
			// Charge and fire
			if (!turret.IsFiring && turret.type == Turret.TurretType.Projectile)
			{
				if (!turret.IsCoolingDown && !turret.IsCharging)
				{
					turret.TurnToTarget();
					if (turret.IsLookingAtPlayer())
					{
						turret.StartChargingRoutine();
					}
                }
            }
			else if (turret.type == Turret.TurretType.Laser) 
			{
				turret.StartFireRoutine();
			}
		}

	}

}
