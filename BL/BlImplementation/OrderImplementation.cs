namespace BlImplementation;

using BlApi;
using BO;
using DalApi;
using DO;
using Helpers;
using System.Collections.Generic;

internal class OrderImplementation : BlApi.IOrder
{

   /// <summary>
   /// Adds a new order for the specified applicant.
   /// </summary>
   /// <param name="applicantId">The unique identifier of the applicant for whom the order is being added. Must correspond to a valid
   /// administrator.</param>
   /// <param name="boOrder">The order to add. Cannot be <see langword="null"/>.</param>
   /// <exception cref="BlUnauthorizedAccessException">Thrown if <paramref name="applicantId"/> does not correspond to a valid administrator.</exception>
   /// <exception cref="BlArgumentNullException">Thrown if <paramref name="boOrder"/> is <see langword="null"/>.</exception>
   /// <exception cref="BlAlreadyExistsException">Thrown if an order with the same identifier already exists in the system.</exception>
   /// <exception cref="BlDataAccessException">Thrown if a data access error occurs while adding the order.</exception>

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
            OrderManager.AddOrder( boOrder);
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


   /// <summary>
   /// Deletes the specified order if the applicant has administrative privileges.
   /// </summary>
   /// <param name="applicantId">The identifier of the user attempting to delete the order. Must represent a valid administrator.</param>
   /// <param name="orderId">The identifier of the order to delete.</param>
   /// <exception cref="BlUnauthorizedAccessException">Thrown if <paramref name="applicantId"/> does not correspond to a valid administrator.</exception>
   /// <exception cref="BlDoesNotExistException">Thrown if the specified order does not exist or cannot be deleted.</exception>
    public void Delete(int applicantId, int orderId) 
    {
        if (!AdminManager.IsValidManagerId(applicantId))
            throw new BlUnauthorizedAccessException("Only admin can delete a order.");
        try
        {
            // delete order via DAL   
            OrderManager.DeleteOrder(orderId);
        }
        catch (DalDoesNotExistException ex)
        {
            // translate unexpected DAL exceptions to a BL-level exception while preserving the inner exception
            throw new BlDoesNotExistException("Failed to delete order.", ex);
        }
    }

    public IEnumerable<OrderInList> GetOrderList(int applicantId,     //done
       filterOrdersByProperty? filterOrdersBy = null, object? type = null, sortOrdersByProperty? sortOrdersBy = null)
    {
        if (!AdminManager.IsValidManagerId(applicantId))
            throw new BO.BlUnauthorizedAccessException("Only admin can view order list.");

        return OrderManager.GetOrderList(
            applicantId,
            filterOrdersBy,
            type,
            sortOrdersBy
        );
    }



    /// <summary>
    /// Cancels the specified order on behalf of an administrator.
    /// </summary>
    /// <remarks>Only users with administrative privileges are permitted to cancel orders. Attempting to
    /// cancel a non-existent order or an order in an invalid state will result in an exception.</remarks>
    /// <param name="applicantId">The unique identifier of the user requesting the cancellation. Must be a valid administrator ID.</param>
    /// <param name="orderId">The unique identifier of the order to cancel.</param>
    /// <exception cref="BO.BlUnauthorizedAccessException">Thrown if <paramref name="applicantId"/> does not correspond to a valid administrator.</exception>
    /// <exception cref="BO.BlDoesNotExistException">Thrown if an order with the specified <paramref name="orderId"/> does not exist.</exception>
    /// <exception cref="BO.BlDataAccessException">Thrown if a data access error occurs while attempting to cancel the order.</exception>
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


    /// <summary>
    /// Marks the specified delivery as successfully completed by the assigned courier.
    /// </summary>
    /// <remarks>Only the courier assigned to the delivery is authorized to end the order handling. This
    /// method updates the delivery's status and completion time.</remarks>
    /// <param name="applicantId">The ID of the user attempting to end the order handling. Must match the assigned courier's ID.</param>
    /// <param name="courierId">The ID of the courier assigned to the delivery.</param>
    /// <param name="orderId">The ID of the order associated with the delivery.</param>
    /// <param name="deliveryId">The ID of the delivery to be marked as completed.</param>
    /// <exception cref="BlUnauthorizedAccessException">Thrown if the applicant is not the assigned courier, or if the courier is not authorized to end this delivery.</exception>
    /// <exception cref="BlDoesNotExistException">Thrown if the specified delivery does not exist.</exception>
    /// <exception cref="BlDataAccessException">Thrown if an error occurs while accessing the data layer.</exception>
    public void EndOrderHandle(int applicantId, int courierId, int orderId, int deliveryId)// done
    {
        // the applicant must be the courier himself
        if (applicantId != courierId)
            throw new BlUnauthorizedAccessException(
                "Only the assigned courier can end order handling.");

        try
        {
            // DO/order from dal
            DO.Delivery? delivery = DeliveryManager.Read(deliveryId);

            
            if (delivery.OrderId != orderId || delivery.CourierId != courierId)
                throw new BlUnauthorizedAccessException(
                    "The courier is not authorized to end this delivery.");

            // update delivery
            DO.Delivery updatedDelivery = delivery with
            {
                DeliveryEndTime = DateTime.Now,
                DeliveryTermintionType = DO.DeliveryTermintionType.DeliveredSeccessfully
            };

            // saving to dal
            DeliveryManager .Update(updatedDelivery);
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


    /// <summary>
    /// Retrieves a collection of closed deliveries assigned to a specific courier, with optional filtering by order
    /// type and sorting criteria.
    /// </summary>
    /// <remarks>Only the courier themselves is authorized to view their delivery history. Attempting to
    /// access another courier's history will result in an exception.</remarks>
    /// <param name="applicantId">The ID of the user requesting the delivery history. Must match <paramref name="courierId"/> to authorize access.</param>
    /// <param name="courierId">The ID of the courier whose closed deliveries are to be retrieved.</param>
    /// <param name="filterOrderByType">An optional order type to filter the closed deliveries. If specified, only deliveries of this order type are
    /// included.</param>
    /// <param name="byProperty">An optional property by which to sort the results. If not specified, deliveries are sorted by delivery
    /// termination type.</param>
    /// <returns>An enumerable collection of <see cref="ClosedDeliveryInList"/> objects representing the closed deliveries for
    /// the specified courier. The collection may be empty if no closed deliveries are found.</returns>
    /// <exception cref="BlUnauthorizedAccessException">Thrown if <paramref name="applicantId"/> does not match <paramref name="courierId"/>.</exception>
    /// <exception cref="BlDoesNotExistException">Thrown if a referenced delivery or order does not exist in the data store.</exception>
    /// <exception cref="BlDataAccessException">Thrown if there is a failure accessing the data layer.</exception>
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
           
            // כל המשלוחים הסגורים של השליח
            var closedDeliveries = DeliveryManager.GetList_DelivriesPerCourier(courierId)
                    .Where(d => d.DeliveryEndTime is not null);

            ////  חיבור להזמנה
            var result = closedDeliveries.Select(d =>
            {
                return new ClosedDeliveryInList
                {
                    DeliveryId = d.Id,
                    OrderId = d.Id,
                    OrderType = (BO.OrderType)  
                            OrderManager.GetOrderDetails( d.Id).OrderType ,
                    AddressOfDelivery = OrderManager.GetOrderDetails( d.Id).FullAddressOfTheOrder,
                    DeliveryType = (BO.DeliveryType)d.DeliveryType,
                    ActualDistance = d.ActualDistance ?? 0,
                    TotalHandlingTime = (TimeSpan)( d.DeliveryEndTime - d.DeliveryStartTime),
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
        if (applicantId != courierId && applicantId != AdminManager.ManagerID)
            throw new BlUnauthorizedAccessException(
                "Only the courier can view open orders.");

        try
        {
            //  שליפת השליח
            DO.Courier courier = CourierManager.Read(courierId) ??
                 throw new BO.BlDoesNotExistException($"Courier {courierId} not found");


            //  כל ההזמנות שאין להן משלוח פעיל

            //  המרה ל-OpenOrderInList
            var result = OrderManager.ReadAll().Select(x => OrderManager.ConvertToOpenOrderInList(x));
              
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

            // 2. בדיקה שההזמנה קיימת
            BO.Order order = OrderManager.GetOrderDetails(orderId);

            // 3. בדיקה שאין משלוח פעיל להזמנה (כלומר הזמנה פתוחה)
            if (DeliveryManager.GetLastDelivery(orderId) is null)
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
                DeliveryTermintionType = DO.DeliveryTermintionType.None,
                ActualDistance = null
            };

            // 5. שמירה ב-DAL
           DeliveryManager.Create(newDelivery);
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
            OrderManager.UpdateOrder(boOrder);
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
