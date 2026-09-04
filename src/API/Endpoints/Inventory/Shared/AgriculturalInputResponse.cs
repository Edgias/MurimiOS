using System.Text.Json;

namespace EdgIQ.MurimiOS.API.Endpoints.Inventory.Shared;

public sealed record AgriculturalInputResponse(Guid Id, Guid FarmId, string Name, string Category, string Unit, string DisplayUnit);
