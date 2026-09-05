# Prompt 01 — Núcleo de importación y registro de adaptadores

Actúa como implementador senior de ASP.NET WebForms, JavaScript y accesibilidad. Lee completa la exploración y el preview indicados en `README.md`, inspecciona el código vigente y crea o continúa un cambio OpenSpec antes de modificar código productivo.

Depende del contrato publicado por el Prompt backend 01 y debe consumir la versión definida en `../CONTRATO-COMPARTIDO-FRONTEND-BACKEND.md`. Si ese contrato no existe todavía, limita la entrega a UI y adaptadores falsos locales.

## Objetivo

Construir el núcleo frontend genérico de **Importar documentos desde servicio**, desacoplado de SII, con resolución del proveedor configurado y registro explícito de adaptadores.

## Rutas canónicas de implementación

```txt
js/workflow/importar-servicio-web/
├── importar-servicio-web-api.js
├── importar-servicio-web-core.js
├── importar-servicio-web-provider-registry.js
└── importar-servicio-web-ui.js

Styles/
└── importar-servicio-web-modern.css

Tests/
├── importar-servicio-web-core.test.cjs
├── importar-servicio-web-provider-registry-ui.test.cjs
└── importar-servicio-web-accessibility.test.cjs
```

- El marcado aditivo vive en `workflow/Webworkflow.aspx`; la carga de assets y gate en `workflow/Webworkflow.aspx.vb`.
- Los módulos nuevos se registran como `<Content>` en el `.vbproj`; no insertar lógica de negocio inline en `.aspx` o `.aspx.vb`.
- `importar-servicio-web-api.js` es el único cliente de operaciones backend modernas; `ui.js` no hace AJAX directo.
- No crear código nuevo en `js/Webworkflow.js`, `js/java_general/JSProgresBar.js`, `App_Code/` ni scripts globales legacy.

## Ruta documental obligatoria

```txt
docs/modulos/workflow/importar-servicio-web/SCRUMCORE-000-nucleo-registro-adaptadores/
```

Sustituir `SCRUMCORE-000` por el ticket real y crear `00-Indice.md` a `07-Metadata.md` y `Diagramas/` según el paquete canónico de `opsxj`.

## Implementa

- Un orquestador de UI con estados cerrado, resolviendo proveedor, consultando, vacío, resultados, preparando, ejecutando, reconciliando, completado y error.
- Un contrato de adaptador basado en capacidades: selección múltiple, vista previa, descarga, tipología, requisitos adicionales y acciones permitidas.
- Un registro que resuelva el adaptador por identidad canónica del proveedor; no uses una cadena de `if` dispersos.
- Apertura desde `ctw-document-action-service`, conservando `btnloadservice` como puente temporal según el gate.
- Manejo explícito de proveedor sin configuración, conocido aún no migrado y desconocido.
- Modal estable, foco inicial, restauración de foco, navegación por teclado y regiones `aria-live`.

## Restricciones

- El núcleo no puede referenciar `CIncripcionSII`, caché SII, libro, registro, matrícula, acto, noticia ni código de barras.
- No cambies endpoints mutadores ni inventes respuestas backend.
- El orquestador de UI coordina presentación y solicitudes; nunca ejecuta las fases mutadoras ni decide su orden.
- No modifiques `AlmacenaDocumentoTareaWorkflow(...)`, `ClassAlmacenamiento`, endpoints ni consumidores legacy.
- Un proveedor desconocido nunca debe dirigirse a SII.
- Mantén el recorrido anterior cuando el gate esté desactivado.

## Aceptación

- Pruebas focales demuestran resolución de adaptador, capacidades y fallos seguros.
- Con el gate apagado se conserva el comportamiento anterior.
- Con el gate encendido existe una sola entrada visible.
- El núcleo puede probarse con un adaptador falso local sin red ni secretos.

## Correcciones opsxj:prompt-review

Estas reglas fueron agregadas desde `opsxj:prompt-review` para cubrir hallazgos estructurales corregibles. Deben ajustarse al contexto real del ticket antes de enviar a implementacion.

## Rol esperado
Definir el rol tecnico esperado para ejecutar el ticket.

## Objetivo
Describir el objetivo funcional y tecnico verificable.

## Restricciones criticas
- No introducir cambios fuera del alcance declarado.
- No romper comportamiento existente ni contratos publicos.

## Criterios de aceptacion
- El comportamiento implementado cumple el flujo esperado y queda validado con evidencia.

## Contexto obligatorio
Leer `workflow/Webworkflow.aspx`, `workflow/Webworkflow.aspx.vb`, `js/workflow/workflow-transition-ui.js`, `js/workflow/workflow-transition-page-presentation.js`, `Styles/workflow-transition-modern.css`, el contrato compartido y fixtures B01. Son referencia de convención; no acoplar ImportarServicioWeb con Terminar.

## Pruebas obligatorias
Ejecutar pruebas unitarias/focales, build/tsc segun impacto y E2E con Playwright cuando el flujo lo requiera; registrar comandos y resultados.

## Documentacion tecnica
Actualizar exclusivamente el paquete de **Ruta documental obligatoria**, con arquitectura, integración, contrato/mapping, estados, pruebas, diagramas y metadata reales.

## Entregable final
Entregar codigo, pruebas, documentacion, diagramas y evidencia coherente con lo realmente implementado.

## Requisitos positivos
- Implementar el comportamiento esperado con contratos tipados y responsabilidades claras.
- Mantener la integracion sobre los puntos de extension existentes del repo.
- Dejar evidencia de pruebas y documentacion tecnica actualizada.

## Reglas de ubicacion de codigo
- Usar exclusivamente `js/workflow/importar-servicio-web/`, `Styles/importar-servicio-web-modern.css`, los puntos aditivos declarados de `workflow/Webworkflow.aspx(.vb)` y las pruebas indicadas en **Rutas canónicas de implementación**.
- No crear `src/app`, `src/modules`, otra raíz frontend, scripts globales o una implementación paralela fuera de la carpeta del feature.

Exigir `npm run build` o `tsc` segun impacto y registrar el resultado.

Exigir pruebas unitarias/focales con Vitest o Testing Library segun el alcance.

Registrar comandos ejecutados, resultados obtenidos y evidencia en `05-PruebasEvidencia.md`.

Cuando el ticket afecte un flujo completo de usuario, navegacion, integracion entre vistas, persistencia de estado u operacion transaccional, exigir E2E real con Playwright; si no aplica, documentar justificacion formal y evidencia manual.
