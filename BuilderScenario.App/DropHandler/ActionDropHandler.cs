using BuilderScenario.App.ViewModels;
using GongSolutions.Wpf.DragDrop;
using System.Collections.ObjectModel;
using System.Windows;

public class ActionDropHandler : IDropTarget
{
    public void DragOver(IDropInfo dropInfo)
    {
        if (dropInfo.Data is ActionViewModel &&
            dropInfo.TargetCollection is ObservableCollection<ActionViewModel>)
        {
            dropInfo.Effects = DragDropEffects.Move;
            dropInfo.DropTargetAdorner = DropTargetAdorners.Insert;
        }
    }

    public void Drop(IDropInfo dropInfo)
    {
        if (dropInfo.Data is ActionViewModel source &&
            dropInfo.TargetCollection is ObservableCollection<ActionViewModel> target)
        {
            int index = dropInfo.InsertIndex;

            target.Remove(source);
            target.Insert(index, source);
        }
    }
}
