using System.Buffers.Binary;
using System.Net.Sockets;
using System.Text;
using Microsoft.Extensions.Logging;

namespace EdgIQ.MurimiOS.Application.Common.Abstractions;

public sealed record MediaScanResult(MediaScanVerdict Verdict, string? Detail = null);
