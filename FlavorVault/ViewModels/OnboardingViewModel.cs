using System.Collections.ObjectModel;
using FlavorVault.Services;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace FlavorVault.ViewModels;

/// <summary>
/// Onboarding ViewModel, manages the 3-step onboarding flow
/// </summary>
public partial class OnboardingViewModel : BaseViewModel
{
    private readonly FirstRunService _firstRunService;

    [ObservableProperty]
    private int _currentStep;

    [ObservableProperty]
    private int _stepCount = 3;

    [ObservableProperty]
    private bool _isLastStep;

    /// <summary>
    /// Onboarding steps list
    /// </summary>
    public ObservableCollection<OnboardingStepModel> Steps { get; }

    /// <summary>
    /// Current step index (for XAML binding, alias of CurrentStep)
    /// </summary>
    public int CurrentIndex => CurrentStep;

    /// <summary>
    /// Whether it is the first step
    /// </summary>
    public bool IsFirstStep => CurrentStep == 0;

    /// <summary>
    /// Whether it is the second step
    /// </summary>
    public bool IsSecondStep => CurrentStep == 1;

    /// <summary>
    /// Whether it is the third step
    /// </summary>
    public bool IsThirdStep => CurrentStep == 2;

    /// <summary>
    /// Whether it is not the last step (for XAML binding, "Skip" button visibility)
    /// </summary>
    public bool IsNotLastStep => !IsLastStep;

    /// <summary>
    /// Next button text
    /// </summary>
    public string NextButtonText => IsLastStep ? "Get Started" : "Next";

    public OnboardingViewModel(FirstRunService firstRunService)
    {
        _firstRunService = firstRunService;
        Title = "Welcome";

        // Initialize onboarding steps
        Steps = new ObservableCollection<OnboardingStepModel>
        {
            new() { IconGlyph = "", Title = "Welcome to FoodDrinkApp", Description = "Your personal food collection catalog, documenting every dish like compiling an encyclopedia." },
            new() { IconGlyph = "", Title = "Photo Recognition", Description = "Use AI photo recognition to identify food and automatically add it to your catalog." },
            new() { IconGlyph = "", Title = "Explore & Discover", Description = "Discover nearby food landmarks and explore specialty flavors from different regions." }
        };
    }

    [RelayCommand]
    private async Task Next()
    {
        try
        {
            CurrentStep++;

            OnPropertyChanged(nameof(CurrentIndex));

            // Update all step-related computed properties
            OnPropertyChanged(nameof(IsFirstStep));
            OnPropertyChanged(nameof(IsSecondStep));
            OnPropertyChanged(nameof(IsThirdStep));
            OnPropertyChanged(nameof(IsNotLastStep));
            OnPropertyChanged(nameof(NextButtonText));

            if (CurrentStep >= StepCount - 1)
            {
                IsLastStep = true;
                OnPropertyChanged(nameof(IsNotLastStep));
                OnPropertyChanged(nameof(NextButtonText));
            }

            if (CurrentStep >= StepCount)
            {
                // Last step, mark completed and navigate to home
                await CompleteOnboarding();
            }
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"[OnboardingViewModel] Next error: {ex.Message}");
        }
    }

    [RelayCommand]
    private async Task Skip()
    {
        try
        {
            await CompleteOnboarding();
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"[OnboardingViewModel] Skip error: {ex.Message}");
        }
    }

    /// <summary>
    /// Complete onboarding, mark first run and navigate to home page
    /// </summary>
    private async Task CompleteOnboarding()
    {
        try
        {
            await _firstRunService.MarkCompletedAsync();
            await Shell.Current.GoToAsync("//HomePage");
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"[OnboardingViewModel] CompleteOnboarding error: {ex.Message}");
        }
    }
}
