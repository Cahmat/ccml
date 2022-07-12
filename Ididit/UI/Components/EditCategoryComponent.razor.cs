﻿using Ididit.App;
using Ididit.Data.Models;
using Microsoft.AspNetCore.Components;
using System.Threading.Tasks;

namespace Ididit.UI.Components;

public sealed partial class EditCategoryComponent
{
    [Inject]
    IRepository _repository { get; set; } = null!;

    [CascadingParameter]
    Blazorise.Size Size { get; set; }

    [Parameter]
    public CategoryModel? Category { get; set; } = null!;

    [Parameter]
    public EventCallback<CategoryModel> CategoryChanged { get; set; }

    [Parameter]
    public CategoryModel? EditCategory { get; set; } = null!;

    [Parameter]
    public EventCallback<CategoryModel> EditCategoryChanged { get; set; }

    Blazorise.TextEdit? _textEdit;

    protected override async Task OnAfterRenderAsync(bool firstRender)
    {
        if (EditCategory == Category && _textEdit != null)
        {
            await _textEdit.Focus();
        }
    }

    async Task EditName()
    {
        if (Category != null)
        {
            EditCategory = Category;
            await EditCategoryChanged.InvokeAsync(EditCategory);
        }
    }

    async Task CancelEdit()
    {
        EditCategory = null;
        await EditCategoryChanged.InvokeAsync(EditCategory);
    }

    async Task SaveName()
    {
        EditCategory = null;
        await EditCategoryChanged.InvokeAsync(EditCategory);

        if (Category != null)
            await _repository.UpdateCategory(Category.Id);

        await CategoryChanged.InvokeAsync(Category);
    }

    async Task DeleteCategory()
    {
        if (Category == null)
            return;

        if (Category.CategoryId.HasValue && _repository.AllCategories.TryGetValue(Category.CategoryId.Value, out CategoryModel? parent))
            parent.CategoryList.Remove(Category);

        await _repository.DeleteCategory(Category.Id);

        Category = null;
        await CategoryChanged.InvokeAsync(Category);
    }
}
