namespace Murimi.API.Interfaces
{
    public interface IMapper<TEntity, TRequestApiModel, TApiModel>
    {
        TEntity Map(TRequestApiModel request);

        TApiModel Map(TEntity entity);

        void Map(TEntity entity, TRequestApiModel request);
    }
}

