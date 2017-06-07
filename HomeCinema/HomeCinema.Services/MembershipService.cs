using HomeCinema.Data.Infrastructure;
using HomeCinema.Data.Repositories;
using HomeCinema.Entities;
using HomeCinema.Services.Abstract;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HomeCinema.Data.Extensions;
using System.Security.Principal;

namespace HomeCinema.Services
{
    public class MembershipService : IMembershipService
    {
        #region Variables 
        private readonly IEntityBaseRepository<User> _userRepository; 
        private readonly IEntityBaseRepository<Role> _roleRepository; 
        private readonly IEntityBaseRepository<UserRole> _userRoleRepository; 
        private readonly IEncryptionService _encryptionService; 
        private readonly IUnitOfWork _unitOfWork; 
        #endregion 
        
        public MembershipService(IEntityBaseRepository<User> userRepository, IEntityBaseRepository<Role> roleRepository, IEntityBaseRepository<UserRole> userRoleRepository, IEncryptionService encryptionService, IUnitOfWork unitOfWork) 
        { 
            _userRepository = userRepository; 
            _roleRepository = roleRepository; 
            _userRoleRepository = userRoleRepository; 
            _encryptionService = encryptionService; 
            _unitOfWork = unitOfWork; 
        }


        private void AddUserToRole(User user, int roleId)
        {
            var role = _roleRepository.GetSingle(roleId);
            if (role == null)
                throw new ApplicationException("Role doesn't exist");

            var userRole = new UserRole 
            { 
                UserId = user.ID,
                RoleId = roleId
            };
            _userRoleRepository.Add(userRole);
        }

        private bool IsPasswordValid(User user, string password)
        {
            return string.Equals(user.HashedPassword, _encryptionService.EncryptPassword(user.Salt, password));
        }

        private bool IsUserValid(User user, string password)
        {
            if (IsPasswordValid(user, password))
            {
                return !user.IsLocked;
            }
            return false;
        }

        public User CreateUser(string username, string email, string password, int[] roleIds)
        {
            var existingUser = _userRepository.GetSingleByUsername(username);
            if (existingUser != null)
                throw new Exception("User already exists");

            var salt = _encryptionService.CreateSalt();
            var user = new User() 
            {
                Username = username,
                Email = email,
                DateCreated = DateTime.Now,
                Salt = salt,
                HashedPassword = _encryptionService.EncryptPassword(salt, password),
                IsLocked = false
            };

            _userRepository.Add(user);
            _unitOfWork.Commit();

            if (roleIds != null && roleIds.Length > 0)
            {
                foreach (var roleId in roleIds)
                {
                    AddUserToRole(user, roleId);
                }
            }

            _unitOfWork.Commit();
            return user;
        }

        public User GetUser(int userId)
        {
            return _userRepository.GetSingle(userId);
        }

        public List<Role> GetUserRoles(string username)
        {
            List<Role> roles = new List<Role>();
            var user = _userRepository.GetSingleByUsername(username);
            if (user != null)
            {
                foreach (var userRole in user.Userroles)
                {
                    roles.Add(userRole.Role);
                }
            }
            return roles.Distinct().ToList();
        }

        public MembershipContext ValidateUser(string username, string password)
        {
            var membershipCtx = new MembershipContext();

            var user = _userRepository.GetSingleByUsername(username);
            if (user != null && IsUserValid(user, password))
            {
                var userRoles = GetUserRoles(user.Username);
                membershipCtx.User = user;

                var identity = new GenericIdentity(user.Username);
                membershipCtx.Principal = new GenericPrincipal(identity, userRoles.Select(x => x.Name).ToArray());
            }

            return membershipCtx;
        }
    }
}
