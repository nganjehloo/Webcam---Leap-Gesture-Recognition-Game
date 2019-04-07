using UnityEngine;
using System.Collections;
using Leap;

public class LeapBehavior : MonoBehaviour {
    Controller controller;

    void Start ()
    {
        controller = new Controller();
    }

    void Update ()
    {
    	//get the current frame of data from the Leap Motion controller when the function is called by the Unity engine.
        Frame frame = controller.Frame();
        // do something with the tracking data in the frame...
    }
}
