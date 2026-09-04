using EdgIQ.MurimiOS.Infrastructure.DependencyInjection;
using EdgIQ.MurimiOS.Infrastructure.Identity;
using EdgIQ.MurimiOS.Application.Common.Abstractions;
using EdgIQ.MurimiOS.Infrastructure.Storage;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Npgsql;
using System.Text.Json;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;

namespace EdgIQ.MurimiOS.Infrastructure.DependencyInjection;
public static class MediaStorageServiceCollectionExtensions
{
    public static (string scannerProvider, ClamAvScannerOptions clamAvOptions) AddMurimiOSMediaStorage(this IServiceCollection services, IConfiguration configuration, IHostEnvironment environment)
    {
string configuredMediaRoot = configuration["Storage:MediaRoot"] ?? "App_Data/media";
string mediaRoot = Path.IsPathRooted(configuredMediaRoot)
    ? configuredMediaRoot
    : Path.Combine(environment.ContentRootPath, configuredMediaRoot);
string storageProvider = configuration["Storage:Provider"] ?? "Local";
if (storageProvider.Equals("S3", StringComparison.OrdinalIgnoreCase))
{
    var s3Options = new S3MediaStorageOptions();
    configuration.GetSection("Storage:S3").Bind(s3Options);
    s3Options.StagingRoot = Path.IsPathRooted(s3Options.StagingRoot)
        ? s3Options.StagingRoot
        : Path.Combine(environment.ContentRootPath, s3Options.StagingRoot);
    services.AddSingleton<IMediaStorage>(new S3MediaStorage(s3Options));
}
else if (storageProvider.Equals("Local", StringComparison.OrdinalIgnoreCase))
{
    services.AddSingleton<IMediaStorage>(new LocalMediaStorage(mediaRoot));
}
else
{
    throw new InvalidOperationException($"Unsupported Storage:Provider '{storageProvider}'.");
}
services.AddSingleton<IMediaStorageHealthProbe>(services =>
    (IMediaStorageHealthProbe)services.GetRequiredService<IMediaStorage>());

var clamAvOptions = new ClamAvScannerOptions();
configuration.GetSection("MediaScanning:ClamAv").Bind(clamAvOptions);
string scannerProvider = configuration["MediaScanning:Provider"] ?? "ClamAv";
if (scannerProvider.Equals("ClamAv", StringComparison.OrdinalIgnoreCase))
{
    services.AddSingleton<IMediaMalwareScanner>(new ClamAvMediaMalwareScanner(clamAvOptions));
}
else if (scannerProvider.Equals("Development", StringComparison.OrdinalIgnoreCase) && environment.IsDevelopment())
{
    services.AddSingleton<IMediaMalwareScanner, DevelopmentMediaMalwareScanner>();
}
else
{
    throw new InvalidOperationException($"Unsupported MediaScanning:Provider '{scannerProvider}'.");
}
string configuredKeyRoot = configuration["Storage:DataProtectionKeyRoot"] ?? "App_Data/keys";
string keyRoot = Path.IsPathRooted(configuredKeyRoot)
    ? configuredKeyRoot
    : Path.Combine(environment.ContentRootPath, configuredKeyRoot);
services.AddDataProtection()
    .SetApplicationName("MurimiOS")
    .PersistKeysToFileSystem(new DirectoryInfo(keyRoot));
services.AddSingleton<IntegrationCredentialProtector>();

        return (scannerProvider, clamAvOptions);
    }
}
