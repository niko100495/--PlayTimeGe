using System.Collections.Generic;
using WebApi.Entities;

namespace WebApi.Dtos.ForRelationship
{
    public class UserRelation
    {
        public int Id { get; set; }
        public int AvatarId { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }
        public int Number { get; set; }
        public string CreateDate { get; set; }
        public string FK_Role { get; set; }
        public bool UserActive { get; set; }

        
    }
}