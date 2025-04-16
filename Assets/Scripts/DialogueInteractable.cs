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
        if (!dialogueBox.talking) {
            base.Interact();
            
            if (UseDangerousRef && dangerousDialogueReference != null & dangerousDialogueReference.HasReference) {
                dialogueBox.PlayCharacterHubDangerous(ArticyDatabase.GetObject<Hub>(dangerousDialogueReference.instanceId)); //Unsafe hack to play a dialogue branch
            } else {
                dialogueBox.GetCharacterDialogue(CharacterName); //Safe way to find a characters dialogue hub
            }
        }
        
    }

}