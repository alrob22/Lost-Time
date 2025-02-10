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

public class DialogueBox : MonoBehaviour, IArticyFlowPlayerCallbacks
{
    [SerializeField]
    private GameObject bottomPanel;
    [SerializeField]
    private TextMeshProUGUI textComponent;
    public string[] lines;
    
    [SerializeField]
    float textSpeed; //Delay between each character being "typed" in textbox. Hook to the settings in future
    [SerializeField]
    int maxChars = 50; //Maximum # of Characters that can fit in this text box
    int index = 0; //Very ugly hack
    IEnumerator lineTypingEffect;

    #region TextDisplayHandling

    public bool talking = false, lineScrolling = false, start = true;
    private bool pressBuffer = false;
    // Start is called before the first frame update
    void Start()
    {
        textComponent.text = string.Empty;

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
        if (talking && Input.GetAxisRaw("Submit") == 1f && !pressBuffer) {
            pressBuffer = true;
            Debug.Log("hit spacebar"); 
            if (index < lines.Length && (textComponent.text == lines[index] || !lineScrolling)) {
                NextLine();
            }
            else if (lineScrolling)
            {
                StopCoroutine(lineTypingEffect);
                lineScrolling = false;
                textComponent.text = lines[index];
            }
            else
            {
                Debug.Log("Nonstandard dialogue box closure");
                CloseDialogueBox();
            }
        } else if (Input.GetAxisRaw("Submit") != 1f) {
            pressBuffer = false;
        }
    }

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
            textComponent.text = string.Empty;
            lineTypingEffect = TypeLine();
            StartCoroutine(lineTypingEffect);
        } else {
            CloseDialogueBox();
        }
    }
    
    void CloseDialogueBox() {
        textComponent.text = string.Empty;
        if (firstBranch != null) {
            Debug.Log("Playing next branch");
            GetComponent<ArticyFlowPlayer>().Play(firstBranch);
        } else {
            bottomPanel.SetActive(false);
            talking = false;
            lineScrolling = false;
            if (lineTypingEffect != null)
                StopCoroutine(lineTypingEffect);
            index = 0;
        }
    }

    IEnumerator TypeLine() {
        lineScrolling = true;
        foreach (char c in lines[index].ToCharArray()) {
            textComponent.text += c;
            yield return new WaitForSeconds(textSpeed);
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
                Debug.Log(displayName.DisplayName);
                //TODO: Display name
            }
            var frag = flowObject as DialogueFragment;
            if (frag != null) {
                txt = frag.Text;
                //TODO: Speaker Portrait/Color?
                Debug.Log("Text from DialogueFragment: " + txt);
            } else {
                var text = flowObject as IObjectWithLocalizableText;
                if (text != null) {
                    txt = text.Text;
                    //Debug.Log("Text from IObjectWithText: " + txt);
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

                    Debug.Log($"Fed the text box {lines.Count} lines: {String.Join("/", lines)}");
                    SetLines(lines.ToArray());
                } else {
                    //Just feed it to the text box as-is
                    Debug.Log($"Fed the text box whole line: {txt}");
                    string[] l = new string[1];
                    l[0] = txt;
                    SetLines(l);
                    Debug.Log(lines[0]);
                }
                StartDialogue();
            } else {
                //Jump to next branch or close the dialogue box
                CloseDialogueBox();
            }
        }
    }

    // Hack where we just pick the first branch of every dialogue
    private Branch firstBranch;

    public void OnBranchesUpdated(IList<Branch> someBranches) {
        //TODO: Handle multiple branches
        if (someBranches.Count > 0) {
            Debug.Log("Updating branches");
            firstBranch = someBranches[0];
        }
    }

    #endregion
}
