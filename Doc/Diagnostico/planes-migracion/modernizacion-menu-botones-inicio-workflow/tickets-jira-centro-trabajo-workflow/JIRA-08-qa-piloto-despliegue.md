# JIRA-08 — QA, piloto, despliegue y estabilización

## Prompt para Jira

**Rol:** Actúa como líder senior de QA y despliegue para aplicaciones empresariales WebForms, con foco en regresión, piloto controlado y reversión operativa.

Ejecuta el piloto controlado del Centro de Trabajo Workflow. Este ticket no debe cambiar arquitectura ni reglas de negocio: valida, evidencia, decide promoción/reversión y documenta defectos.

### Alcance

- Activar bandera solo para entorno local/pruebas y perfil autorizado.
- Ejecutar matriz: abrir tarea/documento, índice, guardar, adjuntar, firmar, devolver, pendiente, enviar, cerrar, sesión vencida y error de visor.
- Registrar evidencia por flujo, navegador, viewport y resultado.
- Recoger validación de usuarios operativos sobre localización de acciones, lectura de contexto y velocidad.
- Mantener la bandera durante una versión posterior a promoción.

### Restricciones no negociables

- No promover si falla una acción de transición, postback parcial, permiso o visor.
- No corregir en producción sin ticket de defecto y evidencia.
- La reversión es desactivar la bandera; no se modifican datos ni flujos de negocio.

### Entregables técnicos

1. `01-MatrizQAEjecutada.md` con resultado, fecha, usuario de prueba y evidencia.
2. `02-InformePiloto.md` con hallazgos, severidad y decisión.
3. `03-PlanRollbackOperativo.md`.
4. `04-ContratoComponentesReutilizables.md` tras estabilización.

### Criterios de aceptación

- Matriz crítica 100 % aprobada o excepciones aprobadas explícitamente.
- Capturas principales aprobadas por operación.
- Bandera de reversión validada en entorno de prueba.
- Documentación técnica y evidencias archivadas junto al cambio.

### Pruebas requeridas

- Tres postbacks parciales consecutivos por flujo crítico.
- Usuario con permisos completos y usuario restringido.
- Desktop, tablet y móvil.

### Reversión

Desactivar la bandera de modo moderno y verificar la recuperación del flujo legacy completo.
