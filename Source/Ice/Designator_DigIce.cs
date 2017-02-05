using RimWorld;
using UnityEngine;
using Verse;

namespace Ice
{
    public class Designator_DigIce : Designator
    {
        public override int DraggableDimensions
        {
            get
            {
                return 2;
            }
        }

        public override bool DragDrawMeasurements
        {
            get
            {
                return true;
            }
        }

        public Designator_DigIce() : base()
        {
            defaultLabel = Translator.Translate("digsand");
            defaultDesc = Translator.Translate("digsanddesc");
            icon = ContentFinder<Texture2D>.Get("UI/shovel", true);
            useMouseIcon = true;
            soundDragSustain = SoundDefOf.DesignateDragStandard;
            soundDragChanged = SoundDefOf.DesignateDragStandardChanged;
            soundSucceeded = SoundDefOf.DesignateSmoothFloor;
            hotKey = KeyBindingDefOf.Misc1;
        }

        public override AcceptanceReport CanDesignateCell(IntVec3 c)
        {
            if (!GenGrid.InBounds(c, Map) || GridsUtility.Fogged(c, Map) || Map.designationManager.DesignationAt(c, DefDatabase<DesignationDef>.GetNamed("DoDigIce", true)) != null)
                return false;

            if (GenGrid.InNoBuildEdgeArea(c, Map))
                return Translator.Translate("TooCloseToMapEdge");

            var edifice = GridsUtility.GetEdifice(c, Map);
            if (edifice != null && edifice.def.Fillage == FillCategory.Full && edifice.def.passability == Traversability.Impassable)
                return false;

            if (Map.terrainGrid.TerrainAt(c).defName != "Ice")
                return Translator.Translate("mustbesand");
            return AcceptanceReport.WasAccepted;
        }

        public override void DesignateSingleCell(IntVec3 c)
        {
            Map.designationManager.AddDesignation(new Designation(c, DefDatabase<DesignationDef>.GetNamed("DoDigIce", true)));
        }

        public override void SelectedUpdate()
        {
            GenUI.RenderMouseoverBracket();
        }
    }
}
