using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;

public class CharacterChangeButton : MonoBehaviour
{
    [SerializeField] private CharacterProfile characterProfile;
    public static UnityEvent<CharacterProfile> OnChangeProfile = new UnityEvent<CharacterProfile>();

    private void Awake()
    {
        GetComponent<Button>().onClick.AddListener(() =>
        {
            if (characterProfile != null)
            {
                OnChangeProfile?.Invoke(characterProfile);
            }
        });
    }
}
