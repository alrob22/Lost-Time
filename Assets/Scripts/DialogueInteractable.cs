using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DialoagueInteractable : Interactable
{

    public DialogueBox dialogueBox;

    [SerializeField]
    private string CharacterName;
    
    public override void Interact()
    {
        if (!dialogueBox.talking) {
            base.Interact();

            dialogueBox.GetCharacterDialogue(CharacterName);
        }
        
    }

}