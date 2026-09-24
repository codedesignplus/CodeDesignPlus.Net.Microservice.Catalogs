using CodeDesignPlus.Net.Exceptions;

namespace CodeDesignPlus.Net.Microservice.Catalogs.Application;

public class Errors: IErrorCodes
{    
    public static readonly Error UnknownError = new("200");
    public static readonly Error InvalidRequest = new("201");
    public static readonly Error TypeDocumentAlreadyExists = new("202");
    public static readonly Error TypeDocumentNotFound = new("203");

}
