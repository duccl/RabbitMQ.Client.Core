using System.Collections.Generic;

namespace RabbitMQ.Client.Core.Options
{
    ///<summary>
    /// Representation of Exchanges Section in appsettings.json to create
    ///</summary>
    public class ExchangesOption
    {
        public const string ExchangesSectionName = "ExchangesOption";
        public IEnumerable<ExchangeOption> Exchanges { get; set; }
    }
}
