using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "キャラクタープロファイル")]
public class CharacterProfile : ScriptableObject
{
    public string characterName;
    public string characterPrompt;
}
