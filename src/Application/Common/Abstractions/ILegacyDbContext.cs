using MediatR;
using System.Reflection;
using Gender = global::EdgIQ.MurimiOS.Domain.LivestockProduction.Gender;
using ContactType = global::EdgIQ.MurimiOS.Domain.Contact.ContactType;
using LivestockType = global::EdgIQ.MurimiOS.Domain.LivestockProduction.LivestockType;
using LivestockStatus = global::EdgIQ.MurimiOS.Domain.LivestockProduction.LivestockStatus;
using LivestockUnitOfMeasure = global::EdgIQ.MurimiOS.Domain.LivestockProduction.LivestockUnitOfMeasure;
using TagLocation = global::EdgIQ.MurimiOS.Domain.LivestockProduction.TagLocation;
using TreatmentApplicationMethod = global::EdgIQ.MurimiOS.Domain.LivestockProduction.TreatmentApplicationMethod;
using TreatmentType = global::EdgIQ.MurimiOS.Domain.LivestockProduction.TreatmentType;
using EdgIQ.MurimiOS.Domain.YieldMeasurement;
using EdgIQ.MurimiOS.Domain.WorkItemSubCategory;
using EdgIQ.MurimiOS.Domain.WorkItemStatus;
using EdgIQ.MurimiOS.Domain.WorkItemCategory;
using EdgIQ.MurimiOS.Domain.WorkItem;
using EdgIQ.MurimiOS.Domain.Warehouse;
using EdgIQ.MurimiOS.Domain.UnitMeasurement;
using EdgIQ.MurimiOS.Domain.UnitGroup;
using EdgIQ.MurimiOS.Domain.Tax;
using EdgIQ.MurimiOS.Domain.Supplier;
using EdgIQ.MurimiOS.Domain.SoilType;
using EdgIQ.MurimiOS.Domain.SeasonStatus;
using EdgIQ.MurimiOS.Domain.Season;
using EdgIQ.MurimiOS.Domain.SalesInvoiceNote;
using EdgIQ.MurimiOS.Domain.EquipmentCategory;
using EdgIQ.MurimiOS.Domain.Equipment;
using EdgIQ.MurimiOS.Domain.Customer;
using EdgIQ.MurimiOS.Domain.Currency;
using EdgIQ.MurimiOS.Domain.CropVariety;
using EdgIQ.MurimiOS.Domain.CropUnit;
using EdgIQ.MurimiOS.Domain.Loan;
using EdgIQ.MurimiOS.Domain.BinType;
using EdgIQ.MurimiOS.Domain.Bins;
using EdgIQ.MurimiOS.Domain.Asset;
using EdgIQ.MurimiOS.Domain.Crop;
using EdgIQ.MurimiOS.Domain.CropCategory;
using EdgIQ.MurimiOS.Domain.Item;
using EdgIQ.MurimiOS.Domain.FieldMeasurement;
using EdgIQ.MurimiOS.Domain.Field;
using EdgIQ.MurimiOS.Domain.ItemCategory;
using EdgIQ.MurimiOS.Domain.OwnershipType;
using EdgIQ.MurimiOS.Domain.NumberSequence;
using EdgIQ.MurimiOS.Domain.Location;
using EdgIQ.MurimiOS.Domain.PriceList;
using EdgIQ.MurimiOS.Domain.CropProduction;
using EdgIQ.MurimiOS.Domain.PurchaseInvoice;
using EdgIQ.MurimiOS.Domain.Quotation;
using EdgIQ.MurimiOS.Domain.SalesInvoice;
using EdgIQ.MurimiOS.Domain.SalesOrder;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.ChangeTracking;
namespace EdgIQ.MurimiOS.Application.Common.Abstractions;
using Asset = global::EdgIQ.MurimiOS.Domain.Asset.Asset;
using Bin = global::EdgIQ.MurimiOS.Domain.Bins.Bin;
using BinType = global::EdgIQ.MurimiOS.Domain.BinType.BinType;
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


public interface ILegacyDbContext
{
    DbSet<Asset> Assets { get; }
    DbSet<Bin> Bins { get; }
    DbSet<BinType> BinTypes { get; }
    DbSet<Crop> Crops { get; }
    DbSet<CropCategory> CropCategories { get; }
    DbSet<CropProduction> CropProductions { get; }
    DbSet<CropProductionField> CropProductionFields { get; }
    DbSet<CropProductionVariety> CropProductionVarieties { get; }
    DbSet<CropUnit> CropUnits { get; }
    DbSet<CropVariety> CropVarieties { get; }
    DbSet<Currency> Currencies { get; }
    DbSet<Customer> Customers { get; }
    DbSet<Equipment> Equipments { get; }
    DbSet<EquipmentCategory> EquipmentCategories { get; }
    DbSet<Field> Fields { get; }
    DbSet<FieldMeasurement> FieldMeasurements { get; }
    DbSet<Item> Items { get; }
    DbSet<ItemCategory> ItemCategories { get; }
    DbSet<Loan> Loans { get; }
    DbSet<Location> Locations { get; }
    DbSet<NumberSequence> NumberSequences { get; }
    DbSet<OwnershipType> OwnershipTypes { get; }
    DbSet<PriceList> PriceLists { get; }
    DbSet<PurchaseInvoice> PurchaseInvoices { get; }
    DbSet<PurchaseInvoiceItem> PurchaseInvoiceItems { get; }
    DbSet<Quotation> Quotations { get; }
    DbSet<QuotationItem> QuotationItems { get; }
    DbSet<SalesInvoice> SalesInvoices { get; }
    DbSet<SalesInvoiceItem> SalesInvoiceItems { get; }
    DbSet<SalesInvoiceNote> SalesInvoiceNotes { get; }
    DbSet<SalesOrder> SalesOrders { get; }
    DbSet<SalesOrderItem> SalesOrderItems { get; }
    DbSet<Season> Seasons { get; }
    DbSet<SeasonStatus> SeasonStatuses { get; }
    DbSet<SoilType> SoilTypes { get; }
    DbSet<Supplier> Suppliers { get; }
    DbSet<Tax> Taxes { get; }
    DbSet<UnitGroup> UnitGroups { get; }
    DbSet<UnitMeasurement> UnitMeasurements { get; }
    DbSet<Warehouse> Warehouses { get; }
    DbSet<WorkItem> WorkItems { get; }
    DbSet<WorkItemCategory> WorkItemCategories { get; }
    DbSet<WorkItemStatus> WorkItemStatuses { get; }
    DbSet<WorkItemSubCategory> WorkItemSubCategories { get; }
    DbSet<YieldMeasurement> YieldMeasurements { get; }
    DatabaseFacade Database { get; }
    ChangeTracker ChangeTracker { get; }
    EntityEntry<TEntity> Add<TEntity>(TEntity entity) where TEntity : class;
    EntityEntry<TEntity> Update<TEntity>(TEntity entity) where TEntity : class;
    DbSet<TEntity> Set<TEntity>() where TEntity : class;
    EntityEntry<TEntity> Entry<TEntity>(TEntity entity) where TEntity : class;
    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
    ValueTask<EntityEntry<TEntity>> AddAsync<TEntity>(TEntity entity, CancellationToken cancellationToken = default) where TEntity : class;
}
