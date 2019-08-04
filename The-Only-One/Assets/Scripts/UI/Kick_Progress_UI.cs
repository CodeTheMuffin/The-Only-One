using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Kick_Progress_UI : MonoBehaviour
{
    public Image fillImage;
    public Text StatusText;

    public Color WaitColor;
    public Color KickColor;

    bool isWaiting = false;
    public float MAX_text_wait = 0.4f;//0.4 seconds to add to the text to be from "Wait" -> "Wait ..."
    public float text_wait = 0f;

    public void StartWaiting()
    {
        isWaiting = true;
        text_wait = MAX_text_wait;
        fillImage.color = WaitColor;
    }

    public void StopWaiting()
    {
        isWaiting = false;
        text_wait = 0f;
        StatusText.text = "KICK!";
        fillImage.color = KickColor;
    }

    public void setvalue(float value, float max_value)
    {
        fillImage.fillAmount = 1 - (value / max_value);
    }

    void Update()
    {
        if (isWaiting)
        {
            if (text_wait > 0)
            {
                text_wait -= Time.deltaTime;

                float divider = 5;
                if (text_wait >= (MAX_text_wait * (divider - 1)) / divider)
                {
                    StatusText.text = "Wait ...";
                }
                else if (text_wait >= (MAX_text_wait * (divider - 2)) / divider)
                {
                    StatusText.text = "Wait ..";
                }
                else if (text_wait >= (MAX_text_wait * (divider - 3)) / divider)
                {
                    StatusText.text = "Wait .";
                }
                else
                {
                    StatusText.text = "Wait";
                }
            }

            if (text_wait < 0)
            {
                text_wait = MAX_text_wait;
            }
            
        }
    }
}
