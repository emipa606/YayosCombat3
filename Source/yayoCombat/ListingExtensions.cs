using Verse;

namespace yayoCombat;

public static class ListingExtensions
{
    extension(Listing_Standard listing)
    {
        public void TextFieldNumericLabeledWithTooltip(string label,
            string tooltip,
            ref int val,
            ref string buffer,
            int min,
            int max)
        {
            var rect = listing.GetRect(Text.LineHeight);
            Widgets.Label(rect.LeftHalf().Rounded(), label);
            Widgets.TextFieldNumeric(rect.RightHalf().Rounded(), ref val, ref buffer, min, max);
            if (Mouse.IsOver(rect))
            {
                TooltipHandler.TipRegion(rect, tooltip);
            }
        }

        public void TextFieldNumericLabeledWithTooltip(string label,
            string tooltip,
            ref float val,
            ref string buffer,
            float min,
            float max)
        {
            var rect = listing.GetRect(Text.LineHeight);
            Widgets.Label(rect.LeftHalf().Rounded(), label);
            Widgets.TextFieldNumeric(rect.RightHalf().Rounded(), ref val, ref buffer, min, max);
            if (Mouse.IsOver(rect))
            {
                TooltipHandler.TipRegion(rect, tooltip);
            }
        }
    }
}