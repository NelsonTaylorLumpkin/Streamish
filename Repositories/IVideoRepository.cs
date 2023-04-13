using Streamish.Models;
using System.Collections.Generic;

namespace Streamish.Repositories
{
    public interface IVideoRepository
    {
        void Add(Video video);
        void Delete(int id);
        List<Video> GetAll();
        List<Video> GetAllWithComments();
        Video GetById(int id);
        void Update(Video video);
        public List<Video> Search(string criterion, bool sortDescending);
        Video GetVideoByIdWithComments(int id);
        //public Video GetByIdWithVideos(int Id);
    }
}