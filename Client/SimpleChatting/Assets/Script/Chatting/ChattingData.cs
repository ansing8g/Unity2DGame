using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChattingData : MonoBehaviour
{
    private UnityEngine.UI.Text text;

    // Start is called before the first frame update
    void Start()
    {
        BindObject();
    }
    
    void BindObject()
    {
        if(text == null)
        {
            text = transform.GetComponent<UnityEngine.UI.Text>();
        }
    }

    public void SetText(string settingText)
    {
        BindObject();
        text.text = settingText;
    }
}
