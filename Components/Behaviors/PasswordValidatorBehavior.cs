using System.Text.RegularExpressions;
using Ixjok.Components.Controls;

namespace Ixjok.Components.Behaviors;

public sealed partial class PasswordValidatorBehavior : Behavior<FillEntry>
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
        if (PasswordRegex().IsMatch(e.NewTextValue))
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
    public static readonly BindableProperty InValidColorProperty = BindableProperty.Create(nameof(InValidColor), typeof(Color), typeof(PasswordValidatorBehavior), Resources.Styles.Colors.CottonCandy);
    public Color InValidColor { get => (Color)GetValue(InValidColorProperty); set => SetValue(InValidColorProperty, value); }
    public static readonly BindableProperty ValidColorProperty = BindableProperty.Create(nameof(ValidColor), typeof(Color), typeof(PasswordValidatorBehavior), Resources.Styles.Colors.DimGrey);
    public Color ValidColor { get => (Color)GetValue(ValidColorProperty); set => SetValue(ValidColorProperty, value); }
    public static readonly BindableProperty IsValidProperty = BindableProperty.Create(nameof(IsValid), typeof(bool), typeof(PasswordValidatorBehavior), false, BindingMode.OneWayToSource);
    public bool IsValid { get => (bool)GetValue(IsValidProperty); set => SetValue(IsValidProperty, value); }
    [GeneratedRegex("""^(?=(.*[A-Z]){2,})(?=(.*[0-9]){2,})(?=(.*[~`!@#$%^&*()--+={}\[\]|\\:;"'<>,.?/_₹]){2,}).{8,}$""")]
    private static partial Regex PasswordRegex();
}