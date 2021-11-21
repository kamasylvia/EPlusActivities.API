using System;
using System.Net.Http;
using System.Threading.Tasks;
using AutoMapper;
using Dapr.Client;
using EPlusActivities.API.Application.Commands.FileCommands;
using EPlusActivities.API.Application.Queries.FileQueries;
using EPlusActivities.API.Infrastructure.Attributes;
using EPlusActivities.Grpc.Messages.FileService;
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

        public async Task<UploadFileGrpcResponse> UploadFileAsync(UploadFileCommand request) =>
            await _daprClient.InvokeMethodGrpcAsync<UploadFileGrpcRequest, UploadFileGrpcResponse>(
                _fileServiceAppId,
                "UploadFile",
                _mapper.Map<UploadFileGrpcRequest>(request)
            );

        public async Task<DownloadFileGrpcResponse> DownloadFileByFileIdAsync(
            DownloadFileByFileIdQuery request
        ) =>
            await _daprClient.InvokeMethodGrpcAsync<
                DownloadFileByFileIdGrpcRequest,
                DownloadFileGrpcResponse
            >(
                _fileServiceAppId,
                "DownloadFileByFileId",
                _mapper.Map<DownloadFileByFileIdGrpcRequest>(request)
            );

        public async Task<DownloadFileGrpcResponse> DownloadFileByKeyAsync(
            DownloadFileByKeyQuery request
        ) =>
            await _daprClient.InvokeMethodGrpcAsync<
                DownloadFileByKeyGrpcRequest,
                DownloadFileGrpcResponse
            >(
                _fileServiceAppId,
                "DownloadFileByKey",
                _mapper.Map<DownloadFileByKeyGrpcRequest>(request)
            );

        public async Task<DeleteFileGrpcResponse> DeleteFileByFileIdAsync(
            DeleteFileByFileIdCommand request
        ) =>
            await _daprClient.InvokeMethodGrpcAsync<
                DeleteFileByFileIdGrpcRequest,
                DeleteFileGrpcResponse
            >(
                _fileServiceAppId,
                "DeleteFileByFileId",
                _mapper.Map<DeleteFileByFileIdGrpcRequest>(request)
            );

        public async Task<DeleteFileGrpcResponse> DeleteFileByKeyAsync(
            DeleteFileByKeyCommand request
        ) =>
            await _daprClient.InvokeMethodGrpcAsync<
                DeleteFileByKeyGrpcRequest,
                DeleteFileGrpcResponse
            >(
                _fileServiceAppId,
                "DeleteFileByKey",
                _mapper.Map<DeleteFileByKeyGrpcRequest>(request)
            );
    }
}
