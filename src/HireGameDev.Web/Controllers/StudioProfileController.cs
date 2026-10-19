using HireGameDev.Application.Features.Skills.GetSkills;
using HireGameDev.Application.Features.StudioProfiles.GetMyStudioProfile;
using HireGameDev.Application.Features.StudioProfiles.RemoveStudioLogo;
using HireGameDev.Application.Features.StudioProfiles.SaveStudioProfile;
using HireGameDev.Application.Features.StudioProfiles.UploadStudioLogo;
using HireGameDev.Domain.Enums;
using HireGameDev.Web.Authorization;
using HireGameDev.Web.ViewModels.StudioProfile;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace HireGameDev.Web.Controllers;

[Authorize(Policy = RolePolicies.Studio)]
[RequestFormLimits(MultipartBodyLengthLimit = UploadStudioLogoCommand.MaxBytes + 64 * 1024)]
public sealed class StudioProfileController(
    GetMyStudioProfileHandler getProfile,
    GetSkillsHandler getSkills,
    SaveStudioProfileHandler saveProfile,
    UploadStudioLogoHandler uploadLogo,
    RemoveStudioLogoHandler removeLogo) : Controller
{
    [HttpGet]
    public async Task<IActionResult> Index(CancellationToken cancellationToken)
    {
        var profile = await getProfile.HandleAsync(
            new GetMyStudioProfileQuery(User.GetUserId()),
            cancellationToken);

        if (profile is null)
        {
            return RedirectToAction(nameof(Edit));
        }

        return View(new StudioProfileViewModel
        {
            Profile = profile,
            StatusMessage = TempData["StatusMessage"] as string,
        });
    }

    [HttpGet]
    public async Task<IActionResult> Edit(CancellationToken cancellationToken)
    {
        var model = await CreateEditViewModelAsync(cancellationToken);
        model.StatusMessage = TempData["StatusMessage"] as string;
        model.ErrorMessage = TempData["ErrorMessage"] as string;
        return View(model);
    }

    [HttpPost]
    public async Task<IActionResult> Edit(
        [Bind(Prefix = "")] StudioProfileEditViewModel model,
        CancellationToken cancellationToken)
    {
        if (!ModelState.IsValid)
        {
            await LoadEditOptionsAsync(model, cancellationToken);
            return View(model);
        }

        var result = await saveProfile.HandleAsync(
            model.Input.ToCommand(User.GetUserId()),
            cancellationToken);

        if (!result.Succeeded)
        {
            foreach (var error in result.Errors)
            {
                ModelState.AddModelError(string.Empty, error);
            }

            await LoadEditOptionsAsync(model, cancellationToken);
            return View(model);
        }

        TempData["StatusMessage"] = "Studio profile saved.";
        return RedirectToAction(nameof(Index));
    }

    [HttpPost]
    public async Task<IActionResult> UploadLogo(IFormFile? logo, CancellationToken cancellationToken)
    {
        if (logo is null)
        {
            TempData["ErrorMessage"] = "Choose a logo to upload.";
            return RedirectToAction(nameof(Edit));
        }

        await using var content = logo.OpenReadStream();
        var result = await uploadLogo.HandleAsync(
            new UploadStudioLogoCommand(User.GetUserId(), content, logo.Length),
            cancellationToken);

        if (result.Succeeded)
        {
            TempData["StatusMessage"] = "Logo updated.";
        }
        else
        {
            TempData["ErrorMessage"] = string.Join(" ", result.Errors);
        }

        return RedirectToAction(nameof(Edit));
    }

    [HttpPost]
    public async Task<IActionResult> RemoveLogo(CancellationToken cancellationToken)
    {
        await removeLogo.HandleAsync(new RemoveStudioLogoCommand(User.GetUserId()), cancellationToken);
        TempData["StatusMessage"] = "Logo removed.";
        return RedirectToAction(nameof(Edit));
    }

    private async Task<StudioProfileEditViewModel> CreateEditViewModelAsync(
        CancellationToken cancellationToken)
    {
        var profile = await getProfile.HandleAsync(
            new GetMyStudioProfileQuery(User.GetUserId()),
            cancellationToken);
        var skills = await getSkills.HandleAsync(new GetSkillsQuery(), cancellationToken);

        return new StudioProfileEditViewModel
        {
            Input = profile is null ? new StudioProfileInputModel() : StudioProfileInputModel.From(profile),
            IsNew = profile is null,
            LogoUrl = profile?.LogoUrl,
            Engines = skills.Where(x => x.Category == SkillCategory.Engines).ToList(),
        };
    }

    private async Task LoadEditOptionsAsync(
        StudioProfileEditViewModel model,
        CancellationToken cancellationToken)
    {
        var profile = await getProfile.HandleAsync(
            new GetMyStudioProfileQuery(User.GetUserId()),
            cancellationToken);
        var skills = await getSkills.HandleAsync(new GetSkillsQuery(), cancellationToken);
        model.IsNew = profile is null;
        model.LogoUrl = profile?.LogoUrl;
        model.Engines = skills.Where(x => x.Category == SkillCategory.Engines).ToList();
    }
}
