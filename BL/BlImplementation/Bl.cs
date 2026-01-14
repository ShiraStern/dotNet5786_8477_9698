
using BlApi;

namespace BlImplementation;

internal class Bl : BlApi.IBl

{
    public ICourier Courier => new CourierImpementation();

    public IOrder Order => new OrderImplementation();

    public IAdmin Admin => new AdminImplementation();
}
