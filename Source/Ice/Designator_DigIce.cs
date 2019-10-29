using RimWorld;
using System.Collections.Generic;
using UnityEngine;
using Verse;

namespace Ice
{
    public class Designator_DigIce : Designator
    {
        public override int DraggableDimensions => 2;

        public override bool DragDrawMeasurements => true;

        protected override DesignationDef Designation => Designations.DoDigIce;

        public Designator_DigIce()
        {
            defaultLabel = "Ice.Dig".Translate();
            defaultDesc = "Ice.DigDesc".Translate();
            icon = ContentFinder<Texture2D>.Get("UI/icesaw", true);
            useMouseIcon = true;
            soundDragSustain = SoundDefOf.Designate_DragStandard;
            soundDragChanged = SoundDefOf.Designate_DragStandard_Changed;
            soundSucceeded = SoundDefOf.Designate_SmoothSurface;
            hotKey = KeyBindingDefOf.Misc1;
        }

        public override AcceptanceReport CanDesignateCell(IntVec3 c)
        {
            if (!GenGrid.InBounds(c, Map) || GridsUtility.Fogged(c, Map) || Map.designationManager.DesignationAt(c, Designation) != null)
                return false;

            if (GenGrid.InNoBuildEdgeArea(c, Map))
                return Translator.Translate("TooCloseToMapEdge");

            var edifice = GridsUtility.GetEdifice(c, Map);
            if (edifice != null && edifice.def.Fillage == FillCategory.Full && edifice.def.passability == Traversability.Impassable)
                return false;

            var terrain = Map.terrainGrid.TerrainAt(c);

            if (terrain != IceTerrain.Ice && terrain != IceTerrain.IceShallow)
                return "Ice.MustBeIce".Translate();

            return AcceptanceReport.WasAccepted;
        }

        public override void DesignateSingleCell(IntVec3 c)
        {
            Map.designationManager.AddDesignation(new Designation(c, Designation));
        }

        public override void SelectedUpdate()
        {
            GenUI.RenderMouseoverBracket();
        }

        public override void RenderHighlight(List<IntVec3> dragCells)
        {
            DesignatorUtility.RenderHighlightOverSelectableCells(this, dragCells);
        }
    }
}