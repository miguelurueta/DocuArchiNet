<!-- opsxj:refinement version=1 state=approved -->

# Refinamiento - doc-91-doble-factor-contrato

## Fuente y alcance

- Ticket: `DOC-91` — DOBLE-FACTOR-CONTRATO.
- Fuente funcional: `specs/doble-factor-contrato/jira-context.md`.
- Fuente arquitectónica: `Doc/Actualizacion/Login/Exploracion/exploracion-doble-factor-autenticacion.md`.
- Perfil verificado: ASP.NET WebForms, VB.NET, .NET Framework 4.6.1 y fuentes explícitas en el `.vbproj`.

La entrega define contratos, modelos, DTO, OTP, HMAC, llaves inyectables y Session mínima. No implementa persistencia, SMTP, ASMX, UI ni integración de login.

## Contexto inspeccionado

- `Modelo/Workflow/Terminar/WorkflowModernModels.vb` y `WorkflowModernInterfaces.vb`: modelos y puertos sin WebForms.
- `DTOs/Workflow/Terminar/TransicionWorkflowDtos.vb`: DTO serializable.
- `webservice/WorkflowPreviewSessionContextGate.vb`: adaptador Session.
- `Infrastructure/Workflow/DevolverUsuarioAnterior/DevolverUsuarioAnteriorTokenCodec.vb`: precedente inspeccionado; `MachineKey` no cumple este HMAC determinista.
- `Infrastructure/Workflow/ImportarServicioWeb/Preview/ImportPreviewDescriptorService.vb`: RNG compatible.
- `GestionDocumental-Docuarchi.net.vbproj`: entradas `Compile` explícitas.

## Decisiones aprobadas

| ID | Decision verificable | Evidencia de codigo | Design | Requirement | Tasks |
| --- | --- | --- | --- | --- | --- |
| D-01 | Limitar la entrega a la fundación inactiva y rutas autorizadas. | `.vbproj` y árbol actual | D-01 | RQ-01 | Origen: D-01, RQ-01 |
| D-02 | Modelar propósito, estados y política cerrada con identidad canónica. | Exploración 2FA | D-02 | RQ-02 | Origen: D-02, RQ-02 |
| D-03 | Generar OTP uniforme con RNG criptográfico y reloj abstracto. | `ImportPreviewDescriptorService.vb` | D-03 | RQ-03 | Origen: D-03, RQ-03 |
| D-04 | Usar HMAC versionado, ligado al contexto y comparación constante. | `DevolverUsuarioAnteriorTokenCodec.vb` y exploración | D-04 | RQ-04 | Origen: D-04, RQ-04 |
| D-05 | Leer llaves externas rotables y fallar de forma cerrada. | Configuración existente y exploración | D-05 | RQ-05 | Origen: D-05, RQ-05 |
| D-06 | Aislar Session en un adaptador de contexto mínimo. | `WorkflowPreviewSessionContextGate.vb` | D-06 | RQ-06 | Origen: D-06, RQ-06 |
| D-07 | Definir DTO públicos sin identidad interna ni secretos. | `TransicionWorkflowDtos.vb` | D-07 | RQ-07 | Origen: D-07, RQ-07 |
| D-08 | Validar comportamiento, estructura, proyecto y archivos legacy. | `tests/*.test.cjs` y `.vbproj` | D-08 | RQ-08 | Origen: D-08, RQ-08 |
| D-09 | Mantener diagramas, casos e inventario verificables con Mermaid y Roslyn. | `tests/doc91-technical-documentation.test.cjs`; `tools/validation/Doc72SourceValidator/` | D-09 | RQ-09 | Origen: D-09, RQ-09 |

## Requisitos verificables

| ID | Resultado observable | Escenario o criterio de aceptacion | Riesgo/compatibilidad |
| --- | --- | --- | --- |
| RQ-01 | Solo existen archivos del inventario y producción no los invoca. | El diff no cambia login, correo, autenticación, SMTP o SQL. | El login conserva su ejecución; la reversa elimina archivos nuevos. |
| RQ-02 | Los modelos aceptan únicamente el propósito, nueve estados y límites 5/60/2, EMAIL=1 y expiración 1..10. | Valores válidos construyen el modelo y los demás se rechazan. | No activa proveedores o configuraciones desconocidas. |
| RQ-03 | El OTP tiene seis dígitos y descarta muestras fuera del rango uniforme. | Fuente determinista cubre aceptación, rechazo y reloj controlado. | No usa `Random`, módulo sesgado ni reloj estático. |
| RQ-04 | `Protect` produce el formato v1 y `Verify` exige mismo contexto, código y llave. | Alterar cualquier componente, versión, longitud o MAC falla. | Impide reutilización y comparación parcial. |
| RQ-05 | Llave activa y anterior válida se resuelven desde configuración inyectada. | Rotación pasa; ausencia, Base64 inválido, llave desconocida o menor a 32 bytes fallan. | No versiona secretos y permite rotación segura. |
| RQ-06 | El contexto hace round-trip, expira y se limpia sin campos prohibidos. | Session simulada confirma datos permitidos y ausencia de secretos. | Limita exposición y mantiene `System.Web` fuera del dominio. |
| RQ-07 | DTO solo contiene estado neutro, destino enmascarado y tiempos. | Análisis estructural confirma ausencia de identidad, SQL y secretos. | Reduce enumeración y filtración futura. |
| RQ-08 | Pruebas focales y MSBuild terminan en cero con evidencia real. | Suite CJS/arnés y build Debug pasan o se registra el bloqueo real. | Detecta incompatibilidad net461 y omisiones del proyecto. |
| RQ-09 | Diagramas requeridos y referencias de código tienen sintaxis y firmas válidas. | Mermaid procesa los cuatro archivos y Roslyn confirma declaraciones, enums, métodos y DTO del manifiesto. | Detecta documentación ausente o divergente; no sustituye pruebas de comportamiento. |

## Reglas de trazabilidad obligatorias

1. Cada decisión aparece en design, spec y tasks con `Origen: D-XX, RQ-XX`.
2. Cada tarea conserva área, archivos, complejidad y verificación reproducible.
3. Una desviación exige nueva decisión sincronizada.
4. DOC-91 no autoriza E2E autenticado, carga real ni cambios de datos.

## Resultado del refinamiento

- Estado: aprobado para implementar únicamente la fundación delimitada.
- La aprobación no acredita pruebas todavía no ejecutadas ni alcance posterior.
- Control: `npm.cmd --prefix tools/opsxj run opsxj:refine -- DOC-91 --sync`.
