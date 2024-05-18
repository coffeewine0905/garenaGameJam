using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[CreateAssetMenu(fileName = "PlayerInputData", menuName = "ScriptableObjects/PlayerInputData")]
public class PlayerInputData : ScriptableObject
{
    public int id;
    public KeyCode MoveUp;
    public KeyCode MoveDown;
}
