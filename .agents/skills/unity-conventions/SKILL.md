---
name: unity-conventions
description: Unity C# 스크립트를 작성하거나 수정할 때 따르는 프로젝트 코딩 규칙과 성능 체크리스트. .cs 파일, MonoBehaviour, ScriptableObject, 프리팹/씬, 게임 로직 구현·리팩터링 작업이면 언급이 없어도 반드시 이 스킬을 사용할 것.
---

# Unity 코딩 규칙

Unity 프로젝트에서 C# 코드를 작성·수정할 때 아래 규칙을 따른다. 규칙과 기존 코드 스타일이 충돌하면 기존 스타일을 우선하고, 차이점만 사용자에게 알린다.

## 1. 구조
- 파일명 = 클래스명. 한 MonoBehaviour는 한 가지 책임만 가진다.
- 인스펙터 노출은 `[SerializeField] private`을 쓰고 `public` 필드는 지양한다.
- 필수 컴포넌트는 `[RequireComponent(typeof(...))]`로 명시한다.
- 직렬화되는 필드 이름을 바꿀 때는 `[FormerlySerializedAs("이전이름")]`로 기존 씬/프리팹 값을 보존한다.
- 공유 데이터와 설정은 ScriptableObject로 분리한다.

## 2. 라이프사이클
- `Awake`: 자기 자신 초기화, `GetComponent` 캐싱.
- `Start`: 다른 오브젝트에 의존하는 초기화.
- `OnEnable`/`OnDisable`: 이벤트 구독과 해제는 반드시 쌍으로 작성한다.
- `OnDestroy`: 남은 리소스, 코루틴, 델리게이트 정리.

## 3. 성능 (Update / FixedUpdate / LateUpdate)
- `GetComponent`, `Find*`, `Camera.main` 반복 호출 금지 -> 필드에 캐싱.
- 매 프레임 `new`(리스트, 배열, 문자열 연결) 금지 -> 재사용, `NonAlloc` API 사용.
- `tag ==` 대신 `CompareTag`.
- `Instantiate`/`Destroy`가 잦으면 오브젝트 풀링.
- `WaitForSeconds` 등 코루틴 yield 객체는 캐싱.
- 물리 이동과 `Rigidbody` 조작은 `FixedUpdate`, 시간 보정은 `Time.deltaTime`/`fixedDeltaTime`.
- 무거운 로그(`Debug.Log`)는 남기지 않거나 `[Conditional("UNITY_EDITOR")]`로 감싼다.

## 4. Unity 특유의 함정
- `UnityEngine.Object`는 `== null` 오버로드가 있다. `?.`, `??`는 파괴된 오브젝트를 걸러내지 못하므로 주의한다.
- Unity API는 메인 스레드에서만 호출한다. `async/await` 사용 시 취소 토큰과 오브젝트 파괴 여부를 확인한다.
- 코루틴은 비활성/파괴된 오브젝트에서 멈춘다. 수명 관리를 명확히 한다.

## 5. 편집하면 안 되는 것
- `Library/`, `Temp/`, `Logs/`, `obj/`, `UserSettings/`, `.meta`, `ProjectSettings/`는 직접 수정하지 않는다. (훅이 차단한다)
- `.unity`/`.prefab` YAML 직접 편집은 최후의 수단이다. 가능하면 에디터 스크립트를 만들거나 사용자에게 에디터 작업을 요청한다.
- 새 `.cs` 파일의 `.meta`는 유니티 에디터가 열릴 때 생성된다. 직접 만들지 않는다.

## 6. 멀티플레이/서버 코드 (해당하는 경우)
- 서버 권위를 기본으로 하고, 클라이언트 입력은 항상 서버에서 검증한다.
- 프레임레이트에 의존하는 로직을 피하고 틱 기반으로 작성한다.
- 네트워크로 주고받는 데이터 구조를 바꿀 때는 버전과 호환성을 확인한다.

## 작업 마무리
- 변경한 파일 목록과 성능/안전상 신경 쓴 부분을 짧게 요약한다.
- 컴파일은 Unity 에디터 콘솔 또는 프로젝트의 배치 모드 빌드/테스트 명령으로 확인하도록 안내한다. (실행 환경이 없으면 확인하지 못했다고 명시한다)
