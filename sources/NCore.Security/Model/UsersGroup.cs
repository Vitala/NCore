using System.Collections.Generic;

namespace NCore.Security.Model
{
    public class UsersGroup : NamedEntity
    {
	    public UsersGroup()
	    {
	        Users = new HashSet<User>();
	        AllParents = new HashSet<UsersGroup>();
	        AllChildren = new HashSet<UsersGroup>();
	        DirectChildren = new HashSet<UsersGroup>();
	    }

	    public virtual ICollection<User> Users { get; set; }
	    public virtual UsersGroup Parent { get; set; }
	    public virtual ICollection<UsersGroup> DirectChildren { get; set; }
	    public virtual ICollection<UsersGroup> AllChildren { get; set; }
	    public virtual ICollection<UsersGroup> AllParents { get; set; }
    }
}
