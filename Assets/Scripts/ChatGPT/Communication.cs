﻿using System;
using System.Threading.Tasks;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using UnityEngine.Networking;

public class Communication
{
    [System.Serializable]
    public class MessageModel
    {
        public string role;
        public string content;
    }
    [System.Serializable]
    public class CompletionRequestModel
    {
        public string model;
        public List<MessageModel> messages;
    }

    [System.Serializable]
    public class ChatGPTRecieveModel
    {
        public string id;
        public string @object;
        public int created;
        public Choice[] choices;
        public Usage usage;

        [System.Serializable]
        public class Choice
        {
            public int index;
            public MessageModel message;
            public string finish_reason;
        }

        [System.Serializable]
        public class Usage
        {
            public int prompt_tokens;
            public int completion_tokens;
            public int total_tokens;
        }
    }
    private MessageModel assistantModel = new()
    {
        role = "system",
        content = "あなたは冒険者ギルドの受付です。"
    };
    private readonly string apiKey = EnvFile.OPENAI_API_KEY;
    private List<MessageModel> communicationHistory = new();
    /*
    */
    public Communication(TalkHistory talkHistory)
    {
        foreach (var model in talkHistory.talkList)
        {
            MessageModel add = new MessageModel()
            {
                role = model.isRight ? "user" : "system",
                content = model.message,
            };
            communicationHistory.Add(add);
            Debug.Log($"role:{add.role} msg:{model.message}");
        }
    }
    public Communication(string systemContent)
    {
        MessageModel initModel = new MessageModel()
        {
            role = "system",
            content = systemContent
        };
        communicationHistory.Add(initModel);
        Debug.Log($"role:{initModel.role} msg:{initModel.content}");
    }
    private async void Test()
    {
        var request = new UnityWebRequest("", "POST")
        {
            uploadHandler = new UploadHandlerRaw(Encoding.UTF8.GetBytes("")),
            downloadHandler = new DownloadHandlerBuffer()
        }; var operation = request.SendWebRequest();

        while (!request.isDone)
        {
            await Task.Yield();
        }
    }
    public void AddHistory(MessageModel history_model)
    {
        communicationHistory.Add(history_model);
    }

    public void Submit(string newMessage, Action<MessageModel> result)
    {
        Debug.Log(newMessage);
        communicationHistory.Add(new MessageModel()
        {
            role = "user",
            content = newMessage
        });

        var apiUrl = "https://api.openai.com/v1/chat/completions";
        var jsonOptions = JsonUtility.ToJson(
            new CompletionRequestModel()
            {
                model = "gpt-3.5-turbo",
                messages = communicationHistory
            }, true);
        var headers = new Dictionary<string, string>
            {
                {"Authorization", "Bearer " + apiKey},
                {"Content-type", "application/json"},
                {"X-Slack-No-Retry", "1"}
            };
        var request = new UnityWebRequest(apiUrl, "POST")
        {
            uploadHandler = new UploadHandlerRaw(Encoding.UTF8.GetBytes(jsonOptions)),
            downloadHandler = new DownloadHandlerBuffer()
        };
        foreach (var header in headers)
        {
            request.SetRequestHeader(header.Key, header.Value);
        }

        var operation = request.SendWebRequest();

        operation.completed += _ =>
        {
            if (operation.webRequest.result == UnityWebRequest.Result.ConnectionError ||
                       operation.webRequest.result == UnityWebRequest.Result.ProtocolError)
            {
                Debug.LogError(operation.webRequest.error);
                throw new Exception();
            }
            else
            {
                var responseString = operation.webRequest.downloadHandler.text;
                var responseObject = JsonUtility.FromJson<ChatGPTRecieveModel>(responseString);
                communicationHistory.Add(responseObject.choices[0].message);
                AddHistory(new MessageModel()
                {
                    role = "system",
                    content = responseObject.choices[0].message.content
                });
                result.Invoke(responseObject.choices[0].message);
                //Debug.Log(responseObject.choices[0].message.content);
            }
            request.Dispose();

        };
    }
    public MessageModel lastMessageModel;
    public IEnumerator SubmitCoroutine(string newMessage)
    {
        lastMessageModel = null;
        Debug.Log(newMessage);
        communicationHistory.Add(new MessageModel()
        {
            role = "user",
            content = newMessage
        });

        var apiUrl = "https://api.openai.com/v1/chat/completions";
        var jsonOptions = JsonUtility.ToJson(
            new CompletionRequestModel()
            {
                model = "gpt-3.5-turbo",
                messages = communicationHistory
            }, true);
        Debug.Log(jsonOptions);
        var headers = new Dictionary<string, string>
            {
                {"Authorization", "Bearer " + apiKey},
                {"Content-type", "application/json"},
                {"X-Slack-No-Retry", "1"}
            };
        var request = new UnityWebRequest(apiUrl, "POST")
        {
            uploadHandler = new UploadHandlerRaw(Encoding.UTF8.GetBytes(jsonOptions)),
            downloadHandler = new DownloadHandlerBuffer()
        };
        foreach (var header in headers)
        {
            request.SetRequestHeader(header.Key, header.Value);
        }

        yield return request.SendWebRequest();


        if (request.result == UnityWebRequest.Result.ConnectionError ||
           request.result == UnityWebRequest.Result.ProtocolError)
        {
            Debug.LogError(request.error);
            yield break;
        }
        else
        {
            var responseString = request.downloadHandler.text;
            var responseObject = JsonUtility.FromJson<ChatGPTRecieveModel>(responseString);
            communicationHistory.Add(responseObject.choices[0].message);
            AddHistory(new MessageModel()
            {
                role = "system",
                content = responseObject.choices[0].message.content
            });
            lastMessageModel = responseObject.choices[0].message;
            //result.Invoke(responseObject.choices[0].message);
            //Debug.Log(responseObject.choices[0].message.content);
        }
        request.Dispose();

        yield return 0;
    }

    public MessageModel simpleCommunicationResponse;
    public IEnumerator SimpleCommunication(string newMessage)
    {
        simpleCommunicationResponse = null;
        var instantHistory = new List<MessageModel>();
        instantHistory.Add(new MessageModel()
        {
            role = "user",
            content = newMessage
        });
        var apiUrl = "https://api.openai.com/v1/chat/completions";
        var jsonOptions = JsonUtility.ToJson(
            new CompletionRequestModel()
            {
                model = "gpt-3.5-turbo",
                messages = instantHistory
            }, true);
        var headers = new Dictionary<string, string>
            {
                {"Authorization", "Bearer " + apiKey},
                {"Content-type", "application/json"},
                {"X-Slack-No-Retry", "1"}
            };
        var request = new UnityWebRequest(apiUrl, "POST")
        {
            uploadHandler = new UploadHandlerRaw(Encoding.UTF8.GetBytes(jsonOptions)),
            downloadHandler = new DownloadHandlerBuffer()
        };
        foreach (var header in headers)
        {
            request.SetRequestHeader(header.Key, header.Value);
        }

        yield return request.SendWebRequest();

        if (request.result == UnityWebRequest.Result.ConnectionError ||
           request.result == UnityWebRequest.Result.ProtocolError)
        {
            Debug.LogError(request.error);
            yield break;
        }
        else
        {
            var responseString = request.downloadHandler.text;
            var responseObject = JsonUtility.FromJson<ChatGPTRecieveModel>(responseString);
            communicationHistory.Add(responseObject.choices[0].message);
            AddHistory(new MessageModel()
            {
                role = "system",
                content = responseObject.choices[0].message.content
            });
            simpleCommunicationResponse = responseObject.choices[0].message;
        }
        request.Dispose();
    }

    public async Task SubmitAsync(string newMessage, Action<MessageModel> result)
    {
        Debug.Log(newMessage);
        communicationHistory.Add(new MessageModel()
        {
            role = "user",
            content = newMessage
        });

        var apiUrl = "https://api.openai.com/v1/chat/completions";
        var jsonOptions = JsonUtility.ToJson(
            new CompletionRequestModel()
            {
                model = "gpt-3.5-turbo",
                messages = communicationHistory
            }, true);
        var headers = new Dictionary<string, string>
            {
                {"Authorization", "Bearer " + apiKey},
                {"Content-type", "application/json"},
                {"X-Slack-No-Retry", "1"}
            };
        var request = new UnityWebRequest(apiUrl, "POST")
        {
            uploadHandler = new UploadHandlerRaw(Encoding.UTF8.GetBytes(jsonOptions)),
            downloadHandler = new DownloadHandlerBuffer()
        };
        foreach (var header in headers)
        {
            request.SetRequestHeader(header.Key, header.Value);
        }

        var operation = request.SendWebRequest();

        while (!request.isDone)
        {
            await Task.Yield();
        }

        operation.completed += _ =>
        {
            if (operation.webRequest.result == UnityWebRequest.Result.ConnectionError ||
                       operation.webRequest.result == UnityWebRequest.Result.ProtocolError)
            {
                Debug.LogError(operation.webRequest.error);
                throw new Exception();
            }
            else
            {
                var responseString = operation.webRequest.downloadHandler.text;
                var responseObject = JsonUtility.FromJson<ChatGPTRecieveModel>(responseString);
                communicationHistory.Add(responseObject.choices[0].message);
                //Debug.Log(responseObject.choices[0].message.content);
                result.Invoke(responseObject.choices[0].message);
            }
            request.Dispose();
        };
    }

}
