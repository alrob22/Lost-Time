
using System;
using System.Collections.Generic;
using System.Numerics;


public enum TimeCategories {
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

    public TimeCategories enumCategorizeInput(float input) {
        return (TimeCategories) base.WeightedCategorizeInput(input);
    }

}