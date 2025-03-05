using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DialoagueInteractable : Interactable
{

    public string[] lines;
    public DialogueBox dialogueBox;
    
    public override void Interact()
    {
        if (!dialogueBox.talking) {
            base.Interact();

            //dialogueBox.SetLines(lines);
            dialogueBox.StartDialogue();
        }
        
    }

}