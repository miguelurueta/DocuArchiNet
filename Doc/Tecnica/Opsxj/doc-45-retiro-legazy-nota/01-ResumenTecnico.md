# RETIRO-LEGAZY-NOTA

- Ticket: DOC-45
- Cambio OpenSpec: doc-45-retiro-legazy-nota
- Clasificacion: cross_cutting

## Objetivo

Retirar la rutina duplicada y el consumidor legacy exclusivo de Notas en `Webworkflow`, dejando como única superficie un acceso moderno con contador y diálogo superpuesto. Se conservan los contratos legacy con consumidores externos.

## Alcance y compatibilidad

Incluye propiedad calculada en servidor, rechazo `NotOwner`, visor de notas extensas, creación descubrible en estado vacío, confirmador moderno, tamaño estable con scroll, corrección de contraste y compatibilidad con `UpdatePanel`. No incluye DDL ni migración de datos.
