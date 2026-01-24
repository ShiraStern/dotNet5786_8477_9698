
using BlApi;

namespace BlImplementation;

internal class Bl : BlApi.IBl

{
    public ICourier Courier { get; } = new CourierImpementation();

    public IOrder Order { get; } = new OrderImplementation();

    public IAdmin Admin { get; } = new AdminImplementation();
}
