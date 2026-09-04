using System.Text.Json;
using EdgIQ.MurimiOS.Domain.Common;



namespace EdgIQ.MurimiOS.Domain.LivestockExchangeSubmission;

public enum LivestockExchangeAttemptOutcome
{
    Submitted = 1,
    Accepted = 2,
    Rejected = 3,
    RetriableFailure = 4
}
