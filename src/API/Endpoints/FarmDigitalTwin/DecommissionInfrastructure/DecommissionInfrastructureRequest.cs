using System.Diagnostics;
using System.Text.Json;

using Microsoft.EntityFrameworkCore;
using NetTopologySuite;
using NetTopologySuite.Geometries;

namespace EdgIQ.MurimiOS.API.Endpoints.FarmDigitalTwin.DecommissionInfrastructure;
public sealed record DecommissionInfrastructureRequest(DateOnly On, string Reason);
