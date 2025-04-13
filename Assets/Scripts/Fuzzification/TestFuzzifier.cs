using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Vector2 = System.Numerics.Vector2;

public class TestFuzzifier : MonoBehaviour {

    private TimeFuzzifier timeFuzzifier;

    private List<List<Vector2>> list = new List<List<Vector2>>(new[] {
        new List<Vector2>(new[] {new Vector2(0f,1f), new Vector2(0.1f, 1f), new Vector2(0.25f, 0f)}.ToList()), 
        new List<Vector2>(new[] {new Vector2(0.1f, 0f), new Vector2(0.25f, 1f), new Vector2(0.4f, 0f)}.ToList()),
        new List<Vector2>(new[] {new Vector2(0.25f, 0f), new Vector2(0.4f, 1f), new Vector2(0.6f, 1f), new Vector2(0.75f, 0f)}.ToList()),
        new List<Vector2>(new[] {new Vector2(0.6f, 0f), new Vector2(0.75f, 1f), new Vector2(0.9f, 0f)}.ToList()),
        new List<Vector2>(new[] {new Vector2(0.75f, 0f), new Vector2(0.9f, 1f), new Vector2(1f, 1f)}.ToList())}.ToList());

    void Awake() {
        /**
        string output = "list: " + list.Count + ":\n";
        for (int i = 0; i < list.Count; i++) {
            output += i + ": " + list[i][0].ToString();
            for (int j = 1; j < list[i].Count; j++) {
                output += ", " + list[i][j].ToString();
            }
            output += "\n";
        }
        Debug.Log(output);*/

        timeFuzzifier = new TimeFuzzifier(list);
        Debug.Log(testClassification(0f) + "Very Late\n" +
                testClassification(0.12f) + "Very Late Still\n" +
                testClassification(0.15f) + "Late/Very Late even split\n" +
                testClassification(0.25f) + "Late\n" +
                testClassification(0.334f) + "On Time\n" +
                testClassification(0.5f) + "On Time\n" +
                testClassification(0.69f) + "Early\n" +
                testClassification(0.75f) + "Early\n" +
                testClassification(0.88f) + "Very Early\n" +
                testClassification(1f) + "Very Early");
    }

    void Update() {

    }

    private string testClassification(float f) {
        return "Tested position " + f + "f: " + timeFuzzifier.softCategorizeInput(f) + ", " + timeFuzzifier.hardCategorizeInput(f) + ", " + timeFuzzifier.enumCategorizeInput(f) + ". ";
    } 

}