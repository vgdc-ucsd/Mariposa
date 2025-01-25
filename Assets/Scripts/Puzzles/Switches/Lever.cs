using UnityEngine;
/// <summary>
/// Interactable object. Lever that turns on when approached or touched and then off when touched again.
/// </summary>
public class Lever  : Switch
{
    /// <summary>
   /// boolean for if lever is pulled or not
   /// </summary>
   public bool SwitchToggled;
   
   /// <summary>
   /// Override of TriggerSwitch from switch class which switchs lever to opposite value
   /// </summary>
   
   @Override
   public abstract void TriggerSwitch() 
   {
    SwitchToggled=!SwitchToggled;
    
   }


}
