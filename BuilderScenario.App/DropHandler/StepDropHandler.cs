using BuilderScenario.App.ViewModels;
using GongSolutions.Wpf.DragDrop;
using System.Collections.ObjectModel;
using System.Windows;

public class StepDropHandler : IDropTarget
{
    public void DragOver(IDropInfo dropInfo)
    {
        if (dropInfo.Data is StepViewModel &&
            dropInfo.TargetCollection is ObservableCollection<StepViewModel>)
        {
            dropInfo.Effects = DragDropEffects.Move;
            dropInfo.DropTargetAdorner = DropTargetAdorners.Insert;
        }
    }

    public void Drop(IDropInfo dropInfo)
    {
        if (dropInfo.Data is StepViewModel source &&
            dropInfo.TargetCollection is ObservableCollection<StepViewModel> target)
        {
            int index = dropInfo.InsertIndex;

            target.Remove(source);
            target.Insert(index, source);
        }
    }
}
