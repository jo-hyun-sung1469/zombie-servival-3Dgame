# PreToolUse 훅: 유니티 프로젝트 보호 규칙
# - 자동 생성 폴더 / .meta / ProjectSettings 등 수정 차단
# - 위험한 셸 명령(강제 푸시, 재귀 삭제 등) 차단
# 차단 시: stderr에 사유 출력 + exit code 2
# 임시 해제: 환경변수 CODEX_UNITY_ALLOW_PROTECTED=1

[Console]::InputEncoding  = [Text.UTF8Encoding]::new($false)
[Console]::OutputEncoding = [Text.UTF8Encoding]::new($false)

if ($env:CODEX_UNITY_ALLOW_PROTECTED -eq '1') { exit 0 }

$raw = [Console]::In.ReadToEnd()
if ([string]::IsNullOrWhiteSpace($raw)) { exit 0 }
try { $payload = $raw | ConvertFrom-Json } catch { exit 0 }

function Block([string]$msg) {
    [Console]::Error.WriteLine("[unity-guard] $msg")
    exit 2
}

$toolInput = $payload.tool_input
$paths = New-Object System.Collections.Generic.List[string]
$cmd = ''

if ($toolInput -is [string]) {
    $cmd = $toolInput
}
elseif ($null -ne $toolInput) {
    $names = @($toolInput.PSObject.Properties.Name)
    if ($names -contains 'command') {
        $c = $toolInput.command
        if ($c -is [array]) { $cmd = ($c -join ' ') } else { $cmd = "$c" }
    }
    foreach ($k in 'file_path', 'path', 'filePath') {
        if ($names -contains $k -and $toolInput.$k) { $paths.Add("$($toolInput.$k)") }
    }
}

# apply_patch 본문에서 대상 파일 경로 추출
$isPatch = $cmd -match '\*\*\* Begin Patch'
if ($isPatch) {
    foreach ($line in ($cmd -split "`r?`n")) {
        if ($line -match '^\*\*\* (?:Add|Update|Delete) File:\s*(.+?)\s*$') { $paths.Add($Matches[1]) }
        elseif ($line -match '^\*\*\* Move to:\s*(.+?)\s*$')                { $paths.Add($Matches[1]) }
    }
}

# 보호 경로 규칙: 정규식 => 사유
$protected = @(
    @{ Re = '(^|/)(Library|Temp|Logs|obj|UserSettings)/'; Why = '유니티가 자동 생성하는 폴더입니다.' },
    @{ Re = '(^|/)\.git/';                                Why = 'Git 내부 파일입니다.' },
    @{ Re = '\.meta$';                                    Why = '.meta는 유니티 에디터가 관리합니다 (GUID가 깨지면 씬/프리팹 참조가 끊어짐).' },
    @{ Re = '(^|/)ProjectSettings/';                      Why = '프로젝트 설정은 유니티 에디터에서 변경해야 합니다.' },
    @{ Re = '(^|/)Packages/packages-lock\.json$';         Why = '패키지 락 파일은 유니티가 관리합니다.' }
)

function Test-Protected([string]$p) {
    $n = ($p -replace '\\', '/')
    foreach ($rule in $protected) {
        if ($n -match $rule.Re) { return $rule.Why }
    }
    return $null
}

foreach ($p in $paths) {
    $why = Test-Protected $p
    if ($why) { Block "수정 차단: $p - $why (필요하면 사용자 확인 후 CODEX_UNITY_ALLOW_PROTECTED=1)" }
}

# 셸 명령 검사 (패치가 아닌 경우)
if ($cmd -and -not $isPatch) {
    $normalized = ($cmd -replace '\\', '/')

    # 1) 보호 경로에 대한 '쓰기성' 명령 (휴리스틱)
    $writeVerb = '(Set-Content|Add-Content|Out-File|Remove-Item|Move-Item|Rename-Item|Copy-Item|\bsed\s+-i|\brm\b|\bdel\b|\bmv\b|\btee\b|>>?)'
    if ($normalized -match $writeVerb) {
        foreach ($rule in $protected) {
            if ($normalized -match $rule.Re.Replace('(^|/)', '(^|[/\s"''])').Replace('$', '($|[\s"''])')) {
                Block "보호 경로를 변경하는 명령으로 보여 차단합니다. $($rule.Why)"
            }
        }
    }

    # 2) 위험 명령
    $dangerous = @(
        @{ Re = 'git\s+reset\s+--hard';                                 Why = '커밋되지 않은 작업이 사라집니다.' },
        @{ Re = 'git\s+clean\s+-\w*f';                                  Why = '추적되지 않는 파일이 삭제됩니다.' },
        @{ Re = 'git\s+push\b.*(--force(?!-with-lease)|\s-f(\s|$))';    Why = '강제 푸시는 금지입니다 (--force-with-lease는 허용).' },
        @{ Re = 'git\s+(checkout\s+--\s+\.|restore\s+\.)';              Why = '전체 변경 사항을 되돌립니다.' },
        @{ Re = '\brm\s+-\w*[rR]';                                      Why = '재귀 삭제입니다.' },
        @{ Re = 'Remove-Item\b.*-Recurse';                              Why = '재귀 삭제입니다.' },
        @{ Re = '\b(rd|rmdir|del)\s+.*/s\b';                            Why = '재귀 삭제입니다.' }
    )
    foreach ($d in $dangerous) {
        if ($normalized -match $d.Re) { Block "위험 명령 차단: $($d.Why) 필요하면 사용자에게 직접 확인받으세요." }
    }
}

exit 0
