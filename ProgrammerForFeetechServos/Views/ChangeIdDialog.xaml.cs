// Copyright © 2025 Timothy Ellis, Fyrby Additive Manufacturing & Engineering

using System.Windows;
using ProgrammerForFeetechServos.ViewModels;

namespace ProgrammerForFeetechServos.Views;

public partial class ChangeIdDialog : Window
{
    public ChangeIdDialog()
    {
        InitializeComponent();
        
        Loaded += (s, e) =>
        {
            if (DataContext is ChangeIdViewModel viewModel)
            {
                viewModel.CloseRequested += () => Close();
            }
        };
    }
}
