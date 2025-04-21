
using UnityEngine;

//Disgusting hack class to work around Articy shenanigans
public class CharacterNames {
    public static string mainCharacterName = "Irwin";
    public static string mainCharacterResourceName = "Irwin_Whitaker_";
    static string[] otherCharacterNames;

    private static bool _pop = false; //Have we populated the names list?

    static void PopulateNames() {
        otherCharacterNames = new string[4];
        otherCharacterNames[0] = "Ebony_Piers_";
        otherCharacterNames[1] = "Gibson_Piers_";
        otherCharacterNames[2] = "Eliza_Gresham_";
        otherCharacterNames[3] = "Melio_Gresham_";
    }

    //Disgusting hack to return the first partial match of the Characters' 1st name
    public static string FindFullName(string name) {
        if (!_pop) {
            _pop = true;
            PopulateNames(); //Hackiest way to populate a static array possible, but this whole class is a disgusting hack
        } 
        
        if (name != null) {
            foreach (string s in otherCharacterNames) {
                if (s.Contains(name)) {
                    return s;
                }
            }
        }

        return name; //Return the given input as a failure mode, which is hopefully correct enough to function
    }

    //Makes a name look nice for display
    public static string NiceDisplayName(string s) {
        if (s != null)
            return FindFullName(s).Replace("_", " ").Trim();
        else
            return s;
    }
}