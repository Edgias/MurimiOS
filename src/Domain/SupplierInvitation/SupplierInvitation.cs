using EdgIQ.MurimiOS.Domain.Common;



namespace EdgIQ.MurimiOS.Domain.SupplierInvitation;

public sealed class SupplierInvitation : TenantEntity
{
    private SupplierInvitation() { }
    public SupplierInvitation(Guid id, string supplierName, string email, string tokenHash, DateTimeOffset expiresAt,
        string invitedBy, DateTimeOffset invitedAt) : base(id)
    {
        if (expiresAt <= invitedAt) throw new ArgumentOutOfRangeException(nameof(expiresAt));
        SupplierName = Text(supplierName, 200); Email = Text(email, 320).ToLowerInvariant(); TokenHash = Hash(tokenHash);
        ExpiresAt = expiresAt.ToUniversalTime(); InvitedBy = Text(invitedBy, 240); InvitedAt = invitedAt.ToUniversalTime(); Status = SupplierInvitationStatus.Pending;
    }
    public string SupplierName { get; private set; } = string.Empty;
    public string Email { get; private set; } = string.Empty;
    public string TokenHash { get; private set; } = string.Empty;
    public DateTimeOffset ExpiresAt { get; private set; }
    public string InvitedBy { get; private set; } = string.Empty;
    public DateTimeOffset InvitedAt { get; private set; }
    public SupplierInvitationStatus Status { get; private set; }
    public Guid? HostedSupplierId { get; private set; }
    public DateTimeOffset? RespondedAt { get; private set; }
    public void Accept(Guid hostedSupplierId, DateTimeOffset at) { EnsurePending(at); if (hostedSupplierId == Guid.Empty) throw new ArgumentException("A hosted supplier is required."); Status = SupplierInvitationStatus.Accepted; HostedSupplierId = hostedSupplierId; RespondedAt = at.ToUniversalTime(); }
    public void Revoke(DateTimeOffset at) { if (Status != SupplierInvitationStatus.Pending) throw new InvalidOperationException("Only a pending invitation can be revoked."); Status = SupplierInvitationStatus.Revoked; RespondedAt = at.ToUniversalTime(); }
    public void Expire(DateTimeOffset at) { if (Status != SupplierInvitationStatus.Pending) throw new InvalidOperationException("The invitation is no longer pending."); if (at < ExpiresAt) throw new InvalidOperationException("The invitation has not expired."); Status = SupplierInvitationStatus.Expired; RespondedAt = at.ToUniversalTime(); }
    private void EnsurePending(DateTimeOffset at) { if (Status != SupplierInvitationStatus.Pending) throw new InvalidOperationException("The invitation is no longer pending."); if (at >= ExpiresAt) throw new InvalidOperationException("The invitation has expired."); }
    internal static string Text(string? value, int max) { string result = value?.Trim() ?? string.Empty; if (result.Length is 0 || result.Length > max) throw new ArgumentException($"Text must contain between 1 and {max} characters."); return result; }
    internal static string? Optional(string? value, int max) { string? result = value?.Trim(); if (result?.Length > max) throw new ArgumentException($"Text cannot exceed {max} characters."); return string.IsNullOrEmpty(result) ? null : result; }
    private static string Hash(string? value) { string result = value?.Trim().ToLowerInvariant() ?? string.Empty; if (result.Length != 64 || result.Any(c => !Uri.IsHexDigit(c))) throw new ArgumentException("A SHA-256 token hash is required."); return result; }
}
