using Lykke.JobTriggers.Triggers.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net.Http;
using System.Net;
using Common.Log;
using Common;
using Newtonsoft.Json;
using Lykke.Service.WithdrawalRequestScheduler.Repositories;
using Lykke.Service.WithdrawalRequestScheduler.Models;

namespace Lykke.Service.WithdrawalRequestScheduler.Functions
{
    public class CheckWithdrawalTransactionStatusFunction
    {
        private readonly CashOutAttemptRepository _cashOutAttemptRepository;
        private readonly WithdrawalRequestSchedulerSettings _settings;
        private readonly ILog _log;

        public CheckWithdrawalTransactionStatusFunction(CashOutAttemptRepository cashOutAttemptRepository, WithdrawalRequestSchedulerSettings settings, ILog log)
        {
            _cashOutAttemptRepository = cashOutAttemptRepository;
            _settings = settings;
            _log = log;


            SerializationSettings = new Newtonsoft.Json.JsonSerializerSettings
            {
                Formatting = Newtonsoft.Json.Formatting.Indented,
                DateFormatHandling = Newtonsoft.Json.DateFormatHandling.IsoDateFormat,
                DateTimeZoneHandling = Newtonsoft.Json.DateTimeZoneHandling.Utc,
                NullValueHandling = Newtonsoft.Json.NullValueHandling.Ignore,
                ReferenceLoopHandling = Newtonsoft.Json.ReferenceLoopHandling.Serialize,
                ContractResolver = new Microsoft.Rest.Serialization.ReadOnlyJsonContractResolver(),
                Converters = new System.Collections.Generic.List<Newtonsoft.Json.JsonConverter>
                    {
                        new Microsoft.Rest.Serialization.Iso8601TimeSpanConverter()
                    }
            };
        }

        public JsonSerializerSettings SerializationSettings { get; private set; }

        [TimerTrigger("00:01:00")]
        public async Task Process()
        {
            try
            {
                var expirationDate = DateTime.UtcNow.AddHours(_settings.HoursTillCanceledByTimeout * -1);

                var transactions = await _cashOutAttemptRepository.GetAllAttempts();
                var expiredTransactions = transactions
                            .Where(tr => tr.Status == CashOutRequestStatus.ClientConfirmation && tr.DateTime < expirationDate)
                            .ToList();

                if (expiredTransactions.Count == 0)
                    return;

                var requestBody = new CancelRequestsByTimeoutModel() {
                    RequestsToCancel = expiredTransactions.Select(tr => new KeyValuePair<string, string>(tr.ClientId, tr.Id)).ToList()
                };

                var apiHost = _settings.ApiHost;

                var _url = new Uri(new System.Uri(apiHost + (apiHost.EndsWith("/") ? "" : "/")), "api/CashOutSwiftRequest/CancelRequestsByTimeout").ToString();

                // Create HTTP transport objects
                var _httpRequest = new HttpRequestMessage();
                HttpResponseMessage _httpResponse = null;
                _httpRequest.Method = new HttpMethod("POST");
                _httpRequest.RequestUri = new Uri(_url);

                // Serialize Request
                string _requestContent = null;
                
                _requestContent = Microsoft.Rest.Serialization.SafeJsonConvert.SerializeObject(requestBody, this.SerializationSettings);
                _httpRequest.Content = new System.Net.Http.StringContent(_requestContent, System.Text.Encoding.UTF8);
                _httpRequest.Content.Headers.ContentType = System.Net.Http.Headers.MediaTypeHeaderValue.Parse("application/json; charset=utf-8");

                // Send Request
                var httpClient = new HttpClient();
                _httpResponse = await httpClient.SendAsync(_httpRequest).ConfigureAwait(false);
                
                string _responseContent = null;
                if (_httpResponse.StatusCode != HttpStatusCode.OK)
                {
                    if (_httpResponse.Content != null)
                    {
                        _responseContent = await _httpResponse.Content.ReadAsStringAsync().ConfigureAwait(false);
                    }
                    else
                    {
                        _responseContent = string.Empty;
                    }

                    await _log.WriteErrorAsync("CheckWithdrawalTransactionStatusFunction", "API Request", (new { URL = _url, Body = _requestContent }).ToJson(), null);
                }
            }
            catch(Exception ex)
            {
                await _log.WriteErrorAsync("CheckWithdrawalTransactionStatusFunction", "Process", null, ex);
            }
        }
    }
}
