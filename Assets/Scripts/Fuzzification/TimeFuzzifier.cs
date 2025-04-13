
using System;
using System.Collections.Generic;
using System.Numerics;


public enum timeCategories {
        veryLate,
        late,
        onTime,
        early,
        veryEarly
    }

public class TimeFuzzifier : Fuzzifier {

    public TimeFuzzifier(List<List<Vector2>> inputCategories) : base(inputCategories) {
        if (inputCategories.Count < 5) {
            throw new ArgumentException("Passed less than 5 input categorizations to instatiate TimeCategorization.");
        }
    }

    public timeCategories enumCategorizeInput(float input) {
        return (timeCategories) base.hardCategorizeInput(input);
    }

}