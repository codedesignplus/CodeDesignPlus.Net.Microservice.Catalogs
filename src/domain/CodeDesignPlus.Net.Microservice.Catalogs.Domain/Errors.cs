namespace CodeDesignPlus.Net.Microservice.Catalogs.Domain;

public class Errors: IErrorCodes
{    
    public const string UnknownError = "100 : UnknownError";

    public const string NameRequired = "101 : Name is required."; 
    public const string CodeRequired = "102 : Code is required."; 
}
