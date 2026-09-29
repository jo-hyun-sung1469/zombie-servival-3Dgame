---
name: unity-cli
description: Unity 에디터를 터미널(배치 모드)에서 실행해 빌드, 테스트, 정적 메서드 실행, 패키지/에셋 처리를 자동화한다. "유니티 빌드해줘", "커맨드라인으로 빌드/테스트", "batchmode", "CI에서 유니티 돌리기" 같은 요청에 사용. Unity 에디터를 직접 조작하는 게 아니라 CLI로 실행하는 모든 작업에 적용.
---

# Unity CLI (배치 모드)

Unity 에디터를 GUI 없이 터미널에서 실행하는 방법. 빌드 자동화, 테스트 실행, CI 파이프라인, 배치 작업에 쓴다.

## 0. 먼저 확인할 것
- **Unity 에디터 실행 파일 경로**와 **프로젝트가 쓰는 Unity 버전**을 모르면 추측하지 말고 사용자에게 묻거나 `ProjectSettings/ProjectVersion.txt`를 읽어 확인한다.
- **다른 Unity 인스턴스가 이미 프로젝트를 열고 있으면 배치 모드 실행이 실패한다.** 실행 전에 확인한다.
- 에디터 경로 예시:
  - Windows: `C:\Program Files\Unity\Hub\Editor\<버전>\Editor\Unity.exe`
  - macOS: `/Applications/Unity/Hub/Editor/<버전>/Unity.app/Contents/MacOS/Unity`
  - Linux: `~/Unity/Hub/Editor/<버전>/Editor/Unity`
  - Unity Hub로 설치했다면 위 경로가 기본이고, 아니라면 사용자에게 실제 경로를 물어본다.

## 1. 기본 원칙
- 자동화(CI, 스크립트)에서는 항상 `-batchmode -nographics -quiet`(필요 시) 조합을 쓰고, 끝나면 `-quit`을 붙인다. 단, `-runTests`는 `-quit` 없이도 테스트 종료 후 자동으로 종료된다.
- `-logFile <경로>`를 항상 지정해서 로그를 파일로 남긴다. (`-logFile -`로 stdout에도 출력 가능)
- `-projectPath <경로>`로 대상 프로젝트를 명시한다. 생략하면 마지막에 연 프로젝트를 사용해 사고가 날 수 있다.
- 종료 코드(`$LASTEXITCODE` / `$?`)를 항상 확인한다. 0이 아니면 실패이며, 로그 파일에서 원인을 찾는다.
- 실행이 끝나면 로그 파일의 에러/경고를 요약해서 사용자에게 보고한다. 로그를 직접 읽지 않고 "성공했다"고 단정하지 않는다.

## 2. 빌드
### 2-1. 커스텀 빌드 스크립트 (`-executeMethod`) — 권장
프로젝트에 `Assets/Editor/` 아래 정적 빌드 메서드가 있으면 이 방식을 쓴다.
```powershell
& "<Unity 경로>" `
  -batchmode -nographics -quit `
  -projectPath "<프로젝트 경로>" `
  -executeMethod BuildScript.BuildWindows `
  -logFile "<프로젝트 경로>\build.log"
```
- `-executeMethod`로 호출하는 메서드는 `static`이어야 하고 `Editor` 폴더 안에 있어야 한다.
- 빌드 실패 시 스크립트에서 예외를 던지거나 `EditorApplication.Exit(1)`을 호출해야 CLI 종료 코드에 반영된다. 프로젝트에 이런 처리가 없으면 실패해도 종료 코드가 0일 수 있으니 로그로 성공 여부를 재확인한다.
- 프로젝트에 이런 빌드 스크립트가 없다면, 만들어 줄지 사용자에게 먼저 확인한다 (Editor 폴더에 새 `.cs` 추가 = 코드 변경이므로).

### 2-2. 레거시 플레이어 빌드 플래그 (스크립트 없이 빠르게)
```powershell
& "<Unity 경로>" -batchmode -nographics -quit -projectPath "<경로>" -buildWindows64Player "<출력경로>\Game.exe"
```
- macOS: `-buildOSXUniversalPlayer`, Linux: `-buildLinux64Player` 등 타깃별 플래그가 다르다. 정확한 플래그명은 사용 중인 Unity 버전 문서에서 확인하고, 확신이 없으면 `-executeMethod` 방식을 권장한다.
- 씬 목록, 플랫폼별 설정(BuildTarget, Scripting Backend 등)을 세밀하게 제어하려면 결국 2-1(커스텀 스크립트)이 필요하다.

## 3. 테스트 실행 (Unity Test Framework)
```powershell
& "<Unity 경로>" `
  -batchmode -nographics `
  -projectPath "<프로젝트 경로>" `
  -runTests `
  -testPlatform EditMode `
  -testResults "<경로>\results.xml" `
  -logFile "<경로>\test.log"
```
- `-testPlatform`: `EditMode` 또는 `PlayMode` (미지정 시 EditMode). 특정 빌드 타깃에서 Play Mode 테스트를 돌리려면 `BuildTarget` 값(예: `StandaloneWindows64`, `Android`)을 넣는다.
- `-testResults`: NUnit XML 형식으로 저장. CI에서 이 파일을 파싱해 pass/fail을 판단한다.
- `-testFilter "이름;이름"`: 특정 테스트만 실행 (정규식 가능, `!`로 제외).
- `-testCategory "카테고리;카테고리"`: 카테고리로 필터링.
- `-assemblyNames "어셈블리;어셈블리"`: 특정 테스트 어셈블리만 실행.
- **주의**: `-runTests`는 종료 코드만으로 테스트 성공/실패를 항상 신뢰할 수 있는 건 아니다. 반드시 `-testResults` XML을 열어 실패한 테스트 개수를 확인하고 보고한다.

## 4. 임의의 에디터 작업 실행 (`-executeMethod`)
빌드/테스트 외에 에셋 임포트 정리, 데이터 생성, 검증 스크립트 등을 CLI로 돌릴 때도 동일 패턴을 쓴다.
```powershell
& "<Unity 경로>" -batchmode -nographics -quit -projectPath "<경로>" -executeMethod MyTools.Validate -logFile "<경로>\validate.log"
```
- 커스텀 파라미터를 넘기려면 명령줄에 `-key value` 형태로 추가하고, 스크립트에서 `System.Environment.GetCommandLineArgs()`로 파싱한다.

## 5. 자주 쓰는 플래그 요약
| 플래그 | 용도 |
|---|---|
| `-batchmode` | GUI 없이 실행 (팝업 방지, 사람 개입 불필요) |
| `-nographics` | 그래픽 디바이스 초기화 생략 (GPU 없는 CI 머신용) |
| `-quit` | 작업 완료 후 에디터 종료 (`-runTests`에는 보통 불필요) |
| `-projectPath <경로>` | 대상 프로젝트 지정 |
| `-logFile <경로 또는 ->` | 로그 파일 경로 (`-`는 stdout) |
| `-executeMethod <Class.Method>` | 지정한 정적 메서드 실행 |
| `-buildTarget <타깃>` | 빌드 타깃 지정 (예: `Win64`, `Android`) |
| `-runTests` | Test Framework로 테스트 실행 |
| `-testPlatform` / `-testResults` / `-testFilter` / `-testCategory` | 테스트 실행 옵션 |
| `-forgetProjectPath` | Unity Hub/런처 히스토리에 프로젝트 기록 안 함 (CI에 유용) |

플래그는 Unity 버전에 따라 추가/변경될 수 있다. 확신이 없으면 실제 실행 전에 웹에서 해당 버전 문서를 확인하거나, `Unity -help`(또는 `-?`)로 로컬에서 확인하도록 안내한다.

## 6. 안전 규칙
- 배치 모드 실행은 시간이 오래 걸리고 대상 프로젝트를 실제로 빌드/수정할 수 있다. **처음 실행하기 전에 정확한 명령을 사용자에게 보여주고 확인받는다** (반복 실행이 확립된 경우는 예외).
- 출력 경로가 기존 파일을 덮어쓸 수 있으면 미리 알린다.
- `.codex/hooks/guard-unity.ps1`이 막는 경로(`Library/`, `ProjectSettings/` 등)를 빌드 출력 경로로 쓰지 않는다.
- 실행 결과(성공/실패, 에러 요약, 산출물 경로)를 항상 명확히 보고한다.
