using System.Security.Cryptography;
using System.Text;
using EdgIQ.MurimiOS.Domain.TenantMembership;
using EdgIQ.MurimiOS.Domain.Common;


using EdgIQ.MurimiOS.Domain.TenantRegionalSettings;
using EdgIQ.MurimiOS.Domain.ReferenceCatalogue;
using EdgIQ.MurimiOS.Domain.FarmField;
using EdgIQ.MurimiOS.Domain.Farm;
using EdgIQ.MurimiOS.Domain.LandTenureRecord;
using EdgIQ.MurimiOS.Domain.SiteInfrastructureAsset;
using EdgIQ.MurimiOS.Domain.SoilZone;
using EdgIQ.MurimiOS.Domain.SoilSample;

using EdgIQ.MurimiOS.Domain.VeterinaryMedicineStockLot;
using EdgIQ.MurimiOS.Domain.VeterinaryMedicine;
using EdgIQ.MurimiOS.Domain.LivestockTreatment;
using EdgIQ.MurimiOS.Domain.LivestockSubject;
using EdgIQ.MurimiOS.Domain.LivestockFeedBatch;

using EdgIQ.MurimiOS.Domain.LivestockRegulatoryProfile;
using EdgIQ.MurimiOS.Domain.LivestockExchangeSubmission;
using EdgIQ.MurimiOS.Domain.LivestockReproductionEvent;
using EdgIQ.MurimiOS.Domain.LivestockMilkRecord;
using EdgIQ.MurimiOS.Domain.LivestockGrazingPlan;
using EdgIQ.MurimiOS.Domain.LivestockRationPlan;
using EdgIQ.MurimiOS.Domain.LivestockHolding;
using EdgIQ.MurimiOS.Domain.LivestockStatutoryMovementDocument;
using EdgIQ.MurimiOS.Domain.CropFeedTransfer;
using EdgIQ.MurimiOS.Domain.LivestockMeasurementRecord;
using EdgIQ.MurimiOS.Domain.LivestockDispatch;
using EdgIQ.MurimiOS.Domain.NotificationRecord;
using EdgIQ.MurimiOS.Domain.NotificationPreference;
using EdgIQ.MurimiOS.Domain.OrganisationNode;
using EdgIQ.MurimiOS.Domain.HarvestLot;
using EdgIQ.MurimiOS.Domain.GrowingSeason;
using EdgIQ.MurimiOS.Domain.CropZone;
using EdgIQ.MurimiOS.Application.Common.Abstractions;
using EdgIQ.MurimiOS.Infrastructure.Data;
using EdgIQ.MurimiOS.Infrastructure.Data.Interceptors;

using Microsoft.EntityFrameworkCore;
using NetTopologySuite.Geometries;

SeedOptions seedOptions;
try
{
    seedOptions = SeedOptions.Parse(args);
}
catch (ArgumentException exception)
{
    Console.Error.WriteLine(exception.Message);
    Console.Error.WriteLine(SeedOptions.Usage);
    return 2;
}

if (seedOptions.ShowHelp)
{
    Console.WriteLine(SeedOptions.Usage);
    return 0;
}

(string Crop, string Variety)[] crops =
[
    ("Maize", "SC 727"),
    ("Soybean", "Pan 1867"),
    ("Wheat", "Dande"),
    ("Groundnut", "Natal Common"),
    ("Sorghum", "Macia")
];

for (int tenantNumber = 1; tenantNumber <= seedOptions.TenantCount; tenantNumber++)
{
    Guid tenantId = StableGuid($"tenant:{seedOptions.Seed}:{tenantNumber}");
    var tenantContext = new SeedTenantContext(tenantId);
    var interceptor = new TenantConnectionInterceptor(tenantContext);
    var dbOptions = new DbContextOptionsBuilder<PlatformDbContext>()
        .UseNpgsql(seedOptions.ConnectionString, npgsql => npgsql.UseNetTopologySuite())
        .UseSnakeCaseNamingConvention()
        .AddInterceptors(interceptor)
        .Options;

    await using var db = new PlatformDbContext(dbOptions, tenantContext);
    await db.Database.OpenConnectionAsync();
    await using var transaction = await db.Database.BeginTransactionAsync();

    Guid rootNodeId = StableGuid($"org-root:{tenantId}");
    Guid ownerMembershipId = StableGuid($"membership-owner:{tenantId}");

    await AddIfMissingAsync(
        db.OrganisationNodes,
        rootNodeId,
        () => new OrganisationNode(
            rootNodeId,
            $"Murimi Demonstration Holding {tenantNumber:00}",
            OrganisationNodeType.Organisation,
            $"seed_t{tenantNumber:00}"));

    await AddIfMissingAsync(
        db.TenantMemberships,
        ownerMembershipId,
        () => new TenantMembership(
            ownerMembershipId,
            $"seed-owner-{tenantNumber:00}",
            $"Seed Owner {tenantNumber:00}",
            $"owner{tenantNumber:00}@seed.murimios.local",
            TenantRole.Owner,
            MembershipStatus.Active,
            rootNodeId));

    Guid regionalSettingsId = StableGuid($"regional-settings:{tenantId}");
    await AddIfMissingAsync(
        db.TenantRegionalSettings,
        regionalSettingsId,
        () => new TenantRegionalSettings(
            regionalSettingsId,
            "ZW",
            "USD",
            "en-ZW",
            "Africa/Harare",
            8,
            "ha",
            "kg",
            "L",
            "C"));

    Guid cropsCatalogueId = StableGuid($"catalogue:{tenantId}:crops:2026.1");
    await AddIfMissingAsync(
        db.ReferenceCatalogues,
        cropsCatalogueId,
        () => new ReferenceCatalogue(
            cropsCatalogueId,
            "crops",
            "Supported crops",
            "2026.1",
            "ZW",
            "MurimiOS demonstration data",
            new DateOnly(2026, 1, 1)));
    foreach ((string code, string name, string category) in new[]
    {
        ("MAIZE", "Maize", "cereal"),
        ("SOYBEAN", "Soybean", "legume"),
        ("WHEAT", "Wheat", "cereal"),
        ("GROUNDNUT", "Groundnut", "legume"),
        ("SORGHUM", "Sorghum", "cereal")
    })
    {
        Guid itemId = StableGuid($"catalogue-item:{cropsCatalogueId}:{code}");
        await AddIfMissingAsync(
            db.ReferenceCatalogueItems,
            itemId,
            () => new ReferenceCatalogueItem(
                itemId,
                cropsCatalogueId,
                code,
                name,
                null,
                $"{{\"category\":\"{category}\"}}",
                new DateOnly(2026, 1, 1)));
    }

    Guid speciesCatalogueId = StableGuid($"catalogue:{tenantId}:livestock-species:2026.1");
    await AddIfMissingAsync(
        db.ReferenceCatalogues,
        speciesCatalogueId,
        () => new ReferenceCatalogue(
            speciesCatalogueId,
            "livestock-species",
            "Livestock species",
            "2026.1",
            "ZW",
            "MurimiOS demonstration data",
            new DateOnly(2026, 1, 1)));
    Guid cattleItemId = StableGuid($"catalogue-item:{speciesCatalogueId}:CATTLE");
    await AddIfMissingAsync(
        db.ReferenceCatalogueItems,
        cattleItemId,
        () => new ReferenceCatalogueItem(
            cattleItemId,
            speciesCatalogueId,
            "CATTLE",
            "Cattle",
            "Bovine individual animals and managed groups.",
            "{\"scientificName\":\"Bos taurus\"}",
            new DateOnly(2026, 1, 1)));

    for (int farmNumber = 1; farmNumber <= seedOptions.FarmsPerTenant; farmNumber++)
    {
        Guid farmNodeId = StableGuid($"org-farm:{tenantId}:{farmNumber}");
        Guid farmId = StableGuid($"farm:{tenantId}:{farmNumber}");
        Guid seasonId = StableGuid($"season:{farmId}:2026-2027");

        await AddIfMissingAsync(
            db.OrganisationNodes,
            farmNodeId,
            () => new OrganisationNode(
                farmNodeId,
                $"Seed Farm {tenantNumber:00}-{farmNumber:00}",
                OrganisationNodeType.Farm,
                $"seed_t{tenantNumber:00}.farm_{farmNumber:00}",
                rootNodeId));

        await AddIfMissingAsync(
            db.Farms,
            farmId,
            () => new Farm(
                farmId,
                farmNodeId,
                $"Seed Farm {tenantNumber:00}-{farmNumber:00}",
                $"T{tenantNumber:00}F{farmNumber:00}",
                "ZW",
                "Africa/Harare"));

        await AddIfMissingAsync(
            db.GrowingSeasons,
            seasonId,
            () => new GrowingSeason(
                seasonId,
                farmId,
                "2026/27 Summer",
                new DateOnly(2026, 8, 1),
                new DateOnly(2027, 7, 31),
                GrowingSeasonStatus.Active));

        for (int fieldNumber = 1; fieldNumber <= seedOptions.FieldsPerFarm; fieldNumber++)
        {
            Guid fieldId = StableGuid($"field:{farmId}:{fieldNumber}");
            Guid boundaryId = StableGuid($"boundary:{fieldId}:1");
            Guid cropZoneId = StableGuid($"crop-zone:{seasonId}:{fieldId}");
            MultiPolygon boundary = CreateFieldBoundary(tenantNumber, farmNumber, fieldNumber);
            (string Crop, string Variety) crop = crops[(fieldNumber - 1) % crops.Length];

            await AddIfMissingAsync(
                db.FarmFields,
                fieldId,
                () => new FarmField(fieldId, farmId, $"Field {fieldNumber:00}"));

            await AddIfMissingAsync(
                db.FieldBoundaryVersions,
                boundaryId,
                () => new FieldBoundaryVersion(
                    boundaryId,
                    fieldId,
                    boundary,
                    new DateOnly(2026, 8, 1),
                    "seed-generator"));

            await AddIfMissingAsync(
                db.CropZones,
                cropZoneId,
                () => new CropZone(
                    cropZoneId,
                    seasonId,
                    fieldId,
                    boundaryId,
                    $"{crop.Crop} {fieldNumber:00}",
                    crop.Crop,
                    crop.Variety,
                    boundary.Copy() as MultiPolygon
                        ?? throw new InvalidOperationException("The crop-zone boundary could not be copied.")));
        }

        for (int animalNumber = 1; animalNumber <= seedOptions.AnimalsPerFarm; animalNumber++)
        {
            Guid subjectId = StableGuid($"livestock:{farmId}:{animalNumber}");
            bool isGroup = animalNumber % 10 == 0;
            await AddIfMissingAsync(
                db.LivestockSubjects,
                subjectId,
                () => new LivestockSubject(
                    subjectId,
                    farmId,
                    isGroup ? LivestockSubjectKind.Group : LivestockSubjectKind.Individual,
                    isGroup ? $"Weaner Group {animalNumber / 10:00}" : $"Cow {animalNumber:000}",
                    "cattle",
                    isGroup ? null : $"ZW-{tenantNumber:00}-{farmNumber:00}-{animalNumber:0000}"));
        }
    }

    Guid welcomeNotificationId = StableGuid($"notification:welcome:{tenantId}");
    await AddIfMissingAsync(
        db.Notifications,
        welcomeNotificationId,
        () => new NotificationRecord(
            welcomeNotificationId,
            ownerMembershipId,
            "platform",
            "Your demonstration holding is ready",
            "Farms, mapped fields, crop zones and livestock records have been generated for backend evaluation.",
            NotificationSeverity.Information,
            "organisation",
            rootNodeId,
            "/farms"));

    int changes = await db.SaveChangesAsync();
    await transaction.CommitAsync();
    Console.WriteLine($"Tenant {tenantNumber:00}: {tenantId} ({changes} database changes)");
}

Console.WriteLine("Seed generation completed. Running the same command again is safe and does not duplicate records.");
return 0;

static async Task AddIfMissingAsync<TEntity>(DbSet<TEntity> set, Guid id, Func<TEntity> factory)
    where TEntity : TenantEntity
{
    if (!await set.AnyAsync(x => x.Id == id))
    {
        set.Add(factory());
    }
}

static Guid StableGuid(string value)
{
    byte[] bytes = SHA256.HashData(Encoding.UTF8.GetBytes(value));
    Span<byte> guidBytes = bytes.AsSpan(0, 16);
    guidBytes[7] = (byte)((guidBytes[7] & 0x0f) | 0x40);
    guidBytes[8] = (byte)((guidBytes[8] & 0x3f) | 0x80);
    return new Guid(guidBytes);
}

static MultiPolygon CreateFieldBoundary(int tenantNumber, int farmNumber, int fieldNumber)
{
    const double width = 0.0038;
    const double height = 0.0030;
    double west = 30.95 + tenantNumber * 0.08 + farmNumber * 0.018 + fieldNumber * 0.0045;
    double south = -18.25 + tenantNumber * 0.04 + farmNumber * 0.012 + fieldNumber * 0.0035;
    var factory = new GeometryFactory(new PrecisionModel(), 4326);
    LinearRing shell = factory.CreateLinearRing(
    [
        new Coordinate(west, south),
        new Coordinate(west + width, south),
        new Coordinate(west + width, south + height),
        new Coordinate(west, south + height),
        new Coordinate(west, south)
    ]);
    return factory.CreateMultiPolygon([factory.CreatePolygon(shell)]);
}

file sealed class SeedTenantContext(Guid tenantId) : ITenantContext
{
    public Guid TenantId { get; } = tenantId;
    public bool IsResolved => true;
}

file sealed record SeedOptions(
    string ConnectionString,
    int TenantCount,
    int FarmsPerTenant,
    int FieldsPerFarm,
    int AnimalsPerFarm,
    string Seed,
    bool ShowHelp)
{
    public const string Usage = """
        Generate deterministic, tenant-isolated MurimiOS demonstration data.

        dotnet run --project tools/SeedData -- [options]

          --connection <value>         PostgreSQL connection string. Defaults to MURIMIOS_SEED_DB,
                                       then the local development migration-owner connection.
          --tenants <1-20>             Tenants to create (default: 2).
          --farms-per-tenant <1-20>    Farms per tenant (default: 2).
          --fields-per-farm <1-100>    Mapped fields and crop zones per farm (default: 5).
          --animals-per-farm <0-1000>  Livestock subjects per farm (default: 20).
          --seed <value>               Stable identity namespace (default: murimios-demo-v1).
          --help                       Show this help.

        The generator is additive and idempotent. It never truncates or deletes data.
        """;

    public static SeedOptions Parse(string[] args)
    {
        string connection = Environment.GetEnvironmentVariable("MURIMIOS_SEED_DB")
            ?? "Host=localhost;Port=55432;Database=murimios;Username=murimios;Password=murimios-dev";
        int tenants = 2;
        int farms = 2;
        int fields = 5;
        int animals = 20;
        string seed = "murimios-demo-v1";
        bool help = false;

        for (int index = 0; index < args.Length; index++)
        {
            string argument = args[index];
            if (argument is "--help" or "-h")
            {
                help = true;
                continue;
            }

            if (index + 1 >= args.Length) throw new ArgumentException($"A value is required for {argument}.");
            string value = args[++index];
            switch (argument)
            {
                case "--connection": connection = Require(value, argument); break;
                case "--tenants": tenants = ParseRange(value, argument, 1, 20); break;
                case "--farms-per-tenant": farms = ParseRange(value, argument, 1, 20); break;
                case "--fields-per-farm": fields = ParseRange(value, argument, 1, 100); break;
                case "--animals-per-farm": animals = ParseRange(value, argument, 0, 1_000); break;
                case "--seed": seed = Require(value, argument); break;
                default: throw new ArgumentException($"Unknown option: {argument}");
            }
        }

        return new SeedOptions(connection, tenants, farms, fields, animals, seed, help);
    }

    private static int ParseRange(string value, string argument, int minimum, int maximum)
    {
        if (!int.TryParse(value, out int parsed) || parsed < minimum || parsed > maximum)
            throw new ArgumentException($"{argument} must be an integer from {minimum} through {maximum}.");
        return parsed;
    }

    private static string Require(string value, string argument) =>
        !string.IsNullOrWhiteSpace(value)
            ? value.Trim()
            : throw new ArgumentException($"{argument} cannot be empty.");
}
