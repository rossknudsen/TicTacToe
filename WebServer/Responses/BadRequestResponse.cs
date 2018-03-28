namespace WebServer.Responses
{
    internal class BadRequestResponse : NoBodyResponse
    {
        public override HttpResponseCode ResponseCode => HttpResponseCode.BadRequest;

        public override string ResponseText => "Bad Request";
    }
}