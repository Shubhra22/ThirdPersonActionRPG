using System.Collections;
using System.Collections.Generic;
using JoystickLab;
using UnityEngine;

public interface IState
{
    void Enter();
    void Update();
    
    void FixedUpdate();
    void Exit();
}
