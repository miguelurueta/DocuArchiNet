# PROMPT ARQUITECTONICO  Ticket 02 FE
# Estrategias de carga y estado por archivo

Rol esperado:
Arquitecto de software senior frontend (React, componentes UI enterprise, accesibilidad, testing)


OBJETIVO

Implementar estrategias de carga `auto`, `manual` y `customRequest` con progreso, cancelacion y reintento, alineado al diagrama de estados de AppUpload.


CONTEXTO EXISTENTE

- arquitectura: `docs/Architecture/AppUpload/AppUpload-Architecture.md`
- diagrama de estados y secuencia incluidos en el documento


UBICACION (OBLIGATORIA)

```
src/app/Components/UI/AppUpload/
```


RESTRICCIONES (OBLIGATORIAS)

- no acoplar a endpoints de negocio
- presigned se implementa via `customRequest`
- no bloquear la UI durante carga


CONTRATO (OBLIGATORIO)

type UploadStrategy = {
  start: (files: AppUploadFile[]) => void;
  abort?: (file: AppUploadFile) => void;
  retry?: (file: AppUploadFile) => void;
};

type AppUploadStrategyProps = {
  strategy?: "auto" | "manual" | "customRequest";
  maxRetries?: number;
  onError?: (file: AppUploadFile, error: unknown) => void;
};


REGLAS DE IMPLEMENTACION (OBLIGATORIAS)

1. TRANSICIONES
   - idle -> queued -> uploading -> done/error -> removed
   - retry: error -> uploading
   - state machine estricta sin saltos

2. PROGRESO
   - `percent` 0-100 sincronizado con la UI
   - `onProgress(file, percent)` se emite con cambios reales

3. CANCELACION
   - `abort(file)` cambia estado a removed o error segun corresponda
   - `retry(file)` reintenta un archivo en error

4. ERRORES
   - errores no rompen la lista
   - `onError` notifica al contenedor
   - `onSuccess` notifica al completar


RIESGOS A EVITAR (OBLIGATORIO)

- saltar transiciones del diagrama de estados
- emitir `onChange` duplicado
- perder el orden de la lista


PRUEBAS UNITARIAS (OBLIGATORIAS)

- estrategia auto inicia carga al seleccionar
- estrategia manual solo carga con `onUpload`
- customRequest ejecuta estrategia externa
- retry cambia de error a uploading
- cancelacion actualiza estado
- `onProgress` emite valores 0-100
- `onSuccess` se emite al completar


PRUEBAS QT (CALIDAD / E2E)

- carga auto muestra progreso
- carga manual requiere click de subir
- error + retry funciona


CRITERIOS DE ACEPTACION

- estrategias funcionales y seleccionables por prop
- estados consistentes con el diagrama
- errores controlados sin romper UI
