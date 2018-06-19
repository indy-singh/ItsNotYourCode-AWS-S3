using System;
using System.Collections.Generic;
using System.IO;
using Amazon;
using Amazon.Runtime;

namespace SometimesItsNotYourCode_AWS_S3
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var basicAwsCredentials = new BasicAWSCredentials(accessKey: "accessKey", secretKey: "secretKey");
            var region = RegionEndpoint.EUWest1;
            const string bucketName = "bucketName";
            const string sampleImage = @"..\..\sample_image.png";

            var dict = new Dictionary<string, Action>()
            {
                ["1"] = () =>
                {
                    Console.WriteLine("V1");
                    var uploader = new UploaderV1(basicAwsCredentials, region, bucketName);
                    Upload(uploader, sampleImage);
                },
                ["2"] = () =>
                {
                    Console.WriteLine("V2");
                    var uploader = new UploaderV2(basicAwsCredentials, region, bucketName);
                    Upload(uploader, sampleImage);
                },
                ["3"] = () =>
                {
                    Console.WriteLine("V3");
                    var uploader = new UploaderV3(basicAwsCredentials, region, bucketName);
                    Upload(uploader, sampleImage);
                },
            };

#if DEBUG
            dict["2"]();
            Environment.Exit(0);
#endif

            if (args.Length == 1 && dict.ContainsKey(args[0]))
            {
                dict[args[0]]();
            }
            else
            {
                Console.WriteLine("Incorrect parameters");
                Environment.Exit(1);
            }
        }

        private static void Upload(IUploader uploader, string filePath)
        {
            for (int iteration = 0; iteration < 100; iteration++)
            {
                Console.WriteLine(iteration);
                using (var stream = File.OpenRead(filePath))
                {
                    uploader.Upload(stream, iteration);
                }
            }
        }
    }

    public interface IUploader
    {
        void Upload(Stream objectToUpload, int iteration);
    }
}