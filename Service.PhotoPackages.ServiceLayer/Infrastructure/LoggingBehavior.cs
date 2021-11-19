using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;
using CorrelationId.Abstractions;
using MediatR;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Serilog;
using Serilog.Events;

namespace Service.PhotoPackages.ServiceLayer.Infrastructure
{
    public class LoggingBehavior<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse>
    {
        private readonly ILogger _logger;
        private readonly ICorrelationContextAccessor _correlationContextAccessor;
        private static IEnumerable<string> PasswordFieldNames => new[] {"$..Pwd", "$..pwd", "$..Password", "$..password"};
        private static IEnumerable<string> ContentFieldNames => new[] {"$..Content", "$..content", "$..PhotoBytes", "$..photoBytes", "$..ContentData", "$..contentData"};
        private const string MessageFormat =
            "{@RequestPath} " +
            "{@RequestBody} " +
            "{@Response} " +
            "{@HttpMethod} " +
            "{@StartTime} " +
            "{@EndTime} " +
            "{@Elapsed} " +
            "{@CorrelationToken} " +
            "{@Exception} " +
            "{@IntegrationType} ";
        private const string IgnoredRequestBody = "<request body ignored>";
        private const string MediaContent = "byte[]";
        private const string Mask = "*****";



        public LoggingBehavior(ILogger logger, ICorrelationContextAccessor correlationContextAccessor)
        {
            _logger = logger;
            _correlationContextAccessor = correlationContextAccessor;
        }

        public async Task<TResponse> Handle(TRequest request, CancellationToken cancellationToken,
            RequestHandlerDelegate<TResponse> next)
        {
            var startTime = DateTime.UtcNow;
            var stopwatch = Stopwatch.StartNew();
            var commandType = $"mediatr://{request.GetType().Name}";
            var type = commandType.Contains("request", StringComparison.InvariantCultureIgnoreCase)
                ? "REQUEST"
                : "COMMAND";
            var correlationToken = _correlationContextAccessor.CorrelationContext?.CorrelationId ??
                                   Guid.NewGuid().ToString();
            Exception exception = default;
            TResponse response = default;

            try
            {
                response = await next();
            }
            catch (Exception e)
            {
                exception = e;
                throw;
            }
            finally
            {
                var endTime = DateTime.UtcNow;
                stopwatch.Stop();
                
                _logger.Write(exception != null ? LogEventLevel.Error : LogEventLevel.Information, MessageFormat,
                    commandType, GetFilteredBody(JsonConvert.SerializeObject(request), "application/json"),
                    GetFilteredBody(JsonConvert.SerializeObject(response),"application/json"), type, startTime, endTime, stopwatch.ElapsedMilliseconds,
                    correlationToken, exception?.Message, "MediatR");
            }

            return response;
        }
        
        private static string GetFilteredBody(string body, string contentType)
        {
            if (string.IsNullOrEmpty(body))
                return string.Empty;
            
            if (string.Equals(IgnoredRequestBody, body))
                return body;

            if (string.IsNullOrEmpty(contentType) || !contentType.Contains("json"))
                return body;

            JToken jToken;
            try
            {
                jToken = JToken.Parse(body);
                if (jToken.Type != JTokenType.Object) return body;
            }
            catch (Exception)
            {
                return body;
            }

            foreach (var passwordFieldName in PasswordFieldNames)
            {
                var tokens = jToken.SelectTokens(passwordFieldName);
                foreach (var token in tokens)
                {
                    token.Replace(Mask);
                }
            }

            foreach (var contentFieldName in ContentFieldNames)
            {
                var tokens = jToken.SelectTokens(contentFieldName);
                foreach (var token in tokens)
                {
                    token.Replace(MediaContent);
                }
            }

            return JsonConvert.SerializeObject(jToken);
        }

    }
}