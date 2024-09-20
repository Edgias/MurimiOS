﻿using Edgias.MurimiOS.Domain.Entities.QuotationAggregate;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Edgias.MurimiOS.Domain.Interfaces
{
    public interface IQuotationService
    {
        Task AddQuotationItemAsync(Guid quotationId, Guid itemId, Guid priceListId, Guid? taxId, int units = 1);

        Task ConvertToSalesOrderAsync(Guid quotationId);

        Task<Quotation> CreateQuotationAsync(DateTimeOffset quotationDate, DateTimeOffset validUntil, Guid? customerId);

        Task<Quotation> CreateQuotationAsync(DateTimeOffset quotationDate, DateTimeOffset validUntil, List<QuotationItem> quotationItems, Guid? customerId);

    }
}
