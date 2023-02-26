using System;

public struct CoolDownTimer
{
    public bool HasTimerBeenInit;
    private DateTime _start;
    private float _coolDownDuration;
    public double TotalSecondsPassedInCoolDown => (DateTime.Now - _start).TotalSeconds;
    public bool HasCoolDownFinished => TotalSecondsPassedInCoolDown > _coolDownDuration;
    
    public void StartCoolDownTimer(float totalSecondsCoolDown)
    {
        _start = DateTime.Now;
        HasTimerBeenInit = true;
        _coolDownDuration = totalSecondsCoolDown;
    } 
    public void ResetCoolDownTimer()
    {
        _start = new DateTime(0);
        HasTimerBeenInit = false;
    } 
}
