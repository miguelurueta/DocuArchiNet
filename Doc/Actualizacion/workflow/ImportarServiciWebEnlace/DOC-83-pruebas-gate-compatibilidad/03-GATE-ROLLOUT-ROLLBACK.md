# Gate, rollout y rollback

## Contrato del gate

`ImportarServicioWebFeatureGate.EstaHabilitado` retorna `true` únicamente cuando el `ContextoModuloWorkflow` existe, es válido y `WorkflowCentroTrabajoModernActive` vale `true`. Aunque `Web.config` conserva las claves históricas de usuarios y grupos, la implementación actual es global para toda sesión Workflow válida. DOC-83 valida esta realidad y no introduce audiencias ficticias.

Estado oficial de despliegue `Release`:

```text
WorkflowCentroTrabajoModernActive=true
WorkflowCentroTrabajoModernUsers=
WorkflowCentroTrabajoModernGroups=
```

No existe segmentación por usuario o grupo: `ImportarServicioWebFeatureGate` no consulta esas claves y habilita cualquier `ContextoModuloWorkflow` válido. Las claves se conservan vacías por compatibilidad de configuración.

La línea base local y el estado obligatorio antes y después de cualquier E2E continúan siendo:

```text
WorkflowCentroTrabajoModernActive=false
WorkflowCentroTrabajoModernUsers=
WorkflowCentroTrabajoModernGroups=
```

## Rollout controlado

1. Confirmar autorización vigente del ambiente, cuenta y etapa.
2. Para mutación, confirmar además ejecución y recurso descartable.
3. Capturar línea base de `workflow/Webworkflow.aspx` y `workflow/Webworkflow.aspx.vb`.
4. Activar temporalmente el gate mediante el runner oficial.
5. Ejecutar únicamente un escenario registrado y controles registrados.
6. Restaurar configuración aun si falla una etapa.
7. Ejecutar `assertPlatformIntegrity`: gate seguro, alcance vacío e igualdad legacy.
8. Emitir solo evidencia saneada.

El despliegue Release aplica la transformación después de estas validaciones y deja el gate activo globalmente; una corrida E2E local no representa ni revierte ese valor desplegado.

## Rollback

El rollback operativo de producción es transformar o configurar `WorkflowCentroTrabajoModernActive=false`, conservando usuarios/grupos vacíos. La ruta legacy permanece versionada; no se elimina ni se ejecuta simultáneamente con la moderna. `E2E_PLATFORM_GATE_INTEGRITY_FAILED`, `E2E_PLATFORM_LEGACY_INTEGRITY_FAILED` y `E2E_PLATFORM_EVIDENCE_INVALID` son fallos de cierre seguro, no resultados que puedan ignorarse.

Un recurso mutador queda consumido conforme a su ciclo de vida aunque la emisión posterior de evidencia falle. Nunca se debe repetir automáticamente una importación incierta ni reutilizar el mismo recurso para validar asignación.
