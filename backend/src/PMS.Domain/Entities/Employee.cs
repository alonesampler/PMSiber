using PMS.Domain.Abstractions;
using PMS.Domain.ValueObjects;

namespace PMS.Domain.Entities;

public class Employee : Entity<Guid>
{
    private readonly List<Project> _managedProjects = new();
    private readonly List<Project> _assignedProjects = new();

    private Employee() : base() { }

    private Employee(
        Guid id,
        FullName fullName,
        Email email) : base(id)
    {
        FullName = fullName;
        Email = email;
    }

    public FullName FullName { get; private set; }
    public Email Email { get; private set; }

    public IReadOnlyCollection<Project> ManagedProjects => _managedProjects.AsReadOnly();
    public IReadOnlyCollection<Project> AssignedProjects => _assignedProjects.AsReadOnly();

    public static Employee Create(
        Guid id,
        FullName fullName,
        Email email)
    {
        return new Employee(id, fullName, email);
    }

    internal void AddManagedProject(Project project)
    {
        if (!_managedProjects.Any(p => p.Id == project.Id))
            _managedProjects.Add(project);
    }

    internal void AddAssignedProject(Project project)
    {
        if (!_assignedProjects.Any(p => p.Id == project.Id))
            _assignedProjects.Add(project);
    }

    internal void RemoveManagedProject(Guid projectId)
    {
        var project = _managedProjects.FirstOrDefault(p => p.Id == projectId);
        if (project != null)
            _managedProjects.Remove(project);
    }

    internal void RemoveAssignedProject(Guid projectId)
    {
        var project = _assignedProjects.FirstOrDefault(p => p.Id == projectId);
        if (project != null)
            _assignedProjects.Remove(project);
    }

    public void Update(FullName fullName, Email email)
    {
        if (!Equals(FullName, fullName))
            FullName = fullName;

        if (!Equals(Email, email))
            Email = email;
    }

    public bool IsManagerOfProject(Guid projectId)
        => _managedProjects.Any(p => p.Id == projectId);
}