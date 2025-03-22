namespace Backend.Features.Customers;

public class CustomersListQuery : IRequest<List<CustomerListQueryResponse>>
{
    public string? SearchText { get; set; }
}

public class CustomerListQueryResponse
{
    public int Id { get; set; }
    public string Name { get; set; } = "";
    public string Address { get; set; } = "";
    public string Email { get; set; } = "";
    public string Phone { get; set; } = "";
    public string Iban { get; set; } = "";
    public CustomersListQueryResponseCustomerCategory? CustomerCategory { get; set; }
}

public class CustomersListQueryResponseCustomerCategory
{
    public string Code { get; set; } = "";
    public string Description { get; set; } = "";
}

internal class CustomerListQueryHandler(BackendContext context) : IRequestHandler<CustomersListQuery, List<CustomerListQueryResponse>>
{
    private readonly BackendContext context = context;

    public async Task<List<CustomerListQueryResponse>> Handle(CustomersListQuery request, CancellationToken cancellationToken)
    {
        var query = context.Customers.Include(q => q.CustomerCategory).AsQueryable();
        if (!string.IsNullOrEmpty(request.SearchText))
        {
            var searchText = request.SearchText.ToLower();
            query = query.Where(q => q.Name.ToLower().Contains(searchText) || q.Email.ToLower().Contains(searchText));
        }

        var data = await query.OrderBy(q => q.Name).ToListAsync(cancellationToken);
        var result = new List<CustomerListQueryResponse>();

        foreach (var item in data)
        {
            var resultItem = new CustomerListQueryResponse
            {
                Id = item.Id,
                Name = item.Name,
                Address = item.Address,
                Email = item.Email,
                Phone = item.Phone,
                Iban = item.Iban,
            };

            var customerCategory = item.CustomerCategory;
            if (customerCategory is not null)
                resultItem.CustomerCategory = new CustomersListQueryResponseCustomerCategory
                {
                    Code = customerCategory.Code,
                    Description = customerCategory.Description
                };

            result.Add(resultItem);
        }

        return result;
    }
}