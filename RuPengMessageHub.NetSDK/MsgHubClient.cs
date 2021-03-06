﻿using Newtonsoft.Json;
using RuPengMessageHub.Helpers;
using System;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace RuPengMessageHub.NetSDK
{
    public class MsgHubClient
    {
        private readonly string serverRoot;
        public MsgHubClient(string serverRoot)
        {
            if (!serverRoot.EndsWith("/"))
            {
                throw new ArgumentException("域名需要以/结尾", nameof(serverRoot));
            }
            this.serverRoot = serverRoot;
        }

        public async Task<string> GetTokenAsync(string UserId, string DisplayName, string AppKey, long Timestamp, string AppSecret)
        {
            string signature = SecurityHelper.GetHash(AppSecret + Timestamp);
            //HttpClient httpClient = HttpClientFactory.Create();
            using (HttpClient httpClient = new HttpClient())
            {
                var req = new
                {
                    UserId = UserId,
                    DisplayName = DisplayName,
                    AppKey = AppKey,
                    Timestamp = Timestamp,
                    Signature = signature
                };
                string json = JsonConvert.SerializeObject(req);
                StringContent content = new StringContent(json, Encoding.UTF8, "application/json");
                var resp = await httpClient.PostAsync(this.serverRoot + "user/GetToken", content);
                if (resp.IsSuccessStatusCode)
                {
                    return await resp.Content.ReadAsStringAsync();
                }
                else
                {
                    throw new ApplicationException("响应错误" + resp.StatusCode);
                }
            }
        }

        public async Task SendGroupMessageAsync(string token, String toGroupId, String objectName, String content)
        {
            //HttpClient httpClient = HttpClientFactory.Create();
            using (HttpClient httpClient = new HttpClient())
            {
                var req = new
                {
                    toGroupId,
                    objectName,
                    content
                };
                string json = JsonConvert.SerializeObject(req);
                StringContent stringContent = new StringContent(json, Encoding.UTF8, "application/json");
                HttpRequestMessage requestMsg = new HttpRequestMessage(HttpMethod.Post, this.serverRoot + "group/SendGroupMessage");
                requestMsg.Headers.Add("Authorization", "Bearer " + token);
                requestMsg.Content = stringContent;
                var resp = await httpClient.SendAsync(requestMsg);
                if (resp.IsSuccessStatusCode)
                {
                    await resp.Content.ReadAsStringAsync();
                }
                else
                {
                    throw new ApplicationException("响应错误" + resp.StatusCode);
                }
            }
        }

        public async Task JoinGroupAsync(string token, string groupId)
        {
            // HttpClient httpClient = HttpClientFactory.Create();
            using (HttpClient httpClient = new HttpClient())
            {
                var req = new
                {
                    groupId
                };
                string json = JsonConvert.SerializeObject(req);
                StringContent stringContent = new StringContent(json, Encoding.UTF8, "application/json");
                HttpRequestMessage requestMsg = new HttpRequestMessage(HttpMethod.Post, this.serverRoot + "group/JoinGroup");
                requestMsg.Headers.Add("Authorization", "Bearer " + token);
                requestMsg.Content = stringContent;
                var resp = await httpClient.SendAsync(requestMsg);
                if (resp.IsSuccessStatusCode)
                {
                    await resp.Content.ReadAsStringAsync();
                }
                else
                {
                    throw new ApplicationException("响应错误" + resp.StatusCode);
                }
            }
        }
    }
}
