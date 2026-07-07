# Prompts Ejecutados - Radicacion Simplificada

## Objetivo

Esta carpeta documenta, un archivo por prompt ejecutado, que hace cada fase, que casos de uso cubre, que arquitectura deja instalada y que revisar si aparece una falla.

Estos documentos son bitacora tecnica. No reemplazan los prompts originales ni la documentacion de frontend; sirven como mapa rapido para diagnostico y continuidad.

## Prompts Documentados

| Prompt | Archivo |
|---|---|
| TD-FE-01 - Unificar fuente de plantilla | `TD-FE-01-Unificar-Fuente-Plantilla-Radicacion.md` |
| TD-FE-02 - Contexto documental unico y guards | `TD-FE-02-Contexto-Documental-Unico-Guards.md` |
| FE-06 - Inicio con estado activo | `FE-06-Inicio-Modulo-Estado-Activo-Contexto-Documental.md` |
| TD-FE-04 - Rutas, tabs y limpieza UI prototipo | `TD-FE-04-Rutas-Tabs-Limpieza-UI-Prototipo.md` |
| FE-05 - Modal de pendientes con AppTable | `FE-05-Modal-Pendientes-AppTable-Asignacion-Radicado.md` |
| FE-07 - Enviar tramite activo a pendiente | `FE-07-Enviar-Tramite-Activo-A-Pendiente.md` |

## Regla Global

El `RadicacionDocumentalContext` es la unica fuente frontend para decidir si el panel `Documentos` esta activo.
