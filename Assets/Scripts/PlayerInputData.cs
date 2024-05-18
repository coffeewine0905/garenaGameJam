using System.Collections;
using System.Collections.Generic;
using Spine.Unity;
using UnityEngine;
[CreateAssetMenu(fileName = "PlayerInputData", menuName = "ScriptableObjects/PlayerInputData")]
public class PlayerInputData : ScriptableObject
{
    public int id;
    public KeyCode MoveUp;
    public KeyCode MoveDown;
    public KeyCode MoveLeft;
    public KeyCode MoveRight;

    public KeyCode Confirm;
    public List<AnimationReferenceAsset> Animations;
}
