using System.Net.Http;
using System.Text.Json.Nodes;
using System.Threading.Tasks;

namespace First.Services
{
    internal interface IPayLoadBuilder
    {
        Task<string> GetPayloadFromHttpResponse(HttpResponseMessage httpResponseMessage);
    }

    internal class PayLoadBuilder : IPayLoadBuilder
    {
        public async Task<string> GetPayloadFromHttpResponse(HttpResponseMessage httpResponseMessage)
        {
            var body = await httpResponseMessage.Content.ReadAsStringAsync();
            var contentNode = JsonNode.Parse(body);
            var contentHeaders = new JsonObject();
            foreach (var header in httpResponseMessage.Content.Headers)
            { 
                contentHeaders.Add(header.Key, string.Join(", ", header.Value));
            }

            var headersNode = new JsonObject();
            foreach (var header in httpResponseMessage.Headers)
            { 
                headersNode.Add(header.Key, string.Join(", ", header.Value));
            }

            var trailingHeadersNode = new JsonObject();
            foreach (var header in httpResponseMessage.TrailingHeaders)
            {
                trailingHeadersNode.Add(header.Key, string.Join(", ", header.Value));
            }

            var payload = new JsonObject
            {
                {"Headers", headersNode},
                {"ContentHeaders", contentHeaders},
                {"Content", contentNode}
            };

            if (trailingHeadersNode.Count > 0)
            {
                payload.Add("TrailingHeaders", trailingHeadersNode);
            }

            return payload.ToJsonString();
        }
    }
}
