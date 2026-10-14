# PostToolUse 훅: 변경된 .cs 파일의 유니티 성능/안전 패턴 점검
# 페이로드에 의존하지 않도록 git status 로 변경된 .cs 를 직접 찾습니다.
# 문제가 있으면 additionalContext 로 모델에게 알립니다 (차단하지 않음).

[Console]::InputEncoding  = [Text.UTF8Encoding]::new($false)
[Console]::OutputEncoding = [Text.UTF8Encoding]::new($false)

$raw = [Console]::In.ReadToEnd()
$cwd = $null
try { if ($raw) { $cwd = ($raw | ConvertFrom-Json).cwd } } catch {}
if ($cwd -and (Test-Path $cwd)) { Set-Location $cwd }

$status = git status --porcelain 2>$null
if (-not $status) { exit 0 }

$files = @()
foreach ($line in $status) {
    if ($line.Length -lt 4) { continue }
    $p = $line.Substring(3).Trim('"')
    if ($p -like '* -> *') { $p = ($p -split ' -> ')[-1] }
    if ($p -match '\.cs$' -and $p -notmatch '(^|/)(Library|Temp|Logs|obj)/' -and (Test-Path $p)) { $files += $p }
}
$files = $files | Select-Object -First 30
if (-not $files) { exit 0 }

$hotPatterns = @(
    @{ Re = 'GetComponent(s)?(InChildren|InParent)?\s*[<(]';                Msg = 'Update 계열에서 GetComponent 호출 -> Awake/Start에서 캐싱' },
    @{ Re = '\bFind(ObjectOfType|ObjectsOfType|ObjectByType|ObjectsByType|WithTag|GameObjectsWithTag)?\s*[<(]|GameObject\.Find'; Msg = 'Update 계열에서 Find 계열 호출 -> 참조 캐싱' },
    @{ Re = '\bnew\s+(List|Dictionary|HashSet|Queue|Stack)\s*<';            Msg = 'Update 계열에서 컬렉션 생성(GC 할당) -> 필드로 재사용' },
    @{ Re = '\bnew\s+\w+(<[^>]+>)?\s*\[';                                   Msg = 'Update 계열에서 배열 할당(GC) -> 재사용/NonAlloc API' },
    @{ Re = 'Debug\.Log(Warning|Error)?\s*\(';                              Msg = '매 프레임 로그 -> 제거하거나 조건부 컴파일' },
    @{ Re = 'Instantiate\s*\(|Destroy\s*\(';                                Msg = 'Update 계열에서 Instantiate/Destroy -> 오브젝트 풀링 검토' }
)
$anyPatterns = @(
    @{ Re = '\.tag\s*==';        Msg = 'tag 문자열 비교 -> CompareTag 사용' },
    @{ Re = '\bSendMessage\s*\('; Msg = 'SendMessage는 느리고 타입 안전하지 않음 -> 직접 호출/이벤트' }
)

$findings = New-Object System.Collections.Generic.List[string]

foreach ($f in $files) {
    $lines = Get-Content -LiteralPath $f -Encoding UTF8
    $inHot = $false; $depth = 0; $opened = $false
    for ($i = 0; $i -lt $lines.Count; $i++) {
        $t = $lines[$i].Trim()
        if ($t.StartsWith('//')) { continue }
        $no = $i + 1

        if (-not $inHot -and $t -match '\bvoid\s+(Update|FixedUpdate|LateUpdate)\s*\(') {
            $inHot = $true; $depth = 0; $opened = $false
        }
        if ($inHot) {
            foreach ($h in $hotPatterns) {
                if ($t -match $h.Re) { $findings.Add("${f}:${no} $($h.Msg)") }
            }
            $depth  += ([regex]::Matches($t, '\{')).Count
            if (([regex]::Matches($t, '\{')).Count -gt 0) { $opened = $true }
            $depth  -= ([regex]::Matches($t, '\}')).Count
            if ($opened -and $depth -le 0) { $inHot = $false }
        }
        foreach ($a in $anyPatterns) {
            if ($t -match $a.Re) { $findings.Add("${f}:${no} $($a.Msg)") }
        }
    }
}

if ($findings.Count -eq 0) { exit 0 }

$shown = $findings | Select-Object -First 20
$msg = "유니티 C# 점검에서 확인이 필요한 항목 (자동 휴리스틱, 오탐 가능):`n" + ($shown -join "`n")
if ($findings.Count -gt 20) { $msg += "`n... 외 $($findings.Count - 20)건" }

$out = @{ hookSpecificOutput = @{ hookEventName = 'PostToolUse'; additionalContext = $msg } }
$out | ConvertTo-Json -Depth 5 -Compress
exit 0
