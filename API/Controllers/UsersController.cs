using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;
using API.Data;
using API.DTOs;
using API.Entities;
using API.Interface;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace API.Controllers
{
  [Authorize]
  public class UsersController : BaseApiController
  {
    private readonly IUserRepository _repo;
    private readonly IMapper _mapper;
    public UsersController(IUserRepository repo, IMapper mapper)
    {
      _mapper = mapper;
      _repo = repo;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<MemberDto>>> GetUsers()
    {
      return Ok(await _repo.GetMembersAsync());

    }

    [HttpGet("{name}")]
    public async Task<ActionResult<MemberDto>> GetUser(string name)
    {
      return Ok(await _repo.GetMemberAsync(name));
    }

    [HttpPut]
    public async Task<ActionResult> UpdateUser(MemberUpdateDto memberUpdateDto)
    {
      var username = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
      var user = await _repo.GetUserByName(username);
      var updatedUser = _mapper.Map(memberUpdateDto, user);
      _repo.Update(updatedUser);
      if (await _repo.SaveAllAsync()) return NoContent();

      return BadRequest("Failed to update user");
    }
  }
}