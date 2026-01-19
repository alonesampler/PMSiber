namespace PMS.Application.DTOs.Projects;

public sealed record ProjectUpsertDto(
    ProjectParamsDto Params,
    Guid ManagerId,
    List<Guid> EmployeesIds);
