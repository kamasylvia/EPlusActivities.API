using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Threading.Tasks;
using EPlusActivities.API.Dtos.FileDtos;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace EPlusActivities.API.Services.FileService
{
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

        public async Task<byte[]> DownloadFileByKeyAsync(DownloadFileByKeyRequestDto requestDto)
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
                    ["OwnerId"] = requestDto.OwnerId.ToString(),
                    ["Key"] = requestDto.Key
                }
            );

            return await _httpClientFactory.CreateClient().GetByteArrayAsync(requestUrl);
        }

        public async Task<string> GetContentTypeByKeyAsync(DownloadFileByKeyRequestDto requestDto)
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
                    ["OwnerId"] = requestDto.OwnerId.ToString(),
                    ["Key"] = requestDto.Key
                }
            );

            return await _httpClientFactory.CreateClient().GetStringAsync(requestUrl);
        }

        public async Task<byte[]> DownloadFileByIdAsync(DownloadFileByIdRequestDto requestDto)
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

            return await _httpClientFactory.CreateClient().GetByteArrayAsync(requestUrl);
        }

        public async Task<string> GetContentTypeByIdAsync(DownloadFileByIdRequestDto requestDto)
        {
            var uriBuilder = new UriBuilder(
                scheme: _configuration["FileServiceUriBuilder:Scheme"],
                host: _configuration["FileServiceUriBuilder:Host"],
                port: Convert.ToInt32(_configuration["FileServiceUriBuilder:Port"]),
                pathValue: "api/file/content-type/id"
            );

            var requestUrl = QueryHelpers.AddQueryString(
                uriBuilder.Uri.ToString(),
                new Dictionary<string, string> { ["FileId"] = requestDto.FileId.ToString() }
            );

            return await _httpClientFactory.CreateClient().GetStringAsync(requestUrl);
        }

        public async Task<int> UploadFileAsync(UploadFileRequestDto requestDto)
        {
            var uriBuilder = new UriBuilder(
                scheme: _configuration["FileServiceUriBuilder:Scheme"],
                host: _configuration["FileServiceUriBuilder:Host"],
                port: Convert.ToInt32(_configuration["FileServiceUriBuilder:Port"]),
                pathValue: "api/file"
            );

            var formData = new MultipartFormDataContent();
            formData.Add(new StringContent(requestDto.OwnerId.ToString()), "ownerId");
            formData.Add(new StringContent(requestDto.Key), "key");

            var streamContent = new StreamContent(requestDto.FormFile.OpenReadStream());
            streamContent.Headers.ContentType = new MediaTypeHeaderValue(
                requestDto.FormFile.ContentType
            );
            formData.Add(streamContent, "formFile", requestDto.FormFile.FileName);

            var response = await _httpClientFactory
                .CreateClient()
                .PostAsync(uriBuilder.Uri, formData);
            return Convert.ToInt32(response.StatusCode);
        }

        public async Task<int> DeleteFileByIdAsync(DownloadFileByIdRequestDto requestDto)
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
            return Convert.ToInt32(response.StatusCode);
        }

        public async Task<int> DeleteFileByKeyAsync(DownloadFileByKeyRequestDto requestDto)
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
                    ["OwnerId"] = requestDto.OwnerId.ToString(),
                    ["Key"] = requestDto.Key
                }
            );

            var response = await _httpClientFactory.CreateClient().DeleteAsync(requestUrl);
            return Convert.ToInt32(response.StatusCode);
        }

        public async Task<IEnumerable<Guid>> DownloadFilesByOwnerIdAsync(Guid ownerId)
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
                .GetFromJsonAsync<IEnumerable<Guid>>(requestUrl);
        }
    }
}
