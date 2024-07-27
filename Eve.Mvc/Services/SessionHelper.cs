


using Eve.Mvc.Services.Interfaces;

public class SessionHelper : ISessionHelper
{
    public void Add(ISession session, string key, object value)
    {
        session[key] = value;
    }

    public object Get(string key)
    {
        return HttpContext.Session[key];
    }
}