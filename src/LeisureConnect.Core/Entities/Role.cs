using System;
using System.Collections.Generic;

namespace LeisureConnect.Core.Entities;

public partial class Role
{
    public int RoleId { get; set; }

    public string Name { get; set; } = null!;

    public string? Description { get; set; }

    public bool CanAuthorizeDiscounts { get; set; }

    public bool CanAuthorizePackages { get; set; }

    public decimal? MaxDiscountPercentage { get; set; }

    public DateTime CreatedDate { get; set; }

    public DateTime? LastModifiedDate { get; set; }

    public virtual ICollection<Employee> Employees { get; set; } = new List<Employee>();
}
