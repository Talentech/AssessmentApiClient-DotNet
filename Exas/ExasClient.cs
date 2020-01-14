using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Extensions.Configuration;
using System.Net.Http;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Auth;
using Auth.Models;
using System.Net.Http.Headers;

namespace Exas
{
    public class ExasClient : HttpClient, IExasClient
    {
        private readonly string ExasSubscriptionKey;
        private readonly string ExasApiKey;
        private Token _token;
        private IAuthClient auth;

        public string ExasBaseUrl { get; private set; }

        public ExasClient(IConfiguration config, IAuthClient authClient)
        {
            ExasBaseUrl = config["Exas:BaseUri"];
            ExasSubscriptionKey = config["Exas:SubscriptionKey"];

            DefaultRequestHeaders.Add("Ocp-Apim-Subscription-Key", ExasSubscriptionKey);
            auth = authClient;
        } 

        private async Task RefreshAccessToken()
        {
            if (_token is null || DateTime.Compare(DateTime.Now, DateTime.Parse(_token.Expires)) < 0)
            {
                _token = await auth.GetBearerToken();
                DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue(_token.Type, _token.BearerToken);
            }
        }

        #region Objects
        //TODO Move to classes
        [JsonObject("assessmentProviderDto")]
        public class AssessmentProviderDto {

            [JsonProperty("partnerId")]
            public string PartnerId { get; set; }

            [JsonProperty("apiKey")]
            public string ApiKey { get; set; }

            [JsonProperty("UserName")]
            public string UserName { get; set; }

            [JsonProperty("password")]
            public string Password { get; set; }

            [JsonProperty("assessmentTypeId")]
            public int AssessmentTypeId { get; set; }

            [JsonProperty("assessmentTypeName")]
            public string AssessmentTypeName { get; set; }

            [JsonProperty("environmentType")]
            public string EnvironmentType { get; set; }
        }

        public class InvitationCreationIncomingDto
        {
            [JsonProperty("tenantId")]
            public Guid TenantId { get; set;}
            [JsonProperty("hrmCandidateId")]
            public Guid HrmCandidateId { get; set; }
            [JsonProperty("firstName")]
            public string FirstName { get; set; }
            [JsonProperty("lastName")]
            public string LastName { get; set; }
            [JsonProperty("email")]
            public string Email { get; set; }
            [JsonProperty("languageCode")]
            public string LanguageCode { get; set; }
        }

        public class ResultDto
        {
            [JsonProperty("partnerId")]
            public Guid PartnerId { get; set; }
            [JsonProperty("invitationId")]
            public Guid InvitationId { get; set; }
            [JsonProperty("assessmentName")]
            public string AssessmentName { get; set; }
            [JsonProperty("score")]
            public string Score { get; set; }
            [JsonProperty("reportUrlsAsCsv")]
            public string ReportUrlsAsCsv { get; set; }
            [JsonProperty("resultJson")]
            public string ResultJson { get; set; }
        }

        public class AssessmentTypeDto {
            [JsonProperty("id")]
            int Id { get; set; }
            [JsonProperty("name")]
            string Name { get; set; }
            [JsonProperty("description")]
            string description { get; set; }
            [JsonProperty("configJson")]
            string ConfigJson { get; set; }
        }

        public class AssessmentDto{
            [JsonProperty("id")]
            Guid Id { get; set; }
        }

        public class ProviderTestDto{
            [JsonProperty("id")]
            Guid Id { get; set; }
            [JsonProperty("assessmentTypeId")]
            int AssessmentTypeId { get; set; }
            [JsonProperty("assessmentTypeName")]
            string AssessmentTypeName { get; set; }
            [JsonProperty("assessmentName")]
            string AssessmentName { get; set; }
            [JsonProperty("languageCode")]
            string LanguageCode { get; set; }
            [JsonProperty("assessmentJson")]
            string AssessmentJson { get; set; }
        }

    #endregion


    #region Post Calls
    //AddAssessmentProviderConfiguration
    public async Task<string> AddAssessmentProviderConfiguration(string tenantId)
        {
            await RefreshAccessToken();
            string url = ExasBaseUrl + $"/v1/customers/{tenantId}/assessmentConfigurations";

            AssessmentProviderDto obj = new AssessmentProviderDto();
            var objJson = JsonConvert.SerializeObject(obj);

            HttpContent content = new StringContent(objJson);
           
            var result = await PostAsync(url, content);
            result.EnsureSuccessStatusCode();

            var resultString = await result.Content.ReadAsStringAsync();
            return resultString;
        }

        //Creates and sends invitation
        public async Task<string> CreateAndSendInvitation(string tenantId, string assessmentTestId, string sendInvitation)
        {
            //Post
            string url = ExasBaseUrl + $"/v1/{tenantId}/invitations/tests/{assessmentTestId}/{sendInvitation}";
            InvitationCreationIncomingDto obj = new InvitationCreationIncomingDto();
            var objJson = JsonConvert.SerializeObject(obj);

            HttpContent content = new StringContent(objJson);

            var result = await PostAsync(url, content);
            result.EnsureSuccessStatusCode();

            var resultString = await result.Content.ReadAsStringAsync();
            return resultString;

        }

        //ResultsPostCallback
        public async Task<string> ResultsPostCallback(string invitationId)
        {
            //Post
            string url = ExasBaseUrl + $"/v1/results/{invitationId}";
            ResultDto obj = new ResultDto();
            var objJson = JsonConvert.SerializeObject(obj);

            HttpContent content = new StringContent(objJson);

            var result = await PostAsync(url, content);
            result.EnsureSuccessStatusCode();

            var resultString = await result.Content.ReadAsStringAsync();
            return resultString;
        }

        ////SaveAsessmentTestConfiguration
        public async Task<string> SaveAsessmentTestConfiguration(string tenantId, string id)
        {
            //Post
            string url = ExasBaseUrl + $"/v1/{tenantId}/tests/{id}";
            ProviderTestDto obj = new ProviderTestDto();
            var objJson = JsonConvert.SerializeObject(obj);

            HttpContent content = new StringContent(objJson);

            var result = await PostAsync(url, content);
            result.EnsureSuccessStatusCode();

            var resultString = await result.Content.ReadAsStringAsync();
            return resultString;
        }

        ////SaveAssessmentType
        public async Task<string> SaveAssessmentType()
        {
            //Post
            string url = ExasBaseUrl + $"/v1/assessmenttypes";
            AssessmentTypeDto obj = new AssessmentTypeDto();
            var objJson = JsonConvert.SerializeObject(obj);

            HttpContent content = new StringContent(objJson);

            var result = await PostAsync(url, content);
            result.EnsureSuccessStatusCode();

            var resultString = await result.Content.ReadAsStringAsync();
            return resultString;

        }

        ////SaveCustomer
        public async Task<string> SaveCustomer(string tenantId)
        {
            //Post
            string url = ExasBaseUrl + $"/v1/customers/{tenantId}";

            var result = await PostAsync(url, null);
            result.EnsureSuccessStatusCode();

            var resultString = await result.Content.ReadAsStringAsync();
            return resultString;
        }

        #endregion


        #region GET calls

        public async Task<string> GetLatestStatusInvitationSentToCandidate(string tenantId, string invitationId)
        {
            await RefreshAccessToken();
            //GET
            string url = ExasBaseUrl + $"/v1/{tenantId}/invitations/{invitationId}/status";
            var result = await GetAsync(url);

            result.EnsureSuccessStatusCode();

            var resultString = await result.Content.ReadAsStringAsync();
            return resultString;
        }


        public async Task<string> GetAllAssessmentTypeIdNames()
        {
            await RefreshAccessToken();
            //GET
            string url = ExasBaseUrl + $"/v1/assessmenttypes/id-names";
            var result = await GetAsync(url);

            result.EnsureSuccessStatusCode();

            var resultString = await result.Content.ReadAsStringAsync();
            return resultString;
        }

        public async Task<string> GetAllAssessmentTypes()
        {
            await RefreshAccessToken();
            //GET
            string url = ExasBaseUrl + $"/v1/assessmenttypes";
            var result = await GetAsync(url);

            result.EnsureSuccessStatusCode();

            var resultString = await result.Content.ReadAsStringAsync();
            return resultString;
        }

        public async Task<string> GetAssessmentConfigurations(string tenantId)
        {
            await RefreshAccessToken();
            //GET
            string url = ExasBaseUrl + $"/v1/customers/{tenantId}/assessmentConfigurations";
            var result = await GetAsync(url);

            result.EnsureSuccessStatusCode();

            var resultString = await result.Content.ReadAsStringAsync();
            return resultString;
        }

        public async Task<string> GetAssessmentProviderConfiguration(string tenantId, string assessmentTypeId)
        {
            await RefreshAccessToken();
            //GET
            string url = ExasBaseUrl + $"/v1/customers/{tenantId}/assessmentConfigurations/{assessmentTypeId}";
            var result = await GetAsync(url);

            result.EnsureSuccessStatusCode();

            var resultString = await result.Content.ReadAsStringAsync();
            return resultString;
        }

        public async Task<string> GetAssessmentTest(string tenantId, string id)
        {
            await RefreshAccessToken();
            //GET
            string url = ExasBaseUrl + $"/v1/{tenantId}/tests/{id}";
            var result = await GetAsync(url);

            result.EnsureSuccessStatusCode();

            var resultString = await result.Content.ReadAsStringAsync();
            return resultString;
        }

        public async Task<string> GetAssessmentType(string id)
        {
            await RefreshAccessToken();
            //GET
            string url = ExasBaseUrl + $"/v1/assessmenttypes/{id}";
            var result = await GetAsync(url);

            result.EnsureSuccessStatusCode();

            var resultString = await result.Content.ReadAsStringAsync();
            return resultString;
        }

        public async Task<string> GetAssessmentTypeIdNames(string tenantId)
        {
            await RefreshAccessToken();
            //GET
            string url = ExasBaseUrl + $"/v1/assessmenttypes/{tenantId}/id-names";
            var result = await GetAsync(url);

            result.EnsureSuccessStatusCode();

            var resultString = await result.Content.ReadAsStringAsync();
            return resultString;
        }

        public async Task<string> GetAssessmentTypes(string tenantId)
        {
            await RefreshAccessToken();
            //GET
            string url = ExasBaseUrl + $"/v1/customers/{tenantId}/assessmentTypes";
            var result = await GetAsync(url);

            result.EnsureSuccessStatusCode();

            var resultString = await result.Content.ReadAsStringAsync();
            return resultString;
        }

        public async Task<string> GetCustomer(string tenantId)
        {
            await RefreshAccessToken();
            //GET
            string url = ExasBaseUrl + $"/v1/customers/{tenantId}";
            var result = await GetAsync(url);

            result.EnsureSuccessStatusCode();

            var resultString = await result.Content.ReadAsStringAsync();
            return resultString;
        }

        public async Task<string> GetResults(string invitationId)
        {
            await RefreshAccessToken();
            //GET
            string url = ExasBaseUrl + $"/v1/results/{invitationId}";
            var result = await GetAsync(url);

            result.EnsureSuccessStatusCode();

            var resultString = await result.Content.ReadAsStringAsync();
            return resultString;
        }

        public async Task<string> ListAllAssessmentTests(string tenantId)
        {
            await RefreshAccessToken();
            //GET
            string url = ExasBaseUrl + $"/v1/{tenantId}/tests/assessments";
            var result = await GetAsync(url);

            result.EnsureSuccessStatusCode();

            var resultString = await result.Content.ReadAsStringAsync();
            return resultString;

        }

        public async Task<string> ListAllAssessmentTestsByTenantId(string tenantId, string assessmentTypeId)
        {
            await RefreshAccessToken();
            //GET
            string url = ExasBaseUrl + $"/v1/{tenantId}/tests/assessmenttypes/{assessmentTypeId}";
            var result = await GetAsync(url);

            result.EnsureSuccessStatusCode();

            var resultString = await result.Content.ReadAsStringAsync();
            return resultString;
        }

        public async Task<string> ListAssessmentTests(string tenantId, string assessmentTypeId)
        {
            await RefreshAccessToken();
            //GET
            string url = ExasBaseUrl + $"/v1/{tenantId}/tests/assessmenttypes/{assessmentTypeId}";
            var result = await GetAsync(url);

            result.EnsureSuccessStatusCode();

            var resultString = await result.Content.ReadAsStringAsync();
            return resultString;
        }
        #endregion

    }
}
