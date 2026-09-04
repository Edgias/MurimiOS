namespace EdgIQ.MurimiOS.Application.Common.Abstractions;

/// <summary>
/// Host-owned output metadata. Writes are immediate so metadata set before an
/// exception retains the same semantics as the original endpoint implementation.
/// </summary>
public interface IResponseMetadata
{
    string? this[string name] { get; set; }
}
