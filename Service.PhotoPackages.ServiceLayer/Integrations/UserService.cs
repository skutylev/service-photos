using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Caching.Memory;
using Service.PhotoPackages.ServiceLayer.Integrations.Contracts;
using Service.Users.Client;
using Service.Users.Client.Contracts;
using Executor = Service.PhotoPackages.Client.Contracts.Executor;

namespace Service.PhotoPackages.ServiceLayer.Integrations
{
    public class UsersService : IUsersService
    {
        private readonly UserServiceClient _userServiceClient;
        private readonly IMemoryCache _memoryCache;

        public UsersService(UserServiceClient userServiceClient, IMemoryCache memoryCache)
        {
            _userServiceClient = userServiceClient;
            _memoryCache = memoryCache;
        }

        public async Task<List<Executor>> GetExecutors(long[] sapNumbers, CancellationToken cancellationToken)
        {
            var users = await GetUsers(sapNumbers, cancellationToken);
            return users.Select(u => new Executor
            {
                AdUserName = u.AdUserName,
                FullName = u.FullName,
                SapNumber = u.SapNumber
            }).ToList();
        }
        public async Task<List<UserModel>> GetUsers(long[] sapNumbers, CancellationToken cancellationToken)
        {
            var users = new List<UserModel>();
            var notInCache = new List<long>();
            
            foreach (var sapNumber in sapNumbers)
            {
                if (_memoryCache.TryGetValue(sapNumber, out UserModel userModel))
                {
                    users.Add(userModel);
                }
                else
                {
                    notInCache.Add(sapNumber);
                }
            }

            if (!notInCache.Any()) return users;

            var notInCacheUsersResponse = await _userServiceClient.GetUsersBySapNumberPost(notInCache.ToArray(), cancellationToken);
            if (!notInCacheUsersResponse.IsSuccessStatusCode || 
                notInCacheUsersResponse.Content == null ||
                !notInCacheUsersResponse.Content.Any()) 
                return users;
            
            users.AddRange(notInCacheUsersResponse.Content);

            foreach (var user in notInCacheUsersResponse.Content)
            {
                _memoryCache.Set(user.SapNumber, user,
                    new MemoryCacheEntryOptions().SetAbsoluteExpiration(TimeSpan.FromDays(1)));
            }

            return users;
        }
    }
}