using CodeDesignPlus.Net.Exceptions;

namespace CodeDesignPlus.Net.Microservice.Catalogs.Infrastructure;

public class Errors: IErrorCodes
{    
    public static readonly Error UnknownError = new("300");
}
