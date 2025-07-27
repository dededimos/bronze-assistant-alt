namespace CommonOrderModels
{
    [Flags]
    public enum OrderStatus
    {
        UnspecifiedOrderStatus = 0,
        PendingOrderStatus = 1 << 0,
        FilledOrderStatus = 1 << 1,
        CancelledOrderStatus = 1 << 3,
        PartiallyFilledOrderStatus = PendingOrderStatus | FilledOrderStatus,
        PartiallyCancelledOrderStatus = PendingOrderStatus | CancelledOrderStatus,
        PartiallyFilledAndCancelledOrderStatus = PartiallyFilledOrderStatus | CancelledOrderStatus,
        FilledAndCancelledOrderStatus = FilledOrderStatus | CancelledOrderStatus,
    }

}
