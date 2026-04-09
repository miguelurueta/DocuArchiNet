# PROMPT ARQUITECTONICO  Ticket 01 FE
# Implementar AppUpload core (wrapper + contrato controlado)

Rol esperado:
Arquitecto de software senior frontend (React, componentes UI enterprise, accesibilidad, testing)


OBJETIVO

Construir `AppUpload` como control reusable en `src/app/Components/UI/AppUpload/`, wrapper desacoplado de Ant Design `Upload`, con contrato controlado, modelo de archivo estable, eventos base y extensibilidad visual.


CONTEXTO EXISTENTE

- especificacion principal: `docs/Architecture/AppUpload/AppUpload-Architecture.md`
- estilos base: `src/app/Components/UI/AppInput`


UBICACION (OBLIGATORIA)

```
src/app/Components/UI/AppUpload/
```


RESTRICCIONES (OBLIGATORIAS)

- no consumir APIs dentro del componente
- no acoplar a modulos o pantallas especificas
- no introducir estilos globales
- no romper consistencia visual con `AppInput`
- mantener control total del estado en el contenedor


CONTRATO (OBLIGATORIO)

type AppUploadFile = {
  uid: string;
  name: string;
  size: number;
  type?: string;
  status: "queued" | "uploading" | "done" | "error" | "removed";
  percent?: number;
  url?: string;
  thumbUrl?: string;
  error?: string;
};

type AppUploadProps = {
  value?: AppUploadFile[];
  defaultValue?: AppUploadFile[];
  layout?: "grid" | "list";
  accept?: string;
  maxSize?: number;
  validateFile?: (file: File) => boolean | Promise<boolean>;
  maxCount?: number;
  disabled?: boolean;
  size?: "sm" | "md" | "lg";
  beforeUpload?: (file: File, fileList: File[]) => boolean | Promise<boolean>;
  onChange: (files: AppUploadFile[]) => void;
  onRemove?: (file: AppUploadFile) => void;
  onUpload?: () => void;
  onProgress?: (file: AppUploadFile, percent: number) => void;
  onSuccess?: (file: AppUploadFile) => void;
  onError?: (file: AppUploadFile, error: unknown) => void;
  renderItem?: (file: AppUploadFile) => React.ReactNode;
  renderActions?: (file: AppUploadFile) => React.ReactNode;
  renderUploadButton?: () => React.ReactNode;
};


REGLAS DE IMPLEMENTACION (OBLIGATORIAS)

1. CONTROLADO VS NO CONTROLADO
   - si se provee `value`, el componente es controlado; `defaultValue` solo aplica cuando no hay `value`
   - no mezclar ambos modos

2. EVENTOS
   - `onChange` se dispara ante cambios en lista o estado
   - `onRemove` elimina del listado y notifica al contenedor
   - `onUpload` dispara la carga cuando es manual

3. LIMITE DE ARCHIVOS
   - si `maxCount` se alcanza, ocultar boton de carga

4. ORDEN Y ESTADO
   - mantener orden estable
   - estados permitidos: `queued | uploading | done | error | removed`
   - state machine estricta sin saltos de estado

5. EXTENSIBILIDAD
   - exponer slots/render props para item, acciones y boton

6. VALIDACION DE ARCHIVOS
   - `accept` filtra tipos permitidos
   - `maxSize` valida tamaño
   - `validateFile` permite validacion custom (sync/async)

7. PERFORMANCE
   - usar `React.memo` en item de archivo
   - evitar re-render completo de la lista


REGLAS DE CONSISTENCIA

- alineado con `AppInput` (radius 12px, focus, hover, disabled)
- no incorporar logica de API


RIESGOS A EVITAR (OBLIGATORIO)

- mezclar controlado y no controlado
- mutar `value` directamente
- ocultar acciones cuando `disabled` es false
- saltar estados en la state machine


PRUEBAS UNITARIAS (OBLIGATORIAS)

- controlado respeta `value`
- no controlado usa `defaultValue`
- `onChange` se dispara ante cambios de estado
- `onRemove` elimina el item
- `maxCount` oculta boton de carga
- `accept` filtra tipos permitidos
- `maxSize` bloquea archivos grandes
- `validateFile` respeta validacion custom
- slots/render props se renderizan


PRUEBAS QT (CALIDAD / E2E)

- agregar archivos: lista se actualiza
- eliminar archivo: se refleja en UI
- cambiar `value` desde contenedor: UI se sincroniza


CRITERIOS DE ACEPTACION

- componente reusable en `src/app/Components/UI/AppUpload`
- contrato estable y tipado claro
- consistente con `AppInput`
- cobertura de pruebas basica
