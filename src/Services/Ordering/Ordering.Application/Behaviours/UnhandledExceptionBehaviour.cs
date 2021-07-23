using MediatR;
using Microsoft.Extensions.Logging;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Ordering.Application.Behaviours
{
    public class UnhandledExceptionBehaviour<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse>
    {
        private readonly ILogger _logger;

        public UnhandledExceptionBehaviour(ILogger<TRequest> logger) =>
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));

        public Task<TResponse> Handle(TRequest request, CancellationToken cancellationToken,
                                      RequestHandlerDelegate<TResponse> next)
        {
            try
            {
                return next();
            }
            catch (Exception Ex)
            {
                var requestName = typeof(TRequest).Name;
                _logger.LogError(Ex, "Application Request: Unhandled Exception for Request {Name} {@Request}" , requestName, request);
                throw;
            }
        }
    }
}
