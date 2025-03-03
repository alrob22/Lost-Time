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
    GameObject[] selectors;
    int curSel = 0, selNum;
    Image curArrow;
    TextMeshProUGUI curText;
    bool on = false;

    delegate void CallbackFunc(String selected);
    private CallbackFunc callbackFunc;

    // Start is called before the first frame update
    void Start()
    {
        selectors = gameObject.GetComponentsInChildren<GameObject>();

    }

    // Update is called once per frame
    void Update()
    {
        if (on) {
            float i = Input.GetAxis("Vertical");
            if (Input.GetButtonUp("Fire1")) {
                //TODO: nice animation
                hide();
            } else if (i > cutoff) {
                curSel--;
                updateCurrent();
            } else if (i < -cutoff) {
                curSel++;
                updateCurrent();
            }
        }
    }

    void setup(IList<String> branches, CallbackFunc func) {
        on = true;
        int i = 0;
        selNum = branches.Count;
        foreach (String s in branches) {
            if (i < 4) {
                if (!selectors[i].activeSelf) {
                    selectors[i].SetActive(true);
                }
                selectors[i].GetComponentInChildren<TextMeshProUGUI>().text = s;
                i++;
            }
        }
        for (;i < selectors.Length; i++) {
            selectors[i].SetActive(false);
        }
        curSel = 0;

        callbackFunc = func;
    }

    void updateCurrent() {
        curText.color = oldTextColor;
        curArrow.color = Color.white;

        curText = selectors[curSel].GetComponentInChildren<TextMeshProUGUI>();
        curArrow = selectors[curSel].GetComponentInChildren<Image>();

        oldTextColor = curText.color;
        curText.color = highlightColor;
    }

    void hide() {
        string s = selectors[curSel].GetComponentInChildren<TextMeshProUGUI>().text;

        curText.color = oldTextColor;
        curArrow.color = Color.white;

        foreach (GameObject g in selectors) {
            if (g.activeSelf) g.SetActive(false);
        }

        callbackFunc(s);
    }

    private int constrain(int i) {
        return (i + 4) % 4;
    }
}
