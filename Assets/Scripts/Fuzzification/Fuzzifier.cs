

using System;
using System.Collections;
using System.Collections.Generic;
using System.Numerics;

public abstract class Fuzzifier {

    List<List<Vector2>> inputCategories;
    
    public Fuzzifier(List<List<Vector2>> inputCategorization) {
        inputCategories = inputCategorization;
    }

    public float softCategorizeInput(float input) {
        return -1;
    }

    public int hardCategorizeInput(float input) {
        return -1;
    }
}