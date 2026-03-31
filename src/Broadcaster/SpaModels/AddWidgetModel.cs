using System;
using System.ComponentModel.DataAnnotations;
using Broadcaster.Business;

namespace Broadcaster.SpaModels;

public class AddWidgetModel
{
    #region Properties

    [Display(Name = "Name")]
    [MaxLength(100)]
    [Required]
    public string Name { get; set; } = null!;

    [Display(Name = "Widget Description", Description = "Describe the Widget")]
    public string? Description { get; set; }

    #endregion

    #region Object Methods

    public WidgetObject ToWidgetObject() => new WidgetObject(Name, Description);

    #endregion
}
