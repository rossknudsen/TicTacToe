namespace WebServer.Responses
{
    internal class ServerErrorResponse : NoBodyResponse
    {
        public override HttpResponseCode ResponseCode => HttpResponseCode.ServerError;

        public override string ResponseText => "Internal Server Error";
    }
}