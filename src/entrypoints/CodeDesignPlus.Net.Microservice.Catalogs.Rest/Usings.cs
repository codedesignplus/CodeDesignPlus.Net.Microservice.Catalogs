global using CodeDesignPlus.Microservice.Api.Dtos;
global using CodeDesignPlus.Net.Logger.Extensions;
global using CodeDesignPlus.Net.Mongo.Extensions;
global using CodeDesignPlus.Net.Observability.Extensions;
global using CodeDesignPlus.Net.RabbitMQ.Extensions;
global using CodeDesignPlus.Net.Redis.Extensions;
global using CodeDesignPlus.Net.Security.Extensions;
global using Mapster;
global using MapsterMapper;
global using MediatR;
global using Microsoft.AspNetCore.Mvc;
global using C = CodeDesignPlus.Net.Core.Abstractions.Models.Criteria;
global using CodeDesignPlus.Net.Serializers;
global using NodaTime;









global using CodeDesignPlus.Net.Microservice.Catalogs.Application.TypeDocument.Commands.CreateTypeDocument;
global using CodeDesignPlus.Net.Microservice.Catalogs.Application.TypeDocument.Commands.UpdateTypeDocument;
global using CodeDesignPlus.Net.Microservice.Catalogs.Application.TypeDocument.Commands.DeleteTypeDocument;
global using CodeDesignPlus.Net.Microservice.Catalogs.Application.TypeDocument.Queries.GetTypeDocumentById;
global using CodeDesignPlus.Net.Microservice.Catalogs.Application.TypeDocument.Queries.GetAllTypeDocument;