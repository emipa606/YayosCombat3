using Verse;
using UnityEngine;

namespace yayoCombat
{
    public static class ListingExtensions
    {
        public static void TextFieldNumericLabeledWithTooltip(
            this Listing_Standard listing,
            string label,
            string tooltip,
            ref int val,
            ref string buffer,
            int min,
            int max)
        {
            Rect rect = listing.GetRect(Text.LineHeight);
            Widgets.Label(rect.LeftHalf().Rounded(), label);
            Widgets.TextFieldNumeric(rect.RightHalf().Rounded(), ref val, ref buffer, min, max);
            if (Mouse.IsOver(rect))
                TooltipHandler.TipRegion(rect, tooltip);
        }

        public static void TextFieldNumericLabeledWithTooltip(
            this Listing_Standard listing,
            string label,
            string tooltip,
            ref float val,
            ref string buffer,
            float min,
            float max)
        {
            Rect rect = listing.GetRect(Text.LineHeight);
            Widgets.Label(rect.LeftHalf().Rounded(), label);
            Widgets.TextFieldNumeric(rect.RightHalf().Rounded(), ref val, ref buffer, min, max);
            if (Mouse.IsOver(rect))
                TooltipHandler.TipRegion(rect, tooltip);
        }
    }
}
