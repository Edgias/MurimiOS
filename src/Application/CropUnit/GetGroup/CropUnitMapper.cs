namespace Edgias.MurimiOS.API.CropUnits
{
    public static class CropUnitMapper
    {
        public static CropUnitResponse AsApiResponse(this CropUnit entity)
        {
            CropUnitResponse response = new()
            {
                Id = entity.Id,
                Name = entity.Name
            };

            return response;
        }

        public static CropUnit ToEntity(this CropUnitRequest request)
        {
            CropUnit entity = new(request.Name);

            return entity;
        }

        public static void Update(this CropUnit entity, CropUnitRequest request)
        {
            entity.Update(request.Name);
        }
    }
}
