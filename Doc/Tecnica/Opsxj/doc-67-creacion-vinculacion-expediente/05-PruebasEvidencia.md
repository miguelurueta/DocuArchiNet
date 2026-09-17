# CREACION-VINCULACION-EXPEDIENTE

- Ticket: DOC-67
- Cambio OpenSpec: doc-67-creacion-vinculacion-expediente
- Clasificacion: cross_cutting (Transversal)
## Evidencia requerida

- [x] Suite local (2026-09-17): `node --test tests/importar-servicio-web-*.test.cjs` → 336 aprobadas, 0 fallidas, incluida la validación estructural de diagramas y firmas VB.NET.
- [x] Integración saga local: intención, expediente, almacenamiento simulado, `ENLASE`, vínculo, caché, índices y reintento incremental aprobados.
- [x] Compilación: MSBuild .NET Framework 4.6.1 → código 0.
- [x] Validación: `openspec validate doc-67-creacion-vinculacion-expediente --strict` → válida.
- [x] E2E autenticada DOC-56: ejecución normal aprobada en ESAL, MERCANTIL y RUP; concurrencia, retry y recovery aprobados; 18 aserciones confirmadas.

## Consulta moderna de sujeto SII

- Pruebas focales: `21` aprobadas y `0` fallidas.
- Compilación de `GestionDocumental-Docuarchi.net.vbproj`: código de salida `0`.
- Demostrado: cliente tipado MERCANTIL/ESAL/RUP, transporte moderno sin callback TLS permisivo, timeout, límite de respuesta, UTF-8, telemetría, normalización ESAL posicional y fallback legacy configurable.
- Conservado: `ClassConsultaExpedienteSII` y todos sus consumidores legacy originales.
- Evidencia real: ejecución normal aprobada para ESAL, MERCANTIL y RUP, con auditoría autoritativa `SELECT` y gate restaurado en cada corrida.
- Evidencia adicional: concurrencia MERCANTIL, retry controlado y recovery de solo lectura aprobados. El caso real multi-expediente se difiere a un ticket específico por dependencia del contrato externo y no se declara probado en DOC-67.

La progresión de errores reales, causas, correcciones y riesgos residuales se conserva en `docs/Architecture/Workflow/ImportarServicioWeb/DOC-67-creacion-vinculacion-expedientes-sii/bitacora-diagnostico-e2e.md`.

## Puerta local de readiness previa a E2E

Ejecutar desde la raíz:

```powershell
node scripts/run-doc67-readiness.cjs
```

El comando no autentica usuarios, no activa el gate y no modifica recursos reales. Falla antes de la E2E si detecta:

- composición productiva conectada a gateways o caché legacy sustituidos;
- reinvocación moderna de las cuatro funciones legacy migradas;
- dependencia de `HttpContext.Session` dentro de los gateways físicos;
- pérdida del contexto explícito de Gestión;
- archivos críticos ausentes o duplicados en el proyecto;
- orden incorrecto de expediente, almacenamiento, relacionados y finalización;
- pérdida de cobertura local MERCANTIL, ESAL o RUP;
- cualquier regresión en la suite DOC-67 o error de compilación VB.NET.

Evidencia del 2026-09-17: `READINESS_OK`, 336 pruebas aprobadas, 0 fallidas y MSBuild con código 0. La E2E queda reservada para comprobar infraestructura e integración real, no para descubrir defectos estructurales ya verificables localmente.

## QA/E2E WebForms

La E2E reutiliza `import-sii-execution`, `import-sii-retry`, `import-sii-recovery` e `import-sii-concurrency`. Todas las consultas de control son `SELECT`; la evidencia debe quedar saneada y el gate debe restaurarse apagado y sin usuarios/grupos.
## Prueba real multi-expediente pendiente

Para cerrar la cobertura multi-expediente debe aprobarse una intención real que afecte
varios expedientes. `sampleSize > 1` no es evidencia suficiente. El perfil debe declarar
`minimumExpedientCount: 2`, y la reconciliación debe exponer como mínimo dos `InscriptionKey`
y dos `ExpedientId` distintos, con todos sus efectos confirmados. La validación focal del
contrato pasó 12/12; falta seleccionar y autorizar un recurso descartable no consumido.

La validación funcional de cómo el proveedor SII expresa la relación persona jurídica–
establecimiento–documento fue escalada para aclaración externa. Hasta recibir ese contrato,
el caso permanece `MULTI_EXPEDIENT_PROVIDER_CONTRACT_UNRESOLVED` y no debe usarse para
declarar equivalencia funcional. Esta incertidumbre no invalida las ejecuciones reales de
expediente único ya aprobadas para MERCANTIL, ESAL y RUP.

## Retry real controlado — tarea 220562

La intención `beab06593c044bcda92b1d33ff7db62c` se preparó mediante una detención
controlada después de confirmar expediente e inscripción y antes del almacenamiento. La
primera versión del orquestador continuaba indebidamente hacia documentos relacionados y
produjo `RELATED_DOCUMENTS_WAITING_FOR_STORAGE`. Se corrigió el orden para retornar el
checkpoint recuperable antes de invocar ese coordinador.

El reintento posterior confirmó exactamente un documento y rechazó la repetición con el
token anterior mediante `VERSION_CONFLICT`; las aserciones `DOC67-E2E-11`, `12` y `13`
quedaron aprobadas. Se corrigió además la expectativa de huellas: el expediente ya
confirmado debe permanecer sin cambios, mientras intención, item, transiciones, relación,
caché documental e índices sí deben cambiar.

La auditoría final exclusivamente `SELECT` confirmó: un item almacenado y completado, dos
documentos relacionados únicos, inscripción/expediente confirmados, efectos de relación,
caché e índices SQL/XML reconciliados, cero errores, transición de reconciliación y
`DOC67_EVIDENCE_VERDICT=PASSED`. El gate terminó apagado y sin alcance.

## Recovery real de solo lectura — tarea 220562

El escenario `import-sii-recovery` consultó la intención completada
`beab06593c044bcda92b1d33ff7db62c` sin ejecutar ninguna operación mutadora. El artefacto
saneado terminó con `success=true`, recuperó exactamente un documento y aprobó
`DOC67-E2E-16`, `DOC67-E2E-17` y `DOC67-E2E-18`.

Los siete controles `SELECT` de intención, item, transiciones, expediente, relación, caché
documental e índices permanecieron sin cambios. El gate quedó en `false`, con usuarios y
grupos vacíos, y las páginas legacy no presentaron diferencias.

## Evidencia de sustitución física moderna

La composición productiva usa `ModernPhysicalExpedientGateway`,
`MySqlImportExpedientCacheRepository` y `ModernDocumentExpedientPhysicalGateway`; la auditoría
estática no encontró construcciones de los gateways legacy ni llamadas desde la clausura
moderna a `AutoRegistraExpedienteTramite`, `SolicitaCacheCreacionExpedienteSII`,
`RegistraCacheCreacionExpedienteSII` o `VinculaDocumentoExpediente`.

Verificación local del 2026-09-17:

- suite DOC-67: 332/332;
- política de plataforma DOC-56/DOC-67: 12/12;
- compilación .NET Framework 4.6.1 con Visual Studio 18 MSBuild: código 0;
- advertencias históricas de conflictos de ensamblados, sin errores de compilación;
- matriz E2E real de expediente único: ESAL, MERCANTIL y RUP aprobada y reconciliada.

## Concurrencia real MERCANTIL — tarea 220579

La carrera autorizada sobre el recurso descartable produjo exactamente dos solicitudes:
una confirmó el documento y la competidora fue bloqueada con `VERSION_CONFLICT`. Los siete
controles autoritativos cambiaron según el escenario mutador y el recurso terminó consumido.
El cierre confirmó `WorkflowCentroTrabajoModernActive=false`, usuarios/grupos vacíos y ninguna
modificación de las páginas legacy. Se corrigió el proyector de evidencia para marcar
`DOC67-E2E-14/15` únicamente después de que la verificación final de integridad termina; las
pruebas del adaptador/plataforma quedaron en 13/13 y 5/5.
