namespace BlImplementation;

using BlApi;
using BO;
using DalApi;
using DO;
using Helpers;
using System.Collections.Generic;

internal class OrderImplementation : BlApi.IOrder
{
    // Adds a new order to the system
    public void AddOrder(int applicantId, BO.Order boOrder)
    {
        // authorization
        if (!AdminManager.IsValidManagerId(applicantId))
            throw new BO.BlUnauthorizedAccessException("Only admin can add an order.");
        OrderManager.AddOrder(applicantId, boOrder);
    }

    //// Cancels an existing order (not implemented at this stage)
    //public void cancelOrder(int applicantId, int orderId)
    //{

    //    if(!AdminManager.IsValidManagerId(applicantId))
    //    {
    //        throw new BO.BlUnauthorizedAccessException("Only managers can cancel orders at this stage");
    //    }
    //    try
    //    {
    //        DO.Order? order = AdminManager.GetDal().Order.Read(orderId);
    //        BO.Order? boOrder = OrderManager.GetBoOrder(order);
    //        if (boOrder is null)
    //        {
    //            throw new BlArgumentNullException($"couldnt find any order with Id:{orderId}");
    //        }
    //        else
    //        {
    //        }

    //    }
    //    catch (DalXMLFileLoadCreateException ex)
    //    {
    //        throw new BlDataAccessException($"fail to read order with Id:{orderId} ", ex);
    //    }

    //}

    //// Deletes an order from the system
    //public void Delete(int applicantId, int id)
    //{
    //    OrderManager.DeleteOrder(applicantId, id);
    //}

    //// Assigns an order to a courier (handled in delivery stage)
    //public void HandleOrder(int applicantId, int courierId, int orderId)
    //{
    //    throw new NotImplementedException("Handle order logic is implemented in Delivery stage");
    //}

    //// Completes order handling after delivery (handled in delivery stage)
    //public void EndOrderHandle(int applicantId, int courierId, int orderId, int deliveryId)
    //{
    //    throw new NotImplementedException("End order handle logic is implemented in Delivery stage");
    //}

    //// Returns full details of a specific order
    //public Order GetDetails(int applicantId, int orderId)
    //{
    //    return OrderManager.GetOrderDetails(applicantId, orderId);
    //}

    //// Returns a list of orders with optional filtering and sorting
    //public IEnumerable<OrderInList> GetOrderList(
    //    int applicantId,
    //    filterOrdersByProperty? filterOrdersBy = null,
    //    object? type = null,
    //    sortOrdersByProperty? sortOrdersBy = null)
    //{
    //    return OrderManager.GetOrderList(applicantId, filterOrdersBy, type, sortOrdersBy);
    //}

    //// Returns counts of orders grouped by status
    //public IEnumerable<int> GetOrdersStatusCounts(int applicantId)
    //{
    //    return OrderManager.GetOrdersStatusCounts(applicantId);
    //}

    //// Returns open deliveries for a specific courier (handled in delivery stage)
    //public IEnumerable<OpenOrderInList> GetDeliveriesPerCourier(
    //    int applicantId,
    //    int courierId,
    //    BO.OrderType? filterOrderByType = null,
    //    sortClosedDeliveriesByProperty? byProperty = null)
    //{
    //    throw new NotImplementedException("Open deliveries per courier are handled in Delivery stage");
    //}

    //// Returns closed deliveries for a specific courier (handled in delivery stage)
    //public IEnumerable<ClosedDeliveryInList> GetClosedDeliveriesPerCourier(
    //    int applicantId,
    //    int courierId,
    //    BO.OrderType? filterOrderByType = null,
    //    sortClosedDeliveriesByProperty? byProperty = null)
    //{
    //    throw new NotImplementedException("Closed deliveries per courier are handled in Delivery stage");
    //}

    public void cancelOrder(int applicantId, int orderId)
    {
        throw new NotImplementedException();
    }

    public void Delete(int applicantId, int id)
    {
        // authorization
        if (!AdminManager.IsValidManagerId(applicantId))
            throw new BO.BlUnauthorizedAccessException("Only admin can add a courier.");
        OrderManager.DeleteOrder(applicantId, id);
    }

    public void EndOrderHandle(int applicantId, int courierId, int orderId, int deliveryId)
    {
        throw new NotImplementedException();
    }

    public IEnumerable<ClosedDeliveryInList> GetClosedDeliveriesPerCourier(int applicantId, int courierId, BO.OrderType? filterOrderByType = null, sortClosedDeliveriesByProperty? byProperty = null)
    {
        throw new NotImplementedException();
    }

    public IEnumerable<OpenOrderInList> GetDeliveriesPerCourier(int applicantId, int courierId, BO.OrderType? filterOrderByType = null, sortClosedDeliveriesByProperty? byProperty = null)
    {
        throw new NotImplementedException();
    }

    public BO.Order GetDetails(int applicantId, int orderId)
    {
        throw new NotImplementedException();
    }

    public IEnumerable<OrderInList> GetOrderList(int applicantId, filterOrdersByProperty? filterOrdersBy = null, object? type = null, sortOrdersByProperty? sortOrdersBy = null)
    {
        throw new NotImplementedException();
    }

    public IEnumerable<int> GetOrdersStatusCounts(int applicantId)
    {

        throw new NotImplementedException();
    }

    public void HandleOrder(int applicantId, int courierId, int orderId)
    {
        throw new NotImplementedException();
    }

    public void UpdateDetails(int applicantId, BO.Order boOrder)
    {
        if (!AdminManager.IsValidManagerId(applicantId))
            throw new BO.BlUnauthorizedAccessException("Only admin can update an order.");
        try
        {
            DO.Order? oldOrder = AdminManager.GetDal().Order.Read(boOrder.ID);
            if (oldOrder is null)
                throw new BlDoesNotExistException($"DO.Order with ID:{boOrder.ID} does not exist");
            DO.Order updatedOrder = oldOrder with
            {
                OrderType = (DO.OrderType)boOrder.OrderType,
                OrderNote = boOrder.VerbalDescription ?? "",
                Latitude = boOrder.Latitude!,
                Longitude= boOrder.Longitude!,
                CustomerAddress = boOrder.FullAddressOfTheOrder ?? oldOrder.CustomerAddress,
                CustomerFullName = boOrder.FullNameOfTheInviter ?? oldOrder.CustomerFullName,
                CustomerPhone = boOrder.OrderersPhoneNumber ?? oldOrder.CustomerPhone,
                OrderProperties = (DO.OrderProperties)boOrder.OrderProperties
                OrderDate= boOrder.OrderOpeningTime
            };

            AdminManager.GetDal().Order.Update(updatedOrder);
        }
        catch(DalXMLFileLoadCreateException)
        {
             throw new BO.BlDataAccessException("Failed to access data layer while updating order.");
        }
        catch(DalDoesNotExistException)
        {
            throw new BO.BlDoesNotExistException($"Order with ID:{boOrder.ID} does not exist.");
        }
    }
}
