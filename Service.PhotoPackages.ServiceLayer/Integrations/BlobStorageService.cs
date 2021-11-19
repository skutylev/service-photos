using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Refit;
using Service.BlobStorage.Client;
using Service.PhotoPackages.ServiceLayer.Integrations.Contracts;

namespace Service.PhotoPackages.ServiceLayer.Integrations
{
    public class BlobStorageService : IBlobStorageService
    {
        private readonly ServiceBlobStorageClient _blobStorageClient;

        public BlobStorageService(ServiceBlobStorageClient blobStorageClient)
        {
            _blobStorageClient = blobStorageClient;
        }

        public async Task<string> UploadPhoto(string bucketId, string contentName, long sapNumber, byte[] contentData,
            string contentType,
            CancellationToken cancellationToken)
        {
            await using var stream = new MemoryStream(contentData);

            var response = await _blobStorageClient.UploadContent(Guid.Parse(bucketId), new List<StreamPart> {new(stream, contentName, contentType)},
                sapNumber.ToString(), cancellationToken: cancellationToken);

            if (!response.IsSuccessStatusCode)
                throw new ArgumentOutOfRangeException(nameof(response.StatusCode),
                    $"Сервис хранилища ответил с ошибкой {response.Error.Content}");

            return response.Content.FirstOrDefault(c => c.ContentName == contentName)?.ContentId.ToString();
        }

        public async Task<byte[]> GetPhoto(string bucketId, string contentId, CancellationToken cancellationToken)
        {
            var response = await _blobStorageClient.GetContent(Guid.Parse(bucketId), Guid.Parse(contentId), cancellationToken);
            
            if (!response.IsSuccessStatusCode)
                throw new ArgumentOutOfRangeException(nameof(response.StatusCode),
                    $"Сервис хранилища ответил с ошибкой {response.Error.Content}");
            
            return await response.Content.Content.ReadAsByteArrayAsync(cancellationToken);
        }
    }
}