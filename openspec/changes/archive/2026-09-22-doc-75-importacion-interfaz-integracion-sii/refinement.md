<!-- opsxj:refinement version=1 state=approved -->

# Refinamiento - doc-75-importacion-interfaz-integracion-sii

## Fuente y alcance

- Ticket: `DOC-75` — IMPORTACION-INTERFAZ-INTEGRACION-SII
- Cambio OpenSpec: `doc-75-importacion-interfaz-integracion-sii`
- Fuente Jira: `specs/*/jira-context.md`
- Perfil tecnologico: `legacy-webforms-vb` con módulos JavaScript UMD verificables mediante `node:test`.

Este artefacto es la compuerta entre el ticket y la implementacion. No se aprueba por generacion automatica: una persona responsable debe confirmar alcance, decisiones, compatibilidad y evidencia de codigo.

## Contexto inspeccionado

- `importar-servicio-web-api.js` ya expone `PreflightImport` y `CreateImportIntent` mediante el transporte ASMX único.
- `ImportarServicioWebDtos.vb` define una colección común, requisitos, huella, planes de efectos e intención idempotente.
- `ServicioPreflightImportacion.vb` valida selección, tipología B09 y plan B11 sin consultar SII; `ServicioIntencionImportacion.vb` repite el preflight antes de crear o reutilizar.
- `intents-v1/` cubre colección individual, múltiple, repetición equivalente y conflicto.
- El modal DOC-74, su CSS y restauración de foco se amplían aditivamente; se preservan `JSExpediente.js`, `JSProgresBar.js`, almacenamiento, ejecutores y mutadores legacy.

## Decisiones aprobadas

| ID | Decision verificable | Evidencia de codigo | Design | Requirement | Tasks |
| --- | --- | --- | --- | --- | --- |
| D-01 | Individual y múltiple usan una sola colección; individual contiene exactamente un elemento. | `PreflightImportRequestDto.Items`; fixtures `single-item-intent.json` y `multiple-items-intent.json` | D-01 | RQ-01 | Origen: D-01, RQ-01 |
| D-02 | La preparación es estado frontend aislado; confirmar solo se habilita con datos completos y preflight ejecutable. | `ServicioPreflightImportacion.Preflight`; `PreflightImportResponseDto.Executable` | D-02 | RQ-02 | Origen: D-02, RQ-02 |
| D-03 | Catálogo, requisitos, huella y plan son autoridad de backend; la UI no los fabrica. | `MySqlImportDocumentTypeCatalogRepository`; `ContextFingerprint`; `EffectPlans` | D-03 | RQ-03 | Origen: D-03, RQ-03 |
| D-04 | Confirmar crea una única intención idempotente para toda la colección mediante el API existente y no la ejecuta. | `ServicioIntencionImportacion.Crear`; `importar-servicio-web-api.js` | D-04 | RQ-04 | Origen: D-04, RQ-04 |
| D-05 | El popup secundario conserva contexto/foco; cancelar y `Guardar todas` pasivo no mutan ni inicializan preparación implícita. | restauración de foco y modal en `importar-servicio-web-ui.js`/`Webworkflow.aspx` | D-05 | RQ-05 | Origen: D-05, RQ-05 |
| D-06 | B03/B09/B11 ausentes o respuestas inválidas bloquean; el plan se rotula previsto y nunca muestra `ExpedientId`. | errores cerrados de preflight; `ImportPlannedEffectDto.Status` | D-06 | RQ-06 | Origen: D-06, RQ-06 |

## Requisitos verificables

| ID | Resultado observable | Escenario o criterio de aceptacion | Riesgo/compatibilidad |
| --- | --- | --- | --- |
| RQ-01 | Un contrato común prepara uno o varios elementos. | WHEN abre individual THEN `Items.length=1`; WHEN abre múltiple THEN contiene exactamente la selección. | Evita caminos divergentes y selección implícita. |
| RQ-02 | Confirmar permanece deshabilitado hasta completar datos y recibir preflight ejecutable. | WHEN falta tipología o preflight falla THEN no se crea intención. | Fallo cerrado ante datos parciales. |
| RQ-03 | Selector, requisitos y plan provienen exclusivamente de backend. | WHEN se renderiza el resumen THEN usa catálogo, requisitos y `EffectPlans` confirmados. | No confiar en IDs o efectos fabricados. |
| RQ-04 | Una confirmación crea una intención para toda la colección y deduplica concurrencia. | WHEN hay doble clic o rerender THEN solo se invoca una creación. | Preserva idempotencia; evita intención por inscripción. |
| RQ-05 | Popup accesible conserva fila, selección, filtros, scroll y foco. | WHEN cancelar/cerrar THEN no hay mutación y el foco vuelve al origen. | No degradar DOC-74 ni legacy con gate apagado. |
| RQ-06 | Estados cerrados no filtran detalles internos ni exageran efectos. | WHEN B03/B09/B11 no están disponibles THEN bloquea y rotula efectos como previstos. | B11 sigue siendo condición productiva. |

## Reglas de trazabilidad obligatorias

1. Cada decision `D-XX` debe estar desarrollada en `design.md`, reflejada en al menos un requirement/scenario de `spec.md` y vinculada a una tarea mediante `Origen: D-XX, RQ-XX`.
2. Cada tarea con checkbox debe conservar su origen. Las tareas de validacion, rollout y documentacion tambien deben indicar la decision o requisito que verifican.
3. Las reglas de frontend, WebForms, Node u otro framework solo se agregan cuando el perfil tecnologico y el codigo afectado las justifican.
4. El estado solo puede cambiar a `approved` cuando no haya marcadores pendientes, las decisiones sean especificas y la matriz sea completa.

## Resultado del refinamiento

- Estado: aprobado tras inspeccionar contratos B03, catálogo B09, plan B11, API frontend y superficies WebForms protegidas.
- Comando: `npm.cmd --prefix tools/opsxj run opsxj:refine -- DOC-75 --sync`.
