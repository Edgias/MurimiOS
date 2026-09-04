using MediatR;
using System.Reflection;

namespace EdgIQ.MurimiOS.Infrastructure.Data;
using Asset = global::EdgIQ.MurimiOS.Domain.Asset.Asset;
using Bin = global::EdgIQ.MurimiOS.Domain.Bins.Bin;
using BinType = global::EdgIQ.MurimiOS.Domain.BinType.BinType;
using BaseDomainEvent = global::EdgIQ.MurimiOS.Domain.Common.BaseDomainEvent;
using BaseEntity = global::EdgIQ.MurimiOS.Domain.Common.BaseEntity;
using Crop = global::EdgIQ.MurimiOS.Domain.Crop.Crop;
using CropCategory = global::EdgIQ.MurimiOS.Domain.CropCategory.CropCategory;
using CropProduction = global::EdgIQ.MurimiOS.Domain.CropProduction.CropProduction;
using CropProductionField = global::EdgIQ.MurimiOS.Domain.CropProduction.CropProductionField;
using CropProductionVariety = global::EdgIQ.MurimiOS.Domain.CropProduction.CropProductionVariety;
using CropUnit = global::EdgIQ.MurimiOS.Domain.CropUnit.CropUnit;
using CropVariety = global::EdgIQ.MurimiOS.Domain.CropVariety.CropVariety;
using Currency = global::EdgIQ.MurimiOS.Domain.Currency.Currency;
using Customer = global::EdgIQ.MurimiOS.Domain.Customer.Customer;
using Equipment = global::EdgIQ.MurimiOS.Domain.Equipment.Equipment;
using EquipmentCategory = global::EdgIQ.MurimiOS.Domain.EquipmentCategory.EquipmentCategory;
using Field = global::EdgIQ.MurimiOS.Domain.Field.Field;
using FieldMeasurement = global::EdgIQ.MurimiOS.Domain.FieldMeasurement.FieldMeasurement;
using Item = global::EdgIQ.MurimiOS.Domain.Item.Item;
using ItemCategory = global::EdgIQ.MurimiOS.Domain.ItemCategory.ItemCategory;
using Loan = global::EdgIQ.MurimiOS.Domain.Loan.Loan;
using Location = global::EdgIQ.MurimiOS.Domain.Location.Location;
using NumberSequence = global::EdgIQ.MurimiOS.Domain.NumberSequence.NumberSequence;
using OwnershipType = global::EdgIQ.MurimiOS.Domain.OwnershipType.OwnershipType;
using PriceList = global::EdgIQ.MurimiOS.Domain.PriceList.PriceList;
using PurchaseInvoice = global::EdgIQ.MurimiOS.Domain.PurchaseInvoice.PurchaseInvoice;
using PurchaseInvoiceItem = global::EdgIQ.MurimiOS.Domain.PurchaseInvoice.PurchaseInvoiceItem;
using Quotation = global::EdgIQ.MurimiOS.Domain.Quotation.Quotation;
using QuotationItem = global::EdgIQ.MurimiOS.Domain.Quotation.QuotationItem;
using SalesInvoice = global::EdgIQ.MurimiOS.Domain.SalesInvoice.SalesInvoice;
using SalesInvoiceItem = global::EdgIQ.MurimiOS.Domain.SalesInvoice.SalesInvoiceItem;
using SalesInvoiceNote = global::EdgIQ.MurimiOS.Domain.SalesInvoiceNote.SalesInvoiceNote;
using SalesOrder = global::EdgIQ.MurimiOS.Domain.SalesOrder.SalesOrder;
using SalesOrderItem = global::EdgIQ.MurimiOS.Domain.SalesOrder.SalesOrderItem;
using Season = global::EdgIQ.MurimiOS.Domain.Season.Season;
using SeasonStatus = global::EdgIQ.MurimiOS.Domain.SeasonStatus.SeasonStatus;
using SoilType = global::EdgIQ.MurimiOS.Domain.SoilType.SoilType;
using Supplier = global::EdgIQ.MurimiOS.Domain.Supplier.Supplier;
using Tax = global::EdgIQ.MurimiOS.Domain.Tax.Tax;
using UnitGroup = global::EdgIQ.MurimiOS.Domain.UnitGroup.UnitGroup;
using UnitMeasurement = global::EdgIQ.MurimiOS.Domain.UnitMeasurement.UnitMeasurement;
using Warehouse = global::EdgIQ.MurimiOS.Domain.Warehouse.Warehouse;
using WorkItem = global::EdgIQ.MurimiOS.Domain.WorkItem.WorkItem;
using WorkItemCategory = global::EdgIQ.MurimiOS.Domain.WorkItemCategory.WorkItemCategory;
using WorkItemStatus = global::EdgIQ.MurimiOS.Domain.WorkItemStatus.WorkItemStatus;
using WorkItemSubCategory = global::EdgIQ.MurimiOS.Domain.WorkItemSubCategory.WorkItemSubCategory;
using YieldMeasurement = global::EdgIQ.MurimiOS.Domain.YieldMeasurement.YieldMeasurement;


public class MurimiOSDbContext(DbContextOptions<MurimiOSDbContext> options, IMediator mediator) : DbContext(options)
,EdgIQ.MurimiOS.Application.Common.Abstractions.ILegacyDbContext{
    public DbSet<Asset> Assets { get; set; }

    public DbSet<Bin> Bins { get; set; }

    public DbSet<BinType> BinTypes { get; set; }

    public DbSet<Crop> Crops { get; set; }

    public DbSet<CropCategory> CropCategories { get; set; }

    public DbSet<CropProduction> CropProductions { get; set; }

    public DbSet<CropProductionField> CropProductionFields { get; set; }

    public DbSet<CropProductionVariety> CropProductionVarieties { get; set; }

    public DbSet<CropUnit> CropUnits { get; set; }

    public DbSet<CropVariety> CropVarieties { get; set; }

    public DbSet<Currency> Currencies { get; set; }

    public DbSet<Customer> Customers { get; set; }

    public DbSet<Equipment> Equipments { get; set; }

    public DbSet<EquipmentCategory> EquipmentCategories { get; set; }

    public DbSet<Field> Fields { get; set; }

    public DbSet<FieldMeasurement> FieldMeasurements { get; set; }

    public DbSet<Item> Items { get; set; }

    public DbSet<ItemCategory> ItemCategories { get; set; }

    public DbSet<Loan> Loans { get; set; }

    public DbSet<Location> Locations { get; set; }

    public DbSet<NumberSequence> NumberSequences { get; set; }

    public DbSet<OwnershipType> OwnershipTypes { get; set; }

    public DbSet<PriceList> PriceLists { get; set; }

    public DbSet<PurchaseInvoice> PurchaseInvoices { get; set; }

    public DbSet<PurchaseInvoiceItem> PurchaseInvoiceItems { get; set; }

    public DbSet<Quotation> Quotations { get; set; }

    public DbSet<QuotationItem> QuotationItems { get; set; }

    public DbSet<SalesInvoice> SalesInvoices { get; set; }

    public DbSet<SalesInvoiceItem> SalesInvoiceItems { get; set; }

    public DbSet<SalesInvoiceNote> SalesInvoiceNotes { get; set; }

    public DbSet<SalesOrder> SalesOrders { get; set; }

    public DbSet<SalesOrderItem> SalesOrderItems { get; set; }

    public DbSet<Season> Seasons { get; set; }

    public DbSet<SeasonStatus> SeasonStatuses { get; set; }

    public DbSet<SoilType> SoilTypes { get; set; }

    public DbSet<Supplier> Suppliers { get; set; }

    public DbSet<Tax> Taxes { get; set; }

    public DbSet<UnitGroup> UnitGroups { get; set; }

    public DbSet<UnitMeasurement> UnitMeasurements { get; set; }

    public DbSet<Warehouse> Warehouses { get; set; }

    public DbSet<WorkItem> WorkItems { get; set; }

    public DbSet<WorkItemCategory> WorkItemCategories { get; set; }

    public DbSet<WorkItemStatus> WorkItemStatuses { get; set; }

    public DbSet<WorkItemSubCategory> WorkItemSubCategories { get; set; }

    public DbSet<YieldMeasurement> YieldMeasurements { get; set; }

    protected override void OnModelCreating(ModelBuilder builder)
    {
        builder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());
    }

    public override async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        int result = await base.SaveChangesAsync(cancellationToken).ConfigureAwait(false);

        // ignore events if no dispatcher provided
        if (mediator == null)
        {
            return result;
        }

        // dispatch events only if save was successful
        BaseEntity[] entitiesWithEvents = ChangeTracker.Entries<BaseEntity>()
            .Select(e => e.Entity)
            .Where(e => e.Events.Count != 0)
            .ToArray();

        foreach (BaseEntity entity in entitiesWithEvents)
        {
            BaseDomainEvent[] events = [.. entity.Events];
            entity.Events.Clear();

            foreach (BaseDomainEvent domainEvent in events)
            {
                await mediator.Publish(domainEvent, cancellationToken).ConfigureAwait(false);
            }
        }

        return result;

    }
}
