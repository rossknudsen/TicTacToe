namespace WebServer.Responses
{
    internal class RedirectResponse : NoBodyResponse
    {
        public RedirectResponse(string redirectUrl)
        {
            Headers["Location"] = redirectUrl;
        }

        public override HttpResponseCode ResponseCode => HttpResponseCode.Redirect;

        public override string ResponseText => "Redirect";
    }
}