using System.Text.Json;
using EdgIQ.MurimiOS.Domain.Common;


using NetTopologySuite.Geometries;
using Farm = global::EdgIQ.MurimiOS.Domain.Farm.Farm;

namespace EdgIQ.MurimiOS.Domain.SoilSample;

public sealed class SoilSample : TenantEntity
{
    private SoilSample() { }
    public SoilSample(Guid id, Guid farmId, Guid fieldId, Guid fieldBoundaryVersionId, Guid? soilZoneBoundaryVersionId,
        string sampleCode, Point location, DateTimeOffset sampledAt, string sampledBy, decimal depthFromCm,
        decimal depthToCm, string? laboratoryName, string? laboratoryReference, string? notes) : base(id)
    {
        if (farmId == Guid.Empty || fieldId == Guid.Empty || fieldBoundaryVersionId == Guid.Empty
            || soilZoneBoundaryVersionId == Guid.Empty || location is null || location.IsEmpty || !location.IsValid
            || location.X is < -180 or > 180 || location.Y is < -90 or > 90 || depthFromCm < 0 || depthToCm <= depthFromCm || depthToCm > 1_000)
            throw new ArgumentException("Soil-sample scope, location, or depth is invalid.");
        FarmId = farmId; FieldId = fieldId; FieldBoundaryVersionId = fieldBoundaryVersionId;
        SoilZoneBoundaryVersionId = soilZoneBoundaryVersionId; SampleCode = DigitalTwinText.Required(sampleCode, 100).ToUpperInvariant();
        Location = location; Location.SRID = 4326; SampledAt = sampledAt.ToUniversalTime();
        SampledBy = DigitalTwinText.Required(sampledBy, 240); DepthFromCm = depthFromCm; DepthToCm = depthToCm;
        LaboratoryName = DigitalTwinText.Optional(laboratoryName, 240); LaboratoryReference = DigitalTwinText.Optional(laboratoryReference, 160);
        Notes = DigitalTwinText.Optional(notes, 2_000); Status = SoilSampleStatus.Collected;
    }
    public Guid FarmId { get; private set; }
    public Guid FieldId { get; private set; }
    public Guid FieldBoundaryVersionId { get; private set; }
    public Guid? SoilZoneBoundaryVersionId { get; private set; }
    public string SampleCode { get; private set; } = "";
    public Point Location { get; private set; } = default!;
    public DateTimeOffset SampledAt { get; private set; }
    public string SampledBy { get; private set; } = "";
    public decimal DepthFromCm { get; private set; }
    public decimal DepthToCm { get; private set; }
    public string? LaboratoryName { get; private set; }
    public string? LaboratoryReference { get; private set; }
    public string? Notes { get; private set; }
    public SoilSampleStatus Status { get; private set; }
    public DateTimeOffset? SubmittedAt { get; private set; }
    public DateTimeOffset? AnalysedAt { get; private set; }
    public DateTimeOffset? RejectedAt { get; private set; }
    public string? RejectionReason { get; private set; }
    public void Submit(DateTimeOffset at)
    {
        if (Status != SoilSampleStatus.Collected) throw new InvalidOperationException("Only a collected soil sample can be submitted.");
        Status = SoilSampleStatus.SubmittedToLaboratory; SubmittedAt = at.ToUniversalTime();
    }
    public void MarkAnalysed(DateTimeOffset at)
    {
        if (Status != SoilSampleStatus.SubmittedToLaboratory) throw new InvalidOperationException("Only a submitted soil sample can be marked analysed.");
        Status = SoilSampleStatus.Analysed; AnalysedAt = at.ToUniversalTime();
    }
    public void Reject(DateTimeOffset at, string reason)
    {
        if (Status is SoilSampleStatus.Analysed or SoilSampleStatus.Rejected) throw new InvalidOperationException("The soil sample is already closed.");
        Status = SoilSampleStatus.Rejected; RejectedAt = at.ToUniversalTime(); RejectionReason = DigitalTwinText.Required(reason, 2_000);
    }
}
