using System.Collections;
using System.Collections.Generic;
using Articy.Lost_Time_Demo;
using Articy.Unity;
using Unity.VisualScripting;
using UnityEngine;

public class DialoagueInteractable : Interactable
{

    public DialogueBox dialogueBox;

    [SerializeField]
    private string CharacterName;

    [SerializeField]
    private ArticyRef dangerousDialogueReference;
    [SerializeField]
    private bool UseDangerousRef = false;
    
    public override void Interact()
    {
        if (!dialogueBox.getTalking()) {
            base.Interact();
            
            if (UseDangerousRef && dangerousDialogueReference != null && dangerousDialogueReference.HasReference) {
                dialogueBox.PlayArticyRefDangerous(dangerousDialogueReference); //Unsafe hack to play a dialogue branch
            } else {
                dialogueBox.GetCharacterDialogue(CharacterName); //Sould be a safe way to find a characters dialogue hub
            }
        }
        
    }

}