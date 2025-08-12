using System;
using System.Collections.Generic;

namespace WebApplication2.Repositories
{
    public interface IUserRepository
    {
        IEnumerable<User> GetAll();
        User? GetById(Guid id);
        User Create(User user);
        bool Update(Guid id, User updatedUser);
        bool Delete(Guid id);
    }
}
