using System.Collections.Generic;

namespace RabbitMQ.Client.Core.Options
{
    public class ExchangesOption
    {
        public const string ExchangesSectionName = "ExchangesOption";
        public IEnumerable<ExchangeOption> Exchanges { get; set; }
    }
}
