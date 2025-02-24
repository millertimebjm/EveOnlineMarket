using Eve.Services.Interfaces.Session;

public class SessionHelper : ISessionHelper
{
    public void Add(string key, object value)
    {
        // session[key] = value;
        throw new NotImplementedException();
    }

    public object Get(string key)
    {
        //return HttpContext.Session[key];
        throw new NotImplementedException();
    }
}