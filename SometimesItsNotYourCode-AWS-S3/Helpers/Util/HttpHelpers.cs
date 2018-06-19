using System;
using System.Collections.Generic;
using System.IO;
using System.Net;

namespace SometimesItsNotYourCode_AWS_S3.Helpers.Util
{
    public static class HttpHelpers
    {
        public static void InvokeHttpRequest(Uri endpointUri, string httpMethod, Dictionary<string, string> headers, Stream objectToSave)
        {
            try
            {
                var request = WebRequest.CreateHttp(endpointUri);
                request.Method = httpMethod;
                SetHeaders(request, headers);
                request.AllowWriteStreamBuffering = false;

                using (var requestStream = request.GetRequestStream())
                using (objectToSave)
                {
                    objectToSave.CopyTo(requestStream, objectToSave.Length > 81920 ? 81920 : (int)objectToSave.Length);
                }

                using (request.GetResponse())
                {
                    
                }
            }
            catch (Exception exception)
            {
                Console.WriteLine(exception);
            }
        }

        private static void SetHeaders(HttpWebRequest request, Dictionary<string, string> headers)
        {
            foreach (var header in headers.Keys)
            {
                // not all headers can be set via the dictionary
                if (header.Equals("host", StringComparison.OrdinalIgnoreCase))
                {
                    request.Host = headers[header];
                }
                else if (header.Equals("content-length", StringComparison.OrdinalIgnoreCase))
                {
                    request.ContentLength = long.Parse(headers[header]);
                }
                else if (header.Equals("content-type", StringComparison.OrdinalIgnoreCase))
                {
                    request.ContentType = headers[header];
                }
                else
                {
                    request.Headers.Add(header, headers[header]);
                }
            }
        }
    }
}
