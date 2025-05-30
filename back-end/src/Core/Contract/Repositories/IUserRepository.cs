﻿using Domain.Abstractions.Base;
using Domain.Entities;

namespace Contract.Repositories;

public interface IUserRepository : IBaseRepository<User>
{
    Task<User?> GetUserByUsername(string username);
}