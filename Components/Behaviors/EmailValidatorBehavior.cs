using System.ComponentModel.DataAnnotations;
using Ixjok.Components.Controls;

namespace Ixjok.Components.Behaviors;

public sealed class EmailValidatorBehavior : Behavior<FillEntry>
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
        if (_validator.IsValid(e.NewTextValue))
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
    public Action<bool> ValidationChange { get; set; } = (_) => { };
    public static readonly BindableProperty InValidColorProperty = BindableProperty.Create(nameof(InValidColor), typeof(Color), typeof(EmailValidatorBehavior), Resources.Styles.Colors.CottonCandy);
    public Color InValidColor { get => (Color)GetValue(InValidColorProperty); set => SetValue(InValidColorProperty, value); }
    public static readonly BindableProperty ValidColorProperty = BindableProperty.Create(nameof(ValidColor), typeof(Color), typeof(EmailValidatorBehavior), Resources.Styles.Colors.DimGrey);
    public Color ValidColor { get => (Color)GetValue(ValidColorProperty); set => SetValue(ValidColorProperty, value); }
    public static readonly BindableProperty IsValidProperty = BindableProperty.Create(nameof(IsValid), typeof(bool), typeof(EmailValidatorBehavior), false, BindingMode.OneWayToSource);
    public bool IsValid { get => (bool)GetValue(IsValidProperty); set => SetValue(IsValidProperty, value); }
    private readonly EmailAddressAttribute _validator = new();
}
