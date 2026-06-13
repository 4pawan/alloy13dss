# Commands Used

This file lists the shell commands used while implementing and verifying the dummy form journey/tool changes.

## Search And Inspect Files

```powershell
rg --files Models Views Controllers | Sort-Object
rg --files | rg "(Dummy|dummy|ContentPage|LandingPage|StandardPage|SettingsPage|FormToolSettingsBlock|maintool|README|\.md$)"
rg "SettingsPageLink|FormToolKey|DirectFormContainerBlock|IFormToolResolver|FormTool" -n
rg "class .*FormTool|IFormToolResolver|SettingsPageLink|FormToolKey|DirectFormContainerBlock|Recaptcha" -n Business Models Controllers Views
rg "SettingsPageLink|FormToolKey|DirectFormContainerBlock|IFormToolResolver|FormTool" -n Models Business Controllers Views
rg "IContainerPage" -n Business Controllers Models
rg "AvailableContentTypes" Models\Pages -n
rg "FormToolKey|DirectFormContainerBlock|class .*Page : SitePageData|IFormToolResolver|Resolve\(" -n Models Business Views Controllers
rg "IToolSettingsResolver|ToolSettingsResolver|IFormToolResolver|FormToolResolver|RootFolder" -n Business Models Views Controllers Startup.cs
rg "IDummyToolSettingsResolver|DummyToolSettingsResolver|IDummyFormToolResolver|DummyFormToolResolver|DummyRootFolder" -n Business Models Views Startup.cs
rg "IMemoryCache|ISynchronizedObjectInstanceCache|AddMemoryCache|CacheEvictionPolicy" -n
rg "SettingsPage" -n Business Models Controllers Views
rg "DummyToolCode|ToolCode|ToolText|ToBodyAttributeValue|IDummySettingsPage|SettingsPage" -n Models Business Views Controllers
rg "SettingsPage" -n Business\Forms Views\Shared\Layouts Models\Pages\SettingsPage.cs
rg "DummyToolCode|ToBodyAttributeValue|ToolCode" -n Models Business Views
```

```powershell
Get-ChildItem -Path Business\Forms -File | Select-Object -ExpandProperty FullName
Get-ChildItem -Path . -Filter *.md -Recurse | Select-Object -ExpandProperty FullName
```

## Read Files

```powershell
Get-Content -Path Models\Pages\LandingPage.cs
Get-Content -Path Views\LandingPage\Index.cshtml
Get-Content -Path Controllers\DefaultPageController.cs
Get-Content -Path Views\_ViewImports.cshtml
Get-Content -Path Models\Pages\StandardPage.cs
Get-Content -Path Models\Pages\SitePageData.cs
Get-Content -Path Models\Pages\ContainerPage.cs
Get-Content -Path Models\Pages\SettingsPage.cs
Get-Content -Path Business\Forms\IFormToolResolver.cs
Get-Content -Path Models\Blocks\FormToolSettingsBlock.cs
Get-Content -Path Startup.cs
Get-Content -Path Business\Forms\FormToolResolver.cs
Get-Content -Path Business\Forms\DummyFormJourneyService.cs
Get-Content -Path Business\PageViewContextFactory.cs
Get-Content -Path Models\ViewModels\LayoutModel.cs
Get-Content -Path Controllers\PageControllerBase.cs
Get-Content -Path Views\Shared\Blocks\DummyFormContainerBlock.cshtml
Get-Content -Path Views\Shared\Layouts\_DummyToolLayout.cshtml
Get-Content -Path Views\Shared\Layouts\_DummyToolHeader.cshtml
Get-Content -Path Business\Rendering\IContainerPage.cs
Get-Content -Path Models\Pages\RootFolder.cs
Get-Content -Path Models\Pages\DummySitePageData.cs
Get-Content -Path Business\Forms\IToolSettingsResolver.cs
Get-Content -Path Business\Forms\ToolSettingsResolver.cs
Get-Content -Path Views\Shared\Layouts\_Root.cshtml
Get-Content -Path Views\AlloyPageBase.cs
Get-Content -Path Business\PageContextActionFilter.cs
Get-Content -Path Models\ViewModels\PageViewModel.cs
Get-Content -Path Views\Shared\Layouts\_DummyToolFooter.cshtml
Get-Content -Path Views\Shared\Footer.cshtml
Get-Content -Path Business\Forms\IDummyToolSettingsResolver.cs
Get-Content -Path Business\Forms\DummyToolSettingsResolver.cs
Get-Content -Path Business\Forms\DummyServiceCollectionExtensions.cs
Get-Content -Path Business\Forms\IDummyFormToolResolver.cs
Get-Content -Path Business\Forms\DummyFormToolResolver.cs
Get-Content -Path Models\Pages\DummyRootFolder.cs
Get-Content -Path Models\Forms\DummyQuestionElementBlock.cs
Get-Content -Path docs\dummy-tool-file-inventory.md
```

## Git Status, Diff, And Branch Checks

```powershell
git status --short
git status --short docs\dummy-tool-file-inventory.md
git status --short Business\Forms\DummyToolSettingsResolver.cs Business\Forms\DummyServiceCollectionExtensions.cs
git branch --show-current
git diff --stat
git diff -- Models\Pages\ContentPage.cs Views\ContentPage\Index.cshtml Controllers\DummyFormContainerBlockController.cs
git diff -- Business\Forms\IToolSettingsResolver.cs Business\Forms\ToolSettingsResolver.cs Business\Forms\FormToolResolver.cs Business\Forms\DummyFormJourneyService.cs Business\PageViewContextFactory.cs Models\Pages\ContainerPage.cs Startup.cs Models\Pages\ContentPage.cs Views\ContentPage\Index.cshtml
git diff -- Business\PageViewContextFactory.cs Models\Pages\SitePageData.cs Models\Pages\ContainerPage.cs Startup.cs Business\Forms\IToolSettingsResolver.cs Business\Forms\ToolSettingsResolver.cs Business\Forms\IFormToolResolver.cs Business\Forms\FormToolResolver.cs Business\Forms\DummyFormJourneyService.cs Models\Pages\DummySitePageData.cs Models\Pages\RootFolder.cs Models\Pages\ContentPage.cs Models\Pages\LandingPage.cs Models\Pages\StandardPage.cs
git diff -- Business\PageViewContextFactory.cs Startup.cs Views\Shared\Layouts\_Root.cshtml Views\Shared\Layouts\_DummyToolLayout.cshtml Views\Shared\Layouts\_DummyToolFooter.cshtml
git diff -- Business\Forms\DummyToolSettingsResolver.cs Business\Forms\DummyServiceCollectionExtensions.cs
git diff -- Models\Pages\SettingsPage.cs
git diff -- Models\Pages\SettingsPage.cs Business\Forms\IDummyToolSettingsResolver.cs Business\Forms\DummyToolSettingsResolver.cs Business\Forms\DummyFormJourneyService.cs Views\Shared\Layouts\_DummyToolLayout.cshtml Views\Shared\Layouts\_DummyToolFooter.cshtml
git show HEAD:Business\PageViewContextFactory.cs
git show HEAD:Models\Pages\SitePageData.cs
git rev-parse --short HEAD
```

## Build Commands

```powershell
$env:TEMP_BUILD_OBJ = Join-Path $env:TEMP 'alloy13dss-verify-obj'; $env:TEMP_BUILD_OUT = Join-Path $env:TEMP 'alloy13dss-verify-build'; dotnet build --no-restore -p:UseAppHost=false -p:BaseIntermediateOutputPath=$env:TEMP_BUILD_OBJ\ -p:OutputPath=$env:TEMP_BUILD_OUT\
dotnet build --no-restore -p:UseAppHost=false
```

## Git Add, Commit, Push Commands

```powershell
git add Business\Forms\DummyToolSettingsResolver.cs Business\Forms\DummyServiceCollectionExtensions.cs
git commit -m "Cache dummy tool settings resolution"
git push
```

```powershell
git add Business\Forms\DummyFormJourneyService.cs Business\Forms\DummyToolSettingsResolver.cs Business\Forms\IDummyToolSettingsResolver.cs Models\Pages\SettingsPage.cs Views\Shared\Layouts\_DummyToolFooter.cshtml Views\Shared\Layouts\_DummyToolLayout.cshtml
git commit -m "Use dummy settings interface in services"
git push
```

## Delete Or Revert Files

Delete a file from the working tree:

```powershell
Remove-Item -LiteralPath .\path\to\file.cs
```

Delete a tracked file and stage the deletion:

```powershell
git rm .\path\to\file.cs
```

Revert unstaged changes in one file back to `HEAD`:

```powershell
git restore .\path\to\file.cs
```

Unstage a staged file but keep its working-tree changes:

```powershell
git restore --staged .\path\to\file.cs
```

Revert a file to a specific commit/version:

```powershell
git restore --source <commit-sha> -- .\path\to\file.cs
```

## File Edits

File edits were applied through Codex `apply_patch` operations, not through direct shell write commands.

Main files created or edited during this architecture work included:

```text
Models/Pages/ContentPage.cs
Views/ContentPage/Index.cshtml
Models/Pages/DummyRootFolder.cs
Models/Pages/DummySitePageData.cs
Business/Forms/IDummyToolSettingsResolver.cs
Business/Forms/DummyToolSettingsResolver.cs
Business/Forms/IDummyFormToolResolver.cs
Business/Forms/DummyFormToolResolver.cs
Business/Forms/DummyServiceCollectionExtensions.cs
Business/Forms/DummyFormJourneyService.cs
Business/Forms/DummyServiceCollectionExtensions.cs
Models/Pages/SettingsPage.cs
Views/Shared/Layouts/_DummyToolLayout.cshtml
Views/Shared/Layouts/_DummyToolFooter.cshtml
Views/LandingPage/Index.cshtml
Views/StandardPage/Index.cshtml
docs/dummy-tool-file-inventory.md
```
