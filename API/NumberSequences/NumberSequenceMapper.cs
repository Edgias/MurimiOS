namespace Edgias.MurimiOS.API.NumberSequences;

public static class NumberSequenceMapper
{
    public static NumberSequenceResponse AsApiResponse(this NumberSequence entity)
    {
        NumberSequenceResponse response = new()
        {
            Id = entity.Id,
            Entity = entity.Entity,
            Prefix = entity.Prefix,
            Seperator = entity.Seperator,
            Suffix = entity.Suffix,
            StartingNumber = entity.StartingNumber
        };

        return response;
    }

    public static NumberSequence ToEntity(this NumberSequenceRequest request)
    {
        NumberSequence entity = new(request.Entity, request.Prefix, request.Seperator, request.StartingNumber, request.Suffix);

        return entity;
    }

    public static void Update(this  NumberSequence entity, NumberSequenceRequest request) 
    {
        entity.Update(request.Prefix, request.Seperator, request.Suffix);
    }
}
