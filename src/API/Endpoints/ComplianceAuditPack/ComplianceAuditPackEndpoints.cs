using System.Diagnostics;
using System.IO.Compression;
using System.Security.Cryptography;
using System.Text.Json;
using EdgIQ.MurimiOS.API.Endpoints.Access.Create;
using EdgIQ.MurimiOS.API.Endpoints.Audit.List;
using EdgIQ.MurimiOS.API.Endpoints.Organisation.Create;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;

namespace EdgIQ.MurimiOS.API.Endpoints.ComplianceAuditPack;
using ManifestEndpoint = global::EdgIQ.MurimiOS.API.Endpoints.ComplianceAuditPack.Manifest.ManifestEndpoint;

using ListEndpoint = global::EdgIQ.MurimiOS.API.Endpoints.Audit.List.ListEndpoint;

public static class ComplianceAuditPackEndpoints
{
    public static void Map(RouteGroupBuilder farms)
    {
        global::EdgIQ.MurimiOS.API.Endpoints.ComplianceAuditPack.List.ListEndpoint.Map(farms);
        global::EdgIQ.MurimiOS.API.Endpoints.ComplianceAuditPack.Create.CreateEndpoint.Map(farms);
        global::EdgIQ.MurimiOS.API.Endpoints.ComplianceAuditPack.Get.GetEndpoint.Map(farms);
        global::EdgIQ.MurimiOS.API.Endpoints.ComplianceAuditPack.Manifest.ManifestEndpoint.Map(farms);
        global::EdgIQ.MurimiOS.API.Endpoints.ComplianceAuditPack.Verify.VerifyEndpoint.Map(farms);
        global::EdgIQ.MurimiOS.API.Endpoints.ComplianceAuditPack.Download.DownloadEndpoint.Map(farms);
    }
}
