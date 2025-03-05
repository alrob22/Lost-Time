using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Articy.Unity;
using Articy.Lost_Time_Demo;
using System;
using UnityEngine.UI;
using TMPro;

public class SelectorBox : MonoBehaviour
{
    [SerializeField]
    Color highlightColor = new Color(178, 122, 40); //The yellow from the mockup
    [SerializeField]
    private float cutoff = 0.7f;
    Color oldTextColor;
    int curSel = 0, selNum;
    Image curArrow;
    TextMeshProUGUI curText;
    bool on = false, delay = true;

    public delegate void CallbackFunc(String selected); //Placeholder type so we can pass a selection handler to the dialogue box
    private CallbackFunc callbackFunc;

    // Start is called before the first frame update
    void Start()
    {
        for(int i = 0; i < transform.childCount; i++) {
            transform.GetChild(i).gameObject.SetActive(false);
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (on) {
            float i = Input.GetAxis("Vertical");
            if (Input.GetAxisRaw("Submit") == 1f) {
                //TODO: nice animation
                //Debug.Log("Chose something");
                hide();
            } else if (!delay && i > cutoff) {
                //Debug.Log("Pressed up on the keyboard");
                curSel = constrain(curSel-1);
                updateCurrent();
            } else if (!delay && i < -cutoff) {
                //Debug.Log("Pressed down on the keyboard");
                curSel = constrain(curSel+1);
                updateCurrent();
            } else if (delay && i == 0) {
                delay = false;
            }
        }
    }

    public void Setup(IList<String> branches, CallbackFunc func) {
        on = true;
        int i = 0;
        selNum = branches.Count;
        foreach (String s in branches) {
            if (i < transform.childCount) {
                if (!transform.GetChild(i).gameObject.activeSelf) {
                    transform.GetChild(i).gameObject.SetActive(true);
                }
                transform.GetChild(i).gameObject.GetComponentInChildren<TextMeshProUGUI>().text = s;
                i++;
            }
        }
        for (;i < transform.childCount; i++) {
            transform.GetChild(i).gameObject.SetActive(false);
        }
        curSel = 0;
        updateCurrent();

        callbackFunc = func;
    }

    void updateCurrent() {
        delay = true;
        Invoke("delayHack", 0.25f); //World's worst hack to add in an input delay

        if (curText)
            curText.color = oldTextColor;
        if (curArrow)
            curArrow.color = new Color(255, 255, 255, 0);

        curText = transform.GetChild(curSel).gameObject.GetComponentInChildren<TextMeshProUGUI>();
        curArrow = transform.GetChild(curSel).gameObject.GetComponentInChildren<Image>();

        oldTextColor = curText.color;
        curText.color = highlightColor;
        curArrow.color = Color.white;
    }

    void hide() {
        string s = transform.GetChild(curSel).gameObject.GetComponentInChildren<TextMeshProUGUI>().text;

        curText.color = oldTextColor;
        curArrow.color = new Color(255, 255, 255, 0); //Set arrow to transparent

        for(int i = 0; i < transform.childCount; i++) {
            if (transform.GetChild(i).gameObject.activeSelf) transform.GetChild(i).gameObject.SetActive(false);
        }

        on = false;
        callbackFunc(s);
    }

    private int constrain(int i) {
        return (i + selNum) % selNum;
    }

    void delayHack() {
        delay = false;
    }
}
