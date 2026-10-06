# 프로젝트 지침 (Codex)

Unity 기반 프로젝트. 아래 규칙을 항상 따른다.

## 스킬
- C#(.cs) 작성/수정 시 `.agents/skills/unity-conventions` 규칙을 따른다.
- 코드 리뷰 요청("리뷰해줘", "PR 검토") 시 `.agents/skills/unity-code-review`로 읽기 전용 리뷰를 수행한다.
- 커밋/PR 요청 시에만 `.agents/skills/commit-and-pr`을 사용한다 (부작용이 있으므로 명시적 요청 없이는 실행하지 않음).

## 절대 직접 수정하지 않는 것
`Library/`, `Temp/`, `Logs/`, `obj/`, `UserSettings/`, `.meta`, `ProjectSettings/`, `Packages/packages-lock.json`
(`.codex/hooks/guard-unity.ps1` 훅이 이 규칙을 강제로 차단한다. 정말 필요하면 이유를 설명하고 사용자 확인을 받는다.)

## 빌드 / 테스트
<!-- TODO: 프로젝트에 맞게 채워주세요 -->
- 빌드: (예: Unity 배치 모드 명령 또는 CI 스크립트 경로)
- 테스트: (예: Unity Test Runner 실행 명령)
- 린트/포맷: (있다면)

## Git
- 작업은 `main`/`master`에 직접 하지 않고 브랜치를 만든다.
- 강제 푸시(`--force`), `git reset --hard`, `git clean -f` 금지 (훅이 차단함).
- `.cs`/`.prefab`/`.unity` 변경 시 대응하는 `.meta`를 함께 커밋한다.

## 응답 언어
- 별도 요청이 없으면 한국어로 답한다.
