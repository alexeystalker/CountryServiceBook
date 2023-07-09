// Global using directives

global using System.IO.Compression;
global using CountryService.BLL.Services;
global using CountryService.DAL.Database;
global using CountryService.DAL.Repositories;
global using CountryService.Domain.Models;
global using CountryService.Domain.Repositories;
global using CountryService.Domain.Services;
global using CountryService.gRPC.Compression;
global using CountryService.gRPC.Interceptors;
global using CountryService.gRPC.Mappers;
global using CountryService.gRPC.Protos.v1;
global using CountryService.gRPC.Services;
global using Google.Protobuf.WellKnownTypes;
global using Grpc.Core;
global using Grpc.Net.Compression;
global using Microsoft.Data.SqlClient;
global using Microsoft.EntityFrameworkCore;
global using static CountryService.gRPC.Browser.v1.CountryServiceBrowser;
global using static CountryService.gRPC.v1.CountryService;