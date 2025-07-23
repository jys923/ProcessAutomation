# WPF MVVM: 상태 기반 UI 제어 방식 5가지 비교

MVVM에서 ViewModel의 상태에 따라 UI를 제어하는 대표적인 5가지 방식은 다음과 같다:

---

## ✅ 1. ViewModel 속성 바인딩 방식

### 개념
- 상태값에 따라 여러 UI 제어 속성(`CanStart`, `SaveButtonVisibility` 등)을 개별로 계산
- 속성이 바뀌면 `OnPropertyChanged()` 수동 호출

### 예시
```csharp
[ObservableProperty]
private InspectionState inspectionState;

public bool CanStart => inspectionState == InspectionState.Idle;

partial void OnInspectionStateChanged(InspectionState oldValue, InspectionState newValue)
{
    OnPropertyChanged(nameof(CanStart));
}
```

### 특징
- MVVM 정석
- 상태값이 바뀔 때 수동으로 여러 속성 갱신 필요

---

## ✅ 2. UpdateUIState() 패턴 (상태값 기준 일괄 계산)

### 개념
- 상태가 바뀌면 `UpdateUIState()`에서 관련된 모든 UI 바인딩 속성을 한 번에 설정

### 예시
```csharp
[ObservableProperty]
private InspectionState inspectionState;

[ObservableProperty]
private bool canStart;

[ObservableProperty]
private Visibility saveButtonVisibility;

partial void OnInspectionStateChanged(InspectionState oldValue, InspectionState newValue)
{
    UpdateUIState();
}

private void UpdateUIState()
{
    canStart = inspectionState == InspectionState.Idle;
    saveButtonVisibility = inspectionState == InspectionState.Completed
        ? Visibility.Visible : Visibility.Collapsed;
}
```

### 특징
- 속성 간 의존 관계를 묶어서 관리
- 유지보수와 디버깅 편리
- ViewModel 내부에서 명시적으로 제어

---

## ✅ 3. NotifyPropertyChangedFor 속성 자동 갱신

### 개념
- `CommunityToolkit.Mvvm`의 `[NotifyPropertyChangedFor]`를 사용하여,
  한 필드 값이 바뀔 때 자동으로 여러 종속 속성들을 `OnPropertyChanged()` 없이 갱신

### 예시
```csharp
[ObservableProperty]
[NotifyPropertyChangedFor(nameof(CanStart))]
[NotifyPropertyChangedFor(nameof(SaveButtonVisibility))]
private InspectionState inspectionState;

public bool CanStart => inspectionState == InspectionState.Idle;

public Visibility SaveButtonVisibility =>
    inspectionState == InspectionState.Completed ? Visibility.Visible : Visibility.Collapsed;
```

### 특징
- 가장 선언적이고 간단한 방식
- 자동화되어 실수 줄일 수 있음
- 단, 계산 속성들만 사용할 수 있고 상태별 분기 처리가 많으면 오히려 불편

---

## ✅ 4. VisualStateManager (WPF XAML 상태 전환)

### 개념
- XAML에서 시각 상태 그룹(`VisualStateGroup`)을 정의하고
  ViewModel 상태값에 따라 `VisualStateManager.GoToState()` 호출로 상태 전환

### 예시 (XAML)
```xml
<VisualStateGroup x:Name="InspectionStates">
    <VisualState x:Name="Idle">
        <Storyboard>
            <ObjectAnimationUsingKeyFrames Storyboard.TargetName="SaveButton"
                                           Storyboard.TargetProperty="Visibility">
                <DiscreteObjectKeyFrame KeyTime="0" Value="{x:Static Visibility.Collapsed}" />
            </ObjectAnimationUsingKeyFrames>
        </Storyboard>
    </VisualState>
    <VisualState x:Name="Completed">
        <Storyboard>
            <ObjectAnimationUsingKeyFrames Storyboard.TargetName="SaveButton"
                                           Storyboard.TargetProperty="Visibility">
                <DiscreteObjectKeyFrame KeyTime="0" Value="{x:Static Visibility.Visible}" />
            </ObjectAnimationUsingKeyFrames>
        </Storyboard>
    </VisualState>
</VisualStateGroup>
```

### 코드 비하인드
```csharp
// View 또는 Behavior에서 ViewModel의 상태 변경을 구독하여 호출
VisualStateManager.GoToState(this, vm.InspectionState.ToString(), true);
```

### 특징
- UI 애니메이션, 시각 효과 중심 제어에 최적
- View 쪽에서 상태 시각 표현 담당
- MVVM 순수성은 낮아지나 뷰 표현력은 뛰어남

---

## ✅ 5. Stateless 라이브러리 (상태 머신)

### 개념
- `Stateless`는 코드 기반의 경량 상태 머신 라이브러리.
- ViewModel 내에서 상태(State), 트리거(Trigger), 그리고 상태 전이(Transition)를 명시적으로 정의.
- 각 상태에 진입(`OnEntry`)하거나 빠져나갈 때(`OnExit`) 실행할 액션을 설정하여 UI 속성을 제어.

### 예시
```csharp
// 1. 상태와 트리거 정의
public enum InspectionState { Idle, Inspecting, Completed }
public enum Trigger { Start, Complete, Reset }

// ViewModel 내부
public class InspectionViewModel : ObservableObject
{
    private readonly StateMachine<InspectionState, Trigger> machine;

    // UI 바인딩 속성
    [ObservableProperty]
    private bool canStart;
    [ObservableProperty]
    private Visibility saveButtonVisibility;

    public InspectionViewModel()
    {
        // 2. 상태 머신 설정
        machine = new StateMachine<InspectionState, Trigger>(InspectionState.Idle);

        machine.Configure(InspectionState.Idle)
            .OnEntry(UpdateUIState)
            .Permit(Trigger.Start, InspectionState.Inspecting);

        machine.Configure(InspectionState.Inspecting)
            .OnEntry(UpdateUIState)
            .Permit(Trigger.Complete, InspectionState.Completed);

        machine.Configure(InspectionState.Completed)
            .OnEntry(UpdateUIState)
            .Permit(Trigger.Reset, InspectionState.Idle);
        
        // 초기 UI 상태 설정
        UpdateUIState();
    }

    // 3. 트리거를 통해 상태 변경 (ICommand 등에서 호출)
    public void Start() => machine.Fire(Trigger.Start);
    public void Complete() => machine.Fire(Trigger.Complete);
    public void Reset() => machine.Fire(Trigger.Reset);

    // 4. 현재 상태에 따라 UI 업데이트
    private void UpdateUIState()
    {
        CanStart = machine.CanFire(Trigger.Start);
        SaveButtonVisibility = (machine.State == InspectionState.Completed)
            ? Visibility.Visible : Visibility.Collapsed;
        // ... 기타 UI 속성 업데이트
    }
}
```

### 특징
- 복잡한 상태 전이 로직을 체계적으로 관리.
- 허용된 동작(Permit)을 명시하여 버그 발생 가능성을 줄임.
- 상태와 행위를 분리하여 ViewModel의 가독성 향상.
- 간단한 UI 제어에는 설정 코드가 길어져 과하게 느껴질 수 있음.

---

## 5가지 방식 비교

| 항목 | 속성 바인딩 방식 | UpdateUIState() | NotifyPropertyChangedFor | VisualStateManager | Stateless (상태 머신) |
|:---|:---:|:---:|:---:|:---:|:---:|
| **MVVM 순수도** | ★★★★★ | ★★★★★ | ★★★★★ | ★★★☆☆ | ★★★★★ |
| **선언적 구성** | ★★☆☆☆ | ★★★☆☆ | ★★★★☆ | ★★★★★ | ★★★★☆ |
| **코드량** | 중간 | 길어질 수 있음 | 매우 적음 | XAML + 코드 | 설정 코드 필요 |
| **유지보수** | 쉬움 | 아주 쉬움 | 쉬움 | 복잡도 ↑ | 복잡한 로직에 매우 쉬움 |
| **UI 애니메이션** | ✖ | ✖ | ✖ | O (Storyboard) | ✖ |
| **적합한 경우** | 상태별 단순 조건 | 여러 속성 일괄 계산 | 단순 계산 속성 자동화 | 복잡한 시각 상태 전이 | 복잡하고 정형화된 상태 전이 |
| **의존성 처리** | 수동 | 수동 일괄 | 자동 | 뷰에서 정의 | 상태 머신에서 중앙 관리 |

---

## ✅ 결론

- **단순 제어**: `NotifyPropertyChangedFor` + 속성 바인딩
- **상태당 여러 UI 속성 묶음 계산**: `UpdateUIState()` 패턴
- **복잡하고 정형화된 워크플로우**: `Stateless` 상태 머신
- **시각적 상태 표현 및 애니메이션**: `VisualStateManager`
- 필요에 따라 **혼합 사용도 가능**
