

using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;

using UnityEngine;
using Vector2 = System.Numerics.Vector2;

public abstract class Fuzzifier {

    List<List<Vector2>> inputCategories;
    
    public Fuzzifier(List<List<Vector2>> inputCategorization) {
        inputCategories = inputCategorization;
        //TODO: Check validity of the given inputCategorization
    }

    //This is suppossed the return an weighted average of the category of beloging
    public float softCategorizeInput(float input) {
        List<Vector2> list = findCategoryMembership(input);

        float sum = 0;

        foreach (Vector2 v in list) {
            sum += v.X * v.Y;
        }

        return sum;
    }

    public int hardCategorizeInput(float input) {
        List<Vector2> membership = findCategoryMembership(input);
        membership.Sort(delegate(Vector2 a, Vector2 b) {return Math.Sign(b.Y - a.Y);}); //Sort by value of highest belonging (Y)
        
        /**
        string debug = "";
        foreach (Vector2 v in membership) {debug += v.ToString();}
        Debug.Log(debug);*/

        return (int) Math.Floor(membership[0].X); //Return category value from X
    }

    private List<Vector2> findCategoryMembership(float input) {
        List<Vector2> membership = new List<Vector2>();

        for (int category = 0; category < inputCategories.Count; category++) {
            //Point is within the x span of the category
            if (inputCategories[category].Count > 0 && inputCategories[category][0].X <= input && inputCategories[category][inputCategories[category].Count-1].X >= input) {
                //Debug.Log("Point " + input + " was within span [" + inputCategories[category][0].X + "-" + inputCategories[category][inputCategories[category].Count-1].X + "]");

                //Find the relevant line segment in the category
                for (int lineSeg = 0; lineSeg < inputCategories[category].Count - 1; lineSeg++) {
                    if (inputCategories[category][lineSeg].X <= input & inputCategories[category][lineSeg+1].X >= input) {
                        //Found one, calculate resultant y value by linear interpolation
                        float result = ((inputCategories[category][lineSeg+1].Y - inputCategories[category][lineSeg].Y)/(inputCategories[category][lineSeg + 1].X - inputCategories[category][lineSeg].X)
                                        * (input - inputCategories[category][lineSeg].X) + inputCategories[category][lineSeg].Y);

                        membership.Add(new Vector2(category, result)); //Add the current category & the level of belonging to the membership list
                        break;
                    }
                }
            }
        }

        if (membership.Count == 0) {
            Debug.Log("Internal Screaming");
        }

        return membership;
    }
}