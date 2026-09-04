using System.Text.Json;

namespace EdgIQ.MurimiOS.API.Endpoints.Inventory.PostApiFarmsFarmIdGuidInputs;

public sealed record CreateAgriculturalInputRequest(string Name, string Category, string Unit);
