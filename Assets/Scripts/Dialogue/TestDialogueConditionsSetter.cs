using System.Collections;
using System.Collections.Generic;

using UnityEngine;

using Articy.Unity;
using Articy.Lost_Time_Demo;
using Articy.Lost_Time_Demo.GlobalVariables;
using System;

// Test Class to set random Articy GlobalVariables for dialogue testing
public class TestDialogueConditionsSetter : MonoBehaviour
{

    TimeVaribles timeVariables;

    [SerializeField]
    private int SetDayTo = -1;
    [SerializeField]
    private int SetHourTo = -1;

    private bool timVarMessage = false;

    // Start is called before the first frame update
    void Start()
    {
        

    }

    void Awake() {
        //Can't set Articy Variables here for some reason.
    }

    // Update is called once per frame
    void Update()
    {
        if (timeVariables == null) {
            if (ArticyDatabase.GetAllOfType<TimeVaribles>().Count > 0) { //Very Ugly initialization hack
                Debug.Log("Articy decided timeVaribles should exist, yay");
                
                timeVariables = ArticyDatabase.GetAllOfType<TimeVaribles>()[0]; //Dirty hack to get the first timevariables item

                if (SetDayTo != -1) timeVariables.Day = SetDayTo;
                if (SetHourTo != -1) timeVariables.TimeOfDay = SetHourTo;

                //TODO: Way to set the other Articy Variables?
                // (Might just be easier to deal with it in code?)
            } else if (!timVarMessage) {
                timVarMessage = true;
                Debug.Log("Internal Screaming"); 
            } //End timeVaribles safety check
        } // Don't initalize any Articy Variables past this point
    }
}
