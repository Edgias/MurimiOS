using EdgIQ.MurimiOS.Domain.Common;



namespace EdgIQ.MurimiOS.Domain.DerivedMediaAsset;

public enum DerivedMediaLifecycleStatus
{
    Active = 1,
    PendingDeletion = 2,
    Deleted = 3,
    DeletionFailed = 4
}
