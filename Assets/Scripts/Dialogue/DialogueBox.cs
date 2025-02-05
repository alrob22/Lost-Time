using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class DialogueBox : MonoBehaviour
{
    [SerializeField]
    private GameObject bottomPanel;
    [SerializeField]
    private TextMeshProUGUI textComponent;
    public string[] lines;
    [SerializeField]
    float textSpeed; //Delay between each character being "typed" in textbox. Hook to the settings in future
    int index = 0; //Very ugly hack
    IEnumerator lineTypingEffect;

    public bool talking = false;
    private bool pressBuffer = false;
    // Start is called before the first frame update
    void Start()
    {
        textComponent.text = string.Empty;

        bottomPanel.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        if (talking && Input.GetAxisRaw("Submit") == 1f && !pressBuffer) {
            pressBuffer = true;
            if (textComponent.text == lines[index]) {
                NextLine();
            }
            else
            {
                StopCoroutine(lineTypingEffect);
                textComponent.text = lines[index];
            }
        }
        else if (Input.GetAxisRaw("Submit") != 1f) {
            pressBuffer = false;
        }
    }

    public void SetLines(string[] newLines) {
        if (!talking) {
            if (!bottomPanel.activeSelf) {
                bottomPanel.SetActive(true);
            }
            lines = newLines;
            index = 0;
        }
    }

    public void StartDialogue() {
        if (!talking) {
            talking = true;
            if (!bottomPanel.activeSelf) {
                bottomPanel.SetActive(true);
            }
            index = 0;
            lineTypingEffect = TypeLine();
            StartCoroutine(lineTypingEffect);
        }
    }

    void NextLine() {
        if (index < lines.Length) {
            index++;
            textComponent.text = string.Empty;
            lineTypingEffect = TypeLine();
            StartCoroutine(lineTypingEffect);
        } else {
            textComponent.text = string.Empty;
            bottomPanel.SetActive(false);
            talking = false;
            index = 0;
        }
    }

    IEnumerator TypeLine() {
        foreach (char c in lines[index].ToCharArray()) {
            textComponent.text += c;
            yield return new WaitForSeconds(textSpeed);
        }
    }
}
