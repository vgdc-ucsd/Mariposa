using UnityEngine;
using UnityEngine.UI;

public class DialogueScript : MonoBehaviour
{
	public GameObject DialogueWindow;
	private bool isActive;
	void Start () 
	{
		/*DialogueWindow.enabled = false;*/
		DialogueWindow.SetActive(false);
		isActive = false;
	}
	void Update () 
	{
		if(Input.GetKeyDown(KeyCode.R))
		{
			/*DialogueWindow.enabled = !DialogueWindow.enabled;*/
			isActive = !isActive;
			DialogueWindow.SetActive(isActive);
			Debug.Log("Toggled window");
		}
	}
}
