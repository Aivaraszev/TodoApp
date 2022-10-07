using Application.DaoInterfaces;
using Domain.DTOs;
using Domain.Models;

namespace FileData.DAOs;

public class UserFileDao : IUserDao
{
    private readonly FileContext _context;

    public UserFileDao(FileContext context)
    {
        _context = context;
    }

    public Task<User> CreateAsync(User user)
    {
        int userId = 1;
        if (_context.Users.Any())
        {
            userId = _context.Users.Max(u => u.Id);
            userId++;
        }

        user.Id = userId;

        _context.Users.Add(user);
        _context.SaveChanges();

        return Task.FromResult(user);
    }

    public Task<User?> GetByUsernameAsync(string userName)
    {
        User? existing =
            _context.Users.FirstOrDefault(u =>
                u.UserName.Equals(userName, StringComparison.OrdinalIgnoreCase)
            );
        return Task.FromResult(existing);
    }

    public Task<IEnumerable<User>> GetAsync(SearchUserParametersDto searchParameters)
    {
        IEnumerable<User> users = _context.Users.AsEnumerable();
        if (searchParameters.UsernameContains != null)
        {
            users = _context.Users.Where(u =>
                u.UserName.Contains(searchParameters.UsernameContains, StringComparison.OrdinalIgnoreCase));
        }

        return Task.FromResult(users);
    }

    public Task<User?> getByIdAsync(int id)
    {
        User? existing = _context.Users.FirstOrDefault(u => u.Id == id);
        return Task.FromResult(existing);
    }
}