using HireGameDev.Application.Features.CandidateProfiles.GetMyCandidateProfile;
using HireGameDev.Application.Features.CandidateProfiles.RemoveCandidatePhoto;
using HireGameDev.Application.Features.CandidateProfiles.SaveCandidateProfile;
using HireGameDev.Application.Features.CandidateProfiles.UploadCandidatePhoto;
using HireGameDev.Application.Features.Skills.GetSkills;
using HireGameDev.Web.Authorization;
using HireGameDev.Web.ViewModels.CandidateProfile;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace HireGameDev.Web.Controllers;

[Authorize(Policy = RolePolicies.Candidate)]
[RequestFormLimits(MultipartBodyLengthLimit = UploadCandidatePhotoCommand.MaxBytes + 64 * 1024)]
public sealed class CandidateProfileController(
    GetMyCandidateProfileHandler getProfile,
    GetSkillsHandler getSkills,
    SaveCandidateProfileHandler saveProfile,
    UploadCandidatePhotoHandler uploadPhoto,
    RemoveCandidatePhotoHandler removePhoto) : Controller
{
    [HttpGet]
    public async Task<IActionResult> Index(CancellationToken cancellationToken)
    {
        var profile = await getProfile.HandleAsync(
            new GetMyCandidateProfileQuery(User.GetUserId()),
            cancellationToken);

        if (profile is null)
        {
            return RedirectToAction(nameof(Edit));
        }

        return View(new CandidateProfileViewModel
        {
            Profile = profile,
            SkillGroups = profile.Skills.GroupBy(x => x.Category).ToList(),
            StatusMessage = TempData["StatusMessage"] as string,
        });
    }

    [HttpGet]
    public async Task<IActionResult> Edit(CancellationToken cancellationToken)
    {
        var profile = await getProfile.HandleAsync(
            new GetMyCandidateProfileQuery(User.GetUserId()),
            cancellationToken);
        var skills = await getSkills.HandleAsync(new GetSkillsQuery(), cancellationToken);

        return View(new CandidateProfileEditViewModel
        {
            Input = profile is null ? new CandidateProfileInputModel() : CandidateProfileInputModel.From(profile),
            IsNew = profile is null,
            PhotoUrl = profile?.PhotoUrl,
            Skills = skills,
            StatusMessage = TempData["StatusMessage"] as string,
            ErrorMessage = TempData["ErrorMessage"] as string,
        });
    }

    [HttpPost]
    public async Task<IActionResult> Edit(
        [Bind(Prefix = "")] CandidateProfileEditViewModel model,
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

        TempData["StatusMessage"] = "Profile saved.";
        return RedirectToAction(nameof(Index));
    }

    [HttpPost]
    public async Task<IActionResult> UploadPhoto(IFormFile? photo, CancellationToken cancellationToken)
    {
        if (photo is null)
        {
            TempData["ErrorMessage"] = "Choose a photo to upload.";
            return RedirectToAction(nameof(Edit));
        }

        await using var content = photo.OpenReadStream();
        var result = await uploadPhoto.HandleAsync(
            new UploadCandidatePhotoCommand(User.GetUserId(), content, photo.Length),
            cancellationToken);

        if (result.Succeeded)
        {
            TempData["StatusMessage"] = "Photo updated.";
        }
        else
        {
            TempData["ErrorMessage"] = string.Join(" ", result.Errors);
        }

        return RedirectToAction(nameof(Edit));
    }

    [HttpPost]
    public async Task<IActionResult> RemovePhoto(CancellationToken cancellationToken)
    {
        await removePhoto.HandleAsync(new RemoveCandidatePhotoCommand(User.GetUserId()), cancellationToken);
        TempData["StatusMessage"] = "Photo removed.";
        return RedirectToAction(nameof(Edit));
    }

    private async Task LoadEditOptionsAsync(
        CandidateProfileEditViewModel model,
        CancellationToken cancellationToken)
    {
        var profile = await getProfile.HandleAsync(
            new GetMyCandidateProfileQuery(User.GetUserId()),
            cancellationToken);
        model.IsNew = profile is null;
        model.PhotoUrl = profile?.PhotoUrl;
        model.Skills = await getSkills.HandleAsync(new GetSkillsQuery(), cancellationToken);
    }
}
