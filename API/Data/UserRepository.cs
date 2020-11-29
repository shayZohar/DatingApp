using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using API.DTOs;
using API.Entities;
using API.Helpers;
using API.Interfaces;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using Microsoft.EntityFrameworkCore;

namespace API.Data
{
    public class UserRepository : IUserRepository
    {
        private readonly DataContext _context;
        private readonly IMapper _mapper;
        public UserRepository(DataContext context, IMapper mapper)
        {
            _mapper = mapper;
            _context = context;
        }

        // using projection instead of converting -- instead of GetUsersAsync()
        public async Task<PagedList<MemberDto>> GetMembersAsync(UserParams userParams)
        {
            var query = _context.Users.AsQueryable();
            query = query.Where(u => u.Username != userParams.CurrentUsername);
            query = query.Where(u => u.Gender == userParams.Gender);

            // calculating age filtering
            var minDob = DateTime.Today.AddYears(-userParams.MaxAge - 1);
            var maxDob = DateTime.Today.AddYears(-userParams.MinAge);

            query = query.Where(u => u.DateOfBirth >= minDob && u.DateOfBirth <= maxDob);

            //new switch statement
            query = userParams.OrderBy switch
            {
                "created" => query.OrderByDescending(u => u.Created), //if the order by is "created"
                _ => query.OrderByDescending(u => u.LastActive) //default value of switch
            };

            // the CreateAsync method execute the query in the PagedList class
            return await PagedList<MemberDto>
            .CreateAsync(query.ProjectTo<MemberDto>(_mapper.ConfigurationProvider)
            .AsNoTracking(), userParams.PageNumber, userParams.PageSize);
        }
        // using projection instead of converting -- instead of GetUserAsync()
        public async Task<MemberDto> GetMemberAsync(string username)
        {
            return await _context.Users.Where(x => x.Username == username)
            .ProjectTo<MemberDto>(_mapper.ConfigurationProvider).SingleOrDefaultAsync();
        }
        public async Task<AppUser> GetUserById(int id)
        {
            return await _context.Users.FindAsync(id);
        }

        public async Task<AppUser> GetUserByUserName(string username)
        {
            return await _context.Users.Include(p => p.Photos).SingleOrDefaultAsync(x => x.Username == username);
        }

        public async Task<IEnumerable<AppUser>> GetUsersAsync()
        {
            return await _context.Users.Include(p => p.Photos).ToListAsync();
        }

        public async Task<bool> SaveAllAsync()
        {
            return await _context.SaveChangesAsync() > 0;
        }

        public void Update(AppUser user)
        {
            _context.Entry(user).State = EntityState.Modified;
        }
    }
}