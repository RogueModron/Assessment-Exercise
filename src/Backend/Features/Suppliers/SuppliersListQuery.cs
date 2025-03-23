namespace Backend.Features.Suppliers;

public class SuppliersListQuery : IRequest<List<SupplierListQueryResponse>>
{
    public string? Name { get; set; }
}

public class SupplierListQueryResponse
{
    public int Id { get; set; }
    public string Name { get; set; } = "";
    public string Address { get; set; } = "";
    public string Email { get; set; } = "";
    public string Phone { get; set; } = "";
}

internal class SuppliersListQueryHandler(BackendContext context) : IRequestHandler<SuppliersListQuery, List<SupplierListQueryResponse>>
{
    private readonly BackendContext context = context;

    public async Task<List<SupplierListQueryResponse>> Handle(SuppliersListQuery request, CancellationToken cancellationToken)
    {
        var query = context.Suppliers.AsQueryable();
        if (!string.IsNullOrEmpty(request.Name))
            query = query.Where(q => q.Name.ToLower().Contains(request.Name.ToLower()));

        var data = await query.OrderBy(q => q.Name).ToListAsync(cancellationToken);
        var result = new List<SupplierListQueryResponse>();

        foreach (var item in data)
        {
            var resultItem = new SupplierListQueryResponse
            {
                Id = item.Id,
                Name = item.Name,
                Address = item.Address,
                Email = item.Email,
                Phone = item.Phone,
            };

            result.Add(resultItem);
        }

        return result;
    }
}