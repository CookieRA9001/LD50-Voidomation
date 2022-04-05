using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class PromptScript : MonoBehaviour
{
    private float timer;
    public TextMeshProUGUI pText;
    void Start()
    { 
        pText = GetComponent<TextMeshProUGUI>();
    }

    // Update is called once per frame
    void Update()
    {
        if (timer > 0) {
            timer -= Time.deltaTime;
        } else {
            ClearPrompt();
        }
    }

    public void ClearPrompt() {
        pText.text = "";
    }

    public void SetPrompt(string prompt, float promptLength, Color color) {
        pText.color = color;
        pText.text = prompt;
        timer = promptLength;
    }
}
