using System.Threading.Tasks;

namespace Edgias.MurimiOS.Domain.Interfaces
{
    public interface IFileSystem
    {
        Task<bool> SavePicture(string pictureName, string pictureBase64);
    }
}
