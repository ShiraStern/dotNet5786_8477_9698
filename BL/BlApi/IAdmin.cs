namespace BlApi;
public interface IAdmin
{
    DateTime GetClock ();   
    void ForwardClock (BO.TimeUnit unit);
    void ResetDB ();  
    void InitializeDB ();
    BO.Config GetConfig (); 
    void SetConfig (BO.Config config);

}
