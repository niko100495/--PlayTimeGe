using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace WebApi.Entities
{
    public class User
    {
        public int Id { get; set; }
        public int AvatarId { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }
        public int Number { get; set; }
        public byte[] PasswordHash { get; set; }
        public byte[] PasswordSalt { get; set; }
        public string CreateDate { get; set; }
        public int FK_Role { get; set; }
        
    }
}