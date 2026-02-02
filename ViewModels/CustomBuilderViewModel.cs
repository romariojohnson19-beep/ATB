using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using AkhenTraderElite.Models;

namespace AkhenTraderElite.ViewModels
{
    /// <summary>
    /// ViewModel for the Custom Strategy Builder tab
    /// </summary>
    public partial class CustomBuilderViewModel : ObservableObject
    {
        private readonly Strategy _strategy;

        #region Observable Properties

        /// <summary>
        /// Collection of entry condition ViewModels
        /// </summary>
        [ObservableProperty]
        private ObservableCollection<ConditionViewModel> entryConditions = new();

        /// <summary>
        /// Collection of exit condition ViewModels
        /// </summary>
        [ObservableProperty]
        private ObservableCollection<ConditionViewModel> exitConditions = new();

        #endregion

        #region Constructor

        public CustomBuilderViewModel(Strategy strategy)
        {
            _strategy = strategy;

            // Initialize from existing conditions
            LoadConditions();
        }

        #endregion

        #region Methods

        /// <summary>
        /// Load existing conditions from strategy
        /// </summary>
        private void LoadConditions()
        {
            EntryConditions.Clear();
            foreach (var condition in _strategy.EntryConditions)
            {
                var vm = new ConditionViewModel(condition);
                vm.RemoveRequested += OnEntryConditionRemoveRequested;
                EntryConditions.Add(vm);
            }

            ExitConditions.Clear();
            foreach (var condition in _strategy.ExitConditions)
            {
                var vm = new ConditionViewModel(condition);
                vm.RemoveRequested += OnExitConditionRemoveRequested;
                ExitConditions.Add(vm);
            }
        }

        /// <summary>
        /// Sync conditions back to the strategy model
        /// </summary>
        public void SyncToStrategy()
        {
            _strategy.EntryConditions.Clear();
            foreach (var vm in EntryConditions)
            {
                _strategy.EntryConditions.Add(vm.GetCondition());
            }

            _strategy.ExitConditions.Clear();
            foreach (var vm in ExitConditions)
            {
                _strategy.ExitConditions.Add(vm.GetCondition());
            }
        }

        #endregion

        #region Commands

        /// <summary>
        /// Add a new entry condition
        /// </summary>
        [RelayCommand]
        private void AddEntryCondition()
        {
            var newCondition = new IndicatorCondition
            {
                Type = IndicatorType.RSI,
                Period = 14,
                Level = 30.0,
                Operator = ComparisonOperator.LessThan,
                IsAnd = true
            };

            var vm = new ConditionViewModel(newCondition);
            vm.RemoveRequested += OnEntryConditionRemoveRequested;
            EntryConditions.Add(vm);

            // Sync to strategy
            _strategy.EntryConditions.Add(newCondition);
        }

        /// <summary>
        /// Add a new exit condition
        /// </summary>
        [RelayCommand]
        private void AddExitCondition()
        {
            var newCondition = new IndicatorCondition
            {
                Type = IndicatorType.RSI,
                Period = 14,
                Level = 70.0,
                Operator = ComparisonOperator.GreaterThan,
                IsAnd = true
            };

            var vm = new ConditionViewModel(newCondition);
            vm.RemoveRequested += OnExitConditionRemoveRequested;
            ExitConditions.Add(vm);

            // Sync to strategy
            _strategy.ExitConditions.Add(newCondition);
        }

        /// <summary>
        /// Clear all entry conditions
        /// </summary>
        [RelayCommand]
        private void ClearEntryConditions()
        {
            foreach (var vm in EntryConditions)
            {
                vm.RemoveRequested -= OnEntryConditionRemoveRequested;
            }
            EntryConditions.Clear();
            _strategy.EntryConditions.Clear();
        }

        /// <summary>
        /// Clear all exit conditions
        /// </summary>
        [RelayCommand]
        private void ClearExitConditions()
        {
            foreach (var vm in ExitConditions)
            {
                vm.RemoveRequested -= OnExitConditionRemoveRequested;
            }
            ExitConditions.Clear();
            _strategy.ExitConditions.Clear();
        }

        #endregion

        #region Event Handlers

        private void OnEntryConditionRemoveRequested(ConditionViewModel vm)
        {
            vm.RemoveRequested -= OnEntryConditionRemoveRequested;
            EntryConditions.Remove(vm);
            _strategy.EntryConditions.Remove(vm.GetCondition());
        }

        private void OnExitConditionRemoveRequested(ConditionViewModel vm)
        {
            vm.RemoveRequested -= OnExitConditionRemoveRequested;
            ExitConditions.Remove(vm);
            _strategy.ExitConditions.Remove(vm.GetCondition());
        }

        #endregion
    }
}
