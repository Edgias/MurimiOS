using System.Buffers.Binary;
using System.Net.Sockets;
using System.Text;
using Microsoft.Extensions.Logging;

namespace EdgIQ.MurimiOS.Application.Common.Abstractions;

public enum MediaScanVerdict
{
    Clean = 1,
    ThreatDetected = 2,
    Unavailable = 3
}
