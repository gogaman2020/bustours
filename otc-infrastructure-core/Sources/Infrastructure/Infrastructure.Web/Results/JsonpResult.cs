using Infrastructure.Common.Json;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.Web.Results
{
    public class JsonpResult : ActionResult
    {
        public string CallbackFunction { get; set; }
        public Encoding ContentEncoding { get; set; }
        public string ContentType { get; set; }
        public object Data { get; set; }

        public JsonpResult(object data) : this(data, null) { }
        public JsonpResult(object data, string callbackFunction)
        {
            Data = data;
            CallbackFunction = callbackFunction;
        }

        public override void ExecuteResult(ActionContext context)
        {
            var bytes = GetBytes(context);
            if (bytes != null)
            {
                context.HttpContext.Response.Body.Write(bytes, 0, bytes.Length);
            }
        }

        public override async Task ExecuteResultAsync(ActionContext context)
        {
            var bytes = GetBytes(context);

            if (bytes != null)
            {
                await context.HttpContext.Response.Body.WriteAsync(bytes, 0, bytes.Length);
            }
        }

        private byte[] GetBytes(ActionContext context)
        {
            if (context == null) throw new ArgumentNullException("context");

            var response = context.HttpContext.Response;

            response.ContentType = string.IsNullOrEmpty(ContentType) ? "application/json" : ContentType;

            //if (ContentEncoding != null) response.ContentEncoding = ContentEncoding;

            if (Data != null)
            {
                var request = context.HttpContext.Request;

                var callback = CallbackFunction ?? request.Query["callback"];
                if (string.IsNullOrEmpty(callback))
                {
                    callback = "callback";
                }

                return Encoding.UTF8.GetBytes(string.Format("{0}({1});", callback, Data.ToJson()));
            }

            return null;
        }
    }
}
