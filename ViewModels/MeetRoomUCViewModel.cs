using Prism.Mvvm;
using Prism.Services.Dialogs;

namespace Meet.ViewModels;

internal class MeetRoomUCViewModel : BindableBase, IDialogAware
{
    public string Title => throw new NotImplementedException();

    public event Action<IDialogResult> RequestClose;

    public bool CanCloseDialog()
    {
        throw new NotImplementedException();
    }

    public void OnDialogClosed()
    {
        throw new NotImplementedException();
    }

    public void OnDialogOpened(IDialogParameters parameters)
    {
        throw new NotImplementedException();
    }
}