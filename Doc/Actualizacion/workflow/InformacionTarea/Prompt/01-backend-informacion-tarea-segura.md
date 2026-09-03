# 01 — Backend seguro de Información de la tarea

## Rol esperado

Arquitecto y desarrollador senior VB.NET con experiencia en ASP.NET Web Forms, MySQL, autorización, privacidad y esquemas dinámicos de Workflow.

## Objetivo

Crear el contrato moderno de solo lectura que entregue un resumen estable y campos adicionales expresamente publicables, desacoplado de controles Web Forms y de `SELECT *`.

## Contexto obligatorio

- Leer `00-contexto-obligatorio.md`, `README.md` y toda la Exploración.
- Requiere aprobación de las decisiones funcionales pendientes.
- Inventariar tablas `DAT_ADIC_TAR<ruta>`, configuración de campos, joins, índices, consumidores y contratos modernos reutilizables.
- Ubicar código nuevo únicamente en `Domain/Workflow`, `DTOs/Workflow`, `Services/Workflow`, `Infrastructure/Repositories/Workflow` y el webservice moderno existente, respetando la organización real encontrada.

## Requisitos positivos

- Definir DTOs para resultado, contexto, secciones y campos con clave funcional, etiqueta, valor de presentación, tipo, orden y sensibilidad.
- Crear servicio de aplicación y puertos independientes de `Page`, `Table`, `UpdatePanel`, sesión, ViewState y controles ocultos.
- Exponer una operación de lectura con `IdTarea` explícito en el servicio moderno existente.
- Resolver sesión, usuario, módulo, tarea y Ruta/Flujo en servidor; aplicar permiso específico fail-closed.
- Obtener el resumen estable desde contexto confiable y los campos adicionales desde una configuración de publicación aprobada.
- Parametrizar valores. Resolver tabla y columnas solo mediante metadatos internos permitidos; nunca concatenar valores cliente.
- Formatear fechas, estados e importes según reglas aprobadas, o devolver valores tipados para formato determinista de presentación.
- Omitir o enmascarar campos sensibles y excluir campos técnicos no autorizados.
- Devolver códigos funcionales saneados para éxito, vacío, no disponible, no autorizado y error controlado.

## Contrato esperado

Documentar request/response, DTOs, códigos, límites, lista blanca, reglas de formato, enmascaramiento y compatibilidad. El navegador no recibe nombre físico de tabla, columna, Ruta ni detalles internos.

## Secuencia funcional

1. El cliente envía `IdTarea`.
2. El servidor reconstruye contexto autenticado y obtiene la tarea autorizada.
3. Resuelve internamente Ruta/Flujo y catálogo publicable.
4. Consulta campos permitidos con parámetros.
5. Aplica sensibilidad, etiquetas, tipos, orden y formato.
6. Devuelve DTO mínimo o código funcional saneado sin mutar estado.

## Restricciones críticas

- No implementar todavía la UI productiva ni retirar `S-DTS`.
- No ejecutar `SELECT *`, aceptar columnas cliente ni devolver el DataSet completo.
- No reutilizar repositorios de transición como autorización implícita; reutilizar patrones o puertos solo cuando el contrato sea compatible.
- No registrar contenido sensible en logs o evidencia.

## Reglas de antirregresión

- Preservar el recorrido legacy mientras 02 no esté aprobado.
- No modificar operaciones de transición, notas, autorizaciones ni otras opciones de Detalle.
- No escribir auditoría funcional por consultar información.

## Criterios de aceptación

- Tarea propia y autorizada devuelve solo campos publicables.
- Tarea inexistente, ajena, inactiva o no consultable falla sin revelar existencia o datos.
- Ruta/columna manipulada no alcanza SQL.
- La respuesta no contiene nombres físicos, excepciones ni campos excluidos.
- Controles `SELECT` demuestran ausencia de mutación.

## Pruebas obligatorias

Agregar pruebas unitarias/focales VB.NET o CJS para sesión inválida, permiso, tarea inexistente/ajena/inactiva, Ruta/Flujo, catálogo vacío, campos fijos y variables, orden, tipos, valores nulos/extensos, caracteres especiales, enmascaramiento, campos técnicos, metadatos manipulados y error saneado. Ejecutar MSBuild y registrar comandos, resultados y evidencia.

La E2E es parte integral del mismo cambio funcional y de su cierre. Reutilizar exclusivamente `tools/e2e`, su sesión, configuración, validadores, evidencias y utilidades; no crear login, arnés, proyecto Playwright, configuración ni `.env` paralelos. Leer antes `AGENTS.md` y `tools/e2e/AGENT-RUNBOOK.md`; ejecutar solo con ambiente, cuentas y datos o tareas descartables expresamente autorizados. Usar secretos efímeros; no imprimir ni persistir credenciales, cookies, tokens o cadenas de conexión; verificaciones solo `SELECT` y evidencia saneada.

Cubrir autorización/control de acceso, lectura sin mutación, tarea cruzada, campos permitidos/excluidos, vacío y regresión. Escrituras autorizadas y concurrencia mutante no aplican. Respetar feature flags, gates, usuarios y grupos sin habilitarlos arbitrariamente; no cerrar sin validación autorizada. Ante falta de datos o autorización, registrar bloqueo explícito sin mocks, simulaciones ni evidencia ficticia.

## Documentación técnica

Actualizar arquitectura, contrato/catálogo, privacidad, matriz de pruebas e inventario en la documentación existente del DOC; registrar la ausencia si la ruta todavía no existe.

## Entregable final

Entregar contrato, servicio, repositorio, pruebas focales, MSBuild, E2E autorizada o bloqueo, documentación, evidencia saneada y relevo a 02.

