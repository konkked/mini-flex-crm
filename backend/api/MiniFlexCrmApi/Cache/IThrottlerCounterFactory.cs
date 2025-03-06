namespace MiniFlexCrmApi.Cache;

public interface IThrottlerCounterFactory
{
    IThrottlerCounter Create();
}