using RestSharp;
using System;
using System.Collections.Generic;
using System.Globalization;
using API_Assessment.Models;
using Newtonsoft.Json;

namespace API_Assessment.Helpers
{
    public class RESTHelper
    {
        private static RestClient _client;
        private static RestRequest _request;
        private const string LocationPath = "https://mobilewebserver9-pokertest8ext.installprogram.eu/TestApi";
        private const string TokenPath = "/token";
        private const string UserName = "testName";
        private const string Password = "test";
        private const string GrantType = "password";
        private readonly string _accessToken;
        internal string EntityCode { get; } = Guid.NewGuid().GetHashCode().ToString(CultureInfo.InvariantCulture);

        public RESTHelper()
        {
            _accessToken = CreateToken();
        }

        internal string CreateToken()
        {
            _client = new RestClient(LocationPath + TokenPath);
            _request = new RestRequest(Method.POST);
            _request.AddHeader("Accept", "application/json");
            _request.AddHeader("Content-Type", "application/x-www-form-urlencoded");
            _request.AddParameter("undefined", $"username={UserName}&password={Password}&grant_type={GrantType}", ParameterType.RequestBody);
            var response = (_client.Execute(_request)).Content;
            var jsonResponse = JsonConvert.DeserializeObject<Dictionary<string, object>>(response)["access_token"].ToString();
            return jsonResponse;
        }

        internal string CreateEntity(RestClient client, string entityName)
        {
            _request = new RestRequest(Method.POST);
            _request.AddHeader("Authorization", "Bearer " + _accessToken);
            _request.AddParameter("undefined", $"{{\"Name\":\"{entityName}\"}}", ParameterType.RequestBody);
            var response = (client.Execute(_request)).StatusCode.ToString();
            return response;

        }

        internal IRestResponse GetAllEntities(RestClient client)
        {
            _request = new RestRequest(Method.GET);
            _request.AddHeader("Authorization", "Bearer " + _accessToken);
            IRestResponse response = client.Execute(_request);
            return response;
        }

        internal IRestResponse GetEntityById(RestClient client, int id)
        {
            _request = new RestRequest($"/id/{id}", Method.GET);
            _request.AddHeader("Authorization", "Bearer " + _accessToken);
            IRestResponse response = client.Execute(_request);
            return response;
        }

        internal IRestResponse DeleteEntityById(RestClient client, int id)
        {
            _request = new RestRequest($"/id/{id}", Method.DELETE);
            IRestResponse response = client.Execute(_request);
            return response;
        }

        internal List<EntityModel> ParseEntityResponse(IRestResponse response)
        {
            return JsonConvert.DeserializeObject<List<EntityModel>>(response.Content);
        }

        internal List<TokenModel> ParseTokenResponse(IRestResponse response)
        {
            return JsonConvert.DeserializeObject<List<TokenModel>>(response.Content);
        }
    }
}
