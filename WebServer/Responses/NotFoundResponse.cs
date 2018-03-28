namespace WebServer.Responses
{
    internal class NotFoundResponse : NoBodyResponse
    {
        public override HttpResponseCode ResponseCode => HttpResponseCode.NotFound;

        public override string ResponseText => "Not Found";
    }
}