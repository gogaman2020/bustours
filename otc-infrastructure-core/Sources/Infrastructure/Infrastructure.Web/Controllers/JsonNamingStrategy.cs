using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using System;

namespace Infrastructure.Web.Controllers
{
    public static class JsonConfigurator
    {
        static JsonConfigurator()
        {
            Configure = (options) =>
            {
                options.SerializerSettings.ContractResolver = new DefaultContractResolver();
                options.SerializerSettings.DateFormatHandling = DateFormatHandling.IsoDateFormat;
                options.SerializerSettings.DateParseHandling = DateParseHandling.DateTime;
            };
        }

        public static Action<MvcNewtonsoftJsonOptions> Configure { get; set; }
    }
}
