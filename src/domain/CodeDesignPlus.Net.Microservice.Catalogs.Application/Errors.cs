namespace CodeDesignPlus.Net.Microservice.Catalogs.Application;

public class Errors: IErrorCodes
{    
    public const string UnknownError = "200 : UnknownError";
    public const string InvalidRequest = "201 : Invalid Request";
    public const string TypeDocumentAlreadyExists = "202 : TypeDocument Already Exists";
    public const string TypeDocumentNotFound = "203 : TypeDocument Not Found";

}
