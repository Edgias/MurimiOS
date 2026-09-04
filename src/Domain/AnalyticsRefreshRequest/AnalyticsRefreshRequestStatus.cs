using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using EdgIQ.MurimiOS.Domain.Common;



namespace EdgIQ.MurimiOS.Domain.AnalyticsRefreshRequest;
public enum AnalyticsRefreshRequestStatus { Pending = 1, Scheduled = 2, Ignored = 3, Failed = 4 }
