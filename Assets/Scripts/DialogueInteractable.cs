using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DialoagueInteractable : Interactable
{

    public string[] lines;
    public DialogueBox dialogueBox;
    
    public override void Interact()
    {
        Debug.Log("Started Talking");
        if (!dialogueBox.talking) {
            base.Interact();

            Debug.Log("This is working atm");

            dialogueBox.SetLines(lines);
            dialogueBox.StartDialogue();
        }
        
    }

}