using System.Diagnostics;
using System.Security.Cryptography;
using System.Text.Json;
using EdgIQ.MurimiOS.Domain.EquipmentAsset;

using Microsoft.EntityFrameworkCore;

namespace EdgIQ.MurimiOS.API.Endpoints.ConnectedAgriculture.CreateEquipment;
using EquipmentType = global::EdgIQ.MurimiOS.Domain.EquipmentAsset.EquipmentType;

public sealed record CreateEquipmentRequest(Guid? Id, string Code, string Name, EquipmentType Type, string? Manufacturer, string? Model, string? SerialNumber, int? ModelYear);
