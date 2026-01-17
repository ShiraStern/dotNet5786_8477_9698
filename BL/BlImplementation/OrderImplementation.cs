namespace BlImplementation;

using BlApi;
using BO;
using DalApi;
using DO;
using Helpers;
using System.Collections.Generic;

internal class OrderImplementation : BlApi.IOrder
{

    // Adds a new order to the system -done
    public void AddOrder(int applicantId, BO.Order boOrder) //done
    {
        // הרשאה – מסך ניהולי
        if (!AdminManager.IsValidManagerId(applicantId))
            throw new BlUnauthorizedAccessException("Only admin can add an order.");

        if (boOrder is null)
            throw new BlArgumentNullException("Order cannot be null.");



        try
        {
            // ה-ID נוצר אוטומטית ב-DAL
            OrderManager.AddOrder(applicantId, boOrder);
        }
        catch (DO.DalAlreadyExistsException ex)
        {
            throw new BlAlreadyExistsException(
                "Order already exists in the system.", ex);
        }
        catch (DO.DalXMLFileLoadCreateException ex)
        {
            throw new BlDataAccessException(
                "Failed to access data layer while adding order.", ex);
        }
        
    }

    public void Delete(int applicantId, int orderId) //done //לפי המסמך רק צריך לזרוק חריגה כי הואלא רשאי למחוק
    {
        throw new BO.BlUnauthorizedAccessException(
            "Orders cannot be deleted from the system.");
    }

    public void CancelOrder(int applicantId, int orderId) //done
    {
        if (!AdminManager.IsValidManagerId(applicantId))
            throw new BO.BlUnauthorizedAccessException("Only admin can cancel orders.");

        try
        {
            OrderManager.CancelOrder(orderId);
        }
        catch (DO.DalDoesNotExistException ex)
        {
            throw new BO.BlDoesNotExistException(
                $"Order with ID {orderId} does not exist.", ex);
        }
        catch (DO.DalXMLFileLoadCreateException ex)
        {
            throw new BO.BlDataAccessException(
                "Failed to access data layer while cancelling order.", ex);
        }
        catch (BO.BlInvalidStatusException)
        {
            throw;
        }
    }

    public void EndOrderHandle(int applicantId, int courierId, int orderId, int deliveryId)// done
    {
        // 1. בדיקת חוקיות המבקש – חייב להיות השליח עצמו
        if (applicantId != courierId)
            throw new BlUnauthorizedAccessException(
                "Only the assigned courier can end order handling.");

        try
        {
            // 2. שליפת המשלוח מה-DAL
            DO.Delivery? delivery = AdminManager.GetDal().Delivery.Read(deliveryId);

            // 3. בדיקה שהמשלוח שייך להזמנה ולשליח
            if (delivery.OrderId != orderId || delivery.CourierId != courierId)
                throw new BlUnauthorizedAccessException(
                    "The courier is not authorized to end this delivery.");

            // 4. עדכון המשלוח – סיום טיפול
            DO.Delivery updatedDelivery = delivery with
            {
                DeliveryEndTime = DateTime.Now,
                DeliveryTermintionType = DO.DeliveryTermintionType.DeliveredSeccessfully
            };

            // 5. שמירה ב-DAL
            AdminManager.GetDal().Delivery.Update(updatedDelivery);
        }
        catch (DalDoesNotExistException ex)
        {
            throw new BlDoesNotExistException(
                $"Delivery with ID {deliveryId} does not exist.", ex);
        }
        catch (DalXMLFileLoadCreateException ex)
        {
            throw new BlDataAccessException(
                "Failed to access data layer while ending order handling.", ex);
        }
    }

    public IEnumerable<ClosedDeliveryInList> GetClosedDeliveriesPerCourier(//done
    int applicantId,
    int courierId,
    BO.OrderType? filterOrderByType = null,
    sortClosedDeliveriesByProperty? byProperty = null)
    {
        //  הרשאה – רק השליח עצמו
        if (applicantId != courierId)
            throw new BlUnauthorizedAccessException(
                "Only the courier can view his delivery history.");

        try
        {
            var dal = AdminManager.GetDal();

            // כל המשלוחים הסגורים של השליח
            var closedDeliveries = dal.Delivery.ReadAll()
                .Where(d => d.CourierId == courierId && d.DeliveryEndTime != null);

            //  חיבור להזמנה
            var result = closedDeliveries.Select(d =>
            {
                DO.Order order = dal.Order.Read(d.OrderId);

                return new ClosedDeliveryInList
                {
                    DeliveryId = d.Id,
                    OrderId = order.Id,
                    OrderType = (BO.OrderType)order.OrderType,
                    AddressOfDelivery = order.CustomerAddress,
                    DeliveryType = (BO.DeliveryType)d.DeliveryType,
                    ActualDistance = d.ActualDistance ?? 0,
                    TotalHandlingTime =
                        d.DeliveryEndTime!.Value - d.DeliveryStartTime,
                    DeliveryTermintionType =
                        (BO.DeliveryTerminationType)d.DeliveryTermintionType
                };
            });


            //  סינון לפי סוג הזמנה (אם קיים)
            if (filterOrderByType != null)
            {
                result = result.Where(r => r.OrderType == filterOrderByType);
            }

            //  מיון
            result = byProperty switch
            {
                null => result
                    .OrderBy(r => r.DeliveryTermintionType),

                sortClosedDeliveriesByProperty.OrderType =>
                    result.OrderBy(r => r.OrderType),

                sortClosedDeliveriesByProperty.DeliveryEndTime =>
                    result.OrderBy(r => r.TotalHandlingTime),

                sortClosedDeliveriesByProperty.DeliveryTerminationType =>
                    result.OrderBy(r => r.DeliveryTermintionType),

                _ => result
            };


            return result;
        }
        catch (DalDoesNotExistException ex)
        {
            throw new BlDoesNotExistException(
                "Delivery or order does not exist.", ex);
        }
        catch (DalXMLFileLoadCreateException ex)
        {
            throw new BlDataAccessException(
                "Failed to access data layer.", ex);
        }
    }

    public IEnumerable<OpenOrderInList> GetDeliveriesPerCourier( // done
    int applicantId,
    int courierId,
    BO.OrderType? filterOrderByType = null,
    sortClosedDeliveriesByProperty? byProperty = null)
    {
        //  הרשאה – רק השליח עצמו
        if (applicantId != courierId)
            throw new BlUnauthorizedAccessException(
                "Only the courier can view open orders.");

        try
        {
            var dal = AdminManager.GetDal();

            //  שליפת השליח
            DO.Courier courier = dal.Courier.Read(courierId);

            //  כל ההזמנות שאין להן משלוח פעיל
            var openOrders = dal.Order.ReadAll()
                .Where(o =>
                    !dal.Delivery.ReadAll()
                        .Any(d => d.OrderId == o.Id && d.DeliveryEndTime == null));

            //  המרה ל-OpenOrderInList
            var result = openOrders.Select(o =>
            {
                BO.Order boOrder = OrderManager.ConvertToOrder(o, null);

                return new OpenOrderInList
                {
                    courierId = courierId,
                    OrderId = o.Id,
                    OrderType = (BO.OrderType)o.OrderType,
                    DeliveryType = (BO.DeliveryType)courier.DeliveryType,
                    CustomerAddress = o.CustomerAddress,

                    AirDistance = 0,       
                    actualDistance = 0,

                    EstimatedDeliveryTime =
                    boOrder.EstimatedDeliveryTime != null
                        ? boOrder.EstimatedDeliveryTime.Value - DateTime.Now
                        : null,
                    ScheduleStatus = boOrder.ScheduleStatus,
                    deliveryTimeLeft = boOrder.TimeLeftToCompleteOrder,
                    MaximumDeliveryTime = boOrder.MaximumDeliveryTime
                };
            });

            // 5. סינון לפי סוג הזמנה
            if (filterOrderByType != null)
            {
                result = result.Where(r => r.OrderType == filterOrderByType);
            }

            // 6. מיון
            result = byProperty switch
            {
                null => result.OrderBy(r => r.ScheduleStatus),

                sortClosedDeliveriesByProperty.OrderType =>
                    result.OrderBy(r => r.OrderType),

                sortClosedDeliveriesByProperty.DeliveryEndTime =>
                    result.OrderBy(r => r.MaximumDeliveryTime),

                _ => result
            };

            return result;
        }
        catch (DalDoesNotExistException ex)
        {
            throw new BlDoesNotExistException(
                "Courier or order does not exist.", ex);
        }
        catch (DalXMLFileLoadCreateException ex)
        {
            throw new BlDataAccessException(
                "Failed to access data layer.", ex);
        }
    }

    public BO.Order GetDetails(int applicantId, int orderId) //done
    {
        if (!AdminManager.IsValidManagerId(applicantId))
            throw new BO.BlUnauthorizedAccessException("Only admin can view order details.");

        try
        {
            return OrderManager.GetOrderDetails(orderId);
        }
        catch (DO.DalDoesNotExistException ex)
        {
            throw new BO.BlDoesNotExistException(
                $"Order with ID {orderId} does not exist.", ex);
        }
        catch (DO.DalXMLFileLoadCreateException ex)
        {
            throw new BO.BlDataAccessException(
                "Failed to access data layer while retrieving order details.", ex);
        }
    }   


    public IEnumerable<OrderInList> GetOrderList(int applicantId,     //done
        filterOrdersByProperty? filterOrdersBy = null, object? type = null, sortOrdersByProperty? sortOrdersBy = null)
    {
        if (!AdminManager.IsValidManagerId(applicantId))
            throw new BO.BlUnauthorizedAccessException("Only admin can view order list.");

        return OrderManager.GetOrderListInternal(
            applicantId,
            filterOrdersBy,
            type,
            sortOrdersBy
        );
    }


    public IEnumerable<int> GetOrdersStatusCounts(int applicantId)//מתודת בקשת סיכום כמויות הזמנות  done
    {
        return OrderManager.GetOrdersStatusCountsInternal(applicantId);
    }


    public void HandleOrder(int applicantId, int courierId, int orderId)// done
    {
        // 1. בדיקת הרשאה – רק השליח עצמו יכול לבחור הזמנה
        if (applicantId != courierId)
            throw new BlUnauthorizedAccessException(
                "Only the assigned courier can handle an order.");

        try
        {
            var dal = AdminManager.GetDal();

            // 2. בדיקה שההזמנה קיימת
            DO.Order order = dal.Order.Read(orderId);

            // 3. בדיקה שאין משלוח פעיל להזמנה (כלומר הזמנה פתוחה)
            bool hasActiveDelivery = dal.Delivery.ReadAll()
                .Any(d => d.OrderId == orderId && d.DeliveryEndTime == null);

            if (hasActiveDelivery)
                throw new BlInvalidStatusException(
                    "The order is already being handled.");

            // 4. יצירת משלוח חדש
            DO.Delivery newDelivery = new DO.Delivery
            {
                Id = 0, // נוצר אוטומטית ב-DAL
                OrderId = orderId,
                CourierId = courierId,
                DeliveryStartTime = DateTime.Now,
                DeliveryEndTime = null,
                DeliveryTermintionType = null,
                ActualDistance = null
            };

            // 5. שמירה ב-DAL
            dal.Delivery.Create(newDelivery);
        }
        catch (DalDoesNotExistException ex)
        {
            throw new BlDoesNotExistException(
                $"Order with ID {orderId} does not exist.", ex);
        }
        catch (DalXMLFileLoadCreateException ex)
        {
            throw new BlDataAccessException(
                "Failed to access data layer while handling order.", ex);
        }
    }


    public void UpdateDetails(int applicantId, BO.Order boOrder)  //done
    {
        if (!AdminManager.IsValidManagerId(applicantId))
            throw new BO.BlUnauthorizedAccessException("Only admin can update an order.");

        if (boOrder is null)
            throw new BO.BlArgumentNullException("Order cannot be null.");

        try
        {
            OrderManager.UpdateOrderDetails(boOrder);
        }
        catch (DO.DalDoesNotExistException ex)
        {
            throw new BO.BlDoesNotExistException(
                $"Order with ID {boOrder.ID} does not exist.", ex);
        }
        catch (DO.DalXMLFileLoadCreateException ex)
        {
            throw new BO.BlDataAccessException(
                "Failed to access data layer while updating order.", ex);
        }
    }

    public void AddObserver(Action listObserver) =>
OrderManager.Observers.AddListObserver(listObserver); //stage 5
    public void AddObserver(int id, Action observer) =>
OrderManager.Observers.AddObserver(id, observer); //stage 5
    public void RemoveObserver(Action listObserver) =>
OrderManager.Observers.RemoveListObserver(listObserver); //stage 5
    public void RemoveObserver(int id, Action observer) =>
CourierManager.Observers.RemoveObserver(id, observer); //stage 5

}
