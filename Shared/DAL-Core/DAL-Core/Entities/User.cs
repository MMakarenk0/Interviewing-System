﻿using Microsoft.AspNetCore.Identity;

namespace DAL_Core.Entities;

public class User : IdentityUser<Guid>
{
    public string FirstName { get; set; }
    public string LastName { get; set; }
}



