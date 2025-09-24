namespace Character
{
    public interface ISelectable
    {
        bool IsSelected { get; }
        void SetSelected(bool on);
    }
}
