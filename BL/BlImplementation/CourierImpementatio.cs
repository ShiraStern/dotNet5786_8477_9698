
namespace BlImplementation;
using BlApi;
using BO;
using DO;
//using BO;
//using BO;
using Helpers;
using System.Collections.Generic;
using System.Reflection.Metadata;
using System.Reflection.Metadata.Ecma335;

internal class CourierImpementation : ICourier
{
    public void AddCourier(int applicantId, BO.Courier boCourier)
    {
        // authorization
        if (!AdminManager.IsValidManagerId(applicantId))
            throw new BO.BlUnauthorizedAccessException("Only admin can add a courier.");


        // basic null check
        if (boCourier is null)
            throw new BO.BlArgumentNullException(nameof(boCourier));

        try
        {
            // create DO and persist via DAL
            var doCourier = CourierManager.ConvertToCourier(boCourier);
            CourierManager.GetDal().Courier.Create(doCourier);
        }
        catch (DalAlreadyExistsException ex)
        {
            // translate unexpected DAL exceptions to a BL-level exception while preserving the inner exception
            throw new BlAlreadyExistsException("Failed to add courier.", ex);
        }
        
    }

    public void Delete(int applicantId, int id)
    {
        if (!AdminManager.IsValidManagerId(applicantId))
            throw new BlUnauthorizedAccessException("Only admin can delete a courier.");
        try
        {
            // delete courier via DAL   
            CourierManager.GetDal().Courier.Delete(id);
        }
        catch (DalDoesNotExistException ex)
        {
            // translate unexpected DAL exceptions to a BL-level exception while preserving the inner exception
            throw new BlDoesNotExistException("Failed to delete courier.", ex);
        }

    }

    public IEnumerable<BO.CourierInList> GetCourierList(
        int applicantId, 
        bool isActive, 
        BO.sortCouriersByProperty? sortCouriersBy)
    {

        if (!AdminManager.IsValidManagerId(applicantId) && !CourierManager.IsValidCourierId(applicantId))
            throw new BlUnauthorizedAccessException("Only admin and courier can view the courier's list.");
        try
        {
            // get all couriers from DAL
            var dalCouriers = CourierManager.GetDal().Courier.ReadAll().Where(c => c.Active == isActive);   
            // convert to BO list
            var boCouriers = from dalCourier in dalCouriers
                             select new BO.CourierInList()
                             {
                                 ID = dalCourier.Id,
                                 FullName = dalCourier.FullName,
                                 Active = dalCourier.Active,
                                 DeliveryType = (BO.DeliveryType)dalCourier.DeliveryType,
                                 EmploymentStartDate = dalCourier.EmploymentStartDate,
                                 NumOfDeliveriesOnTime= CourierManager.GetNumOfDeliveriesOnTime(dalCourier.Id),
                                 NumOfDeliveriesNotOnTime= CourierManager.GetNumOfDeliveriesNotOnTime(dalCourier.Id),
                                 NumberOfDeliveriesInProcess = CourierManager.GetNumberOfDeliveriesInProcess(dalCourier.Id)
                             };
            // apply sorting if requested
            if (sortCouriersBy.HasValue)
            {
                boCouriers = sortCouriersBy.Value switch
                {
                    // להחליט לפי מה למיין את השליחים
                    //sortCouriersByProperty.IsActive => boCouriers.OrderBy(c => c.ID),
                    //sortCouriersByProperty.NumberOfDeliveries => boCouriers.OrderBy(c => c.FullName)
                    // => boCouriers
                };
            }
            return boCouriers;
        }
        catch (DalXMLFileLoadCreateException ex)
        {
            // translate unexpected DAL exceptions to a BL-level exception while preserving the inner exception
            throw new BlDataAccessException("Failed to read courier's data.", ex);
        }
    }

   

    public BO.Courier GetDetails(int applicantId, int courierId)
    {
        if (!AdminManager.IsValidManagerId(applicantId) && !CourierManager.IsValidCourierId(applicantId))
            throw new BlUnauthorizedAccessException("Only admin and courier can view courier's details.");
        try
        {
            DO.Courier courier
                = CourierManager.GetDal().Courier.ReadAll().FirstOrDefault(C => C.Id == courierId) 
                ?? throw new BlDoesNotExistException($"coulden't find courier with ID:{courierId}");
            return CourierManager.ConvertToCourier(courier);    
        }
        catch (DalXMLFileLoadCreateException ex)
        {
            // translate unexpected DAL exceptions to a BL-level exception while preserving the inner exception
            throw new BlDataAccessException("Failed to read courier's data.", ex);
        }
    }

    
    public string Login(string userName, string password)
    {
        try
        {
            DO.Courier? courier = CourierManager.GetDal().Courier.ReadAll().FirstOrDefault(c => c.FullName == userName) ?? null;
            if (courier is null)
                throw new BlDoesNotExistException($"couldent find courier or manager with the name:{userName}");
            if (courier.Password != password)
                throw new BlInvalidPasswordException($"Incorrect password");
            if (courier.Id == AdminManager.GetConfig().ManagerID)
                return "Manager";
            return "Courier";
        }
        catch (DalXMLFileLoadCreateException ex)
        {
            // translate unexpected DAL exceptions to a BL-level exception while preserving the inner exception
            throw new BlDataAccessException("Failed to read courier's data.", ex);
        }
    }

    public void UpdateDetails(int applicantId, BO.Courier boCourier)
    {
        // authorization
        if (!AdminManager.IsValidManagerId(applicantId)  || !CourierManager.IsValidCourierId(applicantId))
            throw new UnauthorizedAccessException("Only admin or coureir can update courier's ditails.");

        // basic null check
        if (boCourier is null)
            throw new ArgumentNullException(nameof(boCourier));

        try
        {
            // create DTO/DO and persist via DAL
            DO.Courier doCourier = CourierManager.ConvertToCourier(boCourier);
            CourierManager.GetDal().Courier.Update(doCourier);
        }
        catch (DalDoesNotExistException ex)
        {
            throw new BlDoesNotExistException($"couldent find courier or manager with the name:{boCourier.FullName}", ex);
        }
    }
}
