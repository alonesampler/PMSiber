using System;
using System.Collections.Generic;
using System.Text;

namespace PMS.Application.DTOs.Projects;

public class CreateProjectDto
{
    public ProjectParamsDto Params { get; init; } = null!;
    public Guid ManagerId { get; init; }
    public List<Guid> EmployeesIds { get; init; } = new();
}
