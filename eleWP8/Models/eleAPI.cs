using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Runtime.Serialization.Json;
using System.Net.Http;
using System.IO;
using System.Text;

namespace eleWP8
{
    public class eleAPI
    {
        public static string host = "http://m.ele.me";
        HttpClient client;

        public static DataContractJsonSerializer placeSelectResponse_ser = new DataContractJsonSerializer(typeof(PlaceSelectionResponse));
        public static DataContractJsonSerializer restList_ser = new DataContractJsonSerializer(typeof(List<RestaurantBlock>));
        public static DataContractJsonSerializer rest_ser = new DataContractJsonSerializer(typeof(RestaurantBlock));
        public static DataContractJsonSerializer menuList_ser = new DataContractJsonSerializer(typeof(List<RestaurantMenu>));
        public static DataContractJsonSerializer placeList_ser = new DataContractJsonSerializer(typeof(List<PlaceData>));
        public static DataContractJsonSerializer codepic_ser = new DataContractJsonSerializer(typeof(CodePicture));
        public static DataContractJsonSerializer loginPackage_ser = new DataContractJsonSerializer(typeof(LoginPackage));
        public static DataContractJsonSerializer loginresult_ser = new DataContractJsonSerializer(typeof(LoginResult));
        public static DataContractJsonSerializer batchPackage_ser = new DataContractJsonSerializer(typeof(List<SingleResponsePackage>));
        public static DataContractJsonSerializer orderList_ser = new DataContractJsonSerializer(typeof(List<Order>));
        public static DataContractJsonSerializer userProfile_ser = new DataContractJsonSerializer(typeof(UserProfile));

        private static eleAPI _eleAPI = null;
        private eleAPI()
        {
            client = new HttpClient();
            string host = "http://m.ele.me";
            client.BaseAddress = new Uri(host);
            client.DefaultRequestHeaders.Accept.Clear();
            client.DefaultRequestHeaders.Accept.Add(new System.Net.Http.Headers.MediaTypeWithQualityHeaderValue("application/json"));
            client.DefaultRequestHeaders.Accept.Add(new System.Net.Http.Headers.MediaTypeWithQualityHeaderValue("text/plain"));
            client.DefaultRequestHeaders.Accept.Add(new System.Net.Http.Headers.MediaTypeWithQualityHeaderValue("*/*"));
            client.DefaultRequestHeaders.UserAgent.Clear();
            client.DefaultRequestHeaders.Add("user-agent", "Mozilla/5.0 (compatible; MSIE 10.0; Windows Phone 8.0; Trident/6.0; ARM; Touch; IEMobile/10.0;)");
            client.DefaultRequestHeaders.Referrer = new Uri(host);
        }
        public static eleAPI Instance { get
            {
                if (_eleAPI == null) _eleAPI = new eleAPI();
                return _eleAPI;
            }
        }

        public async Task<PlaceSelectionResponse> SearchPlace(string place)
        {
            string placeUrl = String.Format("{0}/v1/place?city={1}&keyword={2}", host, "010", UrlEncode(place));
            HttpResponseMessage response = await client.GetAsync(placeUrl);
            response.EnsureSuccessStatusCode();
            Stream stream = await response.Content.ReadAsStreamAsync();
            PlaceSelectionResponse result = placeSelectResponse_ser.ReadObject(stream) as PlaceSelectionResponse;
            return result;
        }

        public async Task<List<RestaurantBlock>> GetResturantList(string geohash, int offline, int limit)
        {
            string showResUrl = String.Format("{0}/restapi/v1/restaurants?extras%5B%5D=restaurant_activity&extras%5B%5D=food_activity&geohash={1}&is_premium=0&limit={3}&offset={2}&type=geohash", host, geohash, offline, limit);
            HttpResponseMessage response = await client.GetAsync(showResUrl);
            response.EnsureSuccessStatusCode();
            Stream stream = await response.Content.ReadAsStreamAsync();
            List<RestaurantBlock> restList = restList_ser.ReadObject(stream) as List<RestaurantBlock>;
            return restList;
        }

        public async Task<List<RestaurantMenu>> GetMenuList(string name_for_url)
        {
            string url = String.Format("{0}/restapi/v1/restaurants/{1}/menu", host, name_for_url);
            HttpResponseMessage response = await client.GetAsync(url);
            response.EnsureSuccessStatusCode();
            Stream stream = await response.Content.ReadAsStreamAsync();
            var menuList = menuList_ser.ReadObject(stream) as List<RestaurantMenu>;
            return menuList;
        }

        public async Task<Stream> GetLoginCodePicture()
        {
            HttpResponseMessage response;
            //string url = String.Format("{0}/login", host);
            //response = await client.GetAsync(url);
            //response.EnsureSuccessStatusCode();
            StreamContent content = new StreamContent(new MemoryStream()); //headers?
            response = await client.PostAsync(host + "/restapi/v1/captchas", content);
            response.EnsureSuccessStatusCode();
            Stream stream = await response.Content.ReadAsStreamAsync();
            var code = codepic_ser.ReadObject(stream) as CodePicture;
            response = await client.GetAsync(String.Format("{0}/restapi/v1/captchas/{1}", host, code.code));
            response.EnsureSuccessStatusCode();
            stream = await response.Content.ReadAsStreamAsync();
            return stream;
        }

        public async Task<int> LoginAccount(string username, string password, string code)
        {
            LoginPackage pack = new LoginPackage() { username = username, password = password, captcha_code = code };
            MemoryStream sendStream = new MemoryStream();
            loginPackage_ser.WriteObject(sendStream, pack);
            sendStream.Position = 0;
            HttpResponseMessage response = await client.PostAsync(host + "/restapi/v1/login", new StreamContent(sendStream));
            response.EnsureSuccessStatusCode();
            Stream stream = await response.Content.ReadAsStreamAsync();
            var result = loginresult_ser.ReadObject(stream) as LoginResult;
            return result.user_id;
        }

        public async Task<UserProfile> GetUserProfile()
        {
            string url = String.Format("{0}/restapi/v1/user?extras%5B%5D=premium_vip", host);
            HttpResponseMessage response = await client.GetAsync(url);
            response.EnsureSuccessStatusCode();
            Stream stream = await response.Content.ReadAsStreamAsync();
            //var batchPack = batchPackage_ser.ReadObject(stream) as List<SingleResponsePackage>;
            //MemoryStream mmstream = new MemoryStream();
            //StreamWriter sw = new StreamWriter(mmstream);
            //sw.Write(batchPack[0].body);
            //sw.Flush();
            //mmstream.Position = 0;
            var userProfile = userProfile_ser.ReadObject(stream) as UserProfile;
            return userProfile;
        }

        public async Task<List<Order>> GetHistoryOrder(int user_id, int offset, int limit)
        {
            string url = String.Format("{0}/restapi/v1/users/{1}/orders?extras%5B%5D=basket&extras%5B%5D=restaurant&extras%5B%5D=order_status&limit={2}&offset={3}", host, user_id, limit, offset);
            HttpResponseMessage response = await client.GetAsync(url);
            response.EnsureSuccessStatusCode();
            Stream stream = await response.Content.ReadAsStreamAsync();
            //var batchPack = batchPackage_ser.ReadObject(stream) as List<SingleResponsePackage>;
            //MemoryStream mmstream = new MemoryStream();
            //StreamWriter sw = new StreamWriter(mmstream);
            //sw.Write(batchPack[0].body);
            //sw.Flush();
            //mmstream.Position = 0;
            var orderList = orderList_ser.ReadObject(stream) as List<Order>;
            return orderList;
        }

        public static string UrlEncode(string str)
        {
            StringBuilder sb = new StringBuilder();
            byte[] byStr = Encoding.UTF8.GetBytes(str); 
            for (int i = 0; i < byStr.Length; i++)
            {
                sb.Append(@"%" + Convert.ToString(byStr[i], 16));
            }

            return (sb.ToString());
        }
        
    }
}
