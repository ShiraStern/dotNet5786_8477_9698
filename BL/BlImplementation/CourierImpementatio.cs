
namespace BlImplementation;
using BlApi;
using BO;
using DO;
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
            CourierManager.Create(doCourier);
            CourierManager.Observers.NotifyListUpdated();

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
            CourierManager.Delete(id);
            CourierManager.Observers.NotifyItemUpdated(id);
            CourierManager.Observers.NotifyListUpdated();//////!!!!!!!!!!1
            
        }
        catch (DalDoesNotExistException ex)
        {
            // translate unexpected DAL exceptions to a BL-level exception while preserving the inner exception
            throw new BlDoesNotExistException("Failed to delete courier.", ex);
        }

    }

    public IEnumerable<BO.CourierInList> GetCourierList(
        int applicantId, 
        bool? isActive, 
        BO.FilterCouriersByProperty? filterCouriersBy)
    {

        if (!AdminManager.IsValidManagerId(applicantId) && !CourierManager.IsValidCourierId(applicantId))
            throw new BlUnauthorizedAccessException("Only admin and courier can view the courier's list.");
        try
        {
            
            
            // get all couriers from DAL
            var dalCouriers = CourierManager.ReadAll();

            // convert to BO list
            var boCouriers = dalCouriers.Select(x => CourierManager.ConvertToCourierInList(x));
            // apply sorting if requested
            if (filterCouriersBy.HasValue)
            {
                boCouriers = filterCouriersBy.Value switch
                {
                    FilterCouriersByProperty.IsActive => boCouriers.Where(c => c.Active),
                    FilterCouriersByProperty.IsNotActive => boCouriers.Where(c => !c.Active),
                    FilterCouriersByProperty.All => boCouriers

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
                = CourierManager.ReadAll().FirstOrDefault(C => C.Id == courierId) 
                ?? throw new BlDoesNotExistException($"coulden't find courier with ID:{courierId}");
            return CourierManager.ConvertToCourier(courier);    
        }
        catch (DalXMLFileLoadCreateException ex)
        {
            // translate unexpected DAL exceptions to a BL-level exception while preserving the inner exception
            throw new BlDataAccessException("Failed to read courier's data.", ex);
        }
    }

    public string Login(string userId, string password)
    {
        try
        {
            if (!int.TryParse(userId, out int id))
                throw new BlArgumentNullException("ID must be numeric");

            var config = AdminManager.GetConfig();

            // בדיקת מנהל
            if (id == config.ManagerID)
            {
                if (config.ManagerPassword != password)
                    throw new BlInvalidPasswordException("Incorrect password");

                return "Manager";
            }

            // בדיקת שליח
            DO.Courier? courier =
                CourierManager.ReadAll().FirstOrDefault(c => c.Id == id);

            if (courier is null)
                throw new BlDoesNotExistException(
                    $"Could not find courier with ID: {id}");

            if (courier.Password != password)
                throw new BlInvalidPasswordException("Incorrect password");

            return "Courier";
        }
        catch (DalXMLFileLoadCreateException ex)
        {
            throw new BlDataAccessException(
                "Failed to read courier data.", ex);
        }
    }

    

    public void UpdateDetails(int applicantId, BO.Courier boCourier)
    {
 
            // authorization
            if (!AdminManager.IsValidManagerId(applicantId) && !CourierManager.IsValidCourierId(applicantId))
            throw new UnauthorizedAccessException("Only admin or coureir can update courier's ditails.");

        // basic null check
        if (boCourier is null)
            throw new ArgumentNullException(nameof(boCourier));

        try
        {
            // create DTO/DO and persist via DAL
            DO.Courier doCourier = CourierManager.ConvertToCourier(boCourier);
            CourierManager.Update(doCourier);
            CourierManager.Observers.NotifyItemUpdated(doCourier.Id);   ///////////////////!!!!!!!!!!!!!!!!!!!
        }
        catch (DalDoesNotExistException ex)
        {
            throw new BlDoesNotExistException($"couldent find courier or manager with the name:{boCourier.FullName}", ex);
        }
    }


    // Observers methods
    public void AddObserver(Action listObserver) =>
        CourierManager.Observers.AddListObserver(listObserver); //stage 5
    public void AddObserver(int id, Action observer) =>
        CourierManager.Observers.AddObserver(id, observer); //stage 5
    public void RemoveObserver(Action listObserver) =>
        CourierManager.Observers.RemoveListObserver(listObserver); //stage 5
    public void RemoveObserver(int id, Action observer) =>
        CourierManager.Observers.RemoveObserver(id, observer); //stage 5

}
