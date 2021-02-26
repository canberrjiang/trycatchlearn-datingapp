using System.Collections.Generic;
using System.Threading.Tasks;
using API.DTOs;
using API.Entities;

namespace API.Interface
{
  public interface IUserRepository
  {
    void Update(AppUser user);
    Task<bool> SaveAllAsync();
    Task<IEnumerable<AppUser>> GetUsersAsync();
    Task<AppUser> GetUserById(int id);
    Task<AppUser> GetUserByName(string name);
    Task<MemberDto> GetMemberAsync(string name);
    Task<IEnumerable<MemberDto>> GetMembersAsync();
  }
}