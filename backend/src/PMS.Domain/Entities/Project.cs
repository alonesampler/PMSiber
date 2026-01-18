using PMS.Domain.Abstractions;

namespace PMS.Domain.Entities;

public class Project : Entity<Guid>
{
    private readonly List<Employee> _employees = new();
    private readonly List<Document> _documents = new();

    private Project() : base() { }

    private Project(
        Guid id,
        string name,
        string customerCompanyName,
        string executorCompanyName,
        DateTime startDate,
        DateTime endDate,
        int priority,
        Guid managerId) : base(id)
    {
        Id = id;
        Name = name;
        CustomerCompanyName = customerCompanyName;
        ExecutorCompanyName = executorCompanyName;
        StartDate = startDate;
        EndDate = endDate;
        Priority = priority;
        ManagerId = managerId;
    }

    public string Name { get; private set; }
    public string CustomerCompanyName { get; private set; }
    public string ExecutorCompanyName { get; private set; }
    public DateTime StartDate { get; private set; }
    public DateTime EndDate { get; private set; }
    public int Priority { get; private set; }

    public Guid ManagerId { get; private set; }
    public Employee Manager { get; private set; } = null!;

    public IReadOnlyCollection<Employee> Employees => _employees.AsReadOnly();
    public IReadOnlyCollection<Document> Documents => _documents.AsReadOnly();

    public static Project Create(
        Guid id,
        string name,
        string customerCompanyName,
        string executorCompanyName,
        DateTime startDate,
        DateTime endDate,
        int priority,
        Guid managerId)
    {
        CanValidate(name, startDate, endDate, priority);

        return new Project(
            id,
            name,
            customerCompanyName,
            executorCompanyName,
            startDate,
            endDate,
            priority,
            managerId);
    }

    private static void CanValidate(
        string name,
        DateTime startDate,
        DateTime endDate,
        int priority)
    {
        if (string.IsNullOrWhiteSpace(name))
            throw new DomainException("Project name is required");

        if (name.Length > 200)
            throw new DomainException("Project name is too long");

        if (startDate >= endDate)
            throw new DomainException("Start date must be before end date");

        if (priority < 1 || priority > 10)
            throw new DomainException("Priority must be between 1 and 10");
    }

    public void Update(
        string name,
        string customerCompanyName,
        string executorCompanyName,
        DateTime startDate,
        DateTime endDate,
        int priority)
    {
        CanValidate(name, startDate, endDate, priority);

        Name = name;
        CustomerCompanyName = customerCompanyName;
        ExecutorCompanyName = executorCompanyName;
        StartDate = startDate;
        EndDate = endDate;
        Priority = priority;
    }

    public void ChangeManager(Employee manager)
    {
        if (manager is null)
            throw new DomainException("Manager cannot be null");

        if (_employees.Any(e => e.Id == manager.Id))
            throw new DomainException("Manager cannot be a project employee");

        ManagerId = manager.Id;
        Manager = manager;
    }

    public void AddEmployee(Employee employee)
    {
        if (employee is null)
            throw new DomainException("Employee cannot be null");

        if (employee.Id == ManagerId)
            throw new DomainException("Manager cannot be added as employee");

        if (_employees.Any(e => e.Id == employee.Id))
            throw new DomainException("Employee already in project");

        _employees.Add(employee);
    }

    public void RemoveEmployee(Guid employeeId)
    {
        if (employeeId == ManagerId)
            throw new DomainException("Cannot remove manager from employees");

        var employee = _employees.FirstOrDefault(e => e.Id == employeeId);
        if (employee is null)
            return;

        _employees.Remove(employee);
    }
}