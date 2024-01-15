using UnityEngine;
using System.Collections;
    
public class MainPlayer : Player
{
    Controller controller;
    Joystick joystick;

    protected override void Start()
    {        
        base.Start();

        GameObject joystickGo = GameObject.Find("Variable Joystick");
        joystick = joystickGo.GetComponent<Joystick>();

        controller = new Controller(transform);
    }

    protected override void Update()
    {
        base.Update();

        controller.HandleInput(joystick.GetInput());
    } 
}

