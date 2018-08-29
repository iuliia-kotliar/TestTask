using RestSharp;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Net;
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
        private const string InvalidAccessToken = null;
        private static string _userName = "testName";
        private static string _password = "test";
        private static string _grantType = "password";
        private readonly string _accessToken;
        internal string EntityCode { get; set; } = Guid.NewGuid().GetHashCode().ToString(CultureInfo.InvariantCulture);

        public RESTHelper()
        {
            _accessToken = GetAccessTokenValue();
        }

        public RESTHelper(bool initializeToken)
        {
            if (!initializeToken) _accessToken = null;
        }

        /// <summary>
        /// Creates token using predefined credentials and grant type.
        /// </summary>
        /// <returns>Response from request execution</returns>
        internal IRestResponse CreateToken()
        {
            _client = new RestClient(LocationPath + TokenPath);
            _request = new RestRequest(Method.POST);
            _request.AddHeader("Content-Type", "application/x-www-form-urlencoded");
            _request.AddParameter("undefined", $"username={_userName}&password={_password}&grant_type={_grantType}", ParameterType.RequestBody);
            var response = _client.Execute(_request);
            return response;
        }

        /// <summary>
        /// Creates token using credentials and grant type as parameters.
        /// </summary>
        /// <param name="username">username value</param>
        /// <param name="password">password value</param>
        /// <param name="grantType">grant_type value</param>
        /// <returns>Response from request execution</returns>
        internal IRestResponse CreateToken(string username, string password, string grantType)
        {
            _userName = username;
            _password = password;
            _grantType = grantType;
            return CreateToken();
        }

        /// <summary>
        /// Creates entity.
        /// </summary>
        /// <param name="client">RestClient to be used</param>
        /// <param name="entityName">Name to be given to the entity</param>
        /// <returns>Status code of the creation operation.</returns>
        internal HttpStatusCode CreateEntityAndGetAStatusCode(RestClient client, string entityName)
        {
            _request = new RestRequest(Method.POST);
            _request.AddHeader("Authorization", "Bearer " + _accessToken);
            _request.AddParameter("undefined", $"{{\"Name\":\"{entityName}\"}}", ParameterType.RequestBody);
            var response = (client.Execute(_request)).StatusCode;
            return response;

        }

        internal void CreateEntity(RestClient client, string entityName)
        {
            _request = new RestRequest(Method.POST);
            _request.AddHeader("Authorization", "Bearer " + _accessToken);
            _request.AddParameter("undefined", $"{{\"Name\":\"{entityName}\"}}", ParameterType.RequestBody);
            client.Execute(_request);
        }

        /// <summary>
        /// Gets all existing entities
        /// </summary>
        /// <param name="client">RestClient to be used</param>
        /// <returns>Response from request execution</returns>
        internal IRestResponse GetAllEntities(RestClient client)
        {
            _request = new RestRequest(Method.GET);
            _request.AddHeader("Authorization", "Bearer " + _accessToken);
            IRestResponse response = client.Execute(_request);
            return response;
        }

        /// <summary>
        /// Gets a specific entity by its Id.
        /// </summary>
        /// <param name="client">RestClient to be used</param>
        /// <param name="id">Id of the entity</param>
        /// <returns>Response from request execution</returns>
        internal IRestResponse GetEntityById(RestClient client, int id)
        {
            _request = new RestRequest($"/id/{id}", Method.GET);
            _request.AddHeader("Authorization", "Bearer " + _accessToken);
            IRestResponse response = client.Execute(_request);
            return response;
        }

        /// <summary>
        /// Gets a list of entity's IDs
        /// </summary>
        /// <param name="client">RestClient to be used</param>
        /// <returns>A list of integers values of the actual entity' IDs</returns>
        internal List<int> GetActualIds(RestClient client)
        {
            var entityResponse = GetAllEntities(client);
            var parsedResponses = ParseEntityResponse(entityResponse);
            var actualIds = new List<int>();
            foreach (var response in parsedResponses)
            {
                actualIds.Add(response.Id);
            }

            return actualIds;
        }

        /// <summary>
        /// Deletes a specific entity by its Id.
        /// </summary>
        /// <param name="client">RestClient to be used</param>
        /// <param name="id">Id of the entity</param>
        /// <returns>Response from request execution</returns>
        internal IRestResponse DeleteEntityById(RestClient client, int id)
        {
            _request = new RestRequest($"/id/{id}", Method.DELETE);
            _request.AddHeader("Authorization", "Bearer " + _accessToken);
            IRestResponse response = client.Execute(_request);
            return response;
        }

        /// <summary>
        /// Deserializes JSON object into EntityModel fields
        /// </summary>
        /// <param name="response">Response from request execution</param>
        /// <returns>Collection with EntityModel fields</returns>
        internal List<EntityModel> ParseEntityResponse(IRestResponse response)
        {
            return JsonConvert.DeserializeObject<List<EntityModel>>(response.Content);
        }

        /// <summary>
        /// Deserializes JSON object into TokenModel fields
        /// </summary>
        /// <param name="response">Response from request execution</param>
        /// <returns>Collection with TokenModel fields</returns>
        internal TokenModel ParseTokenResponse(IRestResponse response)
        {
            return JsonConvert.DeserializeObject<TokenModel>(response.Content);
        }

        /// <summary>
        /// Verifies whether any entitity exists
        /// </summary>
        /// <param name="client">RestClient to be used</param>
        /// <returns>Returns true if any entities exist</returns>
        internal bool DoesAnyEntityExist(RestClient client)
        {
            return GetAllEntities(client).Content.Length > 0;
        }

        /// <summary>
        /// Deletes all existing entitites
        /// </summary>
        /// <param name="client">RestClient to be used</param>
        internal void DeleteAllEntities(RestClient client)
        {
            var entities = GetAllEntities(client);
            var responses = ParseEntityResponse(entities);
            if (responses == null) return;
            foreach (var response in responses)
            {
                DeleteEntityById(client, response.Id);
            }

        }

        /// <summary>
        /// Gets 'access_token' value from response
        /// </summary>
        /// <returns>'access_token' value in string format</returns>
        private string GetAccessTokenValue()
        {
            var response = CreateToken();
            var jsonResponse = ParseTokenResponse(response).AccessToken;
            return jsonResponse;
        }
    }
}
