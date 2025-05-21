# WPF MVVM: 상태 기반 UI 제어 방식 4가지 비교

MVVM에서 ViewModel의 상태에 따라 UI를 제어하는 대표적인 4가지 방식은 다음과 같다:

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
VisualStateManager.GoToState(this, vm.InspectionState.ToString(), true);
```

### 특징
- UI 애니메이션, 시각 효과 중심 제어에 최적
- View 쪽에서 상태 시각 표현 담당
- MVVM 순수성은 낮아지나 뷰 표현력은 뛰어남

---

## 📊 4가지 방식 비교

| 항목 | 속성 바인딩 방식 | UpdateUIState() | NotifyPropertyChangedFor | VisualStateManager |
|------|-------------------|------------------|---------------------------|---------------------|
| MVVM 순수도 | ★★★★★ | ★★★★★ | ★★★★★ | ★★★☆☆ |
| 선언적 구성 | ★★☆☆☆ | ★★★☆☆ | ★★★★☆ | ★★★★★ |
| 코드량 | 중간 | 길어질 수 있음 | 매우 적음 | XAML + 코드 필요 |
| 유지보수 | 쉬움 | 아주 쉬움 | 쉬움 | 복잡도 ↑ |
| UI 애니메이션 | ✖ | ✖ | ✖ | O (Storyboard 기반) |
| 적합한 경우 | 상태별 단순 조건 | 여러 속성 일괄 계산 | 단순 계산 속성 자동화 | 복잡한 시각 상태 전이 |
| 의존성 처리 | 수동 | 수동 일괄 | 자동 | 뷰에서 정의 |

---

## ✅ 결론

- **단순 제어**: `NotifyPropertyChangedFor` + 속성 바인딩
- **상태당 여러 UI 속성 묶음 계산**: `UpdateUIState()` 패턴
- **시각적 상태 표현 및 애니메이션**: `VisualStateManager`
- 필요에 따라 **혼합 사용도 가능**
