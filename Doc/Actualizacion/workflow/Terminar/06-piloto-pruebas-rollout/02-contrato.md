# Contrato de gate, telemetría y reporte — DOC-14

## Configuración

| Clave | Regla | Valor inicial |
| --- | --- | --- |
| `WorkflowCentroTrabajoModernActive` | Debe ser `true` para evaluar la experiencia moderna | `true` |
| `WorkflowCentroTrabajoModernOfficialMode` | Habilita todos los contextos Workflow válidos no excluidos; exige listas vacías | `true` |
| `WorkflowCentroTrabajoModernUsers` / `Groups` | Inclusión explícita solo en modo piloto; deben estar vacíos en modo oficial | Vacío |
| Exclusiones | Prevalecen sobre inclusiones | Vacío |
| `PilotStartUtc`, `PilotOwner`, `PilotReason` | Obligatorios si el piloto está activo | Vacío |
| `RollbackUtc`, `RollbackOwner`, `RollbackReason`, `RollbackCorrelation` | Evidencia del último rollback; no activa el piloto | Vacío |

Resultados seguros: `activo`, `inactivo`, `excluido` o `fallback-legacy`. Los códigos operativos incluyen `WORKFLOW_MODERN_PILOT_SCOPE_REQUIRED`, `WORKFLOW_MODERN_PILOT_METADATA_INVALID`, `WORKFLOW_MODERN_OFFICIAL_SCOPE_CONFLICT` y `WORKFLOW_MODERN_ROLLBACK_ACTIVE`; no contienen la configuración ni identidades del piloto.

## Telemetría

| Campo | Uso | Permitido |
| --- | --- | --- |
| `Referencia` | Correlación técnica | Sí |
| `IdUsuarioWorkflow` | Identidad autorizada numérica | Sí |
| `IdTarea`, ruta, flujo, conector, destino | Agregación técnica | Sí |
| Canal, duración, resultado, código | Métricas y diagnóstico seguro | Sí |
| Login, SQL, credenciales, Session, token, documento, payload | No se registra | No |

Si el repositorio de auditoría falla o lanza excepción, `ServicioTransicionTarea` conserva el resultado funcional y añade una advertencia segura. No reintenta ni transforma el resultado en éxito.

## Reporte de piloto

El reporte solo lee un JSON ya sanitizado:

```powershell
powershell.exe -NoProfile -File tools/validation/Get-Doc14PilotReport.ps1 `
  -InputPath tools/validation/examples/doc14-pilot-events.example.json
```

La salida agrupa `MODERNO` y `LEGACY` y produce volumen, éxitos, bloqueos, errores, duración media/p95, abandonos, divergencias y estado de promoción. Un evento crítico deja el resultado en `BLOQUEADO`; los demás casos requieren aprobación explícita.
