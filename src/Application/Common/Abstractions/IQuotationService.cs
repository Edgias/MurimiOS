using EdgIQ.MurimiOS.Domain.Quotation;

namespace EdgIQ.MurimiOS.Application.Common.Abstractions;
using Quotation = global::EdgIQ.MurimiOS.Domain.Quotation.Quotation;
using QuotationItem = global::EdgIQ.MurimiOS.Domain.Quotation.QuotationItem;

public interface IQuotationService
{
    Task AddQuotationItemAsync(Guid quotationId, Guid itemId, Guid priceListId, Guid? taxId, int units = 1);

    Task ConvertToSalesOrderAsync(Guid quotationId);

    Task<Quotation> CreateQuotationAsync(DateTime quotationDate, DateTime expiryDate, Guid? customerId);

    Task<Quotation> CreateQuotationAsync(DateTime quotationDate, DateTime expiryDate, List<QuotationItem> quotationItems, Guid? customerId);

}
