using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class TalkBanner : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI messageText;
    [SerializeField] private HorizontalLayoutGroup horizontalLayout;

    private bool isRight;
    private List<string> messageList = new List<string>();

    public void Setup(string message, bool isRight)
    {
        messageText.text = message;
        this.isRight = isRight;
    }

    private void MessageLine(string input)
    {
    }

    private List<string> SplitCount(string input, int count)
    {
        List<string> ret = new List<string>();

        int loopCount = input.Length / count;

        for (int i = 0; i < loopCount; i++)
        {
            int length = Mathf.Min(input.Length, count);
            ret.Add(input.Substring(i * count, length));
        }
        return ret;
    }

}
