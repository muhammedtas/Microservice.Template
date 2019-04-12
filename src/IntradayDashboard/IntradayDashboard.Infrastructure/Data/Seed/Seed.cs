using System;
using System.Collections.Generic;
using System.Linq;
using IntradayDashboard.Core.Model.Entities;
using IntradayDashboard.Core.Services.Interfaces;
using IntradayDashboard.Infrastructure.Data.Uow;
using Microsoft.AspNetCore.Identity;
using Newtonsoft.Json;

namespace IntradayDashboard.Infrastructure.Seed
{
    public class Seed
    {
        
        public readonly  IUserService _userService;
        public UserManager<User> _userManager { get; }

        private readonly RoleManager<Role> _roleManager;
        private readonly ITenantService _tenantService;

        private readonly IUnitOfWork _uow;

        public Seed(IUserService userService, UserManager<User> userManager, RoleManager<Role> roleManager, ITenantService tenantService, IUnitOfWork uow)
        {
            _userService = userService;
            _userManager = userManager;
            _roleManager = roleManager;
            _tenantService = tenantService;
            _uow = uow;
        }

        /// We gonna inject all users in here with json file 
        public void SeedUsers()
        {
            var usersToDeletion = _userService.GetAll();
            foreach (var user in usersToDeletion)
            {
                _userService.DeleteAsync(user);
            }
            // Save with Unit of work

            if (!_userManager.Users.Any())
            {
                // var userData = System.IO.File.ReadAllText("Data/SeedFiles/UserSeedData.json");
                // var users = JsonConvert.DeserializeObject<List<User>>(userData);
                var roles = new List<Role>
                {
                    new Role{Name = "Member"},
                    new Role{Name = "Admin"},
                    new Role{Name = "Moderator"},
                    new Role{Name = "VIP"},
                };

                foreach (var role in roles)
                {
                    _roleManager.CreateAsync(role).Wait();
                }

                // foreach (var user in users)
                // {
                //     _userManager.CreateAsync(user, "password").Wait();
                //     _userManager.AddToRoleAsync(user, "Member").Wait();
                // }

                var adminUser = new User
                {
                    UserName = "Admin",
                    DateOfBirth = DateTime.Now,
                    LastEnterance = DateTime.Now,
                    CreatedOn = DateTime.Now,
                };

                IdentityResult result = _userManager.CreateAsync(adminUser, "password").Result;

                if (result.Succeeded)
                {
                    var admin = _userManager.FindByNameAsync("Admin").Result;
                    _userManager.AddToRolesAsync(admin, new[] {"Admin", "Moderator"}).Wait();
                }

                var tenant = new Tenant {
                  TenantName = "Tas Corp"  
                };

                _tenantService.AddAsync(tenant);


                _uow.Commit();

            }
        }
    }
}