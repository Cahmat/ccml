﻿using Ididit.Data;
using Microsoft.AspNetCore.Components;
using System.Threading.Tasks;

namespace Ididit.UI.Components;

public partial class HelpComponent
{
    [Inject]
    IRepository Repository { get; set; } = null!;

    async Task ShowMainScreen()
    {
        Repository.Settings.Screen = Screen.Main;
        await Repository.UpdateSettings(Repository.Settings.Id);
    }
}
