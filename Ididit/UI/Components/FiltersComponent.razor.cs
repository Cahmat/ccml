﻿using Ididit.App;
using Ididit.Data.Models;
using Microsoft.AspNetCore.Components;
using System;
using System.Threading.Tasks;

namespace Ididit.UI.Components;

public partial class FiltersComponent
{
    [Inject]
    IRepository Repository { get; set; } = null!;

    [CascadingParameter]
    Blazorise.Size Size { get; set; }

    [Parameter]
    public SettingsModel Settings { get; set; } = null!;

    [Parameter]
    public EventCallback<SettingsModel> SettingsChanged { get; set; }

    [Parameter]
    public Filters Filters { get; set; } = null!;

    [Parameter]
    public EventCallback<Filters> FiltersChanged { get; set; }

    string _searchFilter = string.Empty;

    async Task SearchFilterChanged(string searchFilter)
    {
        _searchFilter = searchFilter;
        Filters.SearchFilter = searchFilter;
        await FiltersChanged.InvokeAsync(Filters);
    }

    async Task ClearSearchFilter()
    {
        _searchFilter = string.Empty;
        Filters.SearchFilter = string.Empty;
        await FiltersChanged.InvokeAsync(Filters);
    }

    DateTime? _dateFilter;

    bool? IsTodayChecked => _dateFilter == DateTime.Now.Date;

    async Task TodayCheckedChanged(bool? isToday)
    {
        _dateFilter = isToday == true ? DateTime.Now.Date : null;
        Filters.DateFilter = isToday == true ? DateTime.Now.Date : null;
        await FiltersChanged.InvokeAsync(Filters);
    }

    async Task OnDateChanged(DateTime? dateTime)
    {
        _dateFilter = dateTime;
        Filters.DateFilter = dateTime;
        await FiltersChanged.InvokeAsync(Filters);
    }

    async Task ClearDateFilter()
    {
        _dateFilter = null;
        Filters.DateFilter = null;
        await FiltersChanged.InvokeAsync(Filters);
    }

    bool GetShowPriority(Priority priority) => Settings.ShowPriority[priority];

    async Task OnShowPriorityChanged(Priority priority, bool show)
    {
        Settings.ShowPriority[priority] = show;
        await Repository.UpdateSettings(Settings.Id);

        await SettingsChanged.InvokeAsync(Settings);
    }

    bool GetShowTaskKind(TaskKind taskKind) => Settings.ShowTaskKind[taskKind];

    async Task OnShowTaskKindChanged(TaskKind taskKind, bool show)
    {
        Settings.ShowTaskKind[taskKind] = show;
        await Repository.UpdateSettings(Settings.Id);

        await SettingsChanged.InvokeAsync(Settings);
    }

    Sort Sort => Settings.Sort;

    long ElapsedToDesiredRatioMin => Settings.ElapsedToDesiredRatioMin;

    bool ShowElapsedToDesiredRatioOverMin => Settings.ShowElapsedToDesiredRatioOverMin;

    bool HideEmptyGoals => Settings.HideEmptyGoals;

    bool ShowCategoriesInGoalList => Settings.ShowCategoriesInGoalList;

    bool HideCompletedTasks => Settings.HideCompletedTasks;

    async Task OnSortChanged(Sort sort)
    {
        Settings.Sort = sort;
        await Repository.UpdateSettings(Settings.Id);

        await SettingsChanged.InvokeAsync(Settings);
    }

    async Task OnHideEmptyGoalsChanged(bool? val)
    {
        Settings.HideEmptyGoals = val ?? false;
        await Repository.UpdateSettings(Settings.Id);

        await SettingsChanged.InvokeAsync(Settings);
    }

    async Task OnShowCategoriesInGoalListChanged(bool? val)
    {
        Settings.ShowCategoriesInGoalList = val ?? false;
        await Repository.UpdateSettings(Settings.Id);

        await SettingsChanged.InvokeAsync(Settings);
    }

    async Task OnHideCompletedTasksChanged(bool? val)
    {
        Settings.HideCompletedTasks = val ?? false;
        await Repository.UpdateSettings(Settings.Id);

        await SettingsChanged.InvokeAsync(Settings);
    }

    async Task OnShowElapsedToDesiredRatioOverMinChanged(bool? val)
    {
        Settings.ShowElapsedToDesiredRatioOverMin = val ?? false;
        await Repository.UpdateSettings(Settings.Id);

        await SettingsChanged.InvokeAsync(Settings);
    }

    async Task OnElapsedToDesiredRatioMinChanged(long val)
    {
        Settings.ElapsedToDesiredRatioMin = val;
        await Repository.UpdateSettings(Settings.Id);

        await SettingsChanged.InvokeAsync(Settings);
    }
}
