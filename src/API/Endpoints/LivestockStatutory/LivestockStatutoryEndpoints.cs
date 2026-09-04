using System.Data;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using System.Text.RegularExpressions;
using System.Xml.Linq;
using Microsoft.EntityFrameworkCore;

namespace EdgIQ.MurimiOS.API.Endpoints.LivestockStatutory;
using CreateCropFeedTransferEndpoint = global::EdgIQ.MurimiOS.API.Endpoints.LivestockStatutory.CreateCropFeedTransfer.CreateCropFeedTransferEndpoint;
using CreateDocumentEndpoint = global::EdgIQ.MurimiOS.API.Endpoints.LivestockStatutory.CreateDocument.CreateDocumentEndpoint;
using GetDocumentEndpoint = global::EdgIQ.MurimiOS.API.Endpoints.LivestockStatutory.GetDocument.GetDocumentEndpoint;
using GetDocumentArtifactEndpoint = global::EdgIQ.MurimiOS.API.Endpoints.LivestockStatutory.GetDocumentArtifact.GetDocumentArtifactEndpoint;
using ListCropFeedTransfersEndpoint = global::EdgIQ.MurimiOS.API.Endpoints.LivestockStatutory.ListCropFeedTransfers.ListCropFeedTransfersEndpoint;
using ListDocumentsEndpoint = global::EdgIQ.MurimiOS.API.Endpoints.LivestockStatutory.ListDocuments.ListDocumentsEndpoint;
using SubmitDocumentEndpoint = global::EdgIQ.MurimiOS.API.Endpoints.LivestockStatutory.SubmitDocument.SubmitDocumentEndpoint;


public static partial class LivestockStatutoryEndpoints
{
    private const string DefaultSchemaVersion = "1.7.9";

    public static void MapLivestockStatutoryEndpoints(RouteGroupBuilder api)
    {
        global::EdgIQ.MurimiOS.API.Endpoints.LivestockStatutory.CreateDocument.CreateDocumentEndpoint.Map(api);
        global::EdgIQ.MurimiOS.API.Endpoints.LivestockStatutory.ListDocuments.ListDocumentsEndpoint.Map(api);
        global::EdgIQ.MurimiOS.API.Endpoints.LivestockStatutory.GetDocument.GetDocumentEndpoint.Map(api);
        global::EdgIQ.MurimiOS.API.Endpoints.LivestockStatutory.GetDocumentArtifact.GetDocumentArtifactEndpoint.Map(api);
        global::EdgIQ.MurimiOS.API.Endpoints.LivestockStatutory.SubmitDocument.SubmitDocumentEndpoint.Map(api);
        global::EdgIQ.MurimiOS.API.Endpoints.LivestockStatutory.CreateCropFeedTransfer.CreateCropFeedTransferEndpoint.Map(api);
        global::EdgIQ.MurimiOS.API.Endpoints.LivestockStatutory.ListCropFeedTransfers.ListCropFeedTransfersEndpoint.Map(api);
    }
}
