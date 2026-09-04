using System.Threading.Tasks;

namespace EdgIQ.MurimiOS.Application.Common.Abstractions
{
    public interface IFileSystem
    {
        Task<bool> SavePicture(string pictureName, string pictureBase64);
    }
}
