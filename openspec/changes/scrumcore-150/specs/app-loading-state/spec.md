# Spec: app-loading-state

## ADDED Requirements

### Requirement: Delay-Controlled Visibility
`AppLoadingState` MUST NOT render any loading UI until `loading=true` has remained true for at least `delayMs` milliseconds.

#### Scenario: Does not render before delay
WHEN `loading=true` and elapsed time is less than `delayMs`  
THEN the component MUST render nothing (no title, no message, no icon).

#### Scenario: Renders after delay when still loading
WHEN `loading=true` and elapsed time is greater than or equal to `delayMs`  
THEN the component MUST render an inline/card loading state.

### Requirement: Hides When Loading Ends
When `loading` transitions to `false`, `AppLoadingState` MUST hide the loading UI immediately.

#### Scenario: Hides on loading false
WHEN `loading` changes from `true` to `false`  
THEN the component MUST stop rendering the loading UI.

### Requirement: Timer Cleanup
`AppLoadingState` MUST clean up timers when unmounted and when `loading` changes to prevent memory leaks and state updates after unmount.

#### Scenario: Cleans timers on unmount
WHEN the component unmounts while a delay timer is pending  
THEN the component MUST clear the timer and MUST NOT call `setState` afterwards.

### Requirement: Accessibility Status
`AppLoadingState` MUST expose loading text as a polite status update for screen readers.

#### Scenario: Provides aria-live polite status
WHEN the loading UI is rendered  
THEN it MUST include `role="status"` and `aria-live="polite"` on the status container (or equivalent).

### Requirement: Inline Card, Not Full-Screen
`AppLoadingState` MUST render as an inline/card element and MUST NOT behave as a global full-screen overlay.

#### Scenario: Does not capture global interaction
WHEN `AppLoadingState` is rendered inside a container  
THEN it MUST NOT block interaction outside its own rendered subtree.

