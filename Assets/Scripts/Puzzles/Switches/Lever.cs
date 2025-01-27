using UnityEngine;
/// <summary>
/// Interactable object. Lever that turns on when approached or touched and then off when touched again.
/// </summary>
public class Lever  : Switch
{
   
   /// <summary>
   /// Override of TriggerSwitch from switch class which switchs lever to opposite value
   /// calls public ChangeState method to open a door or close a door
   /// </summary>
   
   @Override
   public abstract void TriggerSwitch() 
   {
        SwitchToggled=!SwitchToggled;
        ChangeState();
    
   }


}
