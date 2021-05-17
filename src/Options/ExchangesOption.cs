using System.Collections.Generic;
using RabbitMQ.Client.Core.Interfaces;

namespace RabbitMQ.Client.Core.Options
{
    ///<summary>
    /// Representation of Exchanges Section in appsettings.json to create
    ///</summary>
    public class ExchangesOption: IExchangesOption
    {
        public const string ExchangesSectionName = "ExchangesOption";
        public IEnumerable<ExchangeOption> Exchanges { get; set; }
    }
}
