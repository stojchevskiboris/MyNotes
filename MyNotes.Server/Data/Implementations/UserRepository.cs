using MyNotes.Server.Data.Interfaces;
using MyNotes.Server.Data.Utils;
using MyNotes.Server.Domain.Models;

namespace MyNotes.Server.Data.Implementations
{
    public class UserRepository : Repository<User>, IUserRepository
    {
        private readonly MyNotesDbContext _context;

        public UserRepository(MyNotesDbContext context) : base(context)
        {
            _context = context;
        }        
    }
}
