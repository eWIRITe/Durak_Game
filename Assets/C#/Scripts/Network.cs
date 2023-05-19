using UnityEngine;
using UnityEngine.Networking;
using System;
using System.Collections;
using Newtonsoft.Json.Linq;
using System.Text;
using System.Collections.Generic;
using static RatingScreen;

public class Network : MonoBehaviour
{
    public string m_hostName = "http://localhost:5000/api";

    public IEnumerator Login(string name, string password, Action<string> successed, Action<string> failed)
    {
        var hashAlgorithm = new Org.BouncyCastle.Crypto.Digests.Sha3Digest(224);

        byte[] input = Encoding.UTF8.GetBytes(password);

        hashAlgorithm.BlockUpdate(input, 0, input.Length);

        byte[] result = new byte[28]; // 224 / 8 = 28
        hashAlgorithm.DoFinal(result, 0);

        string hashPassword = BitConverter.ToString(result);
        hashPassword = hashPassword.Replace("-", string.Empty).ToLowerInvariant();

        WWWForm form = new();
        form.AddField("name", name);
        form.AddField("password", hashPassword);

        using var re = UnityWebRequest.Post($"{m_hostName}/login", form);

        yield return re.SendWebRequest();

        if (re.result == UnityWebRequest.Result.Success)
        {
            JObject resp = JObject.Parse(re.downloadHandler.text);

            JToken error = resp["error"];

            if (error != null)
            {
                failed($"error: {Convert.ToString(error)}");
                yield break;
            }

            successed(Convert.ToString(resp["token"]));
        }
        else
        {
            failed($"{re.responseCode}: {re.error}");
        }
    }

    public IEnumerator Logout(string token, Action successed, Action<string> failed)
    {
        using var re = UnityWebRequest.Post($"{m_hostName}/logout?token={token}", string.Empty);

        yield return re.SendWebRequest();

        if (re.result == UnityWebRequest.Result.Success)
        {
            JObject resp = JObject.Parse(re.downloadHandler.text);

            JToken error = resp["error"];

            if (error != null)
            {
                failed($"error: {Convert.ToString(error)}");
                yield break;
            }

            successed();
        }
        else
        {
            failed($"{re.responseCode}: {re.error}");
        }
    }

    public IEnumerator Signin(string name, string email, string password, Action successed, Action<string> failed)
    {
        var hashAlgorithm = new Org.BouncyCastle.Crypto.Digests.Sha3Digest(224);

        byte[] input = Encoding.UTF8.GetBytes(password);

        hashAlgorithm.BlockUpdate(input, 0, input.Length);

        byte[] result = new byte[28]; // 224 / 8 = 28
        hashAlgorithm.DoFinal(result, 0);

        string hashPassword = BitConverter.ToString(result);
        hashPassword = hashPassword.Replace("-", string.Empty).ToLowerInvariant();

        WWWForm form = new();
        form.AddField("name", name);
        form.AddField("email", email);
        form.AddField("password", hashPassword);

        using var re = UnityWebRequest.Post($"{m_hostName}/register_user", form);

        yield return re.SendWebRequest();

        if (re.result == UnityWebRequest.Result.Success)
        {
            JObject resp = JObject.Parse(re.downloadHandler.text);

            JToken error = resp["error"];

            if (error != null)
            {
                failed($"error: {Convert.ToString(error)}");
                yield break;
            }

            successed();
        }
        else
        {
            failed($"{re.responseCode}: {re.error}");
        }
    }

    public IEnumerator GetPlayerId(string token, Action<uint> successed, Action<string> failed)
    {
        using var re = UnityWebRequest.Get($"{m_hostName}/get_uid?token={token}");

        yield return re.SendWebRequest();

        if (re.result == UnityWebRequest.Result.Success)
        {
            JObject resp = JObject.Parse(re.downloadHandler.text);

            JToken error = resp["error"];

            if (error != null)
            {
                failed($"error: {Convert.ToString(error)}");
                yield break;
            }

            uint uid = Convert.ToUInt32(resp.GetValue("uid"));

            successed(uid);
        }
        else
        {
            failed($"{re.responseCode}: {re.error}");
        }
    }

    public IEnumerator GetPlayerName(string token, uint uid, Action<string> successed, Action<string> failed)
    {
        using var re = UnityWebRequest.Get($"{m_hostName}/get_username/{uid}/?token={token}");

        yield return re.SendWebRequest();

        if (re.result == UnityWebRequest.Result.Success)
        {
            JObject resp = JObject.Parse(re.downloadHandler.text);

            JToken error = resp["error"];

            if (error != null)
            {
                failed($"error: {Convert.ToString(error)}");
                yield break;
            }

            string name = Convert.ToString(resp.GetValue("username"));

            successed(name);
        }
        else
        {
            failed($"{re.responseCode}: {re.error}");
        }
    }

    public IEnumerator GetChips(string token, Action<uint> successed, Action<string> failed)
    {
        using var re = UnityWebRequest.Get($"{m_hostName}/get_chips?token={token}");

        yield return re.SendWebRequest();

        if (re.result == UnityWebRequest.Result.Success)
        {
            JObject resp = JObject.Parse(re.downloadHandler.text);

            JToken error = resp["error"];

            if (error != null)
            {
                failed($"error: {Convert.ToString(error)}");
                yield break;
            }

            uint chips = Convert.ToUInt32(resp.GetValue("chips"));

            successed(chips);
        }
        else
        {
            failed($"{re.responseCode}: {re.error}");
        }
    }

    public IEnumerator ChangeEmail(string token, string newEmail, string oldEmail, Action successed, Action<string> failed)
    {
        WWWForm form = new();
        form.AddField("new_email", newEmail);
        form.AddField("old_email", oldEmail);

        using var re = UnityWebRequest.Post($"{m_hostName}/change_email?token={token}", form);

        yield return re.SendWebRequest();

        if (re.result == UnityWebRequest.Result.Success)
        {
            JObject resp = JObject.Parse(re.downloadHandler.text);

            JToken error = resp["error"];

            if (error != null)
            {
                failed($"error: {Convert.ToString(error)}");
                yield break;
            }

            successed();
        }
        else
        {
            failed($"{re.responseCode}: {re.error}");
        }
    }

    public IEnumerator GetRating(string token, int offset, int limit, Action<List<RatingLine>> successed, Action<string> failed)
    {
        using var re = UnityWebRequest.Get($"{m_hostName}/get_rating?token={token}&offset={offset}&limit={limit}");

        yield return re.SendWebRequest();

        if (re.result == UnityWebRequest.Result.Success)
        {
            try
            {
                List<RatingLine> top = Newtonsoft.Json.JsonConvert.DeserializeObject<List<RatingLine>>(re.downloadHandler.text);
                successed(top);
            }
            catch (Exception)
            {
                JObject resp = JObject.Parse(re.downloadHandler.text);

                JToken error = resp["error"];

                failed($"error: {Convert.ToString(error)}");
                yield break;
            }
        }
        else
        {
            failed($"{re.responseCode}: {re.error}");
        }
    }

    public IEnumerator GetAvatar(string token, uint uid, Action<Texture2D> successed, Action<string> failed)
    {
        using var re = UnityWebRequestTexture.GetTexture($"{m_hostName}/get_avatar/{uid}?token={token}");

        yield return re.SendWebRequest();

        if (re.result == UnityWebRequest.Result.Success)
        {
            try
            {
                Texture2D avatar = ((DownloadHandlerTexture)re.downloadHandler).texture;
                successed(avatar);
            }
            catch (Exception)
            {
                JObject resp = JObject.Parse(re.downloadHandler.text);

                JToken error = resp["error"];

                failed($"error: {Convert.ToString(error)}");
                yield break;
            }
        }
        else
        {
            failed($"{re.responseCode}: {re.error}");
        }
    }
}
