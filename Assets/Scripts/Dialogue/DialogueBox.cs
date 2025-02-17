using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Articy.Unity;
using TMPro;
using System;
using System.ComponentModel;
using Articy.Unity.Interfaces;
using Articy.Lost_Time_Demo;
using Articy.Unity.Utils;
using System.Xml.Linq;
using System.Linq;
using UnityEngine.UI;
using UnityEngine.Android;

public class DialogueBox : MonoBehaviour, IArticyFlowPlayerCallbacks
{
    [SerializeField]
    private GameObject bottomPanel;
    [SerializeField]
    private GameObject topPanel;

    private GameObject currentPanel;
    private TextMeshProUGUI dialogueBox, characterNameBox;
    private Image mainCharacterPortrait, otherCharacterPortrait;
    public string[] lines;
    
    [SerializeField]
    int maxChars = 50; //Maximum # of Characters that can fit in this text box
    int index = 0; //Very ugly hack
    IEnumerator lineTypingEffect;

    public bool talking = false;
    bool lineScrolling = false, start = true;
    private bool pressBuffer = false;

    // Start is called before the first frame update
    void Start()
    {
        UpdateBoxReference(bottomPanel);
        bottomPanel.SetActive(false);

        if (lines == null) {
            lines = new string[2];
            lines[0] = "These are the lines in case of an error"; 
            lines[1] = "you shouldn't be seeing these";
        }
    }

    // Update is called once per frame
    void Update()
    {
        //Start the blinky continue cursor once we've hit the end of a line
        if (!lineScrolling && !blinkCursor.Blinking && index < lines.Length && dialogueBox.text.Length == lines[index].Length) {
            blinkCursor.startBlink(blinkCursorSpeed);
        }

        if (talking && Input.GetAxisRaw("Submit") == 1f && !pressBuffer) {
            pressBuffer = true;
            if (index < lines.Length && (dialogueBox.text == lines[index] || !lineScrolling)) {
                NextLine();
                blinkCursor.stopBlink();
            }
            else if (lineScrolling)
            {
                StopCoroutine(lineTypingEffect);
                lineScrolling = false;
                dialogueBox.text = lines[index];
            }
            else
            {
                Debug.Log("Nonstandard dialogue box closure");
                CloseDialogueBox();
            }
        } else if (Input.GetAxisRaw("Submit") != 1f) {
            pressBuffer = false; //Hack input buffer to stop you from accidentally spamming through dialogue
            //TODO: Replace with delay system that lets you just hold to button auto-progress dialogue
        }
    }

    #region TextDisplayHandling

    public void SetLines(string[] newLines) {
        //Are we currently talking?
        if (!lineScrolling) {
            if (!bottomPanel.activeSelf) {
                bottomPanel.SetActive(true);
            }
            lines = newLines;
            index = 0;
        } else {
            //If so, add lines to the end of the current array by switching in place
            List<string> temp = new List<string>(lines);
            temp.AddRange(newLines);
            lines = temp.ToArray();
            Debug.Log($"Switcheroo: {lines[lines.Length - 1]}");
        }
    }

    public void StartDialogue() {
        //Start is a horrible hack to handle the empty starting node in the tree
        if (start) {
            start = false;
            CloseDialogueBox();
        } else if (!talking) {
            talking = true;
            if (!bottomPanel.activeSelf) {
                bottomPanel.SetActive(true);
            }
            index = 0;
            lineTypingEffect = TypeLine();
            StartCoroutine(lineTypingEffect);
        } else if (!lineScrolling) {
            lineTypingEffect = TypeLine();
            StartCoroutine(lineTypingEffect);
        }
    }

    void NextLine() {
        index++;
        if (index < lines.Length && !lineScrolling) {
            dialogueBox.text = string.Empty;
            lineTypingEffect = TypeLine();
            StartCoroutine(lineTypingEffect);
        } else {
            CloseDialogueBox();
        }
    }
    
    void CloseDialogueBox() {
        dialogueBox.text = string.Empty;
        if (index <= lines.Length && branches != null) {
            //Debug.Log("Playing next branch");
            GetComponent<ArticyFlowPlayer>().Play(branches[0]); //TODO: Fix Hack
        } else {
            currentPanel.SetActive(false);
            talking = false;
            lineScrolling = false;
            if (lineTypingEffect != null)
                StopCoroutine(lineTypingEffect);
            index = 0;
        }
    }

    #endregion

    #region VisibleBoxHandling

    private Color transparent = new Color(255, 255, 255, 0);

    void UpdateBoxReference(GameObject currentBox) {
        foreach (TextMeshProUGUI box in currentBox.GetComponentsInChildren<TextMeshProUGUI>()) {
            box.text = String.Empty;
            if (box.gameObject.name.Equals("DialogueTextbox")) dialogueBox = box;
            else if (box.gameObject.name.Equals("NameTextbox")) characterNameBox = box;
        }
        blinkCursor = currentBox.GetComponentInChildren<TextCursorAnimate>();
        foreach (Image i in currentBox.GetComponentsInChildren<Image>()) {
            if (i.gameObject.name.Equals("MainCharacter")) {
                mainCharacterPortrait = i;
                i.color = transparent;
            } 
            else if (i.gameObject.name.Equals("OtherCharacter")) {
                otherCharacterPortrait = i;
                i.color = transparent;
            }
        }

        currentPanel = currentBox;
    }

    void UpdateCharacterPortrait(string cn, string react) {
        if (cn.Equals(CharacterNames.mainCharacterName)) {
            mainCharacterSpeaking();
        } else {
            //TODO: Much better way of handling searching for character names - HashMap?
            otherCharacterSpeaking();
        }
        //TODO: Handle Character Portraits/Reactions
    }

    void mainCharacterSpeaking() {
        mainCharacterPortrait.color = Color.white;
        otherCharacterPortrait.color = transparent;
    }

    void otherCharacterSpeaking() {
        mainCharacterPortrait.color = transparent;
        otherCharacterPortrait.color = Color.white;
    }

    #endregion

    #region Animations

    [SerializeField]
    float textCharacterDelay = 0.03f; //Delay between each character being "typed" in textbox. Hook to the settings in future
    [SerializeField]
    float blinkCursorSpeed = 0.75f; //Speed of cursor blinking

    TextCursorAnimate blinkCursor; //Reference to blinky arrow for when you're done of the curent line

    IEnumerator TypeLine() {
        lineScrolling = true;
        foreach (char c in lines[index].ToCharArray()) {
            dialogueBox.text += c;
            yield return new WaitForSeconds(textCharacterDelay);
        }
        lineTypingEffect = null;
        lineScrolling = false;
    }

    #endregion

    #region ArticyHandling

    public void OnFlowPlayerPaused(IFlowObject flowObject) {
        if (!start) {
            //Don't do crap on startup
            string txt = null;
            var displayName = flowObject as IObjectWithDisplayName;
            if (displayName != null) {
                //Display names aren't a thing?
                Debug.Log(displayName);
            }
            var frag = flowObject as DialogueFragment;
            if (frag != null) {
                txt = frag.Text;
                characterNameBox.text = ArticyDatabase.GetObject<DialogueHelper>(frag.TechnicalName).Speaker.TechnicalName;
                UpdateCharacterPortrait(ArticyDatabase.GetObject<DialogueHelper>(frag.TechnicalName).Speaker.TechnicalName, ArticyDatabase.GetObject<DialogueHelper>(frag.TechnicalName).GetFeatureCutsceneInformation().CharReact.ToString());
            } else {
                //Currently Unused
                var text = flowObject as IObjectWithLocalizableText;
                if (text != null) {
                    txt = text.Text;
                    Debug.Log("Text from IObjectWithLocalizableText: " + txt);
                }
            }

            if (!txt.IsNullOrEmpty()) {
                //There is dialogue to load
                
                //If text is too long for the dialogue box
                if (txt.Length > maxChars) {
                    string cur = "";
                    string[] words = txt.Split(" ");
                    List<string> lines = new List<string>();
                    //Split it up into multiple lines
                    for (int i = 0; i < words.Length;) {
                        if (words[i].Length > maxChars && cur.IsNullOrEmpty()) {
                            //If a word is too long, split it in half & add it to the text box;
                            lines.Add(words[i].Substring(0, maxChars - 1) + "-");
                            words[i] = "-" + words[i].Substring(maxChars);
                        } else {
                            //Add words until the line would be too long to print.
                            if (cur.Length + words[i].Length + 1 <= maxChars) {
                                cur += " " + words[i];
                                i++; //Remember to iterate the loop when necessary, this is the only case where we want to increment i
                            } else {
                                lines.Add(cur);
                                cur = String.Empty;
                            }
                        }
                    }
                    lines.Add(cur);

                    //Debug.Log($"Fed the text box {lines.Count} lines: {String.Join("/", lines)}");
                    SetLines(lines.ToArray());
                } else {
                    //Just feed it to the text box as-is
                    //Debug.Log($"Fed the text box whole line: {txt}");
                    string[] l = new string[1];
                    l[0] = txt;
                    SetLines(l);
                }
                StartDialogue();
            } else {
                //Jump to next branch or close the dialogue box
                CloseDialogueBox();
            }
        }
    }

    // Hack where we just pick the first branch of every dialogue
    private IList<Branch> branches;

    public void OnBranchesUpdated(IList<Branch> someBranches) {
        //TODO: Handle multiple branches
        if (someBranches.Count > 0) {
            //Debug.Log("Updating branches");
            branches = someBranches;
        } else {
            branches = null;
        }
    }

    #endregion
}
