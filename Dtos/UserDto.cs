namespace WebApi.Dtos
{
    public class UserDto
    {
        
         public int Id { get; set; }
        public int AvatarId { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }
        public int Number { get; set; }
        public string Password { get; set; }
        public int FK_Role { get; set; }
    }
}