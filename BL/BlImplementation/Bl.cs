
using BlApi;

namespace BlImplementation;

internal class Bl : BlApi.IBl

{
    public ICourier courier => new CourierImpementation();

    public IOrder order => new OrderImplementation();

    public IAdmin Admin => new AdminImplementation();
}
