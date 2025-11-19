namespace AwtterSDK.Editor
{
    using UnityEngine.Networking;
    using UnityEngine;

    using Newtonsoft.Json;

    using System.Collections;

    using AwtterSDK;
    using AwtterSDK.Editor.Models.API;
    using AwtterSDK.Editor.Enums;

    public class AwtterApi
    {
        public static readonly string BASE_URL = "awtterspace.com";
        
        public static IEnumerator GetProducts(bool isFirst = true)
        {
            using (var www = UnityWebRequest.Get($"https://{BASE_URL}/api/products"))
            {
                www.SetRequestHeader("Authorization", $"Token {TokenCache.Token}");

                yield return www.SendWebRequest();

                if (InvalidateTokenOnError(www)) yield break;

                AwtterSpaceInstaller.Products = JsonConvert.DeserializeObject<ProductsModel>(www.downloadHandler.text);

                if (!AwtterSpaceInstaller.LoggedIn && isFirst)
                {
                    Debug.Log("[<color=orange>Awtter SDK</color>] Logged in using cache!");
                    AwtterSpaceInstaller.LoggedIn = true;
                }
            }
            yield break;
        }

        private static bool InvalidateTokenOnError(UnityWebRequest www)
        {
            if (www.responseCode != 200)
            {
                TokenCache.InvalidateToken();
                return true;
            }

            return false;
        }

        public static IEnumerator GetConfig()
        {
            using (var www = UnityWebRequest.Get($"https://{BASE_URL}/api/config"))
            {
                yield return www.SendWebRequest();

                if (InvalidateTokenOnError(www)) yield break;

                var okResponse = JsonConvert.DeserializeObject<ConfigOkResponseModel>(www.downloadHandler.text);

                if (okResponse.Status == StatusType.Success)
                    AwtterSpaceInstaller.RemoteConfig = okResponse.Data;
            }
            yield break;
        }


        public static IEnumerator GetCurrentUser()
        {
            using (var www = UnityWebRequest.Get($"https://{BASE_URL}/api/users/me"))
            {
                www.SetRequestHeader("Authorization", $"Token {TokenCache.Token}");

                yield return www.SendWebRequest();

                if (InvalidateTokenOnError(www)) yield break;

                var model = JsonConvert.DeserializeObject<CurrentUserResponseModel>(www.downloadHandler.text);

                if (model.Status != StatusType.Success) yield break;

                AwtterSpaceInstaller.LoggedInUser = model.Data;
            }
            yield break;
        }

        public static IEnumerator GetToolbox()
        {
            using (var www = UnityWebRequest.Get($"https://{BASE_URL}/api/products/toolbox"))
            {
                www.SetRequestHeader("Authorization", $"Token {TokenCache.Token}");

                yield return www.SendWebRequest();

                if (InvalidateTokenOnError(www)) yield break;

                var model = JsonConvert.DeserializeObject<ToolboxResponseModel>(www.downloadHandler.text);

                if (model.Status != StatusType.Success) yield break;

                foreach (var tool in model.Data.Files)
                {
                    tool.IsTool = true;
                }

                model.Data.Files.RemoveAll(x => x.Name == "7zip");

                AwtterSpaceInstaller.Toolbox = model.Data;
            }
            yield break;
        }

        public static IEnumerator GetPatreon()
        {
            using (var www = UnityWebRequest.Get($"https://{BASE_URL}/api/patreon/me"))
            {
                www.SetRequestHeader("Authorization", $"Token {TokenCache.Token}");

                yield return www.SendWebRequest();

                if (InvalidateTokenOnError(www)) yield break;

                var model = JsonConvert.DeserializeObject<PatreonResponseModel>(www.downloadHandler.text);

                if (model.Status != StatusType.Success) yield break;

                AwtterSpaceInstaller.Patreon = model.Data;
                AwtterSpaceInstaller.RefreshAwtterPackages = true;
            }
            yield break;
        }
    }
}
