namespace Murimi.API.Interfaces
{
    public interface IMapper<TEntity, TRequestApiModel, TApiModel>
    {
        TEntity Map(TRequestApiModel apiModel);

        TApiModel Map(TEntity entity);

        void Map(TEntity entity, TRequestApiModel apiModel);
    }
}

