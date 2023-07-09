// Global using directives

global using System.IO.Compression;
global using CountryWiki.DAL.Compression;
global using CountryWiki.DAL.Interceptors;
global using CountryWiki.DAL.Mappers;
global using CountryWiki.DAL.Protos.v1;
global using CountryWiki.Domain.Models;
global using CountryWiki.Domain.Repositories;
global using Google.Protobuf.WellKnownTypes;
global using Grpc.Core;
global using Grpc.Core.Interceptors;
global using Grpc.Net.Client;
global using Grpc.Net.Client.Web;
global using Grpc.Net.Compression;
global using Microsoft.Extensions.DependencyInjection;
global using Microsoft.Extensions.Logging;
global using static CountryWiki.DAL.v1.CountryService;