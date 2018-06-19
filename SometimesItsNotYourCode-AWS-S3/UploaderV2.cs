using System;
using System.IO;
using System.Net;
using Amazon;
using Amazon.Runtime;
using Amazon.S3;
using Amazon.S3.Model;
using Amazon.Util;

namespace SometimesItsNotYourCode_AWS_S3
{
    /// <summary>
    /// Total Memory Allocaitons: 17 MB     SOH: 17 MB      LOH: 0.4 MB
    /// </summary>
    public class UploaderV2 : IUploader
    {
        private readonly AWSCredentials _credentials;
        private readonly RegionEndpoint _region;
        private readonly string _bucketName;

        public UploaderV2(AWSCredentials credentials, RegionEndpoint region, string bucketName)
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
                    var request = new GetPreSignedUrlRequest
                    {
                        BucketName = _bucketName,
                        Verb = HttpVerb.PUT,
                        Expires = DateTime.Now.AddMinutes(5),
                        ServerSideEncryptionMethod = ServerSideEncryptionMethod.None,
                        Key = "v2/" + iteration + ".png",
                        Headers = { [HeaderKeys.XAmzAclHeader] = S3CannedACL.PublicRead.Value }
                    };

                    var preSignedUrl = client.GetPreSignedURL(request);
                    var webRequest = WebRequest.CreateHttp(preSignedUrl);
                    webRequest.Method = WebRequestMethods.Http.Put;
                    webRequest.ContentType = "image/*";
                    webRequest.AllowWriteStreamBuffering = false;
                    webRequest.ContentLength = objectToUpload.Length;
                    webRequest.Headers[HeaderKeys.XAmzAclHeader] = S3CannedACL.PublicRead.Value;

                    using (var dataStream = webRequest.GetRequestStream())
                    using (objectToUpload)
                    {
                        objectToUpload.CopyTo(dataStream, objectToUpload.Length > 81920 ? 81920 : (int)objectToUpload.Length);
                    }

                    using (webRequest.GetResponse())
                    {

                    }
                }
            }
            catch (Exception exception)
            {
                Console.WriteLine(exception);
            }
        }
    }
}