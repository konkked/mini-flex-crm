using MiniFlexCrmApi.Models;

namespace MiniFlexCrmApi.Services;

public interface ILeadService : ITenantBoundBaseService<LeadModel>;

public interface IDealService : ITenantBoundBaseService<DealModel>;