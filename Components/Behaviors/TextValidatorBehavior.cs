using System.Text.RegularExpressions;
using Ixjok.Components.Controls;

namespace Ixjok.Components.Behaviors;

public sealed class TextValidatorBehavior : Behavior<FillEntry>
{
    protected override void OnAttachedTo(FillEntry bindable)
    {
        bindable.TextChanged += Validate;
    }

    private void Validate(object? sender, TextChangedEventArgs e)
    {
        if (sender is not FillEntry value)
        {
            IsValid = false;
            return;
        }
        else if (e.NewTextValue.Length >= MinLength)
        {
            IsValid = true;
            value.TextColor = ValidColor;
            return;
        }
        else if (e.NewTextValue.Length <= MaxLength)
        {
            IsValid = true;
            value.TextColor = ValidColor;
            ValidationChange.Invoke(IsValid);
            return;
        }
        IsValid = false;
        value.TextColor = InValidColor;
        ValidationChange.Invoke(IsValid);
    }

    protected override void OnDetachingFrom(FillEntry bindable)
    {
        bindable.TextChanged -= Validate;
    }
    public int? MaxLength { get; set; }
    public int? MinLength { get; set; }
    public Action<bool> ValidationChange { get; set; } = (_) => { };
    public static readonly BindableProperty InValidColorProperty = BindableProperty.Create(nameof(InValidColor), typeof(Color), typeof(TextValidatorBehavior), Resources.Styles.Colors.CottonCandy);
    public Color InValidColor { get => (Color)GetValue(InValidColorProperty); set => SetValue(InValidColorProperty, value); }
    public static readonly BindableProperty ValidColorProperty = BindableProperty.Create(nameof(ValidColor), typeof(Color), typeof(TextValidatorBehavior), Resources.Styles.Colors.CottonCandy);
    public Color ValidColor { get => (Color)GetValue(ValidColorProperty); set => SetValue(ValidColorProperty, value); }
    public static readonly BindableProperty IsValidProperty = BindableProperty.Create(nameof(IsValid), typeof(bool), typeof(TextValidatorBehavior), false, BindingMode.OneWayToSource);
    public bool IsValid { get => (bool)GetValue(IsValidProperty); set => SetValue(IsValidProperty, value); }
}