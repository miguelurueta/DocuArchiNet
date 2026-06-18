## Why

SCRUMCORE-253 agrega procesamiento automatico de imagenes documentales al modulo reutilizable de digitalizacion. El modulo ya cuenta con escaneo, ADF, duplex, blank page removal, Drag & Drop, rotacion manual, preview PDF, generacion PDF, configuracion de escaneo y toolbar corporativo.

El objetivo es mejorar la calidad del PDF final reduciendo intervencion manual mediante Deskew, Auto Crop y Auto Rotate.

## What Changes

- Auditar capacidades reales disponibles en Dynamsoft Web TWAIN 19.3.2.
- Determinar soporte para Deskew, Auto Crop, Auto Rotate, Image Enhancement, Border Detection y Document Detection.
- Agregar configuracion lateral de procesamiento automatico:
  - Deskew.
  - Auto Crop.
  - Auto Rotate.
- Mantener las opciones actuales de ADF, Duplex, Blank Page Removal, Color y Resolucion.
- Integrar el procesamiento al flujo actual de captura.
- Reflejar el procesamiento en miniaturas, preview y PDF final.
- Medir tiempos de procesamiento con logs compactos:
  - `DESKEW_TIME`.
  - `AUTOCROP_TIME`.
  - `AUTOROTATE_TIME`.
- Evitar re-renderizados innecesarios, reconstruccion completa del documento y regeneracion masiva de miniaturas.

## Jira Details

IMPLEMENTACION DE PROCESAMIENTO AUTOMATICO DE IMAGENES DOCUMENTALES

Contexto:

- Escaneo.
- ADF.
- Duplex.
- Blank Page Removal.
- Drag & Drop.
- Rotacion manual.
- Preview PDF.
- Generacion PDF.
- Configuracion de escaneo.
- Toolbar corporativo.

Objetivo:

Mejorar automaticamente la calidad de los documentos capturados mediante:

- Deskew.
- Auto Crop.
- Auto Rotate.

Fases:

1. Auditoria de capacidades Dynamsoft.
2. Configuracion de escaneo.
3. Deskew.
4. Auto Crop.
5. Auto Rotate.
6. Preview y miniaturas.
7. Rendimiento.

## Jira Metadata

- Ticket: SCRUMCORE-253.
- Tipo: Tarea.
- Prioridad: Medium.
- Estado inicial: Tareas por hacer.
- Labels: DIGITALIZACIONDOCUMENTAL, MODULOS, OPCIONES, REUSABLE.

## Capabilities

### New Capabilities

- `modulo-reusable-digitalizaciondocumental-opciones-adicionales`: procesamiento automatico de imagenes documentales para el digitalizador reutilizable.

### Modified Capabilities

- `modulo-reusable-digitalizaciondocumental-opciones-toolbar`: el panel lateral de configuracion se extiende con opciones de procesamiento automatico.

## Impact

- Frontend: `src/modules/digitalizacion`.
- Adapter: `DynamsoftTwainClient`.
- Hook: `useDigitalizacionScanner`.
- Workspace: `DigitalizacionDocumentalWorkspace`.
- Tests focales de adapter, hook, modal y AppDigitalizador.
- Documentacion tecnica de capacidades, limitaciones, evidencia visual y metricas.
