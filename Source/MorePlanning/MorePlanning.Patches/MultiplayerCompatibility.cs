using MorePlanning.Designators;
using MorePlanning.Plan;
using Multiplayer.API;
using Verse;

namespace MorePlanning.Patches;

[StaticConstructorOnStartup]
internal class MultiplayerCompatibility
{
    static MultiplayerCompatibility()
    {
        if(!MP.enabled)
        {
            return;
        }

        MP.RegisterAll();
        MP.RegisterSyncMethod(typeof(PlanColorManager), "ChangeColor");
        MP.RegisterSyncWorker<AddDesignator>(SyncWorkerForPlanDesignation);
    }

    private static void SyncWorkerForPlanDesignation(SyncWorker sync, ref AddDesignator inst)
    {
        if(sync.isWriting)
        {
            sync.Write(MorePlanningMod.Instance.SelectedColor);
            return;
        }

        var selectedColor = sync.Read<int>();
        MorePlanningMod.Instance.SelectedColor = selectedColor;
    }
}