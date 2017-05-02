using System.Collections;
using UnityEngine;

public class CardboardTriggerTest : MonoBehaviour
{
    public UnityEngine.UI.Toggle toggle;

	void Start ()
    {
		
	}
	
	void Update ()
    {
        if (GvrViewer.Instance.Triggered)
        {
            toggle.isOn = !toggle.isOn;
        }
    }
}
