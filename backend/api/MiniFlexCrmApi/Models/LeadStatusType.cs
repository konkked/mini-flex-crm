namespace MiniFlexCrmApi.Models;

public enum LeadStatusType
{
    Unknown,
    Dead, 
    Raw, 
    Bronze, 
    Silver, 
    Gold, 
    Qualified
}

public enum DealStatusType
{
    Unknown,
    Abandoned,
    Qualified,
    Outreach,
    Nurture,
    Closing,
    Closed
}