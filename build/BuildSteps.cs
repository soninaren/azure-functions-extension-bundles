using Colors.Net;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Build
{
    public static class BuildSteps
    {
        public static void Clean()
        {
            Directory.Delete(Settings.OutputDir, recursive: true);
        }

        public static void RestorePackages(string[] feeds)
        {
            feeds.Aggregate(string.Empty, (a, b) => $"{a} --source {b}");
            Shell.Run("dotnet", $"restore {Settings.ProjectFile} {feeds}");
        }

        public static void AddPackages()
        {
            var feeds = Settings.nugetFeed;
            feeds.Aggregate(string.Empty, (a, b) => $"{a} --source {b}");

            var extensions = GetExtensionList();
            foreach (var extension in extensions)
            {
                //                ColoredConsole.Out.WriteLine($"installing extension {extension.Id}:{extension.Version}");
                Shell.Run("dotnet", $"add {Settings.ProjectFile} {feeds} package {extension.Id} -v {extension.Version} -n");
            }
        }

        private static List<Extensions> GetExtensionList()
        {
            var extensionsJsonFileContent = FileUtility.ReadAllText(Settings.ExtensionsJsonFile);
            return JsonConvert.DeserializeObject<List<Extensions>>(extensionsJsonFileContent);
        }


        //public static async Task UploadZip()
        //{
        //    var storageConnection = Settings.SignInfo.AzureSigningConnectionString;
        //    var storageAccount = CloudStorageAccount.Parse(storageConnection);
        //    var blobClient = storageAccount.CreateCloudBlobClient();
        //    var blobContainer = blobClient.GetContainerReference(Settings.SignInfo.AzureToSignContainerName);
        //    await blobContainer.CreateIfNotExistsAsync();
        //    foreach (var supportedRuntime in Settings.SignInfo.RuntimesToSign)
        //    {
        //        var targetDir = Path.Combine(Settings.OutputDir, supportedRuntime);

        //        var toSignBlob = blobContainer.GetBlockBlobReference($"{Settings.SignInfo.ToSignZipName}-{supportedRuntime}");
        //        await toSignBlob.UploadFromFileAsync(Path.Combine(targetDir, Settings.SignInfo.ToSignDir, Settings.SignInfo.ToSignZipName));

        //        var toSignThirdPartyBlob = blobContainer.GetBlockBlobReference($"{Settings.SignInfo.ToSignThirdPartyName}-{supportedRuntime}");
        //        await toSignThirdPartyBlob.UploadFromFileAsync(Path.Combine(targetDir, Settings.SignInfo.ToSignDir, Settings.SignInfo.ToSignThirdPartyName));
        //    }
        //}

        //public static void UploadToStorage()
        //{
        //    if (!string.IsNullOrEmpty(Settings.BuildArtifactsStorage))
        //    {
        //        var version = new Version(CurrentVersion);
        //        var storageAccount = CloudStorageAccount.Parse(Settings.BuildArtifactsStorage);
        //        var blobClient = storageAccount.CreateCloudBlobClient();
        //        var container = blobClient.GetContainerReference("builds");
        //        container.CreateIfNotExistsAsync().Wait();

        //        container.SetPermissionsAsync(new BlobContainerPermissions
        //        {
        //            PublicAccess = BlobContainerPublicAccessType.Blob
        //        });

        //        foreach (var file in Directory.GetFiles(Settings.OutputDir, "Azure.Functions.Cli.*", SearchOption.TopDirectoryOnly))
        //        {
        //            var fileName = Path.GetFileName(file);
        //            ColoredConsole.Write($"Uploading {fileName}...");

        //            var versionedBlob = container.GetBlockBlobReference($"{version.ToString()}/{fileName}");
        //            var latestBlob = container.GetBlockBlobReference($"{version.Major}/latest/{fileName.Replace($".{version.ToString()}", string.Empty)}");
        //            versionedBlob.UploadFromFileAsync(file).Wait();
        //            latestBlob.StartCopyAsync(versionedBlob).Wait();

        //            ColoredConsole.WriteLine("Done");
        //        }

        //        var latestVersionBlob = container.GetBlockBlobReference($"{version.Major}/latest/version.txt");
        //        latestVersionBlob.UploadTextAsync(version.ToString()).Wait();
        //    }
        //    else
        //    {
        //        var error = $"{nameof(Settings.BuildArtifactsStorage)} is null or empty. Can't run {nameof(UploadToStorage)} target";
        //        ColoredConsole.Error.WriteLine(error.Red());
        //        throw new Exception(error);
        //    }
        //}

        //public static void Zip()
        //{
        //    var version = CurrentVersion;
        //    foreach (var runtime in Settings.TargetRuntimes)
        //    {
        //        var path = Path.Combine(Settings.OutputDir, runtime);

        //        var zipPath = Path.Combine(Settings.OutputDir, $"Azure.Functions.Cli.{runtime}.{version}.zip");
        //        ColoredConsole.WriteLine($"Creating {zipPath}");
        //        ZipFile.CreateFromDirectory(path, zipPath, CompressionLevel.Optimal, includeBaseDirectory: false);

        //        var shaPath = $"{zipPath}.sha2";
        //        ColoredConsole.WriteLine($"Creating {shaPath}");
        //        File.WriteAllText(shaPath, ComputeSha256(zipPath));

        //        try
        //        {
        //            Directory.Delete(path, recursive: true);
        //        }
        //        catch
        //        {
        //            ColoredConsole.Error.WriteLine($"Error deleting {path}");
        //        }

        //        ColoredConsole.WriteLine();
        //    }

        //    string ComputeSha256(string file)
        //    {
        //        using (var fileStream = File.OpenRead(file))
        //        {
        //            var sha1 = new SHA256Managed();
        //            return BitConverter.ToString(sha1.ComputeHash(fileStream)).Replace("-", string.Empty);
        //        }
        //    }
        //}
    }
}