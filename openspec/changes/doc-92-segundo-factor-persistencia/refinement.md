<!-- opsxj:refinement version=1 state=approved -->

# Refinamiento — DOC-92 Persistencia transaccional del segundo factor

## Fuente y alcance

- Ticket: `DOC-92` — SEGUNDO-FACTOR-PERSISTENCIA.
- Cambio: `doc-92-segundo-factor-persistencia`.
- Fuente funcional: `specs/segundo-factor-persistencia/jira-context.md`.
- Perfil: VB.NET WebForms, MySQL y ADO.NET compartido.
- Alcance: modelo/contrato, repositorio, scripts SQL, registro de proyecto, pruebas y documentación.
- Exclusiones: ASMX, UI, Controller/Service, SMTP, activación del login, contexto HTTP y ejecución de DDL real sin autorización vigente.

## Contexto inspeccionado

- Esquema y comportamiento objetivo: `Doc/Actualizacion/Login/Exploracion/exploracion-doble-factor-autenticacion.md`.
- Fundación DOC-91: `Modelo/Login/SegundoFactor/SegundoFactorModels.vb` y `SegundoFactorInterfaces.vb`.
- Datos compartidos: `Infrastructure/Shared/Data/ModuleDataContracts.vb`, `AdoNetDataInfrastructure.vb` y `WorkflowModuleConnectionFactory.vb`.
- Patrones transaccionales: `Infrastructure/Repositories/Workflow/MySqlNotasWorkflowRepository.vb` y `Infrastructure/Repositories/Workflow/ImportarServicioWeb/MySqlImportIntentRepository.vb`.
- Proyecto: `GestionDocumental-Docuarchi.net.vbproj`.
- Esquema físico versionado: tabla `ra_auth_second_factor_challenge`, índices `uq_challengeid` e `IX_ra_auth_sfc_authuserid`, motor InnoDB. No se reutilizó autorización previa ni se abrió una conexión a base de datos.

## Decisiones aprobadas

| ID | Decisión verificable | Evidencia de código | Design | Requirement | Tasks |
| --- | --- | --- | --- | --- | --- |
| D-01 | Limitar DOC-92 a persistencia inactiva, sin endpoint, UI, SMTP ni contexto HTTP. | Jira context y estructura `Modelo/`/`Infrastructure/` | D-01 | RQ-01 | 1.1, 3.1, 6.1 |
| D-02 | Agregar nueve columnas v1 nullable para legacy; filas nuevas completas, `SchemaVersion=1` y `AuthPayloadJson=NULL`. | Exploración, tabla física registrada | D-02 | RQ-02 | 2.2, 3.2, 5.1 |
| D-03 | Preservar índices existentes y agregar tres compuestos; exigir InnoDB. | Exploración y patrones transaccionales | D-03 | RQ-03 | 2.1, 2.3, 5.3 |
| D-04 | Inyectar factoría de conexión central, executor y transacciones; parametrizar valores. | `ModuleDataContracts.vb`, `WorkflowModuleConnectionFactory.vb` | D-04 | RQ-04 | 3.1, 5.2 |
| D-05 | Conservar firmas DOC-91 y agregar lectura/delivery/reenvío/expiración; derivar `KeyId` de `v1:keyId:mac`. | `SegundoFactorInterfaces.vb`, `SegundoFactorModels.vb` | D-05 | RQ-05 | 1.1, 1.2, 3.2 |
| D-06 | Proteger cada transición con transacción, `FOR UPDATE` y estado esperado; un ganador de finalización. | Repositorios MySQL inspeccionados | D-06 | RQ-06 | 3.3, 5.1, 5.3 |
| D-07 | Aplicar grafo de estados, consumo solo al completar y reemplazo atómico con cooldown/límite. | Exploración cerrada | D-07 | RQ-07 | 3.4, 3.5, 5.1 |
| D-08 | Entregar ciclo SQL idempotente, rollback y limpieza manual de terminales mayores a 30 días. | Convención Jira y exploración | D-08 | RQ-08 | 2.1-2.5, 6.1 |
| D-09 | Ejecutar dobles/compilación/regresión; MySQL real solo con autorización nueva y evidencia saneada. | Reglas del repositorio y Jira | D-09 | RQ-09 | 5.1-5.4, 6.2 |

## Requisitos verificables

| ID | Resultado observable | Escenario o criterio de aceptación | Riesgo/compatibilidad |
| --- | --- | --- | --- |
| RQ-01 | Persistencia compilable pero no activada. | No aparecen endpoints ni referencias HTTP; login actual no cambia. | Evita regresión transversal. |
| RQ-02 | Esquema v1 aditivo y fila nueva completa. | Apply reejecutable; legacy no elegible; payload legacy no se reutiliza. | Nullable solo por compatibilidad histórica. |
| RQ-03 | Índices requeridos y motor apto. | Preflight falla fuera de InnoDB; postflight verifica orden. | Bloqueo de fila depende del motor. |
| RQ-04 | Repositorio usa abstracciones compartidas y parámetros. | Inspección estructural sin `conect`, Session, HttpContext o concatenación. | Evita acoplamiento y SQL injection. |
| RQ-05 | Contrato DOC-91 permanece compatible. | Compila consumidor previo y nueva lectura entrega challenge + HMAC protegido. | No exponer llaves ni secretos. |
| RQ-06 | Transiciones atómicas. | Quinto fallo bloquea; concurrencia entrega exactamente un ganador. | Requiere integración real para evidencia del motor. |
| RQ-07 | Estados, consumo, expiración y reenvío correctos. | Solo COMPLETED consume; reemplazo revoca y crea en un commit. | Nunca dos challenges elegibles. |
| RQ-08 | Despliegue y reversión operables. | Pre/apply/post/rollback/cleanup documentados, sin scheduler. | Rollback puede perder atributos v1. |
| RQ-09 | Evidencia proporcional y honesta. | Unitarias/estructurales/build siempre; MySQL solo autorizado. | Sin autorización, integración queda registrada como no ejecutada. |

## Reglas de trazabilidad obligatorias

1. Cada decisión `D-XX` aparece en `design.md`, en un requisito `RQ-XX` de `spec.md` y en al menos una tarea con `Origen`.
2. Cada tarea declara resultado, área exacta y verificación reproducible.
3. Ninguna prueba ni script real usa credenciales, cookies, conexiones o datos productivos versionados.
4. Una diferencia del esquema real exige detenerse; solo se permite `information_schema` con autorización nueva y explícita.

## Resultado del refinamiento

- Estado: aprobado para implementación.
- Base: código y documentación versionados; sin acceso a base de datos en el refinamiento.
- Validación: `npm.cmd --prefix tools/opsxj run opsxj:refine -- DOC-92 --sync` y `openspec validate doc-92-segundo-factor-persistencia --strict`.
