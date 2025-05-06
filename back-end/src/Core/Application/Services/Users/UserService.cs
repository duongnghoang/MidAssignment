using Contract.Dtos.Users.Extensions;
using Contract.Dtos.Users.Responses;
using Contract.Shared;
using Contract.UnitOfWork;

namespace Application.Services.Users;

public class UserService(IUnitOfWork unitOfWork) : IUserService
{
    public async Task<Result<GetUserResponse>> GetUserByIdAsync(uint id)
    {
        var user = await unitOfWork.UserRepository.GetByIdAsync(id, user => user.Role);
        if (user == null)
        {
            return Result.Failure<GetUserResponse>("Null result");
        }

        var userResponse = user.ToGetUserResponse();

        return Result.Success(userResponse);
    }
}