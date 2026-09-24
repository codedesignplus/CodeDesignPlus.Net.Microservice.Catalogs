using CodeDesignPlus.Net.Exceptions;

namespace CodeDesignPlus.Net.Microservice.Catalogs.Domain;

public class Errors: IErrorCodes
{    
    public static readonly Error UnknownError = new("100");

    public static readonly Error NameRequired = new("101"); 
    public static readonly Error CodeRequired = new("102"); 
}
