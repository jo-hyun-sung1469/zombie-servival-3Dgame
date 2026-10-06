---
name: commit-and-pr
description: 변경 사항을 논리 단위로 나눠 커밋하고, 요청 시 브랜치 푸시와 GitHub PR까지 만든다. 사용자가 "커밋해줘", "PR 만들어줘", "푸시하고 PR 올려줘"처럼 요청할 때 사용. Unity 프로젝트의 .meta 쌍과 자동 생성 폴더 제외 규칙을 포함한다.
---

# 커밋 & PR 작성

## 0. 범위 확인
- 사용자가 "커밋"만 요청했으면 커밋까지만 한다.
- "PR"이 명시된 경우에만 푸시와 PR 생성까지 진행한다.

## 1. 현재 상태 파악
```
git status --short
git diff --stat
git log -10 --oneline
```
- `git log`로 기존 커밋 메시지의 언어와 스타일을 확인해 맞춘다. 기준이 없으면 한국어로 작성한다.
- 현재 브랜치가 `main`/`master`이면 커밋 전에 작업 브랜치(`feature/...`, `fix/...`)를 만들자고 사용자에게 제안한다.

## 2. 스테이징 원칙 (Unity)
- `git add -A`/`git add .`를 바로 쓰지 말고 관련 파일을 경로로 지정해 추가한다.
- 절대 스테이징하지 않는다: `Library/`, `Temp/`, `Logs/`, `obj/`, `UserSettings/`, `.vs/`, `.idea/`, `*.csproj`, `*.sln` (`.gitignore` 정책이 다르면 프로젝트 정책을 따른다).
- `.cs`, `.prefab`, `.unity`, 에셋 파일은 대응하는 `.meta` 파일과 반드시 함께 스테이징한다. `.meta`만 혼자 변경되어 있으면 이유를 사용자에게 확인한다.
- 대용량 바이너리(텍스처, 오디오, 모델)가 추가되면 크기를 알리고 Git LFS 사용 여부를 확인한다.
- 서로 다른 목적의 변경이 섞여 있으면 커밋을 나눈다.

## 3. 커밋 메시지
Conventional Commits 형식을 쓴다.
```
type(scope): 요약 (50자 안팎, 마침표 없음)

- 무엇을, 왜 바꿨는지 본문에 적는다
```
- type: `feat`, `fix`, `refactor`, `perf`, `test`, `docs`, `chore`, `build`
- scope 예: `player`, `zombie-ai`, `ui`, `network`, `scene`
- 본문에는 어떻게보다 왜를 쓴다. 관련 이슈가 있으면 `Refs #123`.
- 금지: `--no-verify`, 이미 푸시한 커밋의 `--amend`/rebase.

## 4. 푸시 & PR (PR을 요청받은 경우)
1. `git push -u origin <현재 브랜치>` (강제 푸시 금지)
2. 기준 브랜치(기본 `main`)와의 차이를 `git log <base>..HEAD --oneline`, `git diff <base>...HEAD --stat`으로 확인한다.
3. `gh` CLI가 있으면 `gh pr create --base <base> --title "<제목>" --body-file <임시파일>`로 만든다. 없거나 인증이 안 되어 있으면 제목과 본문을 출력하고 직접 생성하도록 안내한다.

### PR 본문 템플릿
```
## 요약
- 무엇을 왜 바꿨는지 1~3줄

## 변경 사항
- 주요 변경 목록

## Unity 체크
- [ ] 씬/프리팹 변경 있음 (있다면 파일명 나열)
- [ ] 새 스크립트/에셋의 .meta 포함
- [ ] 패키지(manifest.json) 변경 있음
- [ ] 에디터에서 컴파일 에러/경고 없음 확인

## 테스트 방법
1. 재현/확인 절차

## 스크린샷 / 영상 (UI·연출 변경 시)
```
- 실제로 확인하지 않은 체크박스는 체크하지 않는다.

## 5. 마무리 보고
- 만든 커밋 목록(해시 + 제목), 푸시한 브랜치, PR 링크(있다면)를 짧게 알린다.
- 실행하지 못한 단계가 있으면 이유와 함께 명시한다.
