using System.Runtime.Serialization;
using XRSpace.OpenAPI;

namespace TPFive.OpenApi.GameServer.Model
{
    public class BaseResponse : IResponse
    {
        public virtual bool IsSuccess => (HttpStatusCode > 199) && (HttpStatusCode < 300);

        /// <summary>
        /// Gets or Sets the http status code of the response.
        /// </summary>
        public int HttpStatusCode { get; set; }

        /// <summary>
        /// Gets or Sets error code from server
        /// </summary>
        /// <value>status code from server.</value>
        [DataMember(Name="err_code", EmitDefaultValue = false)]
        public int ErrorCode { get; set; }

        /// <summary>
        /// Gets or Sets error message from server
        /// </summary>
        /// <value>error message from server.</value>
        [DataMember(Name="msg", EmitDefaultValue = false)]
        public string Message { get; set; }
    }
}