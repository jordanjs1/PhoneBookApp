namespace EventBus;

public class SubscriptionInformation
{
    public bool IsDynamic { get; }
    public Type HandlerType { get; }

    private SubscriptionInformation(bool isDynamic, Type handlerType)
    {
        IsDynamic = isDynamic;
        HandlerType = handlerType;
    }

    public static SubscriptionInformation Dynamic(Type handlerType) => new(true, handlerType);

    public static SubscriptionInformation Typed(Type handlerType) => new(false, handlerType);
}