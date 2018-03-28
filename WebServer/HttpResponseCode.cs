namespace WebServer
{
    public enum HttpResponseCode
    {
        Ok = 200,
        BadRequest = 400,
        NotFound = 404,
        ServerError = 500,
        Redirect = 302
    }
}