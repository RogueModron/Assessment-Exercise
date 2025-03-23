namespace Backend.Features.Employees;

public class EmployeesListQuery : IRequest<List<EmployeeListQueryResponse>>
{
    public string? FirstName { get; set; }
    public string? LastName { get; set; }
}

public class EmployeeListQueryResponse
{
    public int Id { get; set; }
    public string Code { get; internal set; } = "";
    public string FirstName { get; set; } = "";
    public string LastName { get; set; } = "";
    public string Address { get; set; } = "";
    public string Email { get; set; } = "";
    public string Phone { get; set; } = "";
    public EmployeeListQueryResponseDepartment? Department { get; set; }
}

public class EmployeeListQueryResponseDepartment
{
    public string Code { get; set; } = "";
    public string Description { get; set; } = "";
}

internal class EmployeesListQueryHandler(BackendContext context) : IRequestHandler<EmployeesListQuery, List<EmployeeListQueryResponse>>
{
    private readonly BackendContext context = context;

    public async Task<List<EmployeeListQueryResponse>> Handle(EmployeesListQuery request, CancellationToken cancellationToken)
    {
        var query = context.Employees.Include(q => q.Department).AsQueryable();
        if (!string.IsNullOrEmpty(request.FirstName))
            query = query.Where(q => q.FirstName.ToLower().Contains(request.FirstName.ToLower()));
        if (!string.IsNullOrEmpty(request.LastName))
            query = query.Where(q => q.LastName.ToLower().Contains(request.LastName.ToLower()));

        var data = await query.OrderBy(q => q.LastName).ThenBy(q => q.FirstName).ToListAsync(cancellationToken);
        var result = new List<EmployeeListQueryResponse>();

        foreach (var item in data)
        {
            var resultItem = new EmployeeListQueryResponse
            {
                Id = item.Id,
                Code = item.Code,
                FirstName = item.FirstName,
                LastName = item.LastName,
                Address = item.Address,
                Email = item.Email,
                Phone = item.Phone,
            };

            var department = item.Department;
            if (department is not null)
                resultItem.Department = new EmployeeListQueryResponseDepartment
                {
                    Code = department.Code,
                    Description = department.Description
                };

            result.Add(resultItem);
        }

        return result;
    }
}