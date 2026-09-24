# Prompt 03 — Interfaz moderna e integración con la asignación ENLASE

## Contexto de ejecución OPSXJ

Este prompt se ejecuta dentro de un ticket OPSXJ previamente creado y revisado. No ejecutar `opsxj:new`, no crear otro ticket, no reiniciar la orquestación y no crear un cambio OpenSpec manual. Continúa exclusivamente sobre el contexto, identificador y artefactos OPSXJ recibidos.

Consume exclusivamente los contratos publicados por los Prompts 01 y 02; no inventes operaciones backend.

## Rol esperado

Actúa como implementador senior de ASP.NET WebForms, JavaScript, CSS adaptable y accesibilidad, con especial cuidado por integración aditiva sobre páginas legacy.

## Contexto obligatorio

Lee completamente antes de investigar o modificar código:

- `../exploracion/modernizacion-importacion-anexos-sii-enlase.md` como base arquitectónica común.
- Los Prompts 01 y 02 y sus DTO publicados.
- El núcleo, estilos, adaptadores y pruebas modernas de `ImportarServicioWeb`.
- `workflow/Webworkflow.aspx` y `workflow/Webworkflow.aspx.vb`.
- `js/workflow/Webworkflow.js`, solo para caracterizar el flujo vigente.
- Selección y asignación de tareas `ENLASE` en `workflow/Classselecciotarea.vb`.
- El modelo visual y documentación de DOC-72 a DOC-79.

Aplica expresamente el flujo técnico, la separación entre importación y asignación, los estados de error/cierre, la matriz E2E y la matriz de reutilización de la exploración. Registra en OPSXJ las decisiones reutilizadas y cualquier diferencia de interfaz sustentada.

Si los DTO publicados, el comportamiento real de WebForms o evidencia verificable contradicen la exploración, detén el punto afectado, registra el hallazgo y actualiza la documentación mediante OPSXJ antes de alterar el flujo. La UI no puede inventar una solución alternativa ni presentar como garantía una validación que el backend no publique.

## Objetivo

Sustituir de forma aditiva la experiencia legacy de consulta e importación de anexos SII dentro del popup `ENLASE`, reutilizando el núcleo moderno y conservando la asignación como una acción posterior, explícita y revalidada.

## Rutas canónicas propuestas

```text
js/workflow/importar-servicio-web/enlase/
├── importar-servicio-web-enlase-adapter.js
├── importar-servicio-web-enlase-context.js
├── importar-servicio-web-enlase-list.js
└── importar-servicio-web-enlase-assignment-bridge.js

Styles/
└── importar-servicio-web-modern.css

Tests/
├── importar-servicio-web-enlase-ui.test.cjs
├── importar-servicio-web-enlase-context.test.cjs
├── importar-servicio-web-enlase-assignment.test.cjs
└── importar-servicio-web-enlase-accessibility.test.cjs
```

Antes de crear archivos, confirma las convenciones vigentes del feature y evita duplicar módulos que ya resuelvan la responsabilidad.

## Implementación obligatoria

- Abrir la experiencia moderna desde el administrador documental previo a asignación.
- Resolver `INTEGRACIONSII` y la capacidad `ANEXOS_RADICADO_ENLASE` mediante el registro moderno.
- Mostrar tarea, radicado/recibo permitido y estado de conexión sin exponer datos técnicos.
- Presentar tabla con títulos claros, selección individual y selección total.
- Mantener scroll horizontal dentro de la tabla y scroll vertical dentro del cuerpo del modal.
- Ajustar alto y ancho al viewport sin desplazar la página principal.
- Soportar cero, uno y múltiples anexos.
- Deshabilitar selección para elementos no importables.
- Integrar preview seguro sin usar URL externa como autoridad.
- Preparar uno o varios anexos con tipología válida.
- Mostrar tipología predeterminada solo cuando backend la confirme como inequívoca.
- Ejecutar una sola intención para toda la selección.
- Mostrar espera global indeterminada y resultados finales por elemento.
- Impedir cierre mediante controles normales mientras la ejecución no tenga resultado.
- Después de un éxito completo, cerrar o permitir el cierre conforme a la regla UX aprobada y refrescar documentos.
- Ante fallo, resultado parcial o incierto, conservar la vista con detalles seguros y acciones permitidas.
- Actualizar la lista de documentos relacionados sin duplicar `DocumentId` y únicamente para la tarea original.
- Solicitar al backend la revalidación de documentos obligatorios.
- Habilitar `Asignar` solo cuando la validación autoritativa sea satisfactoria.
- Conservar la asignación como acción explícita del usuario.
- Gestionar foco inicial, restauración de foco, teclado y regiones `aria-live`.

## Máquina visible mínima

```text
Cerrado
  -> Validando contexto
  -> Consultando
  -> Vacío | Resultados | Error
  -> Vista previa
  -> Preparando
  -> Ejecutando
  -> Completado | Parcial | Incierto | Fallido
  -> Revalidando asignación
  -> Listo para asignar | Requisitos pendientes
```

## Restricciones críticas y antirregresión

- El frontend no consulta tablas, no persiste documentos y no decide autorización.
- No hacer AJAX directo desde componentes visuales; usar el cliente API moderno.
- No interpretar `YES`, `CTRL`, `CTRLRETURN` ni `dato_lista`.
- No crear otro modal o implementación completa si el núcleo existente puede extenderse por adaptador/capacidad.
- No modificar `ClassAlmacenamiento` ni invocar mutadores legacy directamente.
- No utilizar `JSProgresBar`, porcentajes ficticios, polling innecesario o una ejecución por fila.
- No ocultar errores mediante cierre automático.
- No insertar resultados en otra tarea si el usuario cambió de contexto.
- No usar `localStorage` como autoridad de recuperación.
- No asignar automáticamente la tarea.
- No retirar todavía el flujo legacy; debe conservarse bajo alternancia hasta el Prompt 04.

## Matriz mínima de aceptación

| Caso | Resultado esperado |
|---|---|
| Sin anexos | Estado vacío claro |
| Múltiples anexos | Selección total y deselección total correctas |
| Elemento importado | No seleccionable y con acción autorizada de visualización |
| Preview | Conserva selección, scroll y foco |
| Tipología inequívoca | Predeterminada por contrato backend |
| Tipología ambigua | Usuario debe seleccionarla |
| Ejecución pendiente | Acciones incompatibles bloqueadas |
| Éxito | Lista documental refrescada sin duplicados |
| Error | Modal permanece abierto con detalle seguro |
| Resultado incierto | Opción de consultar/reconciliar, no reintento ciego |
| Documentos obligatorios incompletos | Asignar deshabilitado |
| Documentos completos | Asignar habilitado tras revalidación |
| Cambio de tarea | No contamina la nueva tarea |
| Viewport pequeño | Modal y tabla permanecen utilizables |

## Pruebas obligatorias

- Estado y renderizado de cero, uno y múltiples anexos.
- Selección total, tipología, preview y accesibilidad.
- Una sola ejecución por intención.
- Reglas de cierre para pendiente, éxito y error.
- Actualización y deduplicación de documentos.
- Cambio de tarea y recuperación.
- Revalidación antes de asignar.
- Regresión de controles, botones y estilos de la página anfitriona.
- Pruebas responsivas en resoluciones representativas.

## Documentación técnica

Documenta exclusivamente en:

```text
Doc/Actualizacion/workflow/ImportarServiciWebEnlace/<TICKET>-interfaz-integracion-asignacion/
```

Incluye arquitectura UI, estados, accesibilidad, integración WebForms, contratos consumidos, comportamiento de cierre, pruebas, diagramas y metadata OPSXJ.

## Entregable final

Entrega interfaz, pruebas y documentación. No cierres el cambio si la UI simula garantías que el backend no ofrece o si la asignación puede ocurrir sin revalidación autoritativa.
