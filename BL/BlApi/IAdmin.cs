namespace BlApi;
public interface IAdmin
{
    DateTime GetClock ();   
    void ForwardClock (BO.TimeUnit unit);
    void ResetDB ();  
    void InitializeDB ();
    BO.Config GetConfig (); 
    void SetConfig (BO.Config config);

    #region Stage 5
    void AddConfigObserver(Action configObserver);
    void RemoveConfigObserver(Action configObserver);
    void AddClockObserver(Action clockObserver);
    void RemoveClockObserver(Action clockObserver);
    #endregion Stage 5

    void StartSimulator(int interval);   // stage 7
    void StopSimulator();                // stage 7

}
