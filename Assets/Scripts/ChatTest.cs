using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class ChatTest : MonoBehaviour
{
    private Communication communication;
    public static UnityEvent<TalkModel> OnResponse = new UnityEvent<TalkModel>();
    public static UnityEvent<TalkModel, BingSearchControl.NewsValue> OnResponseNews = new UnityEvent<TalkModel, BingSearchControl.NewsValue>();

    private void Start()
    {
        if (PlayerPrefs.HasKey(Define.KEY_SAVED_MESSAGE))
        {
            var json = PlayerPrefs.GetString(Define.KEY_SAVED_MESSAGE);
            var talkHistory = JsonUtility.FromJson<TalkHistory>(json);
            communication = new Communication(talkHistory);
        }
        else
        {
            string prompt =
            "・キャラクター名：モンキーDルフィ" +
            "・年齢：19歳（シリーズ開始時）、20歳（新世界編）" +
            "・性別：男性" +
            "・職業：海賊船長" +
            "・キャラクター設定：夢は「海賊王」になること。悪魔の実「ゴムゴムの実」を食べたことで、体がゴムのように伸びることができる。また、自分より強い者に一方的に挑戦する「ジャストワン」の信念を持っている。仲間たちを大切に思い、海賊としての戦いを繰り広げる中で、様々な仲間を 集めていく。" +
            "・口癖：" +
            "「俺は海賊王になる男だ！」" +
            "「おれの仲間は絶対に裏切らない！」" +
            "「やめられるもんならやめてみろ！このカッコいい大冒険から逃げることはできねえんだよ！」" +
            "「フッシーーー！」" +
            "・物語での役割：主人公として、仲間たちをまとめて冒険を繰り広げ、海賊王になることを目指す。また、様々な敵との戦いや、四皇や世界政府との対立など、物語の中心的な役割を担っている。";

            var submit_message = $"" +
                $"あなたはマンガ、ワンピースに出てくる登場キャラのモンキー・D・ルフィとしてロールプレイをしてください。" +
                $"次のプロフィールに基づいて話すようにしてください" + prompt;
            communication = new Communication(submit_message);
        }

        PanelTalkList.OnSendStampMessage.AddListener(() =>
        {
            StartCoroutine(NewsTalk());
            /*
            BingSearchControl bingSearchControl = new BingSearchControl();

            bingSearchControl.GetNews((result) =>
            {
                int index = Random.Range(0, result.Count);

                string submit_message = "また、次のニュースから、ルフィが取り上げそうな話題を選択して話しかけてください";

                foreach (var data in result)
                {
                    submit_message += $"・Title:{data.name} Headline:{data.headline} Description:{data.description}";
                }
                Debug.Log(submit_message);

                communication.AddHistory(new Communication.MessageModel()
                {
                    role = "system",
                    content = submit_message
                });

                //var news = result[index];
                //Debug.Log(news.description);
                communication.Submit(
                    $"先程のニュースをあなたから話したように会話を開始してください。あとできれば楽しい気持ちになるような話題を選んでください。", (val) =>
                {
                    Debug.Log(val.content);

                    OnResponseNews?.Invoke(new TalkModel()
                    {
                        message = val.content,
                        isRight = false,
                        role = "system"
                    },
                    result[0]);

                });
            });
                */
        });
        PanelTalkList.OnSendTalkMessage.AddListener((send_message) =>
        {
            communication.Submit($"{send_message}", (val) =>
                {
                    Debug.Log(val.content);
                    OnResponse?.Invoke(new TalkModel()
                    {
                        message = val.content,
                        isRight = false,
                        role = "system",
                    });
                });
        });
    }

    private IEnumerator NewsTalk()
    {
        BingSearchControl bingSearchControl = new BingSearchControl();
        yield return StartCoroutine(bingSearchControl.GetNews());

        string submit_message = "また、次のニュースから、ルフィが取り上げそうな話題を選択して話しかけてください";

        foreach (var data in bingSearchControl.newsValueList)
        {
            submit_message += $"・Title:{data.name} Headline:{data.headline} Description:{data.description}";
        }
        Debug.Log(submit_message);

        communication.AddHistory(new Communication.MessageModel()
        {
            role = "system",
            content = submit_message
        });
        Debug.Log("waitstart");
        yield return new WaitForSeconds(1);
        Debug.Log("waitend");

        yield return StartCoroutine(communication.SubmitCoroutine($"先程のニュースをあなたから話したように会話を開始してください。あとできれば楽しい気持ちになるような話題を選んでください。"));
        Debug.Log(communication.lastMessageModel.content);
        OnResponseNews?.Invoke(new TalkModel()
        {
            message = communication.lastMessageModel.content,
            isRight = false,
            role = "system"
        },
        bingSearchControl.newsValueList[0]);
        /*
        communication.Submit(
            $"先程のニュースをあなたから話したように会話を開始してください。あとできれば楽しい気持ちになるような話題を選んでください。", (val) =>
            {
                Debug.Log(val.content);

                OnResponseNews?.Invoke(new TalkModel()
                {
                    message = val.content,
                    isRight = false,
                    role = "system"
                },
                bingSearchControl.newsValueList[0]);
            });
        */

    }

}
