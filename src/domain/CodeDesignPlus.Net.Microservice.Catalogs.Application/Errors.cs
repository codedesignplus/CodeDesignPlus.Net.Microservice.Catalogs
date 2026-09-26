using CodeDesignPlus.Net.Exceptions;

namespace CodeDesignPlus.Net.Microservice.Catalogs.Application;

public class Errors: IErrorCodes
{
    public static readonly Error UnknownError = new("200");
    public static readonly Error InvalidRequest = new("201");
    public static readonly Error TypeDocumentAlreadyExists = new("202");
    public static readonly Error TypeDocumentNotFound = new("203");
    public static readonly Error TypeDocumentCodeAlreadyExists = new("204");

    // Mensajes de los validadores (plan 050). Sin ellos FluentValidation compone «Name es obligatorio» con el nombre
    // de la propiedad de C#, en ingles dentro de una frase en espanol.
    public static readonly Error IdIsRequired = new("205");
    public static readonly Error NameIsRequired = new("206");
    public static readonly Error NameMaxLengthExceeded = new("207");
    public static readonly Error CodeIsRequired = new("208");
    public static readonly Error CodeMaxLengthExceeded = new("209");
    public static readonly Error DescriptionMaxLengthExceeded = new("210");
}
