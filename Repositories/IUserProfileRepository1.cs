using Streamish.Models;
using System.Collections.Generic;

namespace Streamish.Repositories
{
    internal interface IUserProfileRepository
    {
        public List<UserProfile> GetAll();
        public UserProfile GetById(int id);
        public UserProfile GetUserByIdWithVideosAndComments(int id);
        public void Add(UserProfile user);
        public void Update(UserProfile user);
        public void Delete(int Id);
    }
}