using Edgias.MurimiOS.Domain.Entities.QuotationAggregate;

namespace Edgias.MurimiOS.Domain.Interfaces;

public interface IQuotationService
{
    Task AddQuotationItemAsync(Guid quotationId, Guid itemId, Guid priceListId, Guid? taxId, int units = 1);

    Task ConvertToSalesOrderAsync(Guid quotationId);

    Task<Quotation> CreateQuotationAsync(DateTime quotationDate, DateTime expiryDate, Guid? customerId);

    Task<Quotation> CreateQuotationAsync(DateTime quotationDate, DateTime expiryDate, List<QuotationItem> quotationItems, Guid? customerId);

}
