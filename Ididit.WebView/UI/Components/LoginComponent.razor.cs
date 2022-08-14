﻿using Ididit.App;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using System.Threading.Tasks;

namespace Ididit.WebView.UI.Components;

public partial class LoginComponent
{
    [Inject]
    IUserDisplayName UserDisplayName { get; set; } = null!;

    string _name = string.Empty;

    async Task LogIn(MouseEventArgs args)
    {
        _name = await UserDisplayName.GetUserDisplayName();
    }

    void LogOut(MouseEventArgs args)
    {
        _name = string.Empty;
    }
}
