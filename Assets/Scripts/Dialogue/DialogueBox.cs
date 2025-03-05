using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Articy.Unity;
using TMPro;
using System;
using Articy.Unity.Interfaces;
using Articy.Lost_Time_Demo;
using Articy.Unity.Utils;
using UnityEngine.UI;
using Unity.VisualScripting;

public class DialogueBox : MonoBehaviour, IArticyFlowPlayerCallbacks
{

    [SerializeField]
    private GameObject bottomDialogueBox;
    [SerializeField]
    private GameObject topDialogueBox;

    private GameObject currentBox;
    private TextMeshProUGUI dialogueTextBox, characterNameBox;
    private Image mainCharacterPortrait, otherCharacterPortrait;
    private SelectorBox dialogueSelector;
    public string[] currentDialogueLines;
    
    [SerializeField]
    int maxChars = 50; //Maximum # of Characters that can fit in this text box
    [SerializeField]
    float pressDelaySeconds = 0.25f;

    int index = 0; //Very ugly hack
    IEnumerator lineTypingEffect;

    public bool talking = false;
    bool lineScrolling = false, start = true;
    private bool pressBuffer = false, inputLock = false;

    // Start is called before the first frame update
    void Start()
    {
        bottomDialogueBox.SetActive(false);
        topDialogueBox.SetActive(false);
        UpdateBoxReference(bottomDialogueBox); //Shut up the random errors

        if (currentDialogueLines == null) {
            currentDialogueLines = new string[2];
            currentDialogueLines[0] = "These are the lines in case of an error"; 
            currentDialogueLines[1] = "you shouldn't be seeing these";
        }
    }

    // Update is called once per frame
    void Update()
    {
        //Start the blinky continue cursor once we've hit the end of a line
        if (!lineScrolling && !blinkCursor.Blinking && index < currentDialogueLines.Length && dialogueTextBox.text.Length == currentDialogueLines[index].Length) {
            blinkCursor.startBlink(blinkCursorSpeed);
        }

        if (talking && !inputLock && Input.GetAxisRaw("Submit") == 1f && !pressBuffer) {
            pressBuffer = true;
            if (index < currentDialogueLines.Length && (dialogueTextBox.text == currentDialogueLines[index] || !lineScrolling)) {
                NextLine();
                blinkCursor.stopBlink();
            }
            else if (lineScrolling)
            {
                StopCoroutine(lineTypingEffect);
                lineScrolling = false;
                dialogueTextBox.text = currentDialogueLines[index];
            }
            else
            {
                Debug.Log("Nonstandard dialogue box closure");
                CloseDialogueBox();
            }
            Invoke("noPressBuffer", pressDelaySeconds); //Hack input buffer to stop you from accidentally spamming through dialogue
        } else if (Input.GetAxisRaw("Submit") != 1f) {
            pressBuffer = false; //Additional way to get past the press buffer if the player REALLY wants to spam
        }
    }

    //Buffer inputs so we don't wind update in coversation by holding down the button too long
    private void notTalking() {
        talking = false;
    }

    //Buffer inputs, but for presses
    private void noPressBuffer() {
        pressBuffer = false; 
    }

    #region TextReadHandling

    public void SetLines(string[] newLines) {
        //Are we currently talking?
        if (!lineScrolling) {
            if (!currentBox.activeSelf) {
                currentBox.SetActive(true);
            }
            currentDialogueLines = newLines;
            index = 0;
        } else {
            //If so, add lines to the end of the current array by switching in place
            List<string> temp = new List<string>(currentDialogueLines);
            temp.AddRange(newLines);
            currentDialogueLines = temp.ToArray();
            //Debug.Log($"Switcheroo: {currentDialogueLines[currentDialogueLines.Length - 1]}");
        }
    }

    public void StartDialogue() {
        //Start is a horrible hack to handle the empty starting node in the dialogue tree
        if (start) {
            start = false;
            SelectNonBlockingDiaglouegBox();
            CloseDialogueBox();
        } else if (!talking) {
            talking = true;
            if (!currentBox.activeSelf) {
                currentBox.SetActive(true);
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
        if (index < currentDialogueLines.Length && !lineScrolling) {
            dialogueTextBox.text = string.Empty;
            lineTypingEffect = TypeLine();
            StartCoroutine(lineTypingEffect);
        } else {
            CloseDialogueBox();
        }
    }
    
    void CloseDialogueBox(bool deadEnd = false) {
        dialogueTextBox.text = string.Empty;
        if (!deadEnd && index <= currentDialogueLines.Length && branches != null && branches.Count > 0) {
            //Debug.Log("Playing next branch");
            PlayNextBranch();
        } else {
            start = true;
            currentBox.SetActive(false);
            Invoke("notTalking", 0.25f); // World's hackiest hack to buffer inputs to prevent chain conversations
            lineScrolling = false;
            if (lineTypingEffect != null)
                StopCoroutine(lineTypingEffect);
            //index = 0;
        }
    }

    #endregion

    #region SelectNonObscuringBox

    void SelectNonBlockingDiaglouegBox() {
        Rect playerScreenBounds = getScreenspaceBoundingBox(GameObject.FindGameObjectWithTag("Player"));

        //Debug.Log($"playerBounds: {playerScreenBounds}, top: {screenspaceOverlap(topDialogueBox, playerScreenBounds)}, bottom: {screenspaceOverlap(bottomDialogueBox, playerScreenBounds)}");

        if (!screenspaceOverlap(topDialogueBox, playerScreenBounds) && screenspaceOverlap(bottomDialogueBox, playerScreenBounds)) {
            //When only the top is not overlapping, use it as the dialogue box
            UpdateBoxReference(topDialogueBox);
        } else {
            //Otherwise, be normal & use the bottom one
            UpdateBoxReference(bottomDialogueBox);
        }
    }

    //Quick & dirty way of getting screenspace coordinates from a bounding box
    private Rect getScreenspaceBoundingBox(GameObject gameObject) 
    {
        //Debug.Log($"Finding bounds for: {gameObject.name}, using camera {Camera.main.name}, min {gameObject.GetComponent<Collider>().bounds.min} max {gameObject.GetComponent<Collider>().bounds.max}");

        //Convert the extreme bounds of the given objects bounding box to screen space
        // (ignoring possible issues with getting, say, 2 points which happen to be on a line from the cammera atm)
        Vector3 min = Camera.main.WorldToScreenPoint(gameObject.GetComponent<Collider>().bounds.min);
        Vector3 max = Camera.main.WorldToScreenPoint(gameObject.GetComponent<Collider>().bounds.max);
        //Throw up a bounding Rect from the min corner in screen space
        return new Rect(new Vector2(Math.Min(min.x, max.x), Math.Min(min.y, max.y)), new Vector2(Math.Abs(max.x - min.x), Math.Abs(max.y - min.y)));
    }

    //checks the freshly-minted screenspace bounding box against the ui element given
    private bool screenspaceOverlap(GameObject uiObject, Rect targetBoundingBox) {
        //Convert UI object to Rect - built-in recttransform only works if both object have the same parent (obviously not comparing against non-UI objects)
        Rect uiBounds = new Rect(uiObject.GetComponent<RectTransform>().position.x, uiObject.GetComponent<RectTransform>().position.y, uiObject.GetComponent<RectTransform>().rect.width, uiObject.GetComponent<RectTransform>().rect.height);

        //Debug.Log($"object {uiObject.name} bounds {uiBounds}");

        return uiBounds.Overlaps(targetBoundingBox);
    }

    #endregion

    #region VisibleUIHandling

    private Color transparent = new Color(255, 255, 255, 0);

    void UpdateBoxReference(GameObject currentBox) {
        foreach (TextMeshProUGUI box in currentBox.GetComponentsInChildren<TextMeshProUGUI>()) {
            box.text = String.Empty;
            if (box.gameObject.name.Equals("DialogueTextbox")) dialogueTextBox = box;
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

        dialogueSelector = currentBox.GetComponentInChildren<SelectorBox>();
        this.currentBox = currentBox;
    }

    void UpdateCharacterPortrait(string characterName, string react) {
        if (react.Equals("Nuetral")) {
            react = "Neutral";
        }

        if (characterName.Equals(CharacterNames.mainCharacterName)) {
            mainCharacterSpeaking(react);
        } else {
            otherCharacterSpeaking(CharacterNames.bakerCharacterName
            , react);
        }
    }

    void mainCharacterSpeaking(string react) {
        mainCharacterPortrait.color = Color.white;
        otherCharacterPortrait.color = transparent;
        //TODO: Get an actual main character name
        //loadCharacterPortrait(mainCharacterPortrait, CharacterNames.mainCharacterName, react);
    }

    void otherCharacterSpeaking(string cn, string react) {
        mainCharacterPortrait.color = transparent;
        otherCharacterPortrait.color = Color.white;

        loadCharacterPortrait(otherCharacterPortrait, cn, react);
    }

    void loadCharacterPortrait(Image portrait, string cn, string react) {
        Sprite face = Resources.Load<Sprite>($"{cn.Replace("_","").Replace(" ","")}/{cn+react}");
        //Debug.Log(face);
        if (face != null) {
            portrait.sprite = face;
        } else {
            Debug.Log($"Could not load character's face: {cn+react} @ {cn.Replace("_","").Replace(" ", "")}/{cn+react}");
        }
    }

    #endregion

    #region BranchSelectionAndUI

    //Absolute hack to test the selection UI itself
    /**
    void testFunc() {
        inputLock = true; //Stop advancing dialogue during the test
        string[] testOptions = new string[] {"one", "2"};//, "tres", "IV"};
        dialogueSelector.Setup(testOptions, testFuncCallback);
    }

    void testFuncCallback(int i) {
        inputLock = false; //Dialogue can start again now
        //index = lines.Length; //Don't play a line of dialogue
        Debug.Log(i);
        GetComponent<ArticyFlowPlayer>().Play(branches[0]);
    }
    */

    void PlayNextBranch() {
        if (branches.Count > 1) {
            inputLock = true; //No reading input during selection
            IList<string> options = new List<string>();

            IList<Branch> newBranches = new List<Branch>(branches); //Remove invalid branches from the list, so PlaySelectedBranch() doesn't pick the wrong one
            foreach (Branch b in branches) {
                if (b.IsValid) //Don't pass impossible branches for selection
                    options.Add(b.DefaultDescription);
                else
                    newBranches.Remove(b);
            }

            branches = newBranches; //Can't remove from a structure you're looping over

            dialogueSelector.Setup(options, PlaySelectedBranch); //Pass the dialogue options 
        } else {
            GetComponent<ArticyFlowPlayer>().Play(branches[0]); //If there's only one branch, we follow it
        }
    }

    void PlaySelectedBranch(int i) {
        inputLock = false; //Now we read input again
        GetComponent<ArticyFlowPlayer>().Play(branches[i]);
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
        foreach (char c in currentDialogueLines[index].ToCharArray()) {
            dialogueTextBox.text += c;
            yield return new WaitForSeconds(textCharacterDelay);
        }
        lineTypingEffect = null;
        lineScrolling = false;
    }

    #endregion

    #region ArticyHandling

    public void OnFlowPlayerPaused(IFlowObject flowObject) {
        if (flowObject == null) {
            //Debug.Log("Dead End!");
            CloseDialogueBox(true);
        } else if (!start) {
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
                //Debug.Log(ArticyDatabase.GetObject<DialogueHelper>(frag.TechnicalName).Speaker.TechnicalName);
            } else {
                //Currently Unused
                var text = flowObject as IObjectWithLocalizableText;
                if (text != null) {
                    txt = text.Text;
                    Debug.Log("Text from IObjectWithLocalizableText: " + txt);
                }
            }
            
            // I think we can get this text directly from the Branches themselves, which is when we would need it.
            /**
            var textHaver = flowObject as IObjectWithMenuText;
            if (textHaver != null && !textHaver.MenuText.Value.IsNullOrEmpty()) {
                Debug.Log($"Menu Text: {textHaver.MenuText.Value}");
            }
            */

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
        if (someBranches.Count > 0) {
            branches = someBranches;
        } else {
            branches = null;
        }
    }

    #endregion
}
