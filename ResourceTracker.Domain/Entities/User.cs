using Microsoft.AspNetCore.Identity;

namespace ResourceTracker.Domain.Entities
{
    public class User : IdentityUser<int>
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public byte[] ProfileImage { get; set; }
        public virtual ICollection<GameSave> GameSaves { get; set; }
    }
}
