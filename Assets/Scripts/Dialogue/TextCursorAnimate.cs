using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class TextCursorAnimate : MonoBehaviour
{
    public IEnumerator blinkCursor;
    bool blinking = false;
    float speed, initialAlpha;
    public bool Blinking {
        get {return blinking;}
        private set {blinking = value;}
    }
    Image image;

    void Start()
    {
        image = gameObject.GetComponent<Image>();
        initialAlpha = image.color.a; //Hack to let you keep the color of the compenent in editor working as expected
        Color c = image.color;
        c.a = 0;
        image.color = c;
    }

    public void startBlink(float blinkSpeed) {
        if (!Blinking) {
            Blinking = true;
            blinkCursor = blink(blinkSpeed);
            StartCoroutine(blinkCursor);
        }
    }

    public void stopBlink() {
        Blinking = false;
    }

    IEnumerator blink(float speed) {
        Color c = image.color;
        while (Blinking) {
            
            if (image.color.a == 0) {
                c.a = initialAlpha;
            } else {
                c.a = 0;
            }

            image.color = c;

            yield return new WaitForSeconds(speed);
        }

        //Reset color to off once we're done here
        c.a = 0;
        image.color = c;
    }

}
