using System;
using UnityEngine;

using Articy.Unity;
using Articy.Lost_Time_Demo;
using Articy.Lost_Time_Demo.GlobalVariables;
using Articy.Unity.Utils;

public class TimeHandler {
    int hour, minute, second, day;

    public int Hour {
        get => hour;
    }

    public int Minute {
        get => minute;
    }

    public int Second {
        get => second;
    }

    public int Day {
        get => day;
    }

    TimeVaribles timeVariables;

    public TimeFuzzifier fuzzyTime;

    public TimeHandler() {
        if (ArticyDatabase.GetAllOfType<TimeVaribles>().Count > 0) { //VInitialization Safety Check
                Debug.Log("Articy decided timeVaribles should exist, yay");
                
                timeVariables = ArticyDatabase.GetAllOfType<TimeVaribles>()[0]; //Dirty hack to get the first timevariables item
            } else Debug.LogError("Articy decided timeVaribles shouldn't exist. Queue internal screaming."); 
    }

    // Convert the in-game time from seconds to hours, minutes, and seconds
    public void UpdateTime(float time) {
        if ((hour + day) * 3600 < time - 3600) {
            if (hour >= 23) {
                day++;
                hour = 0;
                timeVariables.Day = day; //Update Articy as needed
            } else {
                hour++;
            }

            timeVariables.TimeOfDay = hour; //Update Articy as needed
        }
        
        if ((minute + 1 + ((hour + day) * 3600)) * 60 < time) {
            minute = Mathf.FloorToInt((time % 3600) / 60);
        }

        second = Mathf.FloorToInt(time % 60);
    }

    //Returns how late it is compared to the current Time value
    public TimeCategories GetLateness(int targetMinute, int targetHour, int windowInMinutes = 60, int targetDay = -1) {
        targetDay = targetDay != -1 ? day : targetDay; //Assume today is the target if not provided

        //Break down the difference in current + target time into minutes, normalize by the time window, & clamp to 0-1 range
        return fuzzyTime.enumCategorizeInput(Math.Clamp(((((float) day - targetDay) * 24 + hour - targetHour) * 60 + minute - targetMinute)/windowInMinutes + 0.5f, 0f, 1f));
    }
}