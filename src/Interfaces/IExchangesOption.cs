using RabbitMQ.Client.Core.Options;
using System.Collections.Generic;

namespace RabbitMQ.Client.Core.Interfaces
{
    public interface IExchangesOption
    {
        public IEnumerable<ExchangeOption> Exchanges { get; set; }
    }
}
