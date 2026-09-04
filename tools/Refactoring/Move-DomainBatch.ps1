param([switch]$Apply, [switch]$Verify, [switch]$RepairImports)

# One-time, approved mechanical namespace/project relocation. Not a business-logic refactor.
# Default invocation is a read-only preflight. Backups and manifests are generated artifacts.
$ErrorActionPreference = 'Stop'
$taskRoot = [IO.Path]::GetFullPath((Join-Path $PSScriptRoot '../..'))
$taskBaseline = Join-Path $taskRoot 'output/refactoring/domain-batch-20260904'
$taskUtf8 = [Text.UTF8Encoding]::new($false)
Set-Location -LiteralPath $taskRoot

function Resolve-TaskPath([string]$Relative) {
    $taskFull = [IO.Path]::GetFullPath((Join-Path $taskRoot $Relative))
    if (!$taskFull.StartsWith($taskRoot + [IO.Path]::DirectorySeparatorChar, [StringComparison]::OrdinalIgnoreCase)) {
        throw "Path escapes workspace: $Relative"
    }
    return $taskFull
}

if ($RepairImports) {
    $taskManifest = Get-Content -LiteralPath (Join-Path $taskBaseline 'baseline.json') -Raw | ConvertFrom-Json
    $taskArchive = [IO.Compression.ZipFile]::OpenRead((Join-Path $taskBaseline 'sources.zip'))
    $taskRepaired = 0
    try {
        foreach ($taskMove in $taskManifest.Moves | Where-Object { $_.Target -like 'src/Domain/*.cs' }) {
            $taskReader = [IO.StreamReader]::new($taskArchive.GetEntry($taskMove.Source).Open())
            try { $taskOriginal = $taskReader.ReadToEnd() } finally { $taskReader.Dispose() }
            $taskOldNs = [regex]::Match($taskOriginal, '\bnamespace\s+([\w.]+)').Groups[1].Value
            if (!$taskOldNs) { continue }
            $taskPath = Resolve-TaskPath $taskMove.Target
            $taskCurrent = [IO.File]::ReadAllText($taskPath)
            $taskNewNs = [regex]::Match($taskCurrent, '\bnamespace\s+([\w.]+)').Groups[1].Value
            $taskNewline = if ($taskCurrent.Contains("`r`n")) { "`r`n" } else { "`n" }
            $taskUpdated = [regex]::Replace($taskCurrent, '(?m)^using EdgIQ\.MurimiOS\.Application\.Common\.Abstractions;\r?\n', '')
            $taskUpdated = [regex]::Replace($taskUpdated, '(?m)^using \w+ = global::EdgIQ\.MurimiOS\.[\w.]+;\r?\n', '')
            $taskImports = @([regex]::Matches($taskOriginal, '(?m)^using\s+([\w.]+);') | ForEach-Object { $_.Groups[1].Value })
            $taskAliases = @(foreach ($taskGroup in $taskManifest.Types | Group-Object { $_.OldName.Substring($_.OldName.LastIndexOf('.') + 1) }) {
                $taskName = $taskGroup.Name
                if (![regex]::IsMatch($taskOriginal, '\b' + [regex]::Escape($taskName) + '\b')) { continue }
                $taskVisible = @($taskGroup.Group | Where-Object {
                    $taskNs = $_.OldName.Substring(0,$_.OldName.LastIndexOf('.'))
                    $taskOldNs -eq $taskNs -or $taskOldNs.StartsWith($taskNs + '.') -or $taskNs -in $taskImports
                })
                if ($taskVisible.Count -ne 1) { continue }
                $taskType = $taskVisible[0]
                $taskTypeNs = $taskType.NewName.Substring(0,$taskType.NewName.LastIndexOf('.'))
                if ($taskTypeNs -ne $taskNewNs -and $taskTypeNs -ne 'EdgIQ.MurimiOS.Domain.Common') {
                    'using ' + $taskName + ' = global::' + $taskType.NewName + ';'
                }
            })
            if ($taskAliases.Count) {
                $taskNamespaceEnd = [regex]::Match($taskUpdated, '\bnamespace\s+[\w.]+\s*[;{]')
                $taskUpdated = $taskUpdated.Insert($taskNamespaceEnd.Index + $taskNamespaceEnd.Length, $taskNewline + ($taskAliases -join $taskNewline))
            }
            if ($taskUpdated -ne $taskCurrent) { [IO.File]::WriteAllText($taskPath,$taskUpdated,$taskUtf8); $taskRepaired++ }
        }
    } finally { $taskArchive.Dispose() }
    [pscustomobject]@{ RepairedNamespaceImports=$taskRepaired }
    exit
}

if ($Verify) {
    $taskManifest = Get-Content -LiteralPath (Join-Path $taskBaseline 'baseline.json') -Raw | ConvertFrom-Json
    $taskMigrations = @($taskManifest.Files | Where-Object { $_.Path -match '/Migrations/' })
    foreach ($taskEntry in $taskMigrations) {
        if ((Get-FileHash -LiteralPath (Resolve-TaskPath $taskEntry.Path) -Algorithm SHA256).Hash -ne $taskEntry.Sha256) {
            throw "Frozen migration changed: $($taskEntry.Path)"
        }
    }
    $taskCurrent = @(rg --files Infrastructure | Where-Object { $_ -match '[\\/]Migrations[\\/]' })
    if ($taskCurrent.Count -ne $taskMigrations.Count) { throw 'Migration file count changed.' }
    foreach ($taskMove in $taskManifest.Moves) {
        if (Test-Path -LiteralPath (Resolve-TaskPath $taskMove.Source)) { throw "Old source remains: $($taskMove.Source)" }
        if (!(Test-Path -LiteralPath (Resolve-TaskPath $taskMove.Target))) { throw "Missing target: $($taskMove.Target)" }
    }
    $taskCanonicalTypes = @{}
    foreach ($taskType in $taskManifest.Types) {
        $taskCanonicalTypes[$taskType.OldName] = $taskType.OldName
        $taskCanonicalTypes[$taskType.NewName] = $taskType.OldName
    }
    function Get-StructuralBody([string]$Text) {
        $taskBody = [regex]::Replace($Text, '(?m)^[ \t]*(?:global\s+)?using\s+(?:static\s+)?[\w.]+(?:\s*=\s*(?:global::)?[\w.]+)?;\r?\n?', '')
        $taskBody = [regex]::Replace($taskBody, '\bnamespace\s+[\w.]+', 'namespace __Namespace')
        $taskBody = [regex]::Replace($taskBody, '(?<![\w.])Domain\.(Entities|SharedKernel|Platform|Interfaces|Events|Exceptions)\.', 'Edgias.MurimiOS.Domain.$1.')
        $taskBody = [regex]::Replace($taskBody, '\b(?:EdgIQ|Edgias)\.MurimiOS\.(?:Domain|Application)(?:\.\w+)+', {
            param($taskMatch)
            $taskQualified = $taskMatch.Value
            $taskSuffix = ''
            while ($taskQualified.Contains('.')) {
                if ($taskCanonicalTypes.ContainsKey($taskQualified)) { return $taskCanonicalTypes[$taskQualified] + $taskSuffix }
                $taskLastDot = $taskQualified.LastIndexOf('.')
                $taskSuffix = $taskQualified.Substring($taskLastDot) + $taskSuffix
                $taskQualified = $taskQualified.Substring(0,$taskLastDot)
            }
            return $taskMatch.Value
        })
        return (@($taskBody -split '\r?\n' | Where-Object { $_.Trim().Length } | ForEach-Object { $_.TrimEnd() }) -join "`n")
    }
    $taskArchive = [IO.Compression.ZipFile]::OpenRead((Join-Path $taskBaseline 'sources.zip'))
    $taskVerifiedBodies = 0
    try {
        foreach ($taskFile in $taskManifest.Files | Where-Object { $_.Path.EndsWith('.cs') -and $_.Path -notmatch '/Migrations/' }) {
            $taskMoved = @($taskManifest.Moves | Where-Object Source -eq $taskFile.Path)
            $taskCurrentPath = if ($taskMoved.Count) { $taskMoved[0].Target } else { $taskFile.Path }
            $taskReader = [IO.StreamReader]::new($taskArchive.GetEntry($taskFile.Path).Open())
            try { $taskBefore = $taskReader.ReadToEnd() } finally { $taskReader.Dispose() }
            $taskAfter = [IO.File]::ReadAllText((Resolve-TaskPath $taskCurrentPath))
            if ((Get-StructuralBody $taskBefore) -cne (Get-StructuralBody $taskAfter)) {
                throw "Non-namespace/import source change found: $taskCurrentPath"
            }
            $taskVerifiedBodies++
        }
    } finally { $taskArchive.Dispose() }
    [pscustomobject]@{ VerifiedMoves = $taskManifest.Moves.Count; UnchangedMigrations = $taskMigrations.Count; NamespaceOnlySourceBodies=$taskVerifiedBodies; Backup = "$taskBaseline/sources.zip" }
    exit
}

if (!(Test-Path -LiteralPath (Resolve-TaskPath 'Domain/Domain.csproj'))) { throw 'This batch has already run or the Domain baseline is missing.' }
if (Test-Path -LiteralPath $taskBaseline) { throw 'Baseline already exists. Do not overwrite or rerun a partially applied batch.' }

$taskTracked = [Collections.Generic.HashSet[string]]::new([StringComparer]::OrdinalIgnoreCase)
git ls-files | ForEach-Object { [void]$taskTracked.Add($_) }
if ($LASTEXITCODE -ne 0) { throw 'Could not read tracked paths.' }
$taskSingleGroups = @{
    'FieldBoundaryVersion' = 'FarmField'; 'Money' = 'Common'; 'MeasurementUnits' = 'Common'
    'BusinessTime' = 'Common'; 'TenantEntity' = 'Common'; 'PlatformPermissions' = 'TenantMembership'
    'MembershipPermissionGrant' = 'TenantMembership'; 'ReferenceCatalogue' = 'ReferenceCatalogue'
}
$taskDeferred = @(
    'DigitalTwinModels','TraceabilityModels','RecallAndQuality','EpcisInteropModels',
    'SupplyNetworkModels','SupplierWorkflowModels','SupplierCompletionModels','IdentitySecurityModels',
    'SaasControlPlaneModels','CropPlanningModels','PublicEnterpriseModels','WorkOrderModels','EudrModels',
    'LivestockStatutory','LivestockRegistry','LivestockProductionModels','LivestockLaunchIntegrations',
    'LivestockHistory','LivestockFeed','ComplianceModels','FinanceModels','EnterpriseConsolidationModels',
    'FieldIntelligenceModels','MasterDataImportModels','GeospatialImportModels','AnalyticsModels',
    'AnalyticsMaturityModels','ProviderIngestionModels','ConnectedDataLifecycle','ConnectedAgricultureModels'
)
$taskMoves = @(foreach ($taskPath in (rg --files Domain)) {
    $taskSource = $taskPath.Replace('\','/')
    $taskParts = $taskSource.Split('/')
    $taskName = $taskParts[-1]
    $taskStem = [IO.Path]::GetFileNameWithoutExtension($taskName)
    $taskTarget = 'src/' + $taskSource
    if ($taskParts[1] -eq 'Interfaces' -or $taskStem -in @('ITenantContext','IAuditActorContext')) {
        $taskTarget = "src/Application/Common/Abstractions/$taskName"
    } elseif ($taskParts[1] -eq 'SharedKernel') {
        $taskTarget = "src/Domain/Common/$taskName"
    } elseif ($taskParts[1] -eq 'Events') {
        $taskTarget = "src/Domain/Loan/Events/$taskName"
    } elseif ($taskParts[1] -eq 'Entities') {
        $taskGroup = if ($taskParts.Count -gt 3) { $taskParts[2] -replace 'Aggregate$','' } elseif ($taskStem -eq 'Address') { 'Common' } elseif ($taskStem -eq 'Bin') { 'Bins' } else { $taskStem }
        $taskTarget = "src/Domain/$taskGroup/$taskName"
    } elseif ($taskParts[1] -eq 'Platform' -and $taskStem -notin $taskDeferred) {
        $taskGroup = if ($taskSingleGroups.ContainsKey($taskStem)) { $taskSingleGroups[$taskStem] } else { $taskStem }
        $taskTarget = "src/Domain/$taskGroup/$taskName"
    }
    $taskSourceFull = Resolve-TaskPath $taskSource
    $taskTargetFull = Resolve-TaskPath $taskTarget
    if (Test-Path -LiteralPath $taskTargetFull) { throw "Destination exists: $taskTarget" }
    $taskText = [IO.File]::ReadAllText($taskSourceFull)
    $taskNamespace = [regex]::Match($taskText, '\bnamespace\s+([\w.]+)').Groups[1].Value
    $taskNewNamespace = if ($taskNamespace) { 'EdgIQ.MurimiOS.' + (($taskTarget -replace '^src/','' -replace '/[^/]+$','').Replace('/','.')) } else { '' }
    [pscustomobject]@{ Source=$taskSource; Target=$taskTarget; Tracked=$taskTracked.Contains($taskSource); OldNamespace=$taskNamespace; NewNamespace=$taskNewNamespace; Text=$taskText }
})
if (($taskMoves.Target | Sort-Object -Unique).Count -ne $taskMoves.Count) { throw 'Duplicate destinations.' }

$taskTypes = @(foreach ($taskMove in $taskMoves | Where-Object { $_.Source -like '*.cs' }) {
    foreach ($taskMatch in [regex]::Matches($taskMove.Text, '\b(?:public|internal)\s+(?:(?:sealed|abstract|static|readonly|partial)\s+)*(?:class|record(?:\s+(?:class|struct))?|struct|interface|enum)\s+(\w+)')) {
        $taskTypeName = $taskMatch.Groups[1].Value
        [pscustomobject]@{ Name=$taskTypeName; OldNamespace=$taskMove.OldNamespace; NewNamespace=$taskMove.NewNamespace; OldName="$($taskMove.OldNamespace).$taskTypeName"; NewName="$($taskMove.NewNamespace).$taskTypeName"; Source=$taskMove.Source }
    }
})
$taskNamespaceMap = @{}
$taskTypeMap = @{}
foreach ($taskType in $taskTypes) {
    $taskTypeMap[$taskType.OldName] = $taskType.NewName
    $taskNamespaceMap[$taskType.OldNamespace] = @($taskTypes | Where-Object OldNamespace -eq $taskType.OldNamespace | Select-Object -ExpandProperty NewNamespace -Unique)
}
$taskFiles = @((rg --files Domain src/Application Infrastructure API Worker tests tools -g '*.cs' -g '*.csproj') + @('MurimiOS.sln','Directory.Build.props') | ForEach-Object { $_.Replace('\','/') })
$taskMoveMap = @{}
foreach ($taskMove in $taskMoves) { $taskMoveMap[$taskMove.Source] = $taskMove }
$taskEdits = @(foreach ($taskPath in $taskFiles | Where-Object { $_ -notmatch '/Migrations/' }) {
    $taskOriginal = [IO.File]::ReadAllText((Resolve-TaskPath $taskPath))
    $taskUpdated = $taskOriginal
    $taskNewPath = if ($taskMoveMap.ContainsKey($taskPath)) { $taskMoveMap[$taskPath].Target } else { $taskPath }
    $taskNewline = if ($taskOriginal.Contains("`r`n")) { "`r`n" } else { "`n" }
    if ($taskPath.EndsWith('.cs')) {
        $taskUpdated = [regex]::Replace($taskUpdated, '(?m)^(?<indent>[ \t]*)(?<global>global\s+)?using\s+(?<ns>Edgias\.MurimiOS\.Domain[\w.]*)[ \t]*;', {
            param($taskMatch)
            $taskNs = $taskMatch.Groups['ns'].Value
            if (!$taskNamespaceMap.ContainsKey($taskNs)) { return $taskMatch.Value }
            return (@($taskNamespaceMap[$taskNs] | Where-Object { !$taskPath.StartsWith('Domain/') -or !$_.StartsWith('EdgIQ.MurimiOS.Application.') } | ForEach-Object { $taskMatch.Groups['indent'].Value + $taskMatch.Groups['global'].Value + 'using ' + $_ + ';' }) -join $taskNewline)
        })
        $taskUpdated = [regex]::Replace($taskUpdated, '(?<![\w.])Domain\.(Entities|SharedKernel|Platform|Interfaces|Events|Exceptions)\.', 'Edgias.MurimiOS.Domain.$1.')
        $taskUpdated = [regex]::Replace($taskUpdated, '\bEdgias\.MurimiOS\.Domain(?:\.\w+)+', {
            param($taskMatch)
            $taskQualified = $taskMatch.Value
            # Also handle a static member following a type's fully qualified name.
            $taskSuffix = ''
            while ($taskQualified.Contains('.')) {
                if ($taskTypeMap.ContainsKey($taskQualified)) { return $taskTypeMap[$taskQualified] + $taskSuffix }
                $taskLastDot = $taskQualified.LastIndexOf('.')
                $taskSuffix = $taskQualified.Substring($taskLastDot) + $taskSuffix
                $taskQualified = $taskQualified.Substring(0,$taskLastDot)
            }
            return $taskMatch.Value
        })
        if ($taskMoveMap.ContainsKey($taskPath) -and $taskMoveMap[$taskPath].OldNamespace) {
            $taskMove = $taskMoveMap[$taskPath]
            $taskUpdated = [regex]::Replace($taskUpdated, '\bnamespace\s+' + [regex]::Escape($taskMove.OldNamespace) + '\b', 'namespace ' + $taskMove.NewNamespace)
            # Aggregate namespaces intentionally match their folders. Local aliases
            # retain unambiguous references when sibling namespaces have type names.
            $taskImports = @([regex]::Matches($taskOriginal, '(?m)^using\s+([\w.]+);') | ForEach-Object { $_.Groups[1].Value }) + @('Edgias.MurimiOS.Domain.SharedKernel')
            $taskCandidates = @($taskTypes | Where-Object { $_.OldNamespace -eq $taskMove.OldNamespace -or $taskMove.OldNamespace.StartsWith($_.OldNamespace + '.') -or $_.OldNamespace -in $taskImports })
            $taskAliases = @(foreach ($taskGroup in $taskCandidates | Group-Object Name) {
                $taskSameNamespace = @($taskGroup.Group | Where-Object OldNamespace -eq $taskMove.OldNamespace)
                $taskResolved = if ($taskSameNamespace.Count -eq 1) { $taskSameNamespace[0] } elseif ($taskGroup.Count -eq 1) { $taskGroup.Group[0] } else { $null }
                if ($null -ne $taskResolved -and $taskResolved.NewNamespace -ne $taskMove.NewNamespace -and $taskResolved.NewNamespace -ne 'EdgIQ.MurimiOS.Domain.Common' -and [regex]::IsMatch($taskOriginal, '\b' + [regex]::Escape($taskResolved.Name) + '\b')) {
                    'using ' + $taskResolved.Name + ' = global::' + $taskResolved.NewName + ';'
                }
            })
            if ($taskAliases.Count) {
                $taskNamespaceEnd = [regex]::Match($taskUpdated, '\bnamespace\s+[\w.]+\s*[;{]')
                $taskUpdated = $taskUpdated.Insert($taskNamespaceEnd.Index + $taskNamespaceEnd.Length, $taskNewline + ($taskAliases -join $taskNewline))
            }
        }
        # Remove only duplicate imports introduced by splitting an old shared namespace.
        $taskSeenImports = [Collections.Generic.HashSet[string]]::new([StringComparer]::Ordinal)
        $taskUpdated = [regex]::Replace($taskUpdated, '(?m)^(?:global\s+)?using\s+EdgIQ\.MurimiOS\.[\w.]+;\r?\n', {
            param($taskMatch)
            if ($taskSeenImports.Add($taskMatch.Value.Trim())) { return $taskMatch.Value }
            return ''
        })
    } elseif ($taskPath.EndsWith('.csproj')) {
        $taskUpdated = [regex]::Replace($taskUpdated, '(?<prefix><ProjectReference\s+Include=")(?<path>[^"]+)(?<suffix>")', {
            param($taskMatch)
            $taskReferencePath = [IO.Path]::GetFullPath((Join-Path ([IO.Path]::GetDirectoryName((Resolve-TaskPath $taskPath))) $taskMatch.Groups['path'].Value))
            if ($taskReferencePath -ne (Resolve-TaskPath 'Domain/Domain.csproj')) { return $taskMatch.Value }
            $taskRelative = [IO.Path]::GetRelativePath([IO.Path]::GetDirectoryName((Resolve-TaskPath $taskNewPath)), (Resolve-TaskPath 'src/Domain/Domain.csproj'))
            return $taskMatch.Groups['prefix'].Value + $taskRelative + $taskMatch.Groups['suffix'].Value
        })
    } elseif ($taskPath -eq 'MurimiOS.sln') {
        $taskUpdated = $taskUpdated.Replace('"Domain\Domain.csproj"','"src\Domain\Domain.csproj"')
    }
    if ($taskUpdated -ne $taskOriginal) { [pscustomobject]@{ Source=$taskPath; Target=$taskNewPath; Content=$taskUpdated } }
})

if (!$Apply) {
    [pscustomobject]@{ Moves=$taskMoves.Count; TrackedMoves=@($taskMoves | Where-Object Tracked).Count; UntrackedMoves=@($taskMoves | Where-Object { !$_.Tracked }).Count; MechanicalEdits=$taskEdits.Count; Contracts=@($taskMoves | Where-Object Target -like 'src/Application/*').Count; DeferredBundles=$taskDeferred.Count }
    $taskMoves | Select-Object Source,Target,Tracked | Format-Table -AutoSize
    exit
}

# Preflight has resolved every source and destination within the workspace and
# rejected overwrites. Archive bytes before git/filesystem moves or text rewrites.
[void][IO.Directory]::CreateDirectory($taskBaseline)
$taskHashes = @($taskFiles | ForEach-Object { [pscustomobject]@{ Path=$_; Sha256=(Get-FileHash -LiteralPath (Resolve-TaskPath $_) -Algorithm SHA256).Hash } })
$taskZip = [IO.Compression.ZipFile]::Open((Join-Path $taskBaseline 'sources.zip'), [IO.Compression.ZipArchiveMode]::Create)
try { foreach ($taskPath in $taskFiles) { [void][IO.Compression.ZipFileExtensions]::CreateEntryFromFile($taskZip, (Resolve-TaskPath $taskPath), $taskPath) } } finally { $taskZip.Dispose() }
$taskManifest = [pscustomobject]@{ Files=$taskHashes; Moves=@($taskMoves | Select-Object Source,Target,Tracked); Types=@($taskTypes | Select-Object OldName,NewName); Edits=@($taskEdits | Select-Object Source,Target) }
[IO.File]::WriteAllText((Join-Path $taskBaseline 'baseline.json'), ($taskManifest | ConvertTo-Json -Depth 6), $taskUtf8)
foreach ($taskMove in $taskMoves) {
    $taskSourceFull = Resolve-TaskPath $taskMove.Source
    $taskTargetFull = Resolve-TaskPath $taskMove.Target
    [void][IO.Directory]::CreateDirectory([IO.Path]::GetDirectoryName($taskTargetFull))
    if ($taskMove.Tracked) {
        git mv -- $taskMove.Source $taskMove.Target
        if ($LASTEXITCODE -ne 0) { throw "git mv failed: $($taskMove.Source). Baseline preserved at $taskBaseline." }
    } else {
        Move-Item -LiteralPath $taskSourceFull -Destination $taskTargetFull
    }
}
foreach ($taskEdit in $taskEdits) { [IO.File]::WriteAllText((Resolve-TaskPath $taskEdit.Target), $taskEdit.Content, $taskUtf8) }
[pscustomobject]@{ Moved=$taskMoves.Count; MechanicalEdits=$taskEdits.Count; Backup="$taskBaseline/sources.zip" }
