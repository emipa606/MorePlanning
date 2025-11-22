using RimWorld.Planet;
using Verse;

namespace MorePlanning
{
    public class MorePlanningWorldComponent : WorldComponent
    {
        public bool PlanningVisibility = true;

        public MorePlanningWorldComponent(World world) : base(world)
        {
            // default set in ctor if needed
        }

        public override void ExposeData()
        {
            base.ExposeData();
            Scribe_Values.Look(ref PlanningVisibility, "planningVisibility", true);
        }
    }
}
