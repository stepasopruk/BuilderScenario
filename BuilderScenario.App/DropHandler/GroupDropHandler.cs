using BuilderScenario.App.ViewModels;
using GongSolutions.Wpf.DragDrop;
using System.Collections;
using System.Linq;

public class GroupDropHandler : DefaultDropHandler
{
    public override void Drop(IDropInfo dropInfo)
    {
        base.Drop(dropInfo);

        if (dropInfo.TargetCollection is IList list)
        {
            for (int i = 0; i < list.Count; i++)
            {
                if (list[i] is ActionGroupViewModel group)
                {
                    group.Model.Order = i;
                }
            }
        }
    }
}
