using HireGameDev.Application.Features.Jobs.ChangeStudioJobStatus;
using HireGameDev.Application.Features.Jobs.GetStudioJob;
using HireGameDev.Application.Features.Jobs.GetStudioJobs;
using HireGameDev.Application.Features.Jobs.SaveStudioJob;
using HireGameDev.Application.Features.Skills.GetSkills;
using HireGameDev.Domain.Enums;
using HireGameDev.Web.Authorization;
using HireGameDev.Web.ViewModels.StudioJobs;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace HireGameDev.Web.Controllers;

[Authorize(Policy = RolePolicies.Studio)]
public sealed class StudioJobsController(
    GetStudioJobsHandler getJobs,
    GetStudioJobHandler getJob,
    SaveStudioJobHandler saveJob,
    ChangeStudioJobStatusHandler changeJobStatus,
    GetSkillsHandler getSkills) : Controller
{
    [HttpGet]
    public async Task<IActionResult> Index(CancellationToken cancellationToken)
    {
        var result = await getJobs.HandleAsync(
            new GetStudioJobsQuery(User.GetUserId()),
            cancellationToken);

        return View(new StudioJobsIndexViewModel
        {
            HasStudioProfile = result.HasStudioProfile,
            Jobs = result.Jobs,
            StatusMessage = TempData["StatusMessage"] as string,
            ErrorMessage = TempData["ErrorMessage"] as string,
        });
    }

    [HttpGet]
    public async Task<IActionResult> Create(CancellationToken cancellationToken)
    {
        var page = await getJobs.HandleAsync(
            new GetStudioJobsQuery(User.GetUserId()),
            cancellationToken);

        if (!page.HasStudioProfile)
        {
            TempData["ErrorMessage"] = "Create a studio profile before posting a job.";
            return RedirectToAction(nameof(Index));
        }

        return View("Edit", new StudioJobEditViewModel
        {
            IsNew = true,
            Status = JobStatus.Draft,
            Skills = await GetSkillOptionsAsync(cancellationToken),
            StatusMessage = TempData["StatusMessage"] as string,
            ErrorMessage = TempData["ErrorMessage"] as string,
        });
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(
        StudioJobEditViewModel model,
        CancellationToken cancellationToken)
    {
        model.IsNew = true;
        model.JobId = null;
        model.Status = JobStatus.Draft;

        if (!ModelState.IsValid)
        {
            await LoadSkillOptionsAsync(model, cancellationToken);
            return View("Edit", model);
        }

        var result = await saveJob.HandleAsync(
            model.Input.ToCommand(User.GetUserId(), jobId: null),
            cancellationToken);

        if (!result.Succeeded || result.JobId is null)
        {
            AddErrors(result.Errors);
            await LoadSkillOptionsAsync(model, cancellationToken);
            return View("Edit", model);
        }

        TempData["StatusMessage"] = "Draft saved. Review it and publish when it is ready.";
        return RedirectToAction(nameof(Edit), new { id = result.JobId.Value });
    }

    [HttpGet]
    public async Task<IActionResult> Edit(Guid id, CancellationToken cancellationToken)
    {
        var job = await getJob.HandleAsync(
            new GetStudioJobQuery(User.GetUserId(), id),
            cancellationToken);

        if (job is null)
        {
            return NotFound();
        }

        return View(new StudioJobEditViewModel
        {
            JobId = job.Id,
            IsNew = false,
            Status = job.Status,
            ExpiresAtUtc = job.ExpiresAtUtc,
            Input = StudioJobInputModel.From(job),
            Skills = await GetSkillOptionsAsync(cancellationToken),
            StatusMessage = TempData["StatusMessage"] as string,
            ErrorMessage = TempData["ErrorMessage"] as string,
        });
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(
        Guid id,
        StudioJobEditViewModel model,
        CancellationToken cancellationToken)
    {
        model.JobId = id;
        model.IsNew = false;

        if (!ModelState.IsValid)
        {
            if (!await LoadExistingContextAsync(model, id, cancellationToken))
            {
                return NotFound();
            }

            return View(model);
        }

        var result = await saveJob.HandleAsync(
            model.Input.ToCommand(User.GetUserId(), id),
            cancellationToken);

        if (!result.Succeeded)
        {
            AddErrors(result.Errors);

            if (!await LoadExistingContextAsync(model, id, cancellationToken))
            {
                return NotFound();
            }

            return View(model);
        }

        TempData["StatusMessage"] = "Job details saved.";
        return RedirectToAction(nameof(Edit), new { id });
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Publish(
        Guid id,
        DateOnly? expiresOn,
        CancellationToken cancellationToken)
    {
        if (expiresOn is null)
        {
            TempData["ErrorMessage"] = "Choose an expiry date before publishing.";
            return RedirectToAction(nameof(Edit), new { id });
        }

        var expiry = DateTime.SpecifyKind(
            expiresOn.Value.ToDateTime(TimeOnly.MaxValue),
            DateTimeKind.Utc);

        return await ChangeStatusAsync(
            id,
            StudioJobLifecycleAction.Publish,
            expiry,
            cancellationToken);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public Task<IActionResult> Pause(Guid id, CancellationToken cancellationToken) =>
        ChangeStatusAsync(id, StudioJobLifecycleAction.Pause, null, cancellationToken);

    [HttpPost]
    [ValidateAntiForgeryToken]
    public Task<IActionResult> Resume(Guid id, CancellationToken cancellationToken) =>
        ChangeStatusAsync(id, StudioJobLifecycleAction.Resume, null, cancellationToken);

    [HttpPost]
    [ValidateAntiForgeryToken]
    public Task<IActionResult> Close(Guid id, CancellationToken cancellationToken) =>
        ChangeStatusAsync(id, StudioJobLifecycleAction.Close, null, cancellationToken);

    private async Task<IActionResult> ChangeStatusAsync(
        Guid id,
        StudioJobLifecycleAction action,
        DateTime? expiresAtUtc,
        CancellationToken cancellationToken)
    {
        var result = await changeJobStatus.HandleAsync(
            new ChangeStudioJobStatusCommand(
                User.GetUserId(),
                id,
                action,
                expiresAtUtc),
            cancellationToken);

        if (result.Succeeded)
        {
            TempData["StatusMessage"] = action switch
            {
                StudioJobLifecycleAction.Publish => "Job published.",
                StudioJobLifecycleAction.Pause => "Job paused.",
                StudioJobLifecycleAction.Resume => "Job resumed.",
                StudioJobLifecycleAction.Close => "Job closed.",
                _ => "Job updated.",
            };
        }
        else
        {
            TempData["ErrorMessage"] = string.Join(" ", result.Errors);
        }

        return RedirectToAction(nameof(Edit), new { id });
    }

    private async Task<bool> LoadExistingContextAsync(
        StudioJobEditViewModel model,
        Guid id,
        CancellationToken cancellationToken)
    {
        var job = await getJob.HandleAsync(
            new GetStudioJobQuery(User.GetUserId(), id),
            cancellationToken);

        if (job is null)
        {
            return false;
        }

        model.JobId = job.Id;
        model.IsNew = false;
        model.Status = job.Status;
        model.ExpiresAtUtc = job.ExpiresAtUtc;
        model.Skills = await GetSkillOptionsAsync(cancellationToken);
        return true;
    }

    private async Task<IReadOnlyList<SkillDto>> GetSkillOptionsAsync(
        CancellationToken cancellationToken) =>
        await getSkills.HandleAsync(new GetSkillsQuery(), cancellationToken);

    private async Task LoadSkillOptionsAsync(
        StudioJobEditViewModel model,
        CancellationToken cancellationToken)
    {
        model.Skills = await GetSkillOptionsAsync(cancellationToken);
    }

    private void AddErrors(IEnumerable<string> errors)
    {
        foreach (var error in errors)
        {
            ModelState.AddModelError(string.Empty, error);
        }
    }
}
