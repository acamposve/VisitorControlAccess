using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AccessControlApi.Domain.Entities;

public class AccessPoint
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;

    public int CreatedBy { get; set; }
    public User Creator { get; set; } = null!;
}
