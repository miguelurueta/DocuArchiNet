<!-- opsxj:refinement-traceability version=1 artifact=spec decisions=D-01,D-02,D-03,D-04,D-05,D-06,D-07,D-08,D-09,D-10,D-11 -->
## ADDED Requirements

Trazabilidad de decisiones del cambio: D-01, D-02, D-03, D-04, D-05, D-06, D-07, D-08, D-09, D-10, D-11.

### Requirement: RQ-10 Finalización independiente de challenge

El contrato `ILegacyLoginFinalizer` SHALL recibir un `LegacyLoginFinalizationContext` que represente solamente la identidad y el módulo ya verificados.

#### Scenario: Finalización sin segundo factor

- **WHEN** el login legacy tiene segundo factor apagado
- **THEN** puede finalizar sin construir un `PendingSecondFactorContext`
- **AND** conserva el mismo resultado observable previo.

#### Scenario: Continuación OTP futura

- **WHEN** una implementación posterior verifique un challenge
- **THEN** podrá traducir la identidad verificada al mismo `LegacyLoginFinalizationContext`
- **AND** reutilizará el único finalizador sin duplicar lógica legacy.
