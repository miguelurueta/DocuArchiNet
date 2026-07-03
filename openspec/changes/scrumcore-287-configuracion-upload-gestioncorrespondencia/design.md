## Context

SCRUMCORE-287: CONFIGURACION-UPLOAD-GESTIONCORRESPONDENCIA

## Jira Details

> PROMPT ARQUITECTONICO - Configuracion upload para adjuntos en Gestion Correspondencia
> Rol esperado
>   Arquitecto frontend senior.
>   React 19, TypeScript estricto, Clean Architecture, componentes enterprise, integracion API documental, configuracion de carga de archivos, accesibilidad, testing y  migracion quirurgica sin romper flujos existentes.
> Objetivo
>   Adaptar el flujo de adjuntos de src/modules/gestionCorrespondencia para que cargue desde backend la configuracion de tipos de archivo permitidos y tamano maximo, y  aplique esa configuracion al componente existente de carga.
>   Este ticket NO incluye tipologias documentales.
>   La solucion debe:
> consumir GET /api/gestor-documental/configuracion-upload?nameProceso=CORRESPO;
> 
> obtener extensiones permitidas desde ExtensionUpload;
> 
> obtener tamano maximo desde LengUpload;
> 
> aplicar esa configuracion al componente de carga existente;
> 
> reutilizar AppUploadBatchView si el flujo ya esta migrado a cola por lote, o AppUpload si el flujo actual sigue siendo simple;
> 
> no hardcodear extensiones ni tamano en UI;
> 
> manejar loading, error, empty y retry;
> 
> no modificar backend.
> 
> Fuera De Alcance
>   No implementar:
> tipologias documentales;
> 
> lista de chequeo;
> 
> metadata por archivo;
> 
> renderMetadata;
> 
> cambios en API de tipologias;
> 
> almacenamiento documental;
> 
> upload por chunks;
> 
> cambios backend.
> 
> Contrato Backend
>   Endpoint:
> GET /api/gestor-documental/configuracion-upload?nameProceso=CORRESPO
> 
> Tabla conocida:
> 
> ID_CONFIG_UPLOAD_GESTION | EXTENSION_UPLOAD                | LENG_UPLOAD | NAME_PROCESO | ESTADO_PROCESO
> 3                        | .PDF,.DOC,.DOCX,.ZIP,.XLS,.XLSX | 600000000   | CORRESPO     | 1
> 
> Respuesta esperada:
> 
> {
>   success: boolean;
>   message: string;
>   data: Array<{
>     IdConfigUploadGestion?: number;
>     ExtensionUpload?: string;
>     LengUpload?: number;
>     NameProceso?: string;
>     EstadoProceso?: number;
>   }>;
>   meta?: unknown;
>   errors?: unknown[];
> }
> 
> Por compatibilidad, normalizar tambien camelCase:
> 
> {
>   idConfigUploadGestion?: number;
>   extensionUpload?: string;
>   lengUpload?: number;
>   nameProceso?: string;
>   estadoProceso?: number;
> }
> 
> ## Mapeo Funcional
> 
> Usar la fila activa:
> 
> EstadoProceso === 1
> 
> Si hay varias filas activas, usar la primera.
> Si no hay fila activa pero hay filas, usar la primera y registrar estado funcional controlado si aplica.
> Si no hay filas, bloquear seleccion de archivos.
> 
> Mapeo:
> 
> ExtensionUpload -> accept
> LengUpload       -> maxSize
> 
> Ejemplo:
> 
> .PDF,.DOC,.DOCX,.ZIP,.XLS,.XLSX
> 
> debe convertirse en:
> 
> .pdf,.doc,.docx,.zip,.xls,.xlsx
> 
> Resultado esperado:
> 
> {
>   nameProceso: "CORRESPO",
>   accept: ".pdf,.doc,.docx,.zip,.xls,.xlsx",
>   allowedExtensions: [".pdf", ".doc", ".docx", ".zip", ".xls", ".xlsx"],
>   maxSizeBytes: 600000000
> }
> 
> ## Componentes Existentes
> 
> Reutilizar el componente de carga existente.
> 
> Si la pantalla usa AppUpload:
> 
> <AppUpload
>   accept={config.accept}
>   maxSize={config.maxSizeBytes}
> />
> 
> Si la pantalla usa o migra a AppUploadBatchView:
> 
> <AppUploadBatchView
>   accept={config.accept}
>   maxSize={config.maxSizeBytes}
> />
> 
> No modificar estos componentes si no es necesario. Ambos ya soportan accept y maxSize.
> 
> ## Ubicacion Esperada
> 
> Servicio:
> 
> src/modules/gestionCorrespondencia/services/configuracionUploadCorrespondencia.service.ts
> 
> Hook:
> 
> src/modules/gestionCorrespondencia/hooks/useConfiguracionUploadCorrespondencia.ts
> 
> Tipos:
> 
> src/modules/gestionCorrespondencia/types/configuracionUploadCorrespondencia.types.ts
> 
> Integracion UI:
> 
> src/modules/gestionCorrespondencia/components/gestionRespuestaMainTab/GestionRespuestaMainTabContent.tsx
> 
> Tests:
> 
> src/modules/gestionCorrespondencia/tests/configuracionUploadCorrespondencia.service.test.ts
> src/modules/gestionCorrespondencia/tests/useConfiguracionUploadCorrespondencia.test.tsx
> src/modules/gestionCorrespondencia/tests/GestionRespuestaMainTabContent.test.tsx
> 
> ## Tipos TypeScript Obligatorios
> 
> export type ConfiguracionUploadCorrespondenciaBackendItem = {
>   IdConfigUploadGestion?: number;
>   ExtensionUpload?: string;
>   LengUpload?: number;
>   NameProceso?: string;
>   EstadoProceso?: number;
> 
>   idConfigUploadGestion?: number;
>   extensionUpload?: string;
>   lengUpload?: number;
>   nameProceso?: string;
>   estadoProceso?: number;
> };
> 
> export type ConfiguracionUploadCorrespondenciaResponse = {
>   success: boolean;
>   message: string;
>   data: ConfiguracionUploadCorrespondenciaBackendItem[];
>   meta?: unknown;
>   errors?: unknown[];
> };
> 
> export type ConfiguracionUploadCorrespondencia = {
>   nameProceso: "CORRESPO";
>   accept: string;
>   allowedExtensions: string[];
>   maxSizeBytes: number;
> };
> 
> No usar any.
> 
> ## Servicio Obligatorio
> 
> Crear:
> 
> getConfiguracionUploadCorrespondencia(
>   options?: { signal?: AbortSignal }
> ): Promise<ConfiguracionUploadCorrespondencia>
> 
> Debe:
> 
> - usar clienteApi;
> - llamar /api/gestor-documental/configuracion-upload;
> - enviar nameProceso=CORRESPO;
> - validar success;
> - normalizar PascalCase y camelCase;
> - seleccionar fila activa;
> - normalizar extensiones;
> - validar que maxSizeBytes > 0;
> - retornar error funcional si no hay configuracion usable;
> - soportar AbortSignal.
> 
> ## Normalizador De Extensiones
> 
> Crear funcion pura y testeable:
> 
> normalizeUploadExtensions(raw: string): string[]
> 
> Reglas:
> 
> - separar por coma;
> - aplicar trim;
> - convertir a lowercase;
> - agregar . si falta;
> - descartar vacios;
> - remover duplicados;
> - conservar orden original.
> 
> Ejemplo:
> 
> normalizeUploadExtensions(".PDF,.DOC,.DOCX,.ZIP,.XLS,.XLSX")
> 
> retorna:
> 
> [".pdf", ".doc", ".docx", ".zip", ".xls", ".xlsx"]
> 
> Construir accept con:
> 
> allowedExtensions.join(",")
> 
> ## Hook Obligatorio
> 
> Crear:
> 
> useConfiguracionUploadCorrespondencia({ enabled = true })
> 
> Debe exponer:
> 
> {
>   config?: ConfiguracionUploadCorrespondencia;
>   loading: boolean;
>   error?: string;
>   reload: () => Promise<void>;
> }
> 
> Reglas:
> 
> - cargar al montar;
> - no cargar si enabled=false;
> - usar AbortController;
> - ignorar respuestas stale;
> - permitir retry;
> - no duplicar requests por render.
> 
> ## Integracion UI
> 
> En GestionRespuestaMainTabContent:
> 
> 1. Invocar el hook.
> 2. Mientras carga:
>     - deshabilitar carga de archivos;
>     - mostrar estado compacto o helper text.
> 
> 3. Si hay error:
>     - deshabilitar carga;
>     - mostrar mensaje;
>     - ofrecer retry.
> 
> 4. Si hay config:
>     - pasar accept={config.accept};
>     - pasar maxSize={config.maxSizeBytes}.
> 
> Ejemplo con AppUpload:
> 
> const uploadConfig = useConfiguracionUploadCorrespondencia();
> 
> <AppUpload
>   value={files}
>   onChange={setFiles}
>   accept={uploadConfig.config?.accept}
>   maxSize={uploadConfig.config?.maxSizeBytes}
>   disabled={uploadConfig.loading || Boolean(uploadConfig.error)}
>   drag
>   layout="list"
>   previewOnClick={false}
>   size="sm"
>   strategy="auto"
> />
> 
> Ejemplo con AppUploadBatchView:
> 
> <AppUploadBatchView
>   files={files}
>   accept={uploadConfig.config?.accept}
>   maxSize={uploadConfig.config?.maxSizeBytes}
>   disabled={uploadConfig.loading || Boolean(uploadConfig.error)}
>   loading={uploadConfig.loading}
> />
> 
> ## Estados UI
> 
> ### Loading
> 
> Texto sugerido:
> 
> Cargando configuracion de adjuntos...
> 
> ### Error
> 
> Texto sugerido:
> 
> No fue posible cargar la configuracion de adjuntos.
> 
> Debe existir accion de retry.
> 
> ### Empty
> 
> Si backend responde success=true con data=[]:
> 
> No hay configuracion de adjuntos para CORRESPO.
> 
> Bloquear seleccion de archivos.
> 
> ## Restricciones Obligatorias
> 
> NO hacer:
> 
> - no mezclar con tipologias documentales;
> - no crear dropdowns;
> - no crear metadata por archivo;
> - no modificar backend;
> - no hardcodear extensiones en el componente;
> - no hardcodear tamano maximo en el componente;
> - no usar any;
> - no consumir clienteApi directamente desde componentes;
> - no usar jQuery, Bootstrap manual ni HTML por strings.
> 
> SI hacer:
> 
> - usar nameProceso=CORRESPO;
> - usar ExtensionUpload como fuente de accept;
> - usar LengUpload como fuente de maxSize;
> - crear servicio tipado;
> - crear hook tipado;
> - manejar loading/error/empty/retry;
> - cubrir pruebas.
> 
> ## Pruebas Obligatorias
> 
> Servicio:
> 
> - llama endpoint con nameProceso=CORRESPO;
> - normaliza PascalCase;
> - normaliza camelCase;
> - normaliza extensiones con espacios;
> - agrega punto si falta;
> - elimina duplicados;
> - retorna accept correcto;
> - retorna maxSizeBytes correcto;
> - falla si success=false;
> - falla si data=[];
> - falla si LengUpload <= 0;
> - soporta AbortSignal.
> 
> Hook:
> 
> - carga al montar;
> - no carga si enabled=false;
> - expone loading;
> - expone error;
> - expone config;
> - reload reintenta;
> - ignora respuesta stale;
> - aborta al desmontar.
> 
> UI:
> 
> - pasa accept al componente de carga;
> - pasa maxSize al componente de carga;
> - deshabilita carga mientras loading;
> - deshabilita carga si error;
> - muestra retry;
> - no rompe flujo existente de agregar/eliminar archivos.
> 
> ## Criterios De Aceptacion
> 
> - Gestion Correspondencia obtiene configuracion upload desde backend.
> - Usa nameProceso=CORRESPO.
> - Aplica extensiones permitidas desde ExtensionUpload.
> - Aplica tamano maximo desde LengUpload.
> - No hay valores hardcodeados de extensiones/tamano en UI.
> - No se implementa tipologia documental en este ticket.
> - No hay cambios backend.
> - No hay any nuevo.
> - Tests focales pasan.
> 
> ## Documentacion Esperada
> 
> Crear o actualizar:
> 
> docs/Architecture/GestionCorrrespondecia/17-FE-ConfiguracionUpload-Adjuntos-Correspo.md
> 
> Debe incluir:
> 
> - endpoint consumido;
> - nameProceso=CORRESPO;
> - contrato de respuesta;
> - mapeo ExtensionUpload -> accept;
> - mapeo LengUpload -> maxSize;
> - manejo de loading/error/empty;
> - pruebas ejecutadas.
> 
> ## Instruccion Final
> 
> Implementar la configuracion upload de adjuntos en Gestion Correspondencia consumiendo GET /api/gestor-documental/configuracion-upload?nameProceso=CORRESPO,
> normalizando ExtensionUpload y LengUpload, aplicando accept y maxSize al componente de carga existente, con servicio, hook, estados UI y pruebas, sin mezclar esta
> tarea con tipologias documentales.

## Goals / Non-Goals

**Goals**
- Refinar alcance tecnico usando el contexto completo de Jira.
- Definir decisiones arquitectonicas, riesgos y plan de migracion.

**Non-Goals**
- Cambios fuera del alcance descrito por el ticket.

## Decisions

1. TBD

## Risks / Trade-offs

- TBD

## Migration Plan

1. TBD

## Open Questions

- TBD
