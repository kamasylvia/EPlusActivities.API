using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Threading.Tasks;
using AutoMapper;
using Dapr.Client;
using EPlusActivities.API.Application.Commands.FileCommands;
using EPlusActivities.API.Dtos.FileDtos;
using EPlusActivities.API.Infrastructure.Attributes;
using EPlusActivities.API.Infrastructure.Exceptions;
using EPlusActivities.Grpc.Messages.FileService;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace EPlusActivities.API.Services.FileService
{
    [CustomDependency(ServiceLifetime.Scoped)]
    public class FileService : IFileService
    {
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly DaprClient _daprClient;
        private readonly IMapper _mapper;
        private readonly ILogger<FileService> _logger;
        private readonly IConfiguration _configuration;
        private readonly string _fileServiceAppId;

        public FileService(
            IHttpClientFactory httpClientFactory,
            DaprClient daprClient,
            IMapper mapper,
            ILogger<FileService> logger,
            IConfiguration configuration
        )
        {
            _httpClientFactory =
                httpClientFactory ?? throw new ArgumentNullException(nameof(httpClientFactory));
            _daprClient = daprClient ?? throw new ArgumentNullException(nameof(daprClient));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _configuration =
                configuration ?? throw new ArgumentNullException(nameof(configuration));
            _fileServiceAppId = _configuration["Dapr:FileService"];
        }

        public async Task<DownloadFileGrpcResponse> DownloadFileByKeyAsync(
            DownloadFileByKeyCommand request
        )
        {
            var response = await _daprClient.InvokeMethodGrpcAsync<
                DownloadFileByKeyGrpcRequest,
                DownloadFileGrpcResponse
            >(
                _fileServiceAppId,
                "DownloadFileByKey",
                _mapper.Map<DownloadFileByKeyGrpcRequest>(request)
            );
            if (response.Data.IsEmpty)
            {
                throw new RemoteServiceException("Could not find the file in the FileService.");
            }
            return response;
        }

        public async Task<string> GetContentTypeByKeyAsync(DownloadFileByKeyCommand request) =>
            await _daprClient.InvokeMethodAsync<DownloadFileByKeyCommand, string>(
                HttpMethod.Get,
                _fileServiceAppId,
                "file/content-type/key",
                request
            );
        /*
        {
            var uriBuilder = new UriBuilder(
                scheme: _configuration["FileServiceUriBuilder:Scheme"],
                host: _configuration["FileServiceUriBuilder:Host"],
                port: Convert.ToInt32(_configuration["FileServiceUriBuilder:Port"]),
                pathValue: "api/file/content-type/key"
            );

            var requestUrl = QueryHelpers.AddQueryString(
                uriBuilder.Uri.ToString(),
                new Dictionary<string, string>
                {
                    ["OwnerId"] = request.OwnerId.ToString(),
                    ["Key"] = request.Key
                }
            );

            return await _httpClientFactory.CreateClient().GetStringAsync(requestUrl);
        }
        */

        public async Task<DownloadFileGrpcResponse> DownloadFileByIdAsync(
            DownloadFileByIdCommand request
        ) =>
            // await _daprClient.InvokeMethodAsync<DownloadFileByIdCommand, byte[]>(
            //     HttpMethod.Get,
            //     _fileServiceAppId,
            //     "file/id",
            //     request
            // );
            await _daprClient.InvokeMethodGrpcAsync<
                DownloadFileByIdGrpcRequest,
                DownloadFileGrpcResponse
            >(
                _fileServiceAppId,
                "DownloadFileByFileId",
                _mapper.Map<DownloadFileByIdGrpcRequest>(request)
            );
        /*
        {
            var uriBuilder = new UriBuilder(
                scheme: _configuration["FileServiceUriBuilder:Scheme"],
                host: _configuration["FileServiceUriBuilder:Host"],
                port: Convert.ToInt32(_configuration["FileServiceUriBuilder:Port"]),
                pathValue: "api/file/id"
            );

            var requestUrl = QueryHelpers.AddQueryString(
                uriBuilder.Uri.ToString(),
                new Dictionary<string, string> { ["FileId"] = request.FileId.ToString() }
            );

            return await _httpClientFactory.CreateClient().GetByteArrayAsync(requestUrl);
        }
        */

        public async Task<string> GetContentTypeByIdAsync(DownloadFileByIdCommand request) =>
            await _daprClient.InvokeMethodAsync<DownloadFileByIdCommand, string>(
                HttpMethod.Get,
                _fileServiceAppId,
                "file/content-type/id",
                request
            );
        /*
        {
            var uriBuilder = new UriBuilder(
                scheme: _configuration["FileServiceUriBuilder:Scheme"],
                host: _configuration["FileServiceUriBuilder:Host"],
                port: Convert.ToInt32(_configuration["FileServiceUriBuilder:Port"]),
                pathValue: "api/file/content-type/id"
            );

            var requestUrl = QueryHelpers.AddQueryString(
                uriBuilder.Uri.ToString(),
                new Dictionary<string, string> { ["FileId"] = request.FileId.ToString() }
            );

            return await _httpClientFactory.CreateClient().GetStringAsync(requestUrl);
        }
        */

        public async Task<bool> UploadFileAsync(UploadFileCommand request) =>
            (
                await _daprClient.InvokeMethodGrpcAsync<
                    UploadFileGrpcRequest,
                    UploadFileGrpcResponse
                >(_fileServiceAppId, "UploadFile", _mapper.Map<UploadFileGrpcRequest>(request))
            ).Succeeded;

        public async Task<bool> DeleteFileByIdAsync(DeleteFileByIdCommand request) =>
            await _daprClient.InvokeMethodAsync<DeleteFileByIdCommand, bool>(
                HttpMethod.Delete,
                _fileServiceAppId,
                "api/file",
                request
            );
        /*
        {
            var uriBuilder = new UriBuilder(
                scheme: _configuration["FileServiceUriBuilder:Scheme"],
                host: _configuration["FileServiceUriBuilder:Host"],
                port: Convert.ToInt32(_configuration["FileServiceUriBuilder:Port"]),
                pathValue: "api/file/id"
            );

            var requestUrl = QueryHelpers.AddQueryString(
                uriBuilder.Uri.ToString(),
                new Dictionary<string, string> { ["FileId"] = requestDto.FileId.ToString() }
            );

            var response = await _httpClientFactory.CreateClient().DeleteAsync(requestUrl);
            return response.IsSuccessStatusCode;
        }
        */

        public async Task<bool> DeleteFileByKeyAsync(DeleteFileByKeyCommand request)
        {
            var uriBuilder = new UriBuilder(
                scheme: _configuration["FileServiceUriBuilder:Scheme"],
                host: _configuration["FileServiceUriBuilder:Host"],
                port: Convert.ToInt32(_configuration["FileServiceUriBuilder:Port"]),
                pathValue: "api/file/key"
            );

            var requestUrl = QueryHelpers.AddQueryString(
                uriBuilder.Uri.ToString(),
                new Dictionary<string, string>
                {
                    ["OwnerId"] = request.OwnerId.ToString(),
                    ["Key"] = request.Key
                }
            );

            var response = await _httpClientFactory.CreateClient().DeleteAsync(requestUrl);
            return response.IsSuccessStatusCode;
        }

        public async Task<IEnumerable<DownloadFilesByOwnerIdDto>> DownloadFilesByOwnerIdAsync(
            Guid ownerId
        )
        {
            var uriBuilder = new UriBuilder(
                scheme: _configuration["FileServiceUriBuilder:Scheme"],
                host: _configuration["FileServiceUriBuilder:Host"],
                port: Convert.ToInt32(_configuration["FileServiceUriBuilder:Port"]),
                pathValue: "api/file/ownerId"
            );

            var requestUrl = QueryHelpers.AddQueryString(
                uriBuilder.Uri.ToString(),
                new Dictionary<string, string> { ["OwnerId"] = ownerId.ToString() }
            );

            return await _httpClientFactory
                .CreateClient()
                .GetFromJsonAsync<IEnumerable<DownloadFilesByOwnerIdDto>>(requestUrl);
        }
    }
}
