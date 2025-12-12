using DalApi;
using DO;

namespace Dal;
internal class CourierImplementation : ICourier
{
    public void Create(Courier item)
    {
        // בדיקה אם השליח כבר קיים
        if (DataSource.Couriers.Any(c => c.Id == item.Id))
            throw new InvalidOperationException("Courier with the same Id already exists.");

        // הוספת השליח למאגר
        DataSource.Couriers.Add(item);
    }

    public void Delete(int id)
    {
        Courier? courier = DataSource.Couriers.FirstOrDefault(c => c.Id == id);
        if (courier is null)
            throw new InvalidOperationException("Courier with the given Id does not exist.");
        DataSource.Couriers.Remove(courier);
    }

    public void DeleteAll() => DataSource.Couriers.Clear();


    public Courier? Read(int id) =>
        DataSource.Couriers.Find(c => c.Id == id);

    public List<Courier> ReadAll()
    {
        return new List<Courier>(DataSource.Couriers);
    }
    public IEnumerable<Courier> ReadAll(Func<Courier, bool>? filter = null) //stage 2
         => filter == null
              ? DataSource.Couriers.Select(item => item)
              : DataSource.Couriers.Where(filter);

    public void Update(Courier item)
    {
        Delete(item.Id);
        DataSource.Couriers.Add(item);
    }

    Courier? ICrud<Courier>.Read(Func<Courier, bool> filter)
    {
        return DataSource.Couriers.FirstOrDefault(filter);
        // הערה: שורה זו (throw) לא ניתנת להשגה לאחר שורת ה-return שלפניה. 
        // מומלץ להסיר אותה אם ה-return הוא הפונקציה הממומשת.
        throw new NotImplementedException();
    }
}