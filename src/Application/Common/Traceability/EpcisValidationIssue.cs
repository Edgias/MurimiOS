using System.Globalization;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using System.Text.Json.Nodes;
using System.Text.RegularExpressions;

namespace EdgIQ.MurimiOS.Application.Common.Traceability;

public sealed record EpcisValidationIssue(string Code, string Path, string Message);
