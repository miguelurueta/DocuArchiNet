# LECTURA-LISTADO-CONTADOR

- Ticket: DOC-41
- Cambio OpenSpec: doc-41-lectura-listado-contador
- Clasificacion: cross_cutting (Transversal)
## Objetivo

Habilitar para Workflow las operaciones modernas de listado, conteo y consulta
de Notas como lecturas aisladas. La implementación devuelve DTOs públicos,
valida cursores firmados y evita exponer detalles internos o contenido fuera de
la operación solicitada.

## Alcance y compatibilidad

- Componentes incorporados: el ASMX especializado de Notas, el servicio,
  repositorio, DTOs y códec de cursor bajo las capas Workflow existentes.
- Componentes preservados: `workflow/Webworkflow.aspx`, su code-behind y las
  páginas legacy no cambian. El gate moderno permanece apagado, sin usuarios ni
  grupos configurados.
- Reversa: retirar el endpoint especializado y sus capas asociadas revierte la
  capacidad; no se requieren migraciones ni cambios en datos de negocio.
