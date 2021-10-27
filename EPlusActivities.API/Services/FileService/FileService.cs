using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Threading.Tasks;
using EPlusActivities.API.Application.Commands.FileCommands;
using EPlusActivities.API.Dtos.FileDtos;
using EPlusActivities.API.Infrastructure.Attributes;
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
        private readonly ILogger<FileService> _logger;
        private readonly IConfiguration _configuration;

        public FileService(
            IHttpClientFactory httpClientFactory,
            ILogger<FileService> logger,
            IConfiguration configuration
        )
        {
            _httpClientFactory =
                httpClientFactory ?? throw new ArgumentNullException(nameof(httpClientFactory));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _configuration =
                configuration ?? throw new ArgumentNullException(nameof(configuration));
        }

        public async Task<byte[]> DownloadFileByKeyAsync(DownloadFileByKeyCommand request)
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

            return await _httpClientFactory.CreateClient().GetByteArrayAsync(requestUrl);
        }

        public async Task<string> GetContentTypeByKeyAsync(DownloadFileByKeyCommand request)
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

        public async Task<byte[]> DownloadFileByIdAsync(DownloadFileByIdCommand request)
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

        public async Task<string> GetContentTypeByIdAsync(DownloadFileByIdCommand request)
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

        public async Task<bool> UploadFileAsync(UploadFileCommand request)
        {
            var uriBuilder = new UriBuilder(
                scheme: _configuration["FileServiceUriBuilder:Scheme"],
                host: _configuration["FileServiceUriBuilder:Host"],
                port: Convert.ToInt32(_configuration["FileServiceUriBuilder:Port"]),
                pathValue: "api/file"
            );

            var formData = new MultipartFormDataContent();
            formData.Add(new StringContent(request.OwnerId.ToString()), "ownerId");
            formData.Add(new StringContent(request.Key), "key");

            var streamContent = new StreamContent(request.FormFile.OpenReadStream());
            streamContent.Headers.ContentType = new MediaTypeHeaderValue(
                request.FormFile.ContentType
            );
            formData.Add(streamContent, "formFile", request.FormFile.FileName);

            var response = await _httpClientFactory
                .CreateClient()
                .PostAsync(uriBuilder.Uri, formData);
            return response.IsSuccessStatusCode;
        }

        public async Task<bool> DeleteFileByIdAsync(DeleteFileByIdCommand requestDto)
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
