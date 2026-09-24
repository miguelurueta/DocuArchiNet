# Prompts de modernización de ImportarServicioWeb para actividades ENLASE

Esta carpeta organiza la modernización de la importación de anexos SII que ocurre antes de asignar una tarea cuya actividad es de tipo histórico `ENLASE`.

La base arquitectónica común y de lectura obligatoria es `../exploracion/modernizacion-importacion-anexos-sii-enlase.md`. Sus decisiones, riesgos, flujo técnico y matriz de reutilización aplican a todos los prompts. También son normativos, cuando correspondan, los contratos y componentes modernos existentes en `../../ImportarServicioWeb/`.

La exploración no reemplaza el código vigente, el contrato real del servicio SII ni la evidencia técnica. Si durante una entrega aparece evidencia verificable que contradice la exploración, no se debe continuar mediante una decisión silenciosa: registrar el hallazgo en OPSXJ, justificar la desviación, actualizar primero la documentación afectada y conservar trazabilidad con el ticket.

## Flujo de trabajo obligatorio

- Antes de entregar un prompt al agente, el operador inicia externamente su ticket mediante `opsxj:new -- <TICKET>` o selecciona el ticket OPSXJ que deba continuar.
- Una vez entregado el prompt, el agente trabaja dentro del ticket recibido y no vuelve a ejecutar `opsxj:new`, no crea otro ticket ni reinicia la orquestación.
- Antes de investigar o implementar, leer completamente la exploración y citar en los artefactos OPSXJ las secciones que gobiernan el alcance del ticket.
- No crear ni administrar cambios OpenSpec manualmente.
- OPSXJ gobierna revisión del prompt, artefactos, implementación, validación, publicación y cierre.
- Ningún prompt autoriza pruebas E2E reales, activación de gates, uso de credenciales ni mutaciones en ambientes compartidos.

## Orden de ejecución

1. `01-capacidad-consulta-preview-anexos-sii.md`
2. `02-preparacion-persistencia-reconciliacion.md`
3. `03-interfaz-moderna-integracion-asignacion.md`
4. `04-pruebas-gate-compatibilidad-cierre.md`

## Decisiones normativas

- El proveedor canónico continúa siendo `INTEGRACIONSII`.
- Las credenciales, autenticación, token y contrato de seguridad son compartidos.
- La operación nueva se identifica mediante la capacidad `ANEXOS_RADICADO_ENLASE`.
- No crear `INTEGRACIONSII2`, `INTEGRACIONSII_ENLASE` ni una segunda fila del proveedor por defecto.
- La importación de anexos y la asignación de la tarea son operaciones separadas.
- El servidor reconstruye el contexto autoritativo; el navegador no es autoridad para tarea, ruta, trámite, recibo, código de barras o gabinete.
- La modernización es inicialmente aditiva y reversible. El flujo legacy permanece disponible bajo el mecanismo de alternancia aprobado hasta completar evidencia.
- Las cadenas `YES`, `CTRL`, `CTRLRETURN` y `dato_lista` no forman parte del contrato moderno de interfaz.

## Frontera de reutilización

Se debe reutilizar, sin copiar:

- Núcleo moderno de `ImportarServicioWeb`.
- Registro de proveedores y adaptadores.
- Transporte HTTP, autenticación y telemetría SII.
- Intenciones, idempotencia y reconciliación.
- Preview mediado y descarga segura.
- Estilos, accesibilidad y estados de interfaz existentes.
- Infraestructura de pruebas y E2E en `tools/e2e`.

La lógica particular de anexos `ENLASE` debe permanecer aislada de la capacidad de constancias e inscripciones.

## Compuertas de avance

- El Prompt 01 no puede completar integración productiva sin confirmar el contrato real del servicio SII de anexos y una identidad externa estable.
- El Prompt 02 no puede persistir sin contratos modernos publicados por el Prompt 01.
- El Prompt 03 no puede inventar endpoints, estados o efectos que el backend no haya publicado.
- El Prompt 04 no puede ejecutar E2E ni activar gates sin autorización expresa para ambiente, cuentas y datos descartables.

## Ruta documental

Cada ticket debe documentarse exclusivamente bajo:

```text
Doc/Actualizacion/workflow/ImportarServiciWebEnlace/<TICKET>-<alcance>/
```

No duplicar documentación bajo `docs/` ni dentro de `ImportarServicioWeb`.
