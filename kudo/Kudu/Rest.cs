﻿//
// Copyright 2018 Web Matrix Pty Ltd
//
// Permission is hereby granted, free of charge, to any person obtaining a copy of this software and associated documentation files (the "Software"),
// to deal in the Software without restriction, including without limitation the rights to use, copy, modify, merge, publish, distribute, sublicense,
// and/or sell copies of the Software, and to permit persons to whom the Software is furnished to do so, subject to the following conditions:
//
// The above copyright notice and this permission notice shall be included in all copies or substantial portions of the Software.
//
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
// FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
// LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS
// IN THE SOFTWARE.
//

using Newtonsoft.Json;
using System;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;

namespace Kudo.Kudu
{
    public class Rest
    {
        public String BaseUri { get; set; }
        public String Username { get; set; }
        public String Password { get; set; }

        public T Post<T>(String method, Object obj)
        {
            HttpClient client = new HttpClient()
            {
                BaseAddress = new Uri(BaseUri)
            };

            client.DefaultRequestHeaders.Authorization = GetBasicAuth();

            HttpContent content = GetContent(obj);

            HttpResponseMessage response = client.PostAsync(method, content).Result;
            if(response != null && response.Content != null)
            {
                String json = response.Content.ReadAsStringAsync().Result;
                return JsonConvert.DeserializeObject<T>(json);
            }

            return default(T);
        }

        private HttpContent GetContent(Object obj)
        {
            String json = JsonConvert.SerializeObject(obj);
            HttpContent content = new StringContent(json, Encoding.UTF8, "application/json");
            return content;
        }

        private AuthenticationHeaderValue GetBasicAuth()
        {
            Byte[] bytes = Encoding.UTF8.GetBytes($"{Username}:{Password}");
            return new AuthenticationHeaderValue("Basic", Convert.ToBase64String(bytes));
        }
    }
}
