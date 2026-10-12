# DOBLE-FACTOR-DESPLIEGUE

- Ticket: DOC-98
- Cambio OpenSpec: doc-98-doble-factor-despliegue
- Clasificacion: cross_cutting (Transversal)
## Objetivo

Preparar el despliegue reversible y la activacion transversal de la implementacion 2FA entregada por DOC-91..97. DOC-98 no modifica logica de producto ni esquema: aporta runbook, SQL guardado, observabilidad agregada y validacion automatica.

## Alcance y compatibilidad

- Componentes de producto: sin cambios.
- Configuracion de activacion: `gestor_modulos` central por empresa/modulo.
- Alcance transversal: al activar la fila del modulo, 2FA aplica a cualquier principal; no hay flag ni lista por usuario.
- Compatibilidad: con el flag `0`/`NULL` se conserva el login legacy.
- Reversa: restauracion exacta de los tres valores del snapshot; challenges pendientes expiran naturalmente.
- Documentacion operativa: `Doc/Actualizacion/Login/Implementacion/DOC-98/`.
- Estado de ambiente: `BLOQUEADO`; no se ejecuto mutacion ni activacion real durante la preparacion local.
