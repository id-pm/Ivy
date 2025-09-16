using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using Ivy.Shared;
using Ivy.Core;
using Ivy.Core.Docs;

// ReSharper disable once CheckNamespace
namespace Ivy;

/// <summary>Dropdown menu widget providing interactive menu options with customizable positioning, alignment, and selection handling.</summary>
public record DropDownMenu : WidgetBase<DropDownMenu>
{
    /// <summary>Available positions where dropdown menu can appear relative to trigger element.</summary>
    public enum SideOptions
    {
        /// <summary>Position dropdown menu above trigger element.</summary>
        Top,
        /// <summary>Position dropdown menu to the right of trigger element.</summary>
        Right,
        /// <summary>Position dropdown menu below trigger element. Default behavior.</summary>
        Bottom,
        /// <summary>Position dropdown menu to the left of trigger element.</summary>
        Left
    }

    /// <summary>Alignment options for positioning dropdown menu relative to trigger element.</summary>
    public enum AlignOptions
    {
        /// <summary>Align dropdown menu to start of trigger element (left for horizontal, top for vertical).</summary>
        Start,
        /// <summary>Center dropdown menu relative to trigger element.</summary>
        Center,
        /// <summary>Align dropdown menu to end of trigger element (right for horizontal, bottom for vertical).</summary>
        End
    }

    /// <summary>Initializes DropDownMenu with specified selection handler, trigger element, and menu items.</summary>
    /// <param name="onSelect">Event handler called when menu item is selected.</param>
    /// <param name="trigger">Element that triggers dropdown menu when clicked or activated.</param>
    /// <param name="items">MenuItem collections defining menu structure and content.</param>
    [OverloadResolutionPriority(1)]
    public DropDownMenu(Func<Event<DropDownMenu, object>, ValueTask> onSelect, object trigger, params IEnumerable<MenuItem> items) : base([new Slot("Trigger", trigger)])
    {
        OnSelect = onSelect;
        Items = items.ToArray();
    }

    /// <summary>Provides default selection handler that automatically processes menu item selections using their built-in handlers.</summary>
    /// <returns>Default event handler that processes menu item selections automatically.</returns>
    public static Func<Event<DropDownMenu, object>, ValueTask> DefaultSelectHandler()
    {
        return (@evt) =>
        {
            @evt.Sender.Items.GetSelectHandler(@evt.Value)?.Invoke();
            return ValueTask.CompletedTask;
        };
    }

    /// <summary>Compatibility constructor for Action-based event handlers.</summary>
    /// <param name="onSelect">Action-based event handler called when menu item is selected.</param>
    /// <param name="trigger">Element that triggers dropdown menu when clicked or activated.</param>
    /// <param name="items">MenuItem collections defining menu structure.</param>
    public DropDownMenu(Action<Event<DropDownMenu, object>> onSelect, object trigger, params IEnumerable<MenuItem> items)
        : this(e => { onSelect(e); return ValueTask.CompletedTask; }, trigger, items)
    {
    }

    /// <summary>Array of menu items making up dropdown menu content including options, separators, and nested submenus.</summary>
    [Prop] public MenuItem[] Items { get; set; }

    /// <summary>Side where dropdown menu appears relative to trigger. Default is <see cref="SideOptions.Bottom"/>.</summary>
    [Prop] public SideOptions Side { get; set; } = SideOptions.Bottom;

    /// <summary>Offset distance from trigger element in side direction. Default is 8 pixels.</summary>
    [Prop] public int SideOffset { get; set; } = 8;

    /// <summary>Alignment of dropdown menu relative to trigger. Default is <see cref="AlignOptions.Start"/>.</summary>
    [Prop] public AlignOptions Align { get; set; } = AlignOptions.Start;

    /// <summary>Offset distance from trigger element in alignment direction. Default is 0 pixels.</summary>
    [Prop] public int AlignOffset { get; set; } = 0;

    /// <summary>Event handler called when menu item is selected.</summary>
    [Event] public Func<Event<DropDownMenu, object>, ValueTask> OnSelect { get; set; }

    /// <summary>Allows adding MenuItem objects using pipe operator for convenient menu construction.</summary>
    /// <param name="widget">DropDownMenu to add menu item to.</param>
    /// <param name="child">MenuItem to add to dropdown menu.</param>
    /// <returns>New DropDownMenu instance with additional menu item appended.</returns>
    /// <exception cref="NotSupportedException">Thrown when adding non-MenuItem children.</exception>
    public static DropDownMenu operator |(DropDownMenu widget, object child)
    {
        if (child is MenuItem menuItem)
        {
            return widget with { Items = widget.Items.Append(menuItem).ToArray() };
        }

        throw new NotSupportedException("DropDownMenu does not support children other then MenuItem.");
    }
}

/// <summary>Extension methods for DropDownMenu widget providing fluent API for configuring appearance, positioning, and behavior.</summary>
public static class DropDownMenuExtensions
{
    /// <summary>Creates dropdown menu from button with specified menu items using default selection handler.</summary>
    /// <param name="button">Button that will trigger dropdown menu.</param>
    /// <param name="items">Array of MenuItem objects defining dropdown menu structure.</param>
    /// <returns>New DropDownMenu instance with button as trigger and default selection handling.</returns>
    public static DropDownMenu WithDropDown(this Button button, params MenuItem[] items)
    {
        return new DropDownMenu(DropDownMenu.DefaultSelectHandler(), button, items);
    }

    /// <summary>Adds header section to dropdown menu for informational content.</summary>
    /// <param name="dropDownMenu">DropDownMenu to add header to.</param>
    /// <param name="header">Header content to display at top of dropdown menu.</param>
    /// <returns>New DropDownMenu instance with header content added.</returns>
    public static DropDownMenu Header(this DropDownMenu dropDownMenu, object header)
    {
        return dropDownMenu with { Children = [.. dropDownMenu.Children, new Slot("Header", header)] };
    }

    /// <summary>Sets alignment of dropdown menu relative to trigger.</summary>
    /// <param name="dropDownMenu">DropDownMenu to configure.</param>
    /// <param name="align">Alignment option to apply to dropdown menu.</param>
    /// <returns>New DropDownMenu instance with updated alignment setting.</returns>
    public static DropDownMenu Align(this DropDownMenu dropDownMenu, DropDownMenu.AlignOptions align)
    {
        return dropDownMenu with { Align = align };
    }

    /// <summary>Sets alignment offset for dropdown menu positioning.</summary>
    /// <param name="dropDownMenu">DropDownMenu to configure.</param>
    /// <param name="offset">Offset distance in pixels to apply in alignment direction.</param>
    /// <returns>New DropDownMenu instance with updated alignment offset.</returns>
    public static DropDownMenu AlignOffset(this DropDownMenu dropDownMenu, int offset)
    {
        return dropDownMenu with { AlignOffset = offset };
    }

    /// <summary>Sets side where dropdown menu appears relative to trigger.</summary>
    /// <param name="dropDownMenu">DropDownMenu to configure.</param>
    /// <param name="side">Side option to apply for menu positioning.</param>
    /// <returns>New DropDownMenu instance with updated side setting.</returns>
    public static DropDownMenu Side(this DropDownMenu dropDownMenu, DropDownMenu.SideOptions side)
    {
        return dropDownMenu with { Side = side };
    }

    /// <summary>Sets side offset for dropdown menu positioning.</summary>
    /// <param name="dropDownMenu">DropDownMenu to configure.</param>
    /// <param name="offset">Offset distance in pixels to apply in side direction.</param>
    /// <returns>New DropDownMenu instance with updated side offset.</returns>
    public static DropDownMenu SideOffset(this DropDownMenu dropDownMenu, int offset)
    {
        return dropDownMenu with { SideOffset = offset };
    }

    /// <summary>Sets dropdown menu to appear above trigger element.</summary>
    /// <param name="dropDownMenu">DropDownMenu to configure.</param>
    /// <returns>New DropDownMenu instance positioned above trigger.</returns>
    [RelatedTo(nameof(DropDownMenu.Side))]
    public static DropDownMenu Top(this DropDownMenu dropDownMenu)
    {
        return dropDownMenu with { Side = DropDownMenu.SideOptions.Top };
    }

    /// <summary>Sets dropdown menu to appear to the right of trigger element.</summary>
    /// <param name="dropDownMenu">DropDownMenu to configure.</param>
    /// <returns>New DropDownMenu instance positioned to the right of trigger.</returns>
    [RelatedTo(nameof(DropDownMenu.Side))]
    public static DropDownMenu Right(this DropDownMenu dropDownMenu)
    {
        return dropDownMenu with { Side = DropDownMenu.SideOptions.Right };
    }

    /// <summary>Sets dropdown menu to appear below trigger element. Default behavior.</summary>
    /// <param name="dropDownMenu">DropDownMenu to configure.</param>
    /// <returns>New DropDownMenu instance positioned below trigger.</returns>
    [RelatedTo(nameof(DropDownMenu.Side))]
    public static DropDownMenu Bottom(this DropDownMenu dropDownMenu)
    {
        return dropDownMenu with { Side = DropDownMenu.SideOptions.Bottom };
    }

    /// <summary>Sets dropdown menu to appear to the left of trigger element.</summary>
    /// <param name="dropDownMenu">DropDownMenu to configure.</param>
    /// <returns>New DropDownMenu instance positioned to the left of trigger.</returns>
    [RelatedTo(nameof(DropDownMenu.Side))]
    public static DropDownMenu Left(this DropDownMenu dropDownMenu)
    {
        return dropDownMenu with { Side = DropDownMenu.SideOptions.Left };
    }

    /// <summary>Sets menu items for dropdown menu enabling dynamic menu content.</summary>
    /// <param name="dropDownMenu">DropDownMenu to configure.</param>
    /// <param name="items">Array of MenuItem objects defining new menu structure.</param>
    /// <returns>New DropDownMenu instance with updated menu items.</returns>
    public static DropDownMenu Items(this DropDownMenu dropDownMenu, MenuItem[] items)
    {
        return dropDownMenu with { Items = items };
    }

    /// <summary>Sets selection event handler for dropdown menu.</summary>
    /// <param name="dropDownMenu">DropDownMenu to configure.</param>
    /// <param name="onSelect">Event handler to call when menu item is selected.</param>
    /// <returns>New DropDownMenu instance with updated selection handler.</returns>
    [OverloadResolutionPriority(1)]
    public static DropDownMenu HandleSelect(this DropDownMenu dropDownMenu, Func<Event<DropDownMenu, object>, ValueTask> onSelect)
    {
        return dropDownMenu with { OnSelect = onSelect };
    }

    /// <summary>Sets event handler for menu item selection with Action-based handler.</summary>
    /// <param name="dropDownMenu">DropDownMenu to configure.</param>
    /// <param name="onSelect">Action-based event handler to call when menu item is selected.</param>
    /// <returns>New DropDownMenu instance with updated selection handler.</returns>
    public static DropDownMenu HandleSelect(this DropDownMenu dropDownMenu, Action<Event<DropDownMenu, object>> onSelect)
    {
        return dropDownMenu with { OnSelect = onSelect.ToValueTask() };
    }

    /// <summary>Sets event handler for menu item selection with simplified handler receiving only selected value.</summary>
    /// <param name="dropDownMenu">DropDownMenu to configure.</param>
    /// <param name="onSelect">Simplified handler receiving only selected item value.</param>
    /// <returns>New DropDownMenu instance with updated selection handler.</returns>
    public static DropDownMenu HandleSelect(this DropDownMenu dropDownMenu, Action<object> onSelect)
    {
        return dropDownMenu with { OnSelect = @event => { onSelect(@event.Value); return ValueTask.CompletedTask; } };
    }
}