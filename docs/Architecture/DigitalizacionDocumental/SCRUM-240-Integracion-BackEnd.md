# SCRUM-240 - Integracion BackEnd

Esta fase no integra backend documental ni upload temporal.

## Alcance FE-02

- Preparar infraestructura scanner PDF-only.
- Generar `File` local con MIME `application/pdf`.
- Dejar lista la salida para fases posteriores de upload temporal y persistencia documental.

## Contratos futuros

La fase siguiente podra usar el resultado de `generatePdf` para:

- upload temporal;
- metadata resolve;
- crear documento digitalizado;
- adjuntar digitalizacion a documento existente.

## Licencia y runtime

La licencia Dynamsoft se valida en runtime dentro del adapter. No se loguea ni se expone en errores funcionales.
