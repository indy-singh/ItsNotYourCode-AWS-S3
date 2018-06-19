using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using Amazon;
using SometimesItsNotYourCode_AWS_S3.Helpers.Signers;
using SometimesItsNotYourCode_AWS_S3.Helpers.Util;

namespace SometimesItsNotYourCode_AWS_S3.Helpers
{
    /// <summary>
    /// Sample code showing how to PUT objects to Amazon S3 with Signature V4 authorization.
    /// Code taken from https://docs.aws.amazon.com/AmazonS3/latest/API/sig-v4-examples-using-sdks.html#sig-v4-examples-using-sdk-dotnet
    /// </summary>
    public static class PutS3ObjectSample
    {
        /// <summary>
        /// Uploads content to an Amazon S3 object in a single call using Signature V4 authorization.
        /// </summary>
        public static void Run(string filePath, Stream objectToSave, RegionEndpoint region, string bucketName, string accessKey, string secretKey)
        {
            var endpointUri = $"https://{bucketName}.s3-{region.SystemName}.amazonaws.com/{filePath}";
            var uri = new Uri(endpointUri);

            var contentHash = AWS4SignerBase.CanonicalRequestHashAlgorithm.ComputeHash(objectToSave);
            objectToSave.Position = 0;
            var contentHashString = AWS4SignerBase.ToHexString(contentHash, true);

            var headers = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase)
            {
                {AWS4SignerBase.X_Amz_Content_SHA256, contentHashString},
                {AWS4SignerBase.X_Amz_Expires, TimeSpan.FromMinutes(5).TotalSeconds.ToString()},
                {AWS4SignerBase.X_Amz_Acl, "public-read"},
                {"content-length", objectToSave.Length.ToString()},
                {"content-type", "image/*"},
            };

            var signer = new AWS4SignerForAuthorizationHeader
            {
                EndpointUri = uri,
                HttpMethod = WebRequestMethods.Http.Put,
                Service = "s3",
                Region = region.SystemName
            };

            var authorization = signer.ComputeSignature(headers, "", contentHashString, accessKey, secretKey);
            headers.Add("Authorization", authorization);
            HttpHelpers.InvokeHttpRequest(uri, WebRequestMethods.Http.Put, headers, objectToSave);
        }
    }
}
