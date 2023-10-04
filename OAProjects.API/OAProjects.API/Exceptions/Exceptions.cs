//using System.Net;

//namespace OAProjects.API.Exceptions;

//public class ForbiddenException : BaseException
//{
//    public ForbiddenException(string message) : base(HttpStatusCode.Forbidden, message)
//    {
//    }

//    public ForbiddenException(string message, object info) : base(HttpStatusCode.Forbidden, message, info)
//    {
//    }
//}

//public abstract class BaseException : Exception
//{
//    protected BaseException(HttpStatusCode httpErrorCode, string message, Exception innerException = null) : base(message,
//        innerException)
//    {
//        HttpErrorCode = httpErrorCode;
//    }

//    protected BaseException(HttpStatusCode httpErrorCode, string message, object info, Exception innerException = null) :
//        this(httpErrorCode,
//            message, innerException)
//    {
//        Info = info;
//    }

//    public HttpStatusCode HttpErrorCode { get; set; }

//    public object Info { get; set; }
//}

