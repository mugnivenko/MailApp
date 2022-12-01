namespace MailApp.State;

public class UserState
{
    private Guid? enteredUser;

    public Guid? Id
    {
        get => enteredUser;
        set
        {
            enteredUser = value;
            NotifyStateChanged();
        }
    }

    public string? Name { get; set; }

    public event Action? OnChange;

    private void NotifyStateChanged() => OnChange?.Invoke();
}