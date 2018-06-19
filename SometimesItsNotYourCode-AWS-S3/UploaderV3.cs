using System;
using System.IO;
using Amazon;
using Amazon.Runtime;
using SometimesItsNotYourCode_AWS_S3.Helpers;

namespace SometimesItsNotYourCode_AWS_S3
{
    public class UploaderV3 : IUploader
    {
        private readonly AWSCredentials _credentials;
        private readonly RegionEndpoint _region;
        private readonly string _bucketName;

        public UploaderV3(AWSCredentials credentials, RegionEndpoint region, string bucketName)
        {
            _credentials = credentials;
            _region = region;
            _bucketName = bucketName;
        }

        public void Upload(Stream objectToUpload, int iteration)
        {
            try
            {
                PutS3ObjectSample.Run("v3/" + iteration + ".png", objectToUpload, _region, _bucketName, _credentials.GetCredentials().AccessKey, _credentials.GetCredentials().SecretKey);
            }
            catch (Exception exception)
            {
                Console.WriteLine(exception);
            }
        }
    }
}