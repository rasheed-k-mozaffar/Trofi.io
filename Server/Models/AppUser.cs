﻿namespace Trofi.io.Server.Models;

public class AppUser : IdentityUser
{
    public string? FirstName { get; set; }
    public string? LastName { get; set; }
    public string? Location { get; set; }

    public Guid CartId { get; set; }
    public virtual Cart? Cart { get; set; }
    public virtual List<RefreshToken>? RefreshTokens { get; set; }
    public virtual ICollection<CustomerReview>? Reviews { get; set; } = new List<CustomerReview>();
}
