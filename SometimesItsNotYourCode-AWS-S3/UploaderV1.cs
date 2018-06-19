using System;
using System.IO;
using Amazon;
using Amazon.Runtime;
using Amazon.S3;
using Amazon.S3.Model;

namespace SometimesItsNotYourCode_AWS_S3
{
    /// <summary>
    /// When using AWSSDK.Core version 3.3.21.18 or below and uploading the same image one hundred times:
    ///     Total Memory Allocaitons: 59 MB     SOH: 33 MB      LOH: 25 MB
    ///
    /// When using AWSSDK.Core version 3.3.21.19 or above and uploading the same image one hundred times:
    ///     Total Memory Allocaitons: 51 MB     SOH: 51 MB      LOH: 0.4 MB
    /// </summary>
    public class UploaderV1 : IUploader
    {
        private readonly AWSCredentials _credentials;
        private readonly RegionEndpoint _region;
        private readonly string _bucketName;

        public UploaderV1(AWSCredentials credentials, RegionEndpoint region, string bucketName)
        {
            _credentials = credentials;
            _region = region;
            _bucketName = bucketName;
        }

        public void Upload(Stream objectToUpload, int iteration)
        {
            try
            {
                using (var client = new AmazonS3Client(_credentials, _region))
                {
                    var putObjectRequest = new PutObjectRequest
                    {
                        BucketName = _bucketName,
                        Timeout = TimeSpan.FromSeconds(5),
                        InputStream = objectToUpload,
                        Key = "v1/" + iteration + ".png",
                        CannedACL = S3CannedACL.PublicRead,
                        StorageClass = S3StorageClass.Standard,
                        ServerSideEncryptionMethod = ServerSideEncryptionMethod.None
                    };

                    client.PutObject(putObjectRequest);
                }
            }
            catch (Exception exception)
            {
                Console.WriteLine(exception);
            }
        }
    }
}