# Documentacion Tecnica Frontend - Radicacion Simplificada

## Objetivo

Consolidar la documentacion tecnica frontend para migrar `src/modules/radicacion` desde el comportamiento legacy de `RadicadorSimplificado` hacia el modelo moderno del repo React.

Esta carpeta no reemplaza los prompts de implementacion. Los organiza como especificacion tecnica estable para:

- entender requerimientos;
- validar contratos frontend/backend;
- implementar la navegacion contextual;
- controlar estados de tramite documental;
- evitar desajustes entre prompts FE, BE y deuda tecnica.

## Documentos

| Orden | Documento | Contenido |
|---:|---|---|
| 1 | `01-Requerimientos-Frontend-Radicacion-Simplificada.md` | Requerimientos funcionales, no funcionales, reglas y criterios de aceptacion. |
| 2 | `02-Modelo-Tecnico-Frontend.md` | Arquitectura objetivo, componentes, hooks, services, contexto documental y deuda tecnica. |
| 3 | `03-Diagrama-Clases-Frontend.md` | Diagrama Mermaid de clases/componentes frontend y relaciones. |
| 4 | `04-Diagramas-Estado-Frontend.md` | Estados del modulo, estado documental y guardas de UI. |
| 5 | `05-Diagramas-Secuencia-Frontend.md` | Secuencias principales: inicio, tomar pendiente, enviar a pendiente, limpiar formulario. |
| 6 | `06-Casos-Uso-Frontend.md` | Casos de uso frontend con actores, precondiciones, flujos y criterios. |
| 7 | `07-Trazabilidad-Prompts-Implementacion.md` | Mapa entre documentos, prompts FE/BE/TD y orden recomendado. |

## Fuentes De Contexto

| Fuente | Uso |
|---|---|
| `src/modules/radicacion` | Estado actual del modulo React. |
| `src/app/Components/UI/AppTable` | Tabla institucional requerida para pendientes. |
| `src/modules/gestionCorrespondencia` | Patron de navegacion contextual y parametros desplegados sobre componente. |
| `docs/Architecture/RadicacionSimplificadaMigration/*.md` | Prompts y analisis ya construidos. |
| `D:\imagenesda\GestorDocumental\DocuArchiCore\DocuArchiCore\Docs\DocumentProcessing\Ocr\Core\SCRUM-308-consumo-interno-texto-chunks-ocr` | Patron documental de referencia. |

## Regla Principal Del Modulo

`Documentos` solo se activa cuando existe un tramite documental activo de radicacion en `estado = 0`.

No se activa para consulta, no se activa por solo tener un radicado, no se activa por ver un pendiente en lista. Se activa despues de radicar o tomar/re-radicar un pendiente y dejar el tramite en estado activo.

