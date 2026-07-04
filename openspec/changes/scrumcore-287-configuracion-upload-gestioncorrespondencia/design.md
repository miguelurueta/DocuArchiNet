## Context

SCRUMCORE-287 complementa la integracion documental ya cerrada:

- SCRUMCORE-277: `AppUploadDocumental` integrado como carga documental/anexo.
- SCRUMCORE-284: tipologias workflow por archivo usando `IdTareaWf` + `IdRutaWf`.
- SCRUMCORE-287: configuracion backend de extensiones/tamano para adjuntos `CORRESPO`.

El punto real de integracion actual es:

```txt
src/modules/gestionCorrespondencia/services/gestionRespuestaUploadDocumental.service.ts
```

Ahi existe `loadGestionRespuestaUploadConfig()` con valores hardcodeados. `GestionRespuestaUploadDocumental` pasa ese loader a `AppUploadDocumental`, y `AppUploadDocumental` ya propaga `config.accept` y `config.maxSizeBytes` hacia `AppUploadBatchView`.

## Backend Contract

Endpoint:

```txt
GET /api/gestor-documental/configuracion-upload
Query:
  nameProceso=CORRESPO
```

Respuesta esperada:

```ts
type ConfiguracionUploadCorrespondenciaResponse = {
  success: boolean;
  message: string;
  data: ConfiguracionUploadCorrespondenciaBackendItem[];
  meta?: unknown;
  errors?: unknown[];
};
```

Items aceptados en PascalCase y camelCase:

```ts
type ConfiguracionUploadCorrespondenciaBackendItem = {
  IdConfigUploadGestion?: number;
  ExtensionUpload?: string;
  LengUpload?: number;
  NameProceso?: string;
  EstadoProceso?: number;

  idConfigUploadGestion?: number;
  extensionUpload?: string;
  lengUpload?: number;
  nameProceso?: string;
  estadoProceso?: number;
};
```

## Decisions

### 1. Servicio de modulo, no clienteApi en React

Crear:

```txt
src/modules/gestionCorrespondencia/services/configuracionUploadCorrespondencia.service.ts
```

Exportar:

```ts
getConfiguracionUploadCorrespondencia(
  options?: { signal?: AbortSignal },
): Promise<ConfiguracionUploadCorrespondencia>
```

El servicio es la unica capa que usa `clienteApi`.

### 2. Hook reusable de modulo

Crear:

```txt
src/modules/gestionCorrespondencia/hooks/useConfiguracionUploadCorrespondencia.ts
```

Exponer:

```ts
{
  config?: ConfiguracionUploadCorrespondencia;
  loading: boolean;
  error?: string;
  empty: boolean;
  reload: () => Promise<void>;
}
```

Aunque `GestionRespuestaUploadDocumental` consume un loader async y no necesariamente necesita renderizar el hook directamente, el hook queda disponible para pantallas simples con `AppUpload` o para mostrar estados compactos si se integra en UI.

### 3. Reemplazar fuente hardcodeada

`loadGestionRespuestaUploadConfig()` debe delegar en `getConfiguracionUploadCorrespondencia()`.

Valores funcionales que se conservan desde el loader actual:

```ts
multiple: true
requiereTipologia: true
requiereFechaCarga: false
fechaCargaObligatoria: false
validationMode: "queue-with-error"
```

Valores que vienen de backend:

```ts
accept
allowedExtensions
maxSizeBytes
```

### 4. Seleccion de fila

Reglas:

- Preferir primera fila con `EstadoProceso === 1`.
- Si hay filas pero ninguna activa, usar la primera como fallback funcional controlado.
- Si `data=[]`, fallar con error funcional.
- Si `ExtensionUpload` queda vacio tras normalizar, fallar.
- Si `LengUpload <= 0`, fallar.

### 5. Normalizacion de extensiones

Crear funcion pura testeable:

```ts
normalizeUploadExtensions(raw: string): string[]
```

Reglas:

- separar por coma;
- trim;
- lowercase;
- agregar punto si falta;
- descartar vacios;
- remover duplicados;
- conservar orden.

Ejemplo:

```txt
".PDF, DOC, .pdf, XLSX"
-> [".pdf", ".doc", ".xlsx"]
```

`accept = allowedExtensions.join(",")`.

### 6. Estados UI

Si `AppUploadDocumental` recibe error del loader:

- no debe habilitar seleccion;
- debe mostrar el error funcional existente del componente;
- debe permitir reintento mediante el flujo de recarga que ya tenga `AppUploadDocumental` o mediante el hook si se integra estado externo.

Si se usa el hook en `GestionRespuestaMainTabContent` o pantallas futuras:

- loading: deshabilita upload y muestra "Cargando configuracion de adjuntos...";
- error: deshabilita upload y ofrece retry;
- empty: deshabilita upload con "No hay configuracion de adjuntos para CORRESPO.".

## Risks / Trade-offs

- El endpoint puede no estar desplegado localmente; el componente debe fallar cerrado y no permitir seleccionar archivos sin configuracion.
- Si backend retorna extensiones sin punto o en mayuscula, el normalizador debe soportarlo.
- Si `LengUpload` es muy alto, el frontend permitira archivos grandes, pero el upload por chunks ya existe en `AppUploadDocumental`.
- `npm audit` y cambios de dependencias quedan fuera de alcance.

## Migration Plan

1. Crear tipos de configuracion upload.
2. Crear servicio con normalizador puro.
3. Crear hook abortable/retry/anti-stale.
4. Reemplazar `loadGestionRespuestaUploadConfig()` para delegar al servicio nuevo.
5. Mantener intacta la logica de tipologias.
6. Agregar pruebas focales.
7. Crear documentacion enterprise SCRUMCORE-287.
8. Validar OpenSpec y tests.

## Open Questions

- Ninguna bloqueante. El contrato de backend y `nameProceso=CORRESPO` estan definidos en el ticket.
