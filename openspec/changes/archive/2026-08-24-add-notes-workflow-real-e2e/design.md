## Context

El arnés `tools/e2e` ya implementa una sesión Gestión→Workflow, configuración Playwright, validación previa de variables, consultas MySQL de solo lectura, huellas antes/después, evidencia saneada y controles del gate. DOC-32 es la ejecución oficial de devolución de actividad; las comprobaciones de Notas cubren sus contratos y no sustituyen esa transición. Ver `proposal.md` para la motivación y `specs/e2e-notas-workflow/spec.md` para el contrato de comportamiento.

La capacidad moderna de Notas debe exponer contratos de lectura y escritura antes de que una E2E autenticada pueda pasar. Por ello, la suite valida configuración y se detiene de forma segura si aún no se han desplegado los endpoints o no hay autorización explícita; no simula esos contratos.

## Goals / Non-Goals

**Goals:**

- Añadir una familia `test:notes:*` al arnés existente con casos contractuales anónimo, lectura real, escritura autorizada y concurrencia fija de dos solicitudes.
- Usar `test:doc32:preview`, `test:doc32:execute` y `test:doc32:concurrency` como única ejecución E2E de transición Workflow.
- Reutilizar autenticación, Playwright, validación de SQL de lectura, comprobación de gate y saneamiento de evidencia ya probados por DOC-28/DOC-32.
- Separar datos de lectura de datos descartables de escritura y exigir autorización de ambiente y tarea antes de abrir una sesión mutante.
- Probar por HTTP el contrato moderno de Notas, incluido idempotencia y conflicto de versión, sin inspeccionar ni suplantar contexto de sesión desde el cliente.

**Non-Goals:**

- No implementar ni alterar el backend, la UI, el gate ni la semántica de borrado de Notas.
- No crear usuarios, tareas, datos de prueba, credenciales, archivos `.env` ni conexiones MySQL de escritura.
- No ejecutar carga, concurrencia configurable, navegador alternativo ni un segundo mecanismo de login.
- No sustituir una E2E autorizada pendiente por mocks o una evidencia inventada.

## Decisions

### Una suite y convenciones del arnés existentes

Se añadirá `tools/e2e/tests/notes-workflow.spec.cjs`, su validador de configuración en `tools/e2e/scripts/` y comandos npm `test:notes:anonymous`, `test:notes:read`, `test:notes:write` y `test:notes:concurrency`.

La prueba importará `tests/support/authenticated-workflow-session.cjs`, utilizará `@playwright/test`, `mysql2/promise` y las opciones de navegador ya utilizadas por DOC-32. Los artefactos se guardarán en `tools/e2e/artifacts/` y se sanearán antes de escribirse.

Alternativa descartada: crear un proyecto Playwright de Notas o un helper de login propio. Duplicaría selectores, controles de secretos y semántica de sesión, y contradice el runbook.

### Configuración explícita y segmentada por modo

El validador aceptará solo variables efímeras con prefijo `NOTES_E2E_` o `DOC32_E2E_`. Los comandos `test:notes:*` y `test:doc32:*` las construirán mediante un único iniciador interactivo compartido: solicita desde TTY los valores del modo, oculta contraseña y URL MySQL, y las transmite solo a los procesos hijos de validación y prueba. No crea `.env`, no usa `setx`, no registra valores y falla antes de abrir sesión si no hay TTY. Los modos no mutantes requerirán URL, módulo, cuenta, ambiente autorizado, tarea y las dos consultas MySQL de control. Escritura requerirá además una tarea descartable distinta y una confirmación explícita de autorización. Concurrencia requerirá otra tarea descartable y una segunda confirmación explícita; ejecutará exactamente dos solicitudes, nunca carga.

Las consultas de estado y auditoría se validarán como una única sentencia `SELECT` con exactamente un `?`, y la cuenta MySQL se usará exclusivamente para ejecutar dichas consultas. Ninguna variable entrega autor, grupo, permiso, actividad, versión ni identificadores equivalentes de autorización.

Alternativa descartada: usar un único conjunto de variables para lectura y escritura. Aumentaría el riesgo de ejecutar una mutación sobre una tarea no descartable. También se descarta exigir que la persona cargue manualmente las variables antes de cada ejecución: es propenso a errores y conserva secretos más tiempo del necesario en la consola.

### Contrato HTTP, no acoplamiento con WebForms

La suite invocará las operaciones modernas publicadas de listar, consultar, crear, actualizar y eliminar mediante HTTP desde el `BrowserContext` autenticado. Construirá los payloads únicamente con identificadores de intención permitidos por el contrato: `idTarea`, `idNota`, contenido de prueba no sensible, versión y `clientRequestId`.

El endpoint y los nombres finales de operación se centralizarán en la suite de acuerdo con el contrato implementado; no se llamarán rutas legacy ni se leerá `Session("ID_TAREA_SELECCIONDA")`. Toda respuesta se verificará como DTO público sin SQL, excepciones ni contenido de secretos.

Alternativa descartada: automatizar solamente UI WebForms. No cubriría de forma determinista los bordes de autorización, idempotencia y concurrencia del contrato de servidor.

### Huellas y evidencia mínima

Antes y después de cada lectura real, la suite calculará huellas SHA-256 de las filas devueltas por las consultas aprobadas de estado y auditoría. Las operaciones de escritura autorizadas comprobarán el cambio esperado de esas huellas y los códigos/resultados de DTO, sin guardar el contenido de nota ni cuerpos HTTP completos.

La evidencia solo contiene fecha UTC, modo, códigos funcionales, conteos, latencias, banderas de cambio y huellas. Un filtro rechaza claves o valores que parezcan secretos, cookies, conexiones, tokens, contenido de nota, usuarios o destinos.

Alternativa descartada: guardar solicitudes y respuestas completas para depuración. Incrementa innecesariamente la exposición de información sensible.

### Controles de cierre obligatorios

La suite comprueba antes y después que `WorkflowCentroTrabajoModernActive` permanezca `false` y que sus listas de usuario/grupo estén vacías. También ejecuta la comprobación de integridad de páginas legacy requerida por el runbook. Cualquier desviación falla la corrida sin corregir configuración ni cambios ajenos.

## Risks / Trade-offs

- [Los contratos modernos de Notas aún no están desplegados] → La suite se implementa contra el contrato acordado, pero los modos reales fallarán de forma explícita hasta que exista un ambiente con esos endpoints.
- [Falta de ambiente, secretos o tarea descartable] → El validador falla antes de iniciar sesión o enviar una mutación; el informe registra bloqueo, nunca éxito simulado.
- [La política de borrado cambia] → El caso de eliminación se ajusta a la decisión de negocio aprobada y compara las huellas adecuadas, sin asumir borrado físico.
- [Una consulta de control revela datos] → Solo se conservan huellas, no filas; el validador limita SQL a `SELECT` de un parámetro y MySQL permanece en solo lectura.
- [La comprobación de páginas legacy encuentra cambios] → Se detiene la corrida y se solicita dirección; no se ocultan ni revierten cambios del árbol de trabajo.

## Migration Plan

1. Añadir validador y comandos npm sin ejecutar ninguna E2E.
2. Añadir suite Playwright contractual de Notas y captura interactiva protegida por configuración.
3. Añadir la sección de Notas al runbook, con variables, autorizaciones, comandos, evidencia y cierre.
4. Ejecutar primero la validación local de configuración y pruebas no autenticadas.
5. Con ambiente, cuentas y tareas expresamente autorizados, ejecutar la transición E2E DOC-32 mediante preview, ejecución y concurrencia; las pruebas de Notas permanecen como cobertura contractual complementaria.
6. Si falla una corrida, conservar solo evidencia saneada, mantener gate apagado y corregir el arnés o contrato antes de repetir. El rollback de código elimina los nuevos comandos/suite; no toca datos ni configuración de ambiente.

## Open Questions

- Los nombres definitivos del ASMX y de las operaciones modernas de Notas deben coincidir con la implementación del cambio de backend antes de ejecutar los modos autenticados. Esto no altera el enfoque ni los controles de la suite.
