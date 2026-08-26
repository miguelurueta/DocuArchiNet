# BACKEND-DEVOLUCION-USUARIO-ANTERIOR

- Ticket: DOC-36
- Cambio OpenSpec: doc-36-backend-devolucion-usuario-anterior
- Clasificacion: cross_cutting (Transversal)
## Objetivo

Entregar el backend seguro de **Devolver → Usuario anterior**. El servidor resuelve el único usuario histórico inmediato de la misma tarea; el navegador solo aporta tarea y token opaco en ejecución.

## Alcance y compatibilidad

- Se agregaron endpoints ASMX, contexto de autorización, DTOs/modelos/puertos exclusivos, repositorio MySQL de solo lectura, token, lock, adaptador de motor y auditoría.
- Se preservan sin cambios los contratos de actividad anterior, Continuar flujo, Enviar a usuario, Enviar a grupo y sus gates.
- No se modificaron páginas, controles Web Forms, scripts, configuración ni ambiente. La reversa consiste en retirar los archivos y referencias de proyecto de DOC-36; no hay migración de esquema.

La documentación detallada está en [el paquete de Usuario anterior](../../../Actualizacion/workflow/DevolverUsuarioAnterior/01-implementacion-devolver-usuario-anterior/00-indice.md).
