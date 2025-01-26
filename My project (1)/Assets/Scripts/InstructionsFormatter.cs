using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class InstructionsFormatter : MonoBehaviour
{

    string instructions = "Welcome to <i>Lost in the Woods: Bubble Battles!</i><br><br>" +
    "You are a boy lost in a mysterious forest, armed with magical bubbles. Watch out for dangerous creatures lurking around—they will attack you and pop your bubbles.<br><br>" +
    "<b>How to Play:</b><br>" +
    "- Press <b>1</b>, <b>2</b>, or <b>3</b> to switch between bubble types, each with unique powers.<br>" +
    "- <b>Click and hold</b> to blow a bubble. The longer you hold, the bigger the bubble!<br>" +
    "- Be careful! If a bubble gets too big, it will pop in your face and hurt you.<br>" +
    "- Use your bubbles to defeat all enemies and escape the forest.<br><br>" +
    "<b>Tips for Survival:</b><br>" +
    "- Stay alert—some enemies are fast, and others aim to destroy your bubbles.<br>" +
    "- Master the different bubble types to outsmart your foes.<br><br>" +
    "Good luck, Bubble Warrior! The forest is counting on your courage!";


    [SerializeField]
    TMP_Text textBox;


    // Start is called before the first frame update
    void Start()
    {
        textBox.text = instructions;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
