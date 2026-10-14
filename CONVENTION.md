# Unity 네이밍 컨벤션

Unity 프로젝트에서 스크립트를 작성하거나, 에셋/폴더 이름을 정하거나, 기존 이름을 리뷰·리네이밍할 때 이 규칙을 따른다. 프로젝트에 이미 다른 규칙이 문서화되어 있으면 그것을 우선하고, 충돌 지점만 사용자에게 알린다.

---

## 0. 공통 원칙

- **영어만 사용**한다. 한글·공백·특수문자(`- . ( ) # & 등`)는 파일/폴더/에셋 이름에 쓰지 않는다. (빌드, 버전관리, 플랫폼 경로 문제 방지)
- 구분자는 **언더스코어 `_`** 하나만 쓴다. 단어 내부는 **PascalCase**.
- 의미 있는 이름을 쓴다. `Test`, `New`, `Temp`, `Final`, `asdf` 같은 이름은 커밋하지 않는다.
- 축약어는 널리 알려진 것만(`UI`, `HP`, `VFX`, `NPC`, `ID`). 2글자 축약어는 대문자 유지(`UIManager`), 3글자 이상은 PascalCase(`HttpClient`, `XmlParser`).
- 번호는 **두 자리 0 패딩**: `_01`, `_02` … (`_1`, `_10` 정렬 문제 방지)
- 단수/복수: 하나를 나타내면 단수(`Enemy`), 컬렉션이면 복수(`enemies`).

---

## 1. C# 코드

### 1.1 요약표

| 대상 | 규칙 | 예시 |
|---|---|---|
| 네임스페이스 | PascalCase, `회사.프로젝트.모듈` | `Studio.Game.Combat` |
| 클래스 / 구조체 | PascalCase, 명사 | `PlayerController`, `DamageInfo` |
| 인터페이스 | `I` + PascalCase, 형용사/능력 | `IDamageable`, `IInteractable` |
| 추상 클래스 | PascalCase, 필요 시 `Base` 접미사 | `WeaponBase` |
| 열거형 타입 | PascalCase 단수 (Flags면 복수) | `WeaponType`, `[Flags] DamageFlags` |
| 열거형 값 | PascalCase | `WeaponType.Sword` |
| 메서드 | PascalCase, 동사로 시작 | `TakeDamage()`, `GetNearestEnemy()` |
| 코루틴 | PascalCase + `Routine` 접미사 | `FadeOutRoutine()` |
| 비동기 메서드 | PascalCase + `Async` 접미사 | `LoadSceneAsync()` |
| 프로퍼티 | PascalCase | `CurrentHealth`, `IsDead` |
| public 필드 | PascalCase (가급적 프로퍼티 사용) | `MaxSpeed` |
| private / protected 필드 | `_camelCase` | `_currentHealth` |
| `[SerializeField] private` | `_camelCase` | `[SerializeField] private float _moveSpeed;` |
| static private 필드 | `s_camelCase` | `s_instance` |
| 상수 (`const`) | PascalCase | `MaxPlayerCount` |
| `static readonly` | PascalCase | `DefaultColor` |
| 지역 변수 / 매개변수 | camelCase | `hitPoint`, `targetTransform` |
| 제네릭 타입 매개변수 | `T` 또는 `T` + PascalCase | `T`, `TItem` |
| 이벤트 / 델리게이트 필드 | `On` + 과거형/현재형 동사 | `OnDied`, `OnHealthChanged` |
| 이벤트 핸들러 메서드 | `Handle` + 이벤트명 또는 `대상_이벤트` | `HandleDied()`, `Player_OnDied()` |

### 1.2 세부 규칙

- **불리언**은 질문형 접두사: `is`, `has`, `can`, `should` → `IsGrounded`, `_hasKey`, `CanJump`.
- **컴포넌트 참조 필드**는 타입이 드러나게: `_rigidbody`, `_animator`, `_playerTransform`. Unity 내장 이름(`rigidbody`, `collider`, `renderer`, `camera`)을 언더스코어 없이 그대로 쓰지 않는다.
- **MonoBehaviour 파일명 = 클래스명** (한 파일에 하나의 public 클래스). 다르면 컴포넌트 추가가 불가하다.
- **ScriptableObject 클래스**는 `Data`, `Config`, `Definition`, `Settings` 등 역할 접미사: `WeaponData`, `AudioSettings`.
- **Manager 남용 금지**: 책임이 분명하면 `Spawner`, `Pool`, `Registry`, `Service`, `Controller`, `Handler` 등 구체적 이름을 쓴다.
- **Editor 전용 스크립트**는 `Editor` 폴더에 두고 접미사 `Editor` / `Window` / `Drawer`: `WeaponDataEditor`, `LevelBuilderWindow`, `RangeAttributeDrawer`.
- **Attribute 클래스**는 `Attribute` 접미사: `ReadOnlyAttribute`.
- **Animator 파라미터/상태 해시**는 static readonly로 캐싱: `private static readonly int s_speedHash = Animator.StringToHash("Speed");`
- **문자열 키(태그, 레이어, Animator 파라미터, Resources 경로)**는 상수 클래스로 모은다: `Tags.Player`, `Layers.Ground`, `AnimParams.Speed`.

### 1.3 예시

```csharp
namespace Studio.Game.Combat
{
    public interface IDamageable
    {
        void TakeDamage(DamageInfo info);
    }

    public class EnemyHealth : MonoBehaviour, IDamageable
    {
        public const int MaxHitStack = 3;
        private static readonly int s_hitHash = Animator.StringToHash("Hit");

        [SerializeField] private int _maxHealth = 100;
        [SerializeField] private Animator _animator;

        private int _currentHealth;

        public event Action<int> OnHealthChanged;
        public event Action OnDied;

        public bool IsDead => _currentHealth <= 0;

        public void TakeDamage(DamageInfo info)
        {
            int finalDamage = Mathf.Max(0, info.Amount);
            _currentHealth -= finalDamage;
            _animator.SetTrigger(s_hitHash);
            OnHealthChanged?.Invoke(_currentHealth);

            if (IsDead) OnDied?.Invoke();
        }

        private IEnumerator FlashRoutine() { yield break; }
    }
}
```

---

## 2. 에셋 네이밍

### 2.1 기본 형식

```text
[Prefix]_[BaseName]_[Variant]_[Suffix]
```

- **Prefix**: 에셋 종류 (아래 표)
- **BaseName**: 무엇인지 (PascalCase) – `Goblin`, `WoodenCrate`
- **Variant**: 변형/번호 (선택) – `Red`, `Large`, `01`
- **Suffix**: 텍스처 맵 종류 등 (선택)

예: `SM_WoodenCrate_Large_01`, `T_Goblin_Body_N`, `PF_Enemy_Goblin_Archer`

### 2.2 접두사 표

| 종류 | 접두사 | 예시 |
|---|---|---|
| Prefab | `PF_` | `PF_Player`, `PF_Enemy_Goblin` |
| Prefab Variant | `PF_` + Variant | `PF_Enemy_Goblin_Elite` |
| Static Mesh (FBX 등) | `SM_` | `SM_Rock_01` |
| Skeletal Mesh | `SK_` | `SK_Goblin` |
| Material | `M_` | `M_Rock` |
| Texture | `T_` | `T_Rock_D` |
| Sprite | `SPR_` | `SPR_Coin` |
| Sprite Atlas | `SA_` | `SA_UI_Common` |
| UI 이미지/아이콘 | `UI_` | `UI_Icon_Sword`, `UI_Btn_Close` |
| Shader (코드) | `SH_` | `SH_Dissolve` |
| Shader Graph | `SG_` | `SG_Water` |
| Sub Graph | `SSG_` | `SSG_Fresnel` |
| Animation Clip | `A_` | `A_Goblin_Run` |
| Animator Controller | `AC_` | `AC_Goblin` |
| Animator Override Controller | `AOC_` | `AOC_Goblin_Elite` |
| Avatar / Avatar Mask | `AV_` / `AM_` | `AM_UpperBody` |
| Timeline | `TL_` | `TL_Intro_Cutscene` |
| ScriptableObject 인스턴스 | `SO_` 또는 데이터 타입 축약 | `SO_Weapon_Sword` |
| Physics Material | `PM_` | `PM_Ice` |
| VFX Graph / Particle Prefab | `VFX_` | `VFX_Explosion_Large` |
| 사운드 효과 | `SFX_` | `SFX_Sword_Swing_01` |
| 배경 음악 | `BGM_` | `BGM_Town_Day` |
| 보이스 | `VO_` | `VO_Npc_Merchant_Greet_01` |
| Audio Mixer | `AMX_` | `AMX_Master` |
| Font / TMP Font Asset | `FNT_` | `FNT_NotoSans_SDF` |
| Render Texture | `RT_` | `RT_Minimap` |
| Cubemap / Skybox | `CUBE_` / `SKY_` | `SKY_Sunset` |
| Lighting Settings | `LS_` | `LS_Dungeon` |
| Post-process Volume Profile | `PP_` | `PP_Night` |
| Terrain Layer | `TLY_` | `TLY_Grass` |
| Input Action Asset | `IA_` | `IA_Player` |

> 스크립트(.cs)와 씬(.unity)은 접두사를 붙이지 않는다 (아래 참고).

### 2.3 텍스처 접미사

| 맵 | 접미사 |
|---|---|
| Base Color / Albedo | `_D` |
| Normal | `_N` |
| Mask (Metallic/AO/Detail/Smoothness 패킹) | `_MS` |
| Metallic | `_M` |
| Roughness / Smoothness | `_R` / `_S` |
| Ambient Occlusion | `_AO` |
| Emission | `_E` |
| Height | `_H` |
| Opacity / Alpha | `_A` |

예: `T_Goblin_Body_D`, `T_Goblin_Body_N`, `T_Goblin_Body_MS`

### 2.4 애니메이션 클립

`A_[캐릭터]_[동작]_[방향/변형]`

- `A_Goblin_Idle`, `A_Goblin_Walk_Fwd`, `A_Goblin_Attack_01`
- 루프 여부를 이름에 넣지 않는다 (임포트 설정으로 관리).

### 2.5 씬

- 접두사 없이 PascalCase, 용도별 묶음 이름을 앞에: `Boot`, `MainMenu`, `Level_01_Forest`, `Test_Combat`
- 테스트/개발 전용 씬은 `Test_`로 시작하고 빌드 설정에서 제외한다.

---

## 3. 폴더 구조

- 폴더명은 **PascalCase, 복수형**(`Prefabs`, `Materials`), 공백 금지.
- 서드파티 에셋과 섞이지 않게 프로젝트 고유 루트 폴더를 둔다. `_`로 시작하면 정렬상 맨 위에 온다.
- Unity 예약 폴더(`Editor`, `Resources`, `Plugins`, `StreamingAssets`, `Gizmos`, `Editor Default Resources`)는 의도할 때만 사용한다.

```text
Assets/
├── _Project/
│   ├── Art/
│   │   ├── Characters/
│   │   │   └── Goblin/          ← 기능(대상)별로 메시·텍스처·머티리얼 묶음
│   │   │       ├── SK_Goblin.fbx
│   │   │       ├── M_Goblin.mat
│   │   │       └── T_Goblin_Body_D.png
│   │   ├── Environment/
│   │   ├── UI/
│   │   └── VFX/
│   ├── Audio/
│   │   ├── BGM/
│   │   ├── SFX/
│   │   └── Mixers/
│   ├── Animations/
│   ├── Data/                    ← ScriptableObject 인스턴스
│   ├── Prefabs/
│   ├── Scenes/
│   ├── Scripts/
│   │   ├── Runtime/
│   │   │   ├── Core/
│   │   │   ├── Combat/
│   │   │   └── UI/
│   │   └── Editor/
│   ├── Settings/                ← URP/HDRP, Input, Volume Profile
│   └── Shaders/
├── Plugins/
└── ThirdParty/                  ← 에셋스토어 패키지 (수정 금지 원칙)
```

- Assembly Definition: `[회사].[프로젝트].[모듈]` → `Studio.Game.Combat.asmdef`, 에디터용은 `.Editor` 접미사.

---

## 4. 씬 / 하이어라키 오브젝트

- 하이어라키 오브젝트 이름은 PascalCase, 공백 대신 붙여쓰기: `Player`, `MainCamera`, `EnemySpawner`.
- 정리용 빈 오브젝트(그룹)는 `---` 대신 `[Group]` 형식 또는 `_` 접두사: `[Environment]`, `[Lighting]`, `[UI]`, `[Managers]`.
- 프리팹 인스턴스의 `(1)`, `(Clone)` 접미사는 의미 있는 이름이나 `_01` 번호로 바꾼다.
- UI 요소는 `[종류]_[이름]`: `Btn_Start`, `Txt_Title`, `Img_Background`, `Pnl_Settings`, `Sld_Volume`, `Tgl_Mute`, `Inp_Nickname`, `Scr_Inventory`.

---

## 5. 기타 문자열 식별자

| 대상 | 규칙 | 예시 |
|---|---|---|
| Tag | PascalCase | `Player`, `Enemy`, `Interactable` |
| Layer | PascalCase | `Ground`, `PlayerHitbox` |
| Sorting Layer | PascalCase | `Background`, `Foreground` |
| Animator 파라미터 | PascalCase | `Speed`, `IsGrounded`, `Attack` (Trigger) |
| Animator 상태 | PascalCase, 클립과 동일 동작명 | `Idle`, `Run` |
| Input Action / Map | PascalCase | Map `Gameplay`, Action `Jump` |
| Addressables 주소 | 경로형 소문자 또는 에셋명 그대로, 팀 내 하나로 통일 | `characters/goblin` |
| PlayerPrefs 키 | `프로젝트.카테고리.키` | `Game.Audio.MasterVolume` |
| 로컬라이제이션 키 | `SCREAMING_SNAKE` 또는 `category.key` 중 택1 | `UI_BTN_START` |

---

## 6. 적용 방법

### 새 이름을 지을 때

1. 대상 종류(코드/에셋/폴더/씬/오브젝트)를 판별한다.
2. 위 표에서 해당 규칙을 찾아 적용한다.
3. 표에 없는 종류는 같은 원칙(영어, PascalCase, `_` 구분, 종류 접두사)으로 새 접두사를 제안하고, 기존 접두사와 겹치지 않는지 확인한다.

### 기존 이름을 검토할 때

1. 위반 항목을 `현재 이름 → 수정 제안 (이유)` 형식의 목록으로 정리한다.
2. 영향 범위를 알린다:
   - **스크립트 클래스명 변경**: 파일명도 함께 바꾼다. 직렬화된 참조는 GUID 기반이라 유지되지만, `[FormerlySerializedAs("oldName")]` 없이 **필드명**을 바꾸면 인스펙터 값이 초기화된다.
   - **에셋 리네임**은 반드시 Unity 에디터 안에서 하거나 `.meta` 파일을 함께 옮긴다. 탐색기에서만 바꾸면 참조가 끊긴다. 이 프로젝트에서는 `.meta` 직접 수정 금지 지침에 따라 Unity 에디터에서 리네임한다.
   - `Resources.Load`, Addressables 주소, 문자열로 찾는 Tag/Layer/Animator 파라미터는 코드 검색으로 사용처를 함께 수정한다.
3. 대량 리네임이 필요하면 `AssetDatabase.RenameAsset`을 쓰는 에디터 스크립트를 제안한다.

### 체크리스트

- [ ] 한글·공백·특수문자 없음
- [ ] 에셋 종류 접두사 있음 (스크립트·씬 제외)
- [ ] 번호는 `_01` 형식
- [ ] 클래스명 = 파일명
- [ ] private 필드 `_camelCase`, 불리언은 `is/has/can`
- [ ] 이벤트는 `On` 접두사, 코루틴은 `Routine`, 비동기는 `Async` 접미사
- [ ] 직렬화 필드명 변경 시 `FormerlySerializedAs` 적용
- [ ] 텍스처 맵 접미사 일관됨
