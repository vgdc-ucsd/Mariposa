using UnityEngine;
/// <summary>
/// Interactable object. Lever that turns on when approached or touched and then off when touched again.
/// </summary>
public class Lever  : Switch
{
    /// <summary>
   /// boolean for if lever is pulled or not
   /// </summary>
   public bool isPulled;
   
   /// <summary>
   /// Override of TriggerSwitch from switch class which checks if lever is 
   /// pulled and either pushes it back up or pulls it if unpulled
   /// when pulled lever should "do some action" that will be implemented later
   /// </summary>
   
   @Override
   public abstract void TriggerSwitch() 
   {
    if(isPulled)
    {
        isPulled=false;
    }
    isPulled=true;
    
   }


}
