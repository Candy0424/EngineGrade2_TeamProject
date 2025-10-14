namespace Work.CUH.Code.Commandable
{
    public interface ISwitch
    {
        void Activate();
        void UnActivate();
        
        bool isActive { get; }
    }
}