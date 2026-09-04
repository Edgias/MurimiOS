using System.Text.Json;
using EdgIQ.MurimiOS.Domain.Common;



namespace EdgIQ.MurimiOS.Domain.LivestockExchangeSubmission;

public enum LivestockExchangeSubmissionStatus
{
    Ready = 1,
    Submitted = 2,
    Accepted = 3,
    Rejected = 4
}
